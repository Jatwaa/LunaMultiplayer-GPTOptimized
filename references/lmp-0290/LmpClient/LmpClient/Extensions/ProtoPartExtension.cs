// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.ProtoPartExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Extensions
{
  public static class ProtoPartExtension
  {
    public static ProtoPartResourceSnapshot FindResourceInProtoPart(
      this ProtoPartSnapshot protoPart,
      string resourceName)
    {
      if (protoPart == null)
        return (ProtoPartResourceSnapshot) null;
      for (int index = 0; index < protoPart.resources.Count; ++index)
      {
        if (protoPart.resources[index].resourceName == resourceName)
          return protoPart.resources[index];
      }
      return (ProtoPartResourceSnapshot) null;
    }

    public static ProtoPartModuleSnapshot FindProtoPartModuleInProtoPart(
      this ProtoPartSnapshot part,
      string moduleName)
    {
      if (part == null)
        return (ProtoPartModuleSnapshot) null;
      for (int index = 0; index < part.modules.Count; ++index)
      {
        if (part.modules[index].moduleName == moduleName)
          return part.modules[index];
      }
      return (ProtoPartModuleSnapshot) null;
    }
  }
}
