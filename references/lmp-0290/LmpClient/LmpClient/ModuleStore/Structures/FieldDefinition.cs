// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Structures.FieldDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Xml;

namespace LmpClient.ModuleStore.Structures
{
  public class FieldDefinition
  {
    [XmlComment(Value = "Name of the field that we are customizing")]
    public string FieldName { get; set; }

    [XmlComment(Value = "Max interval to sync this field")]
    public int MaxIntervalInMs { get; set; }
  }
}
