// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Scenario.ScenarioBaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Scenario
{
  public abstract class ScenarioBaseMsgData : MessageData
  {
    internal ScenarioBaseMsgData()
    {
    }

    public override ushort SubType => (ushort) this.ScenarioMessageType;

    public virtual ScenarioMessageType ScenarioMessageType => throw new NotImplementedException();

    public override string ClassName { get; } = nameof (ScenarioBaseMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
    }

    internal override int InternalGetMessageSize() => 0;
  }
}
