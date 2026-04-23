// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Scenario.ScenarioDataMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Scenario
{
  public class ScenarioDataMsgData : ScenarioBaseMsgData
  {
    public int ScenarioCount;
    public ScenarioInfo[] ScenariosData = new ScenarioInfo[0];

    internal ScenarioDataMsgData()
    {
    }

    public override ScenarioMessageType ScenarioMessageType => ScenarioMessageType.Data;

    public override string ClassName { get; } = nameof (ScenarioDataMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.ScenarioCount);
      for (int index = 0; index < this.ScenarioCount; ++index)
        this.ScenariosData[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ScenarioCount = lidgrenMsg.ReadInt32();
      if (this.ScenariosData.Length < this.ScenarioCount)
        this.ScenariosData = new ScenarioInfo[this.ScenarioCount];
      for (int index = 0; index < this.ScenarioCount; ++index)
      {
        if (this.ScenariosData[index] == null)
          this.ScenariosData[index] = new ScenarioInfo();
        this.ScenariosData[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.ScenarioCount; ++index)
        num += this.ScenariosData[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
