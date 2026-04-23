// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Structures.MethodDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Xml;

namespace LmpClient.ModuleStore.Structures
{
  public class MethodDefinition
  {
    [XmlComment(Value = "Name of the method that we are customizing")]
    public string MethodName { get; set; }

    [XmlComment(Value = "Max interval to sync this method call")]
    public int MaxIntervalInMs { get; set; }
  }
}
