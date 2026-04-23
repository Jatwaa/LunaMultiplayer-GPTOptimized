// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Facility.FacilityCollapseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Facility
{
  public class FacilityCollapseMsgData : FacilityBaseMsgData
  {
    internal FacilityCollapseMsgData()
    {
    }

    public override FacilityMessageType FacilityMessageType => FacilityMessageType.Collapse;

    public override string ClassName { get; } = nameof (FacilityCollapseMsgData);
  }
}
