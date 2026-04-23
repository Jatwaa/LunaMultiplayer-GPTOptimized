// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselResourceMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselResourceMsgData : VesselBaseMsgData
  {
    public int ResourcesCount;
    public VesselResourceInfo[] Resources = new VesselResourceInfo[0];

    internal VesselResourceMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Resource;

    public override string ClassName { get; } = nameof (VesselResourceMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.ResourcesCount);
      for (int index = 0; index < this.ResourcesCount; ++index)
        this.Resources[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ResourcesCount = lidgrenMsg.ReadInt32();
      if (this.Resources.Length < this.ResourcesCount)
        this.Resources = new VesselResourceInfo[this.ResourcesCount];
      for (int index = 0; index < this.ResourcesCount; ++index)
      {
        if (this.Resources[index] == null)
          this.Resources[index] = new VesselResourceInfo();
        this.Resources[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num1 = 0;
      for (int index = 0; index < this.ResourcesCount; ++index)
      {
        int num2 = num1;
        VesselResourceInfo resource = this.Resources[index];
        int num3 = resource != null ? resource.GetByteCount() : 0;
        num1 = num2 + num3;
      }
      return base.InternalGetMessageSize() + 4 + num1;
    }
  }
}
