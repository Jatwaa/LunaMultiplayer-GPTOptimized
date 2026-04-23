// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Types.VesselMessageType
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpCommon.Message.Types
{
  public enum VesselMessageType
  {
    Proto,
    Remove,
    Position,
    Flightstate,
    Update,
    Resource,
    Sync,
    PartSyncField,
    PartSyncUiField,
    PartSyncCall,
    ActionGroup,
    Fairing,
    Decouple,
    Couple,
    Undock,
  }
}
