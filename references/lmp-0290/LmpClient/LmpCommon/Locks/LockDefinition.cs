// Decompiled with JetBrains decompiler
// Type: LmpCommon.Locks.LockDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Locks
{
  public class LockDefinition : IEquatable<LockDefinition>, ICloneable
  {
    public string PlayerName { get; internal set; } = string.Empty;

    public string KerbalName { get; private set; } = string.Empty;

    public Guid VesselId { get; private set; } = Guid.Empty;

    public LockType Type { get; private set; }

    internal LockDefinition()
    {
    }

    public LockDefinition(LockType type, string playerName)
    {
      this.Type = type == LockType.Contract || type == LockType.AsteroidComet || type == LockType.Spectator ? type : throw new Exception("This constructor is only for Contract/Asteroid/Spectator type!");
      this.PlayerName = playerName;
    }

    public LockDefinition(LockType type, string playerName, Guid vesselId)
    {
      this.Type = type == LockType.Control || type == LockType.Update || type == LockType.UnloadedUpdate ? type : throw new Exception("This constructor is only for Control/Update/UnlUpdate type!");
      this.PlayerName = playerName;
      this.VesselId = vesselId;
    }

    public LockDefinition(LockType type, string playerName, string kerbalName)
    {
      this.Type = type == LockType.Kerbal ? type : throw new Exception("This constructor is only for kerbal type!");
      this.PlayerName = playerName;
      this.KerbalName = kerbalName;
    }

    public override string ToString() => this.VesselId != Guid.Empty ? string.Format("{0} - {1} - {2}", (object) this.Type, (object) this.VesselId, (object) this.PlayerName) : (!string.IsNullOrEmpty(this.KerbalName) ? string.Format("{0} - {1} - {2}", (object) this.Type, (object) this.KerbalName, (object) this.PlayerName) : string.Format("{0} - {1}", (object) this.Type, (object) this.PlayerName));

    public object Clone() => (object) new LockDefinition()
    {
      KerbalName = (this.KerbalName.Clone() as string),
      PlayerName = (this.PlayerName.Clone() as string),
      VesselId = this.VesselId,
      Type = this.Type
    };

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.PlayerName);
      lidgrenMsg.Write(this.KerbalName);
      GuidUtil.Serialize(this.VesselId, lidgrenMsg);
      lidgrenMsg.Write((int) this.Type);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.PlayerName = lidgrenMsg.ReadString();
      this.KerbalName = lidgrenMsg.ReadString();
      this.VesselId = GuidUtil.Deserialize(lidgrenMsg);
      this.Type = (LockType) lidgrenMsg.ReadInt32();
    }

    public int GetByteCount() => this.PlayerName.GetByteCount() + this.KerbalName.GetByteCount() + GuidUtil.ByteSize + 4;

    public bool Equals(LockDefinition other) => !(other == (LockDefinition) null) && this.PlayerName == other.PlayerName && this.VesselId == other.VesselId && this.Type == other.Type && this.KerbalName == other.KerbalName;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      LockDefinition other = obj as LockDefinition;
      return other != (LockDefinition) null && this.Equals(other);
    }

    public override int GetHashCode() => (((-423896247 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PlayerName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.KerbalName)) * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(this.VesselId)) * -1521134295 + this.Type.GetHashCode();

    public static bool operator ==(LockDefinition lock1, LockDefinition lock2) => (object) lock1 == null || (object) lock2 == null ? object.Equals((object) lock1, (object) lock2) : lock1.Equals(lock2);

    public static bool operator !=(LockDefinition lock1, LockDefinition lock2) => (object) lock1 == null || (object) lock2 == null ? !object.Equals((object) lock1, (object) lock2) : !lock1.Equals(lock2);
  }
}
