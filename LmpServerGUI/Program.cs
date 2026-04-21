using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WinTimer = System.Windows.Forms.Timer;

namespace LmpServerGUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        // ── KSP colour palette ──────────────────────────────────────────────
        private static readonly Color C_BG        = Color.FromArgb(0x0D, 0x0F, 0x14);
        private static readonly Color C_PANEL     = Color.FromArgb(0x15, 0x18, 0x20);
        private static readonly Color C_BORDER    = Color.FromArgb(0x2A, 0x2D, 0x3A);
        private static readonly Color C_TEXT      = Color.FromArgb(0xC8, 0xC8, 0xC8);
        private static readonly Color C_DIM       = Color.FromArgb(0x66, 0x68, 0x72);
        private static readonly Color C_GREEN     = Color.FromArgb(0x5C, 0xDD, 0x3E);
        private static readonly Color C_ORANGE    = Color.FromArgb(0xF0, 0xA0, 0x30);
        private static readonly Color C_RED       = Color.FromArgb(0xE0, 0x50, 0x50);
        private static readonly Color C_BLUE      = Color.FromArgb(0x7C, 0xB8, 0xFF);
        private static readonly Color C_BTN_START = Color.FromArgb(0x1E, 0x5C, 0x1E);
        private static readonly Color C_BTN_STOP  = Color.FromArgb(0x5C, 0x1E, 0x1E);
        private static readonly Color C_BTN_MAP   = Color.FromArgb(0x1E, 0x3A, 0x5C);

        private static readonly Font F_TITLE  = new Font("Consolas", 11F, FontStyle.Bold);
        private static readonly Font F_LABEL  = new Font("Consolas",  9F, FontStyle.Bold);
        private static readonly Font F_MONO   = new Font("Consolas",  8F);
        private static readonly Font F_BTN    = new Font("Consolas",  9F, FontStyle.Bold);

        // ── Win32 console signal API ─────────────────────────────────────────
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(IntPtr handler, bool add);
        private const uint CTRL_C_EVENT = 0;

        /// <summary>Sends CTRL+C to the given process via its console group.</summary>
        private static bool SendCtrlC(int processId)
        {
            if (!AttachConsole((uint)processId)) return false;
            SetConsoleCtrlHandler(IntPtr.Zero, true);   // ignore in this process
            try   { return GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0); }
            finally
            {
                FreeConsole();
                SetConsoleCtrlHandler(IntPtr.Zero, false); // restore
            }
        }

        // ── State ────────────────────────────────────────────────────────────
        private Process?       _serverProcess;
        private StreamWriter?  _serverInput;
        private volatile bool  _stopping;               // true = we requested stop
        private string         _serverPath = "";
        private string         _webUrl     = "http://localhost:8900";
        private readonly HttpClient _http  = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };

        // ── Timers ───────────────────────────────────────────────────────────
        private WinTimer? _statusTimer;
        private WinTimer? _chatTimer;
        private WinTimer? _playerTimer;

        // ── Controls ─────────────────────────────────────────────────────────
        private Label   lblTitle       = null!;
        private Label   lblStatusDot   = null!;
        private Label   lblStatusText  = null!;
        private Label   lblPlayers     = null!;
        private TextBox txtServerPath  = null!;
        private Button  btnBrowse      = null!;
        private Button  btnStart       = null!;
        private Button  btnStop        = null!;
        private Button  btnOpenMap     = null!;
        private Label   lblCommHeader  = null!;
        private ListBox lstChat        = null!;
        private TextBox txtChatInput   = null!;
        private Button  btnSend        = null!;
        private Label   lblLogHeader   = null!;
        private TextBox txtConsole     = null!;
        private Button  btnClearLog    = null!;

        public MainForm()
        {
            BuildLayout();
            LoadSettings();
            SetupTimers();
        }

        // ── Layout ────────────────────────────────────────────────────────────
        private void BuildLayout()
        {
            SuspendLayout();
            BackColor  = C_BG;
            ForeColor  = C_TEXT;
            ClientSize = new Size(720, 620);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Text        = "Luna Multiplayer — Server Control";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing  += OnFormClosing;

            int x = 12, y = 12, w = 696;

            // ── Title row ────────────────────────────────────────────────────
            lblTitle = KspLabel("◈ LUNA MULTIPLAYER  SERVER CONTROL", F_TITLE, C_GREEN, new Point(x, y));
            lblTitle.AutoSize = true;
            Controls.Add(lblTitle);

            lblStatusDot  = KspLabel("●", F_TITLE, C_RED, new Point(460, y));
            lblStatusDot.AutoSize = true;
            Controls.Add(lblStatusDot);

            lblStatusText = KspLabel("OFFLINE", F_LABEL, C_RED, new Point(482, y + 2));
            lblStatusText.AutoSize = true;
            Controls.Add(lblStatusText);

            lblPlayers = KspLabel("PLAYERS: 0", F_LABEL, C_DIM, new Point(570, y + 2));
            lblPlayers.AutoSize = true;
            Controls.Add(lblPlayers);

            // ── Divider ──────────────────────────────────────────────────────
            y += 32;
            Controls.Add(HRule(x, y, w));

            // ── Server path row ──────────────────────────────────────────────
            y += 8;
            var pathLabel = KspLabel("SERVER EXE:", F_LABEL, C_DIM, new Point(x, y + 3));
            pathLabel.AutoSize = true;
            Controls.Add(pathLabel);

            txtServerPath = new TextBox
            {
                Font      = F_MONO,
                BackColor = C_PANEL,
                ForeColor = C_TEXT,
                BorderStyle = BorderStyle.FixedSingle,
                Location  = new Point(x + 90, y),
                Size      = new Size(490, 22)
            };
            Controls.Add(txtServerPath);

            btnBrowse = KspButton("BROWSE", C_BORDER, new Point(x + 586, y - 1), new Size(82, 24));
            btnBrowse.Click += BtnBrowse_Click;
            Controls.Add(btnBrowse);

            // ── Control buttons row ──────────────────────────────────────────
            y += 34;
            btnStart = KspButton("▶  START", C_BTN_START, new Point(x, y), new Size(100, 30));
            btnStart.ForeColor = C_GREEN;
            btnStart.Click += BtnStart_Click;
            Controls.Add(btnStart);

            btnStop = KspButton("■  STOP", C_BTN_STOP, new Point(x + 108, y), new Size(100, 30));
            btnStop.ForeColor = C_RED;
            btnStop.Enabled   = false;
            btnStop.Click    += BtnStop_Click;
            Controls.Add(btnStop);

            btnOpenMap = KspButton("🗺  MAP", C_BTN_MAP, new Point(x + 216, y), new Size(90, 30));
            btnOpenMap.ForeColor = C_BLUE;
            btnOpenMap.Enabled  = false;
            btnOpenMap.Click   += (_, _) => Process.Start(new ProcessStartInfo(_webUrl + "/map/") { UseShellExecute = true });
            Controls.Add(btnOpenMap);

            // ── Divider ──────────────────────────────────────────────────────
            y += 40;
            Controls.Add(HRule(x, y, w));

            // ── COMM CHANNEL section ─────────────────────────────────────────
            y += 8;
            lblCommHeader = KspLabel("◈ COMM CHANNEL", F_LABEL, C_ORANGE, new Point(x, y));
            lblCommHeader.AutoSize = true;
            Controls.Add(lblCommHeader);

            y += 22;
            lstChat = new ListBox
            {
                BackColor        = C_PANEL,
                ForeColor        = C_TEXT,
                Font             = F_MONO,
                BorderStyle      = BorderStyle.FixedSingle,
                Location         = new Point(x, y),
                Size             = new Size(w, 110),
                HorizontalScrollbar = false,
                SelectionMode    = SelectionMode.None
            };
            Controls.Add(lstChat);

            y += 118;
            txtChatInput = new TextBox
            {
                Font        = F_MONO,
                BackColor   = C_PANEL,
                ForeColor   = C_TEXT,
                BorderStyle = BorderStyle.FixedSingle,
                Location    = new Point(x, y),
                Size        = new Size(w - 88, 24),
                PlaceholderText = "Type a message to all players..."
            };
            txtChatInput.KeyPress += (_, e) => { if (e.KeyChar == (char)13) { SendChat(); e.Handled = true; } };
            Controls.Add(txtChatInput);

            btnSend = KspButton("SEND", Color.FromArgb(0x1E, 0x3A, 0x1E), new Point(x + w - 84, y - 1), new Size(84, 26));
            btnSend.ForeColor = C_GREEN;
            btnSend.Click    += (_, _) => SendChat();
            Controls.Add(btnSend);

            // ── SERVER LOG section ───────────────────────────────────────────
            y += 34;
            Controls.Add(HRule(x, y, w));
            y += 8;

            lblLogHeader = KspLabel("◈ SERVER LOG", F_LABEL, C_ORANGE, new Point(x, y));
            lblLogHeader.AutoSize = true;
            Controls.Add(lblLogHeader);

            btnClearLog = KspButton("CLEAR", C_BORDER, new Point(x + w - 60, y - 2), new Size(60, 22));
            btnClearLog.Click += (_, _) => txtConsole.Clear();
            Controls.Add(btnClearLog);

            y += 22;
            txtConsole = new TextBox
            {
                Font        = F_MONO,
                BackColor   = Color.FromArgb(0x08, 0x0A, 0x0E),
                ForeColor   = Color.FromArgb(0x5C, 0xDD, 0x3E),
                BorderStyle = BorderStyle.FixedSingle,
                Location    = new Point(x, y),
                Multiline   = true,
                ReadOnly    = true,
                ScrollBars  = ScrollBars.Vertical,
                Size        = new Size(w, 220),
                WordWrap    = false
            };
            Controls.Add(txtConsole);

            ResumeLayout(false);
            PerformLayout();
        }

        // ── Layout helpers ────────────────────────────────────────────────────
        private static Label KspLabel(string text, Font font, Color fore, Point loc)
            => new Label { Text = text, Font = font, ForeColor = fore, Location = loc, BackColor = Color.Transparent };

        private static Button KspButton(string text, Color back, Point loc, Size size)
        {
            var b = new Button
            {
                Text             = text,
                Font             = F_BTN,
                BackColor        = back,
                ForeColor        = C_TEXT,
                FlatStyle        = FlatStyle.Flat,
                Location         = loc,
                Size             = size,
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderColor = C_BORDER;
            b.FlatAppearance.BorderSize  = 1;
            return b;
        }

        private static Panel HRule(int x, int y, int w)
            => new Panel { Location = new Point(x, y), Size = new Size(w, 1), BackColor = C_BORDER };

        // ── Settings ──────────────────────────────────────────────────────────
        private void LoadSettings()
        {
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var pathMatch = Regex.Match(json, "\"serverPath\"\\s*:\\s*\"([^\"]+)\"");
                if (pathMatch.Success)
                    _serverPath = pathMatch.Groups[1].Value.Replace("\\\\", "\\");

                var portMatch = Regex.Match(json, "\"webPort\"\\s*:\\s*(\\d+)");
                if (portMatch.Success)
                    _webUrl = "http://localhost:" + portMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(_serverPath))
                _serverPath = Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..", "Server", "bin", "Debug", "net8.0", "Server.exe"));

            txtServerPath.Text = _serverPath;
        }

        private void SaveSettings()
        {
            try
            {
                var path    = txtServerPath.Text;
                var escaped = path.Replace("\\", "\\\\");
                var port    = _webUrl.Contains("8900") ? "8900" : Regex.Match(_webUrl, @":(\d+)").Groups[1].Value;
                File.WriteAllText(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"),
                    $"{{\n  \"serverPath\": \"{escaped}\",\n  \"webPort\": {port},\n  \"gamePort\": 8800\n}}");
            }
            catch { }
        }

        // ── Timers ────────────────────────────────────────────────────────────
        private void SetupTimers()
        {
            _statusTimer = new WinTimer { Interval = 4000 };
            _statusTimer.Tick += (_, _) => CheckProcess();

            _chatTimer = new WinTimer { Interval = 2000 };
            _chatTimer.Tick += async (_, _) => await PollChatAsync();

            _playerTimer = new WinTimer { Interval = 5000 };
            _playerTimer.Tick += async (_, _) => await PollPlayersAsync();
        }

        private void StartTimers()
        {
            _statusTimer?.Start();
            _chatTimer?.Start();
            _playerTimer?.Start();
        }

        private void StopTimers()
        {
            _statusTimer?.Stop();
            _chatTimer?.Stop();
            _playerTimer?.Stop();
        }

        // ── Server control ────────────────────────────────────────────────────
        private void BtnStart_Click(object? sender, EventArgs e)
        {
            var exePath = txtServerPath.Text;
            if (!File.Exists(exePath))
            {
                MessageBox.Show($"Server executable not found:\n{exePath}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName               = exePath,
                        WorkingDirectory       = Path.GetDirectoryName(exePath) ?? ".",
                        UseShellExecute        = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput  = true,
                        RedirectStandardError  = true,
                        CreateNoWindow         = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding  = Encoding.UTF8
                    },
                    EnableRaisingEvents = true
                };

                _serverProcess.OutputDataReceived += (_, d) =>
                {
                    if (!string.IsNullOrEmpty(d.Data))
                        BeginInvoke(() => AppendLog(d.Data));
                };
                _serverProcess.ErrorDataReceived += (_, d) =>
                {
                    if (!string.IsNullOrEmpty(d.Data))
                        BeginInvoke(() => AppendLog("[ERR] " + d.Data));
                };
                _serverProcess.Exited += (_, _) => BeginInvoke(() =>
                {
                    if (_stopping)
                    {
                        SetStatus("OFFLINE", C_DIM);
                        AppendLog("=== Server stopped ===");
                    }
                    else
                    {
                        SetStatus("CRASHED", C_RED);
                        AppendLog("=== SERVER CRASHED ===");
                    }
                    _stopping = false;
                    lblPlayers.Text      = "PLAYERS: 0";
                    lblPlayers.ForeColor = C_DIM;
                    SetRunningState(false);
                });

                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
                _serverInput = _serverProcess.StandardInput;
                _serverInput.AutoFlush = true;

                SetStatus("ONLINE", C_GREEN);
                SetRunningState(true);
                StartTimers();
                AppendLog($"=== Server started (PID {_serverProcess.Id}) ===");
                AppendLog($"=== Map: {_webUrl}/map/ ===");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start server:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            if (_serverProcess?.HasExited == false)
                GracefulStop(_serverProcess);
        }

        private void GracefulStop(Process proc)
        {
            _stopping = true;
            btnStop.Enabled = false;
            AppendLog("Stopping server...");
            try
            {
                // Try Ctrl+C first (triggers the server's graceful shutdown path)
                if (!SendCtrlC(proc.Id))
                {
                    // Fallback: stdin "exit" command
                    _serverInput?.WriteLine("exit");
                }

                // Give the server up to 8 seconds to shut down cleanly
                if (!proc.WaitForExit(8000))
                {
                    AppendLog("Server did not exit in time — force killing.");
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Stop error: {ex.Message}");
                _stopping = false;
            }
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Server Executable|Server.exe|All Files|*.*",
                Title  = "Select Server Executable"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtServerPath.Text = dlg.FileName;
                _serverPath        = dlg.FileName;
                SaveSettings();
            }
        }

        private void SetRunningState(bool running)
        {
            btnStart.Enabled  = !running;
            btnStop.Enabled   = running;
            btnOpenMap.Enabled = running;
            if (!running) StopTimers();
        }

        private void SetStatus(string text, Color color)
        {
            lblStatusDot.ForeColor  = color;
            lblStatusText.Text      = text;
            lblStatusText.ForeColor = color;
        }

        private void CheckProcess()
        {
            // The Exited event handles status; this is just a safety-net for
            // cases where the event didn't fire (e.g. process died before we
            // subscribed). Only act when _stopping is false so we don't fight
            // with the Exited handler.
            if (_serverProcess?.HasExited == true && !_stopping)
            {
                SetStatus("CRASHED", C_RED);
                SetRunningState(false);
                lblPlayers.Text      = "PLAYERS: 0";
                lblPlayers.ForeColor = C_DIM;
            }
        }

        // ── Console log ───────────────────────────────────────────────────────
        private void AppendLog(string text)
        {
            if (InvokeRequired) { BeginInvoke(() => AppendLog(text)); return; }
            var ts = DateTime.Now.ToString("HH:mm:ss");
            txtConsole.AppendText($"[{ts}] {text}\r\n");
        }

        // ── Chat ──────────────────────────────────────────────────────────────
        private void SendChat()
        {
            var text = txtChatInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;
            txtChatInput.Clear();

            _ = SendChatAsync(text);
        }

        private async System.Threading.Tasks.Task SendChatAsync(string text)
        {
            try
            {
                var content  = new FormUrlEncodedContent(new Dictionary<string, string> { { "text", text } });
                var response = await _http.PostAsync(_webUrl + "/api/chat", content);
                if (!response.IsSuccessStatusCode)
                    AppendLog($"Chat send failed: {response.StatusCode}");
            }
            catch (Exception ex) { AppendLog($"Chat error: {ex.Message}"); }
        }

        private async System.Threading.Tasks.Task PollChatAsync()
        {
            if (_serverProcess?.HasExited != false) return;
            try
            {
                var json = await _http.GetStringAsync(_webUrl + "/api/chat");
                var data = JsonSerializer.Deserialize<ChatApiResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data?.Messages == null) return;

                if (InvokeRequired) { BeginInvoke(() => UpdateChatList(data.Messages)); return; }
                UpdateChatList(data.Messages);
            }
            catch { }
        }

        private void UpdateChatList(List<ChatMessage> messages)
        {
            lstChat.BeginUpdate();
            lstChat.Items.Clear();
            foreach (var msg in messages)
            {
                var time = msg.Timestamp.ToLocalTime().ToString("HH:mm");
                lstChat.Items.Add($"[{time}] {msg.From}: {msg.Text}");
            }
            if (lstChat.Items.Count > 0)
                lstChat.TopIndex = lstChat.Items.Count - 1;
            lstChat.EndUpdate();
        }

        // ── Player count ──────────────────────────────────────────────────────
        private async System.Threading.Tasks.Task PollPlayersAsync()
        {
            if (_serverProcess?.HasExited != false) return;
            try
            {
                var json = await _http.GetStringAsync(_webUrl + "/api/telemetry");
                var data = JsonSerializer.Deserialize<TelemetryApiResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var count = data?.TotalPlayers ?? 0;
                if (InvokeRequired) { BeginInvoke(() => SetPlayerCount(count)); return; }
                SetPlayerCount(count);
            }
            catch { }
        }

        private void SetPlayerCount(int count)
        {
            lblPlayers.Text      = $"PLAYERS: {count}";
            lblPlayers.ForeColor = count > 0 ? C_GREEN : C_DIM;
        }

        // ── Form closing ──────────────────────────────────────────────────────
        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveSettings();
            if (_serverProcess?.HasExited == false)
            {
                if (MessageBox.Show("Server is running. Stop it and exit?", "Confirm Exit",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                try { GracefulStop(_serverProcess); }
                catch { }
            }
            _statusTimer?.Dispose();
            _chatTimer?.Dispose();
            _playerTimer?.Dispose();
        }

        // ── JSON model classes ────────────────────────────────────────────────
        private class ChatApiResponse
        {
            public List<ChatMessage>? Messages { get; set; }
        }

        private class ChatMessage
        {
            public string   From      { get; set; } = "";
            public string   Text      { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }

        private class TelemetryApiResponse
        {
            public int TotalPlayers { get; set; }
        }
    }
}
