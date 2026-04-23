// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Scenario.ScenarioSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Expansions;
using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.Scenario
{
  public class ScenarioSystem : 
    MessageSystem<ScenarioSystem, ScenarioMessageSender, ScenarioMessageHandler>
  {
    private static readonly ConcurrentDictionary<string, Type> _allScenarioTypesInAssemblies = new ConcurrentDictionary<string, Type>();
    private static readonly List<Tuple<string, ConfigNode>> ScenariosConfigNodes = new List<Tuple<string, ConfigNode>>();

    private ConcurrentDictionary<string, string> CheckData { get; } = new ConcurrentDictionary<string, string>();

    public ConcurrentQueue<ScenarioEntry> ScenarioQueue { get; private set; } = new ConcurrentQueue<ScenarioEntry>();

    private static ConcurrentDictionary<string, Type> AllScenarioTypesInAssemblies
    {
      get
      {
        if (!Enumerable.Any<KeyValuePair<string, Type>>((IEnumerable<KeyValuePair<string, Type>>) ScenarioSystem._allScenarioTypesInAssemblies))
        {
          foreach (Type type in Enumerable.Where<Type>(Enumerable.SelectMany<AssemblyLoader.LoadedAssembly, Type>((IEnumerable<AssemblyLoader.LoadedAssembly>) AssemblyLoader.loadedAssemblies, (Func<AssemblyLoader.LoadedAssembly, IEnumerable<Type>>) (a => a.assembly.GetLoadableTypes())), (Func<Type, bool>) (s => s.IsSubclassOf(typeof (ScenarioModule)) && !ScenarioSystem._allScenarioTypesInAssemblies.ContainsKey(s.Name))))
            ScenarioSystem._allScenarioTypesInAssemblies.TryAdd(type.Name, type);
        }
        return ScenarioSystem._allScenarioTypesInAssemblies;
      }
    }

    private static List<string> ScenarioName { get; } = new List<string>();

    private static List<byte[]> ScenarioData { get; } = new List<byte[]>();

    public override string SystemName { get; } = nameof (ScenarioSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(30000, RoutineExecution.Update, new Action(this.SendScenarioModules)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.CheckData.Clear();
      this.ScenarioQueue = new ConcurrentQueue<ScenarioEntry>();
      ScenarioSystem.AllScenarioTypesInAssemblies.Clear();
    }

    public void LoadMissingScenarioDataIntoGame()
    {
      foreach (KSPScenarioType kspScenarioType in Enumerable.Where<KSPScenarioType>((IEnumerable<KSPScenarioType>) KSPScenarioType.GetAllScenarioTypesInAssemblies(), (Func<KSPScenarioType, bool>) (s => !HighLogic.CurrentGame.scenarios.Exists((Predicate<ProtoScenarioModule>) (psm => psm.moduleName == s.ModuleType.Name)) && ScenarioSystem.LoadModuleByGameMode(s) && ScenarioSystem.IsDlcScenarioInstalled(s.ModuleType.Name))))
      {
        LunaLog.Log("[LMP]: Creating new scenario module " + kspScenarioType.ModuleType.Name);
        HighLogic.CurrentGame.AddProtoScenarioModule(kspScenarioType.ModuleType, kspScenarioType.ScenarioAttributes.TargetScenes);
      }
    }

    public void SendScenarioModules()
    {
      if (!this.Enabled)
        return;
      try
      {
        ScenarioSystem.ParseModulesToConfigNodes(Enumerable.Where<ScenarioModule>((IEnumerable<ScenarioModule>) ScenarioRunner.GetLoadedModules(), (Func<ScenarioModule, bool>) (s => Object.op_Inequality((Object) s, (Object) null))));
        SystemBase.TaskFactory.StartNew(new Action(this.SendModulesConfigNodes));
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error while trying to send the scenario modules!. Details {0}", (object) ex));
      }
    }

    private static void ParseModulesToConfigNodes(IEnumerable<ScenarioModule> modules)
    {
      ScenarioSystem.ScenariosConfigNodes.Clear();
      foreach (ScenarioModule module in modules)
      {
        string name = ((object) module).GetType().Name;
        if (!IgnoredScenarios.IgnoreSend.Contains(name) && ScenarioSystem.IsScenarioModuleAllowed(name))
        {
          ConfigNode configNode = new ConfigNode();
          module.Save(configNode);
          ScenarioSystem.ScenariosConfigNodes.Add(new Tuple<string, ConfigNode>(name, configNode));
        }
      }
    }

    private void SendModulesConfigNodes()
    {
      ScenarioSystem.ScenarioData.Clear();
      ScenarioSystem.ScenarioName.Clear();
      foreach (Tuple<string, ConfigNode> scenariosConfigNode in ScenarioSystem.ScenariosConfigNodes)
      {
        byte[] data = scenariosConfigNode.Item2.Serialize();
        string sha256Hash = Common.CalculateSha256Hash(data);
        if (data.Length == 0)
          LunaLog.Log("[LMP]: Error writing scenario data for " + scenariosConfigNode.Item1);
        else if (!this.CheckData.ContainsKey(scenariosConfigNode.Item1) || !(this.CheckData[scenariosConfigNode.Item1] == sha256Hash))
        {
          this.CheckData[scenariosConfigNode.Item1] = sha256Hash;
          ScenarioSystem.ScenarioName.Add(scenariosConfigNode.Item1);
          ScenarioSystem.ScenarioData.Add(data);
        }
      }
      if (!Enumerable.Any<string>((IEnumerable<string>) ScenarioSystem.ScenarioName))
        return;
      this.MessageSender.SendScenarioModuleData(ScenarioSystem.ScenarioName, ScenarioSystem.ScenarioData);
    }

    public void LoadScenarioDataIntoGame()
    {
      ScenarioEntry result;
      while (this.ScenarioQueue.TryDequeue(out result))
      {
        ProtoScenarioModule protoScenarioModule = new ProtoScenarioModule(result.ScenarioNode);
        if (ScenarioSystem.IsScenarioModuleAllowed(protoScenarioModule.moduleName) && !IgnoredScenarios.IgnoreReceive.Contains(protoScenarioModule.moduleName))
        {
          LunaLog.Log("[LMP]: Loading " + protoScenarioModule.moduleName + " scenario data");
          HighLogic.CurrentGame.scenarios.Add(protoScenarioModule);
        }
        else
          LunaLog.Log(string.Format("[LMP]: Skipping {0} scenario data in {1} mode", (object) protoScenarioModule.moduleName, (object) SettingsSystem.ServerSettings.GameMode));
      }
    }

    private static bool LoadModuleByGameMode(KSPScenarioType validScenario)
    {
      switch ((int) HighLogic.CurrentGame.Mode)
      {
        case 0:
          return validScenario.ScenarioAttributes.HasCreateOption((ScenarioCreationOptions) 2);
        case 1:
          return validScenario.ScenarioAttributes.HasCreateOption((ScenarioCreationOptions) 32);
        case 4:
          return validScenario.ScenarioAttributes.HasCreateOption((ScenarioCreationOptions) 8);
        default:
          return false;
      }
    }

    private static bool IsDlcScenarioInstalled(string scenarioName) => !(scenarioName == "DeployedScience") || ExpansionsLoader.IsExpansionInstalled("Serenity");

    private static bool IsScenarioModuleAllowed(string scenarioName)
    {
      if (string.IsNullOrEmpty(scenarioName) || scenarioName == "DeployedScience" && !ExpansionsLoader.IsExpansionInstalled("Serenity") || !ScenarioSystem.IsDlcScenarioInstalled(scenarioName) || !ScenarioSystem.AllScenarioTypesInAssemblies.ContainsKey(scenarioName))
        return false;
      KSPScenario[] customAttributes = (KSPScenario[]) ScenarioSystem.AllScenarioTypesInAssemblies[scenarioName].GetCustomAttributes(typeof (KSPScenario), true);
      if ((uint) customAttributes.Length <= 0U)
        return true;
      KSPScenario kspScenario = customAttributes[0];
      bool flag = false;
      if (HighLogic.CurrentGame.Mode == 1)
        flag = kspScenario.HasCreateOption((ScenarioCreationOptions) 64) | kspScenario.HasCreateOption((ScenarioCreationOptions) 32);
      if (HighLogic.CurrentGame.Mode == 4)
        flag = flag | kspScenario.HasCreateOption((ScenarioCreationOptions) 16) | kspScenario.HasCreateOption((ScenarioCreationOptions) 8);
      if (HighLogic.CurrentGame.Mode == 0)
        flag = flag | kspScenario.HasCreateOption((ScenarioCreationOptions) 4) | kspScenario.HasCreateOption((ScenarioCreationOptions) 2);
      return flag;
    }
  }
}
