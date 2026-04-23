// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ShareProgressContractsMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ShareProgressContractsMsgData : ShareProgressBaseMsgData
  {
    public int ContractCount;
    public ContractInfo[] Contracts = new ContractInfo[0];

    internal ShareProgressContractsMsgData()
    {
    }

    public override ShareProgressMessageType ShareProgressMessageType => ShareProgressMessageType.ContractsUpdate;

    public override string ClassName { get; } = nameof (ShareProgressContractsMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.ContractCount);
      for (int index = 0; index < this.ContractCount; ++index)
        this.Contracts[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ContractCount = lidgrenMsg.ReadInt32();
      if (this.Contracts.Length < this.ContractCount)
        this.Contracts = new ContractInfo[this.ContractCount];
      for (int index = 0; index < this.ContractCount; ++index)
      {
        if (this.Contracts[index] == null)
          this.Contracts[index] = new ContractInfo();
        this.Contracts[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.ContractCount; ++index)
        num += this.Contracts[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
