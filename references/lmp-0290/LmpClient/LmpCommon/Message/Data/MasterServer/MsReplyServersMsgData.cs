// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.MasterServer.MsReplyServersMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System.Net;

namespace LmpCommon.Message.Data.MasterServer
{
  public class MsReplyServersMsgData : MsBaseMsgData
  {
    public long Id;
    public string ServerVersion;
    public IPEndPoint InternalEndpoint;
    public IPEndPoint InternalEndpoint6;
    public IPEndPoint ExternalEndpoint;
    public bool Password;
    public bool Cheats;
    public bool ModControl;
    public bool DedicatedServer;
    public bool RainbowEffect;
    public byte[] Color = new byte[3];
    public int GameMode;
    public int MaxPlayers;
    public int PlayerCount;
    public string ServerName;
    public string Description;
    public string Country;
    public string Website;
    public string WebsiteText;
    public int WarpMode;
    public int TerrainQuality;
    public int VesselUpdatesSendMsInterval;

    internal MsReplyServersMsgData()
    {
    }

    public override MasterServerMessageSubType MasterServerMessageSubType => MasterServerMessageSubType.ReplyServers;

    public override string ClassName { get; } = nameof (MsReplyServersMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.Id);
      lidgrenMsg.Write(this.ServerVersion);
      lidgrenMsg.Write(this.InternalEndpoint);
      lidgrenMsg.Write(this.InternalEndpoint6);
      lidgrenMsg.Write(this.ExternalEndpoint);
      lidgrenMsg.Write(this.Password);
      lidgrenMsg.Write(this.Cheats);
      lidgrenMsg.Write(this.ModControl);
      lidgrenMsg.Write(this.DedicatedServer);
      lidgrenMsg.Write(this.RainbowEffect);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.Color[index]);
      lidgrenMsg.Write(this.GameMode);
      lidgrenMsg.Write(this.MaxPlayers);
      lidgrenMsg.Write(this.PlayerCount);
      lidgrenMsg.Write(this.ServerName);
      lidgrenMsg.Write(this.Description);
      lidgrenMsg.Write(this.Country);
      lidgrenMsg.Write(this.Website);
      lidgrenMsg.Write(this.WebsiteText);
      lidgrenMsg.Write(this.WarpMode);
      lidgrenMsg.Write(this.TerrainQuality);
      lidgrenMsg.Write(this.VesselUpdatesSendMsInterval);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Id = lidgrenMsg.ReadInt64();
      this.ServerVersion = lidgrenMsg.ReadString();
      this.InternalEndpoint = lidgrenMsg.ReadIPEndPoint();
      this.InternalEndpoint6 = lidgrenMsg.ReadIPEndPoint();
      this.ExternalEndpoint = lidgrenMsg.ReadIPEndPoint();
      this.Password = lidgrenMsg.ReadBoolean();
      this.Cheats = lidgrenMsg.ReadBoolean();
      this.ModControl = lidgrenMsg.ReadBoolean();
      this.DedicatedServer = lidgrenMsg.ReadBoolean();
      this.RainbowEffect = lidgrenMsg.ReadBoolean();
      for (int index = 0; index < 3; ++index)
        this.Color[index] = lidgrenMsg.ReadByte();
      this.GameMode = lidgrenMsg.ReadInt32();
      this.MaxPlayers = lidgrenMsg.ReadInt32();
      this.PlayerCount = lidgrenMsg.ReadInt32();
      this.ServerName = lidgrenMsg.ReadString();
      this.Description = lidgrenMsg.ReadString();
      this.Country = lidgrenMsg.ReadString();
      this.Website = lidgrenMsg.ReadString();
      this.WebsiteText = lidgrenMsg.ReadString();
      this.WarpMode = lidgrenMsg.ReadInt32();
      this.TerrainQuality = lidgrenMsg.ReadInt32();
      this.VesselUpdatesSendMsInterval = lidgrenMsg.ReadInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 8 + this.ServerVersion.GetByteCount() + this.InternalEndpoint.GetByteCount() + this.InternalEndpoint6.GetByteCount() + this.ExternalEndpoint.GetByteCount() + this.ServerName.GetByteCount() + this.Description.GetByteCount() + this.Country.GetByteCount() + this.Website.GetByteCount() + this.WebsiteText.GetByteCount() + 5 + 24 + 3;
  }
}
