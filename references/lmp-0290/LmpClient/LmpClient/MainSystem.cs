// Decompiled with JetBrains decompiler
// Type: LmpClient.MainSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using CommNet;
using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Events.Base;
using LmpClient.Localization;
using LmpClient.ModuleStore;
using LmpClient.ModuleStore.Patching;
using LmpClient.Network;
using LmpClient.Systems;
using LmpClient.Systems.Flag;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.Mod;
using LmpClient.Systems.ModApi;
using LmpClient.Systems.Network;
using LmpClient.Systems.Scenario;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.Warp;
using LmpClient.Utilities;
using LmpClient.Windows;
using LmpCommon;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace LmpClient
{
  [KSPAddon]
  public class MainSystem : MonoBehaviour
  {
    private static volatile ClientState _networkState;
    public const int WindowOffset = 1664147604;
    private static int _mainThreadId;

    public static MainSystem Singleton { get; set; }

    public static string KspPath { get; private set; }

    public static string UniqueIdentifier { get; private set; }

    public static ClientState NetworkState
    {
      get => MainSystem._networkState;
      set
      {
        if (value == ClientState.Disconnected)
          NetworkMain.ResetNetworkSystem();
        MainSystem._networkState = value;
        NetworkSystem.NetworkStatus = new ClientState?(value);
      }
    }

    public string Status { get; set; }

    public static bool ToolbarShowGui { get; set; } = true;

    public static ServerEntry CommandLineServer { get; set; }

    public bool LmpSaveChecked { get; set; }

    public bool ForceQuit { get; set; }

    public bool StartGame { get; set; }

    public bool Enabled { get; set; } = true;

    public static bool IsUnityThread => Thread.CurrentThread.ManagedThreadId == MainSystem._mainThreadId;

    public static Dictionary<CelestialBody, double> BodiesGees { get; } = new Dictionary<CelestialBody, double>();

    public void Update()
    {
      LunaLog.ProcessLogMessages();
      LunaScreenMsg.ProcessScreenMessages();
      if (!this.Enabled)
        return;
      try
      {
        if (HighLogic.LoadedScene == 2 && !this.LmpSaveChecked)
        {
          this.LmpSaveChecked = true;
          MainSystem.SetupBlankGameIfNeeded();
        }
        MainSystem.HandleWindowEvents();
        SystemsHandler.Update();
        WindowsHandler.Update();
        if (this.ForceQuit)
        {
          this.ForceQuit = false;
          MainSystem.NetworkState = ClientState.Disconnected;
          MainSystem.StopGame();
        }
        if (MainSystem.NetworkState == ClientState.DisconnectRequested)
          MainSystem.NetworkState = ClientState.Disconnected;
        if (MainSystem.NetworkState >= ClientState.Running)
        {
          if (HighLogic.LoadedScene == 2)
            NetworkConnection.Disconnect("Quit to main menu");
          if (HighLogic.LoadedScene == 7 && MainSystem.BodiesGees.Count == 0)
          {
            foreach (CelestialBody body in FlightGlobals.Bodies)
              MainSystem.BodiesGees.Add(body, body.GeeASL);
          }
          if (!SettingsSystem.ServerSettings.AllowCheats)
          {
            CheatOptions.NonStrictAttachmentOrientation = false;
            CheatOptions.BiomesVisible = false;
            CheatOptions.AllowPartClipping = false;
            CheatOptions.IgnoreMaxTemperature = false;
            CheatOptions.UnbreakableJoints = false;
            CheatOptions.InfiniteElectricity = false;
            CheatOptions.InfinitePropellant = false;
            CheatOptions.NoCrashDamage = false;
            foreach (KeyValuePair<CelestialBody, double> bodiesGee in MainSystem.BodiesGees)
              bodiesGee.Key.GeeASL = bodiesGee.Value;
          }
        }
        if (!this.StartGame)
          return;
        this.StartGame = false;
        this.StartGameNow();
      }
      catch (Exception ex)
      {
        this.HandleException(ex, "Main system- update");
      }
    }

    public void FixedUpdate()
    {
      if (!this.Enabled)
        return;
      SystemsHandler.FixedUpdate();
    }

    public void LateUpdate()
    {
      if (!this.Enabled)
        return;
      SystemsHandler.LateUpdate();
    }

    public void OnApplicationQuit() => this.OnExit();

    public void OnDestroy() => this.OnExit();

    public void Start()
    {
      CompatibilityChecker.SpawnDialog();
      InstallChecker.SpawnDialog();
      LocalizationContainer.LoadLanguages();
      LocalizationContainer.LoadLanguage(SettingsSystem.CurrentSettings.Language);
      SystemsHandler.FillUpSystemsList();
      WindowsHandler.FillUpWindowsList();
      LmpClient.Base.System<ModApiSystem>.Singleton.Enabled = true;
      LmpClient.Base.System<NetworkSystem>.Singleton.Enabled = true;
      if (!SettingsSystem.CurrentSettings.DisclaimerAccepted)
      {
        this.Enabled = false;
        DisclaimerDialog.SpawnDialog();
      }
      else
        this.StartCoroutine(UpdateHandler.CheckForUpdates());
    }

    public void Awake()
    {
      MainSystem.Singleton = this;
      Object.DontDestroyOnLoad((Object) this);
      MainSystem.FixBindingRedirects();
      MainSystem.KspPath = UrlDir.ApplicationRootPath;
      MainSystem.UniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
      MainSystem._mainThreadId = Thread.CurrentThread.ManagedThreadId;
      LunaLog.Log(string.Format("[LMP]: LMP {0} Starting at: {1}", (object) LmpVersioning.CurrentVersion, (object) MainSystem.KspPath));
      LunaLog.Log("[LMP]: Process ID: " + CommonUtil.ProcessId);
      if (!CompatibilityChecker.IsCompatible() || !InstallChecker.IsCorrectlyInstalled())
      {
        this.Enabled = false;
      }
      else
      {
        FieldModuleStore.ReadCustomizationXml();
        LmpBaseEvent.Awake();
        HarmonyPatcher.Awake();
        PartModuleRunner.Awake();
        MainSystem.SetupDirectoriesIfNeeded();
        MainSystem.HandleCommandLineArgs();
        NetworkMain.AwakeNetworkSystem();
        LmpClient.Base.System<ModSystem>.Singleton.BuildDllFileList();
        LunaLog.Log("[LMP]: LMP Finished awakening");
        MainSystem.NetworkState = ClientState.Disconnected;
      }
    }

    public void OnGUI()
    {
      if (Object.op_Equality((Object) StyleLibrary.DefaultSkin, (Object) null))
        StyleLibrary.DefaultSkin = GUI.skin;
      WindowsHandler.OnGui();
    }

    public void OnExit()
    {
      NetworkConnection.Disconnect("Quit game");
      MainSystem.NetworkState = ClientState.Disconnected;
      LunaLog.ProcessLogMessages();
    }

    public Game.Modes ConvertGameMode(GameMode inputMode)
    {
      switch (inputMode)
      {
        case GameMode.Sandbox:
          return (Game.Modes) 0;
        case GameMode.Science:
          return (Game.Modes) 4;
        case GameMode.Career:
          return (Game.Modes) 1;
        default:
          return (Game.Modes) 0;
      }
    }

    public void HandleException(Exception e, string eventName)
    {
      LunaLog.LogError(string.Format("[LMP]: Threw in {0} event, exception: {1}", (object) eventName, (object) e));
      NetworkConnection.Disconnect("Unhandled error in " + eventName + " event! exception: " + eventName);
      this.ForceQuit = true;
      MainSystem.NetworkState = ClientState.Disconnected;
    }

    public void DisconnectFromGame()
    {
      this.ForceQuit = true;
      NetworkConnection.Disconnect("Quit");
      LmpClient.Base.System<ScenarioSystem>.Singleton.SendScenarioModules();
    }

    private static void StopGame()
    {
      HighLogic.SaveFolder = "LunaMultiplayer";
      if (HighLogic.LoadedScene != 2)
        HighLogic.LoadScene((GameScenes) 2);
      MainSystem.BodiesGees.Clear();
      FlightGlobals.Vessels.Clear();
      FlightGlobals.VesselsLoaded.Clear();
      FlightGlobals.VesselsUnloaded.Clear();
      FlightGlobals.fetch.activeVessel = (Vessel) null;
      CraftBrowserDialog objectOfType = Object.FindObjectOfType<CraftBrowserDialog>();
      if (!Object.op_Inequality((Object) objectOfType, (Object) null))
        return;
      objectOfType.Dismiss();
    }

    private static void HandleWindowEvents()
    {
      if (MainSystem.CommandLineServer == null || HighLogic.LoadedScene != 2 || (double) Time.timeSinceLevelLoad <= 1.0)
        return;
      NetworkConnection.ConnectToServer(MainSystem.CommandLineServer.Address, MainSystem.CommandLineServer.Port, MainSystem.CommandLineServer.Password);
      MainSystem.CommandLineServer = (ServerEntry) null;
    }

    private void StartGameNow()
    {
      HighLogic.CurrentGame = MainSystem.CreateBlankGame();
      HighLogic.CurrentGame.Mode = this.ConvertGameMode(SettingsSystem.ServerSettings.GameMode);
      HighLogic.CurrentGame.Parameters = SettingsSystem.ServerSettings.ServerParameters;
      this.SetAdvancedParams(HighLogic.CurrentGame);
      this.SetCommNetParams(HighLogic.CurrentGame);
      HighLogic.CurrentGame.flightState.universalTime = LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTime;
      LmpClient.Base.System<KerbalSystem>.Singleton.LoadKerbalsIntoGame();
      LmpClient.Base.System<ScenarioSystem>.Singleton.LoadScenarioDataIntoGame();
      LmpClient.Base.System<ScenarioSystem>.Singleton.LoadMissingScenarioDataIntoGame();
      LunaLog.Log(string.Format("[LMP]: Starting {0} game...", (object) SettingsSystem.ServerSettings.GameMode));
      GamePersistence.SaveGame(HighLogic.CurrentGame, "persistent", HighLogic.SaveFolder, (SaveMode) 0);
      HighLogic.CurrentGame.Start();
      LunaLog.Log("[LMP]: Started!");
    }

    public void SetAdvancedParams(Game currentGame)
    {
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().EnableKerbalExperience = SettingsSystem.ServerSettings.ServerAdvancedParameters.EnableKerbalExperience;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().ImmediateLevelUp = SettingsSystem.ServerSettings.ServerAdvancedParameters.ImmediateLevelUp;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().AllowNegativeCurrency = SettingsSystem.ServerSettings.ServerAdvancedParameters.AllowNegativeCurrency;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().ActionGroupsAlways = SettingsSystem.ServerSettings.ServerAdvancedParameters.ActionGroupsAlways;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().GKerbalLimits = SettingsSystem.ServerSettings.ServerAdvancedParameters.GKerbalLimits;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().GPartLimits = SettingsSystem.ServerSettings.ServerAdvancedParameters.GPartLimits;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().KerbalGToleranceMult = SettingsSystem.ServerSettings.ServerAdvancedParameters.KerbalGToleranceMult;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().PressurePartLimits = SettingsSystem.ServerSettings.ServerAdvancedParameters.PressurePartLimits;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().BuildingImpactDamageMult = SettingsSystem.ServerSettings.ServerAdvancedParameters.BuildingImpactDamageMult;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().PartUpgradesInCareer = SettingsSystem.ServerSettings.ServerAdvancedParameters.PartUpgradesInCareer;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().PartUpgradesInSandbox = SettingsSystem.ServerSettings.ServerAdvancedParameters.PartUpgradesInSandbox;
      currentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().ResourceTransferObeyCrossfeed = SettingsSystem.ServerSettings.ServerAdvancedParameters.ResourceTransferObeyCrossfeed;
    }

    public void SetCommNetParams(Game currentGame)
    {
      currentGame.Parameters.CustomParams<CommNetParams>().plasmaBlackout = SettingsSystem.ServerSettings.ServerCommNetParameters.plasmaBlackout;
      currentGame.Parameters.CustomParams<CommNetParams>().enableGroundStations = SettingsSystem.ServerSettings.ServerCommNetParameters.enableGroundStations;
      currentGame.Parameters.CustomParams<CommNetParams>().requireSignalForControl = SettingsSystem.ServerSettings.ServerCommNetParameters.requireSignalForControl;
      currentGame.Parameters.CustomParams<CommNetParams>().rangeModifier = SettingsSystem.ServerSettings.ServerCommNetParameters.rangeModifier;
      currentGame.Parameters.CustomParams<CommNetParams>().DSNModifier = SettingsSystem.ServerSettings.ServerCommNetParameters.DSNModifier;
      currentGame.Parameters.CustomParams<CommNetParams>().occlusionMultiplierVac = SettingsSystem.ServerSettings.ServerCommNetParameters.occlusionMultiplierVac;
      currentGame.Parameters.CustomParams<CommNetParams>().occlusionMultiplierAtm = SettingsSystem.ServerSettings.ServerCommNetParameters.occlusionMultiplierAtm;
    }

    private static void HandleCommandLineArgs()
    {
      bool flag = false;
      int result = 8800;
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      if (!((IEnumerable<string>) commandLineArgs).Any<string>())
        return;
      if (ArrayExtensions.IndexOf<string>(commandLineArgs, "-debug") >= 0)
      {
        NetworkMain.Config.ConnectionTimeout = float.MaxValue;
        NetworkMain.RandomizeBadConnectionValues();
      }
      int index1 = ArrayExtensions.IndexOf<string>(commandLineArgs, "-logFile") + 1;
      if (index1 > 0 && commandLineArgs.Length > index1)
        CommonUtil.OutputLogFilePath = commandLineArgs[index1];
      int index2 = ArrayExtensions.IndexOf<string>(commandLineArgs, "-lmp") + 1;
      if (index2 > 0 && commandLineArgs.Length > index2)
      {
        string str = commandLineArgs[index2];
        if (str.Contains("lmp://"))
        {
          if (str.Substring("lmp://".Length).Contains(":"))
          {
            str = str.Substring("lmp://".Length).Substring(0, str.LastIndexOf(":", StringComparison.Ordinal));
            flag = int.TryParse(str.Substring(str.LastIndexOf(":", StringComparison.Ordinal) + 1), out result);
          }
          else
          {
            str = str.Substring("lmp://".Length);
            flag = true;
          }
        }
        if (flag)
        {
          MainSystem.CommandLineServer = new ServerEntry()
          {
            Address = str,
            Port = result
          };
          LunaLog.Log(string.Format("[LMP]: Connecting via command line to: {0}, port: {1}", (object) str, (object) result));
        }
        else
          LunaLog.LogError(string.Format("[LMP]: Command line address is invalid: {0}, port: {1}", (object) str, (object) result));
      }
    }

    private static void SetupDirectoriesIfNeeded()
    {
      string path = CommonUtil.CombinePaths(MainSystem.KspPath, "saves", "LunaMultiplayer");
      MainSystem.CreateIfNeeded(path);
      MainSystem.CreateIfNeeded(CommonUtil.CombinePaths(path, "Ships"));
      MainSystem.CreateIfNeeded(CommonUtil.CombinePaths(path, CommonUtil.CombinePaths("Ships", "VAB")));
      MainSystem.CreateIfNeeded(CommonUtil.CombinePaths(path, CommonUtil.CombinePaths("Ships", "SPH")));
      MainSystem.CreateIfNeeded(CommonUtil.CombinePaths(path, "Subassemblies"));
      MainSystem.CreateIfNeeded(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Flags"));
    }

    private static void CreateIfNeeded(string path)
    {
      if (Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
    }

    private static void SetupBlankGameIfNeeded()
    {
      if (File.Exists(CommonUtil.CombinePaths(MainSystem.KspPath, "saves", "LunaMultiplayer", "persistent.sfs")))
        return;
      LunaLog.Log("[LMP]: Creating new blank persistent.sfs file");
      Game blankGame = MainSystem.CreateBlankGame();
      HighLogic.SaveFolder = "LunaMultiplayer";
      GamePersistence.SaveGame(blankGame, "persistent", HighLogic.SaveFolder, (SaveMode) 0);
    }

    private static Game CreateBlankGame()
    {
      Game blankGame = new Game()
      {
        additionalSystems = new ConfigNode()
      };
      blankGame.additionalSystems.AddNode("MESSAGESYSTEM");
      blankGame.flightState = new FlightState();
      if (blankGame.flightState.mapViewFilterState == 0)
        blankGame.flightState.mapViewFilterState = -1026;
      blankGame.startScene = (GameScenes) 5;
      if (LmpClient.Base.System<FlagSystem>.Singleton.FlagExists(SettingsSystem.CurrentSettings.SelectedFlag))
      {
        blankGame.flagURL = SettingsSystem.CurrentSettings.SelectedFlag;
        LmpClient.Base.System<FlagSystem>.Singleton.SendFlag(SettingsSystem.CurrentSettings.SelectedFlag);
      }
      else
      {
        SettingsSystem.CurrentSettings.SelectedFlag = blankGame.flagURL = "Squad/Flags/default";
        SettingsSystem.SaveSettings();
      }
      blankGame.Title = "LunaMultiplayer";
      if (SettingsSystem.ServerSettings.WarpMode == WarpMode.Subspace)
      {
        blankGame.Parameters.Flight.CanQuickLoad = true;
        blankGame.Parameters.Flight.CanRestart = true;
        blankGame.Parameters.Flight.CanLeaveToEditor = true;
      }
      else
      {
        blankGame.Parameters.Flight.CanQuickLoad = false;
        blankGame.Parameters.Flight.CanRestart = false;
        blankGame.Parameters.Flight.CanLeaveToEditor = false;
      }
      HighLogic.SaveFolder = "LunaMultiplayer";
      return blankGame;
    }

    private static void FixBindingRedirects()
    {
      Dictionary<string, Assembly> dictionary = (Dictionary<string, Assembly>) typeof (AssemblyLoader).GetField("bindingRedirect", AccessTools.all)?.GetValue((object) null);
      if (dictionary == null)
        return;
      dictionary.Add("LmpClient.XmlSerializers, Culture=neutral, PublicKeyToken=null", Assembly.Load("System.Xml"));
      dictionary.Add("LmpCommon.XmlSerializers, Culture=neutral, PublicKeyToken=null", Assembly.Load("System.Xml"));
    }
  }
}
