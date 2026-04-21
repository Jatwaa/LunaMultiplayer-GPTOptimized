using LmpClient.Base;
using LmpClient.Network;
using LmpCommon;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Windows.Connecting
{
    /// <summary>
    /// Displays a step-by-step connection progress panel while LMP is connecting to a server.
    /// Shows each sync stage, the current status message, and any failure reason.
    /// Automatically visible during all connecting states (main menu only) and lingers
    /// for a few seconds after a failure so the player can read the reason.
    /// </summary>
    public class ConnectingWindow : Window<ConnectingWindow>
    {
        // ── Constants ─────────────────────────────────────────────────────────

        private const float WindowWidth      = 390f;
        private const float WindowHeight     = 350f;
        private const int   FailureLingerMs  = 8000;   // keep visible 8 s after failure

        // ── Steps ─────────────────────────────────────────────────────────────
        // Each entry maps a ClientState value (the "we are currently doing this" state)
        // to a human-readable label.  A step is DONE when the current state is
        // numerically greater than its trigger state.

        private static readonly (ClientState Trigger, string Label)[] Steps =
        {
            (ClientState.Connecting,           "Connect to server"),
            (ClientState.Handshaking,          "Handshake"),
            (ClientState.SyncingSettings,      "Sync settings"),
            (ClientState.SyncingKerbals,       "Sync kerbals"),
            (ClientState.SyncingWarpsubspaces, "Sync warp subspaces"),
            (ClientState.SyncingColors,        "Sync player colours"),
            (ClientState.SyncingFlags,         "Sync flags"),
            (ClientState.SyncingPlayers,       "Sync players"),
            (ClientState.SyncingScenarios,     "Sync scenarios"),
            (ClientState.SyncingLocks,         "Sync locks"),
            (ClientState.Starting,             "Start game"),
        };

        // ── Styles ────────────────────────────────────────────────────────────

        private static GUIStyle _styleDone;
        private static GUIStyle _styleCurrent;
        private static GUIStyle _stylePending;
        private static GUIStyle _styleFail;
        private static GUIStyle _styleStatus;
        private static GUIStyle _styleLinger;

        // ── Display logic ─────────────────────────────────────────────────────

        public override bool Display
        {
            get
            {
                if (!base.Display) return false;
                if (HighLogic.LoadedScene != GameScenes.MAINMENU) return false;

                var state = MainSystem.NetworkState;

                // Show during any active connection stage
                if (state >= ClientState.Connecting && state < ClientState.Running)
                    return true;

                // Linger after a failure so the player can read the reason
                if (state <= ClientState.Disconnected && !string.IsNullOrEmpty(NetworkConnection.LastFailureReason))
                {
                    var elapsed = unchecked(System.Environment.TickCount - NetworkConnection.LastFailureTickCount);
                    return elapsed >= 0 && elapsed < FailureLingerMs;
                }

                return false;
            }
        }

        // ── Window setup ──────────────────────────────────────────────────────

        public override void SetStyles()
        {
            WindowRect = new Rect(
                Screen.width  / 2f - WindowWidth  / 2f,
                Screen.height / 2f - WindowHeight / 2f,
                WindowWidth, WindowHeight);
            MoveRect = new Rect(0, 0, int.MaxValue, TitleHeight);

            LayoutOptions = new[]
            {
                GUILayout.MinWidth(WindowWidth),   GUILayout.MaxWidth(WindowWidth),
                GUILayout.MinHeight(WindowHeight), GUILayout.MaxHeight(WindowHeight),
            };

            _styleDone = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal   = { textColor = new Color(0.40f, 0.90f, 0.40f) },
            };

            _styleCurrent = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };

            _stylePending = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal   = { textColor = new Color(0.45f, 0.45f, 0.45f) },
            };

            _styleFail = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                wordWrap  = true,
                normal    = { textColor = new Color(1.00f, 0.30f, 0.30f) },
            };

            _styleStatus = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                normal   = { textColor = new Color(0.80f, 0.80f, 0.80f) },
            };

            _styleLinger = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 11,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = new Color(0.55f, 0.55f, 0.55f) },
            };
        }

        // ── Rendering ─────────────────────────────────────────────────────────

        protected override void DrawGui()
        {
            WindowRect = FixWindowPos(GUILayout.Window(
                6720 + MainSystem.WindowOffset,
                WindowRect,
                DrawContent,
                $"LMP {LmpVersioning.CurrentVersion} - Connecting",
                LayoutOptions));
        }

        protected override void DrawWindowContent(int windowId)
        {
            var state   = MainSystem.NetworkState;
            var failed  = state <= ClientState.Disconnected || state == ClientState.DisconnectRequested;
            var failAt  = NetworkConnection.LastFailedAtState;
            var reason  = NetworkConnection.LastFailureReason;

            GUILayout.Space(4);

            // ── Step list ──────────────────────────────────────────────────
            foreach (var (trigger, label) in Steps)
            {
                string   prefix;
                GUIStyle style;

                if (failed)
                {
                    // Show progress up to the point of failure
                    if ((int)trigger < (int)failAt)
                    {
                        prefix = "[+] "; style = _styleDone;
                    }
                    else if ((int)trigger == (int)failAt)
                    {
                        prefix = "[!] "; style = _styleFail;
                    }
                    else
                    {
                        prefix = "[ ] "; style = _stylePending;
                    }
                }
                else
                {
                    var stateInt    = (int)state;
                    var triggerInt  = (int)trigger;

                    if (stateInt > triggerInt)
                    {
                        prefix = "[+] "; style = _styleDone;
                    }
                    else if (stateInt == triggerInt ||
                             // Connected (2) is the "done-connecting" intermediate before Handshaking
                             (trigger == ClientState.Connecting && stateInt == (int)ClientState.Connected))
                    {
                        prefix = "[>] "; style = _styleCurrent;
                    }
                    else
                    {
                        prefix = "[ ] "; style = _stylePending;
                    }
                }

                GUILayout.Label(prefix + label, style);
            }

            GUILayout.Space(6);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(4);

            // ── Status / failure message ────────────────────────────────────
            if (failed && !string.IsNullOrEmpty(reason))
            {
                GUILayout.Label("Failed: " + reason, _styleFail);
                GUILayout.Space(2);

                var elapsedMs  = unchecked(System.Environment.TickCount - NetworkConnection.LastFailureTickCount);
                var remainSecs = Mathf.CeilToInt((FailureLingerMs - elapsedMs) / 1000f);
                if (remainSecs > 0)
                    GUILayout.Label($"This dialog closes in {remainSecs}s", _styleLinger);
            }
            else if (!failed)
            {
                GUILayout.Label(MainSystem.Singleton?.Status ?? "", _styleStatus);
            }

            GUILayout.FlexibleSpace();

            // ── Cancel button (only while actively connecting) ──────────────
            if (!failed)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(90), GUILayout.Height(24)))
                    NetworkConnection.Disconnect("Cancelled by user");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4);
        }
    }
}
