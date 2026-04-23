// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.MessageBase`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Base
{
  public abstract class MessageBase<T> : IMessageBase where T : class, IMessageData
  {
    private IMessageData _data;

    internal MessageBase()
    {
    }

    protected virtual Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (T)
    };

    protected abstract ushort MessageTypeId { get; }

    protected abstract int DefaultChannel { get; }

    public void SetData(IMessageData data) => this.Data = data;

    public abstract string ClassName { get; }

    public IMessageData Data
    {
      get => this._data;
      private set
      {
        int num;
        switch (value)
        {
          case null:
          case T _:
            num = 0;
            break;
          default:
            num = typeof (T) != value.GetType() ? 1 : 0;
            break;
        }
        if (num != 0)
          throw new InvalidOperationException("Cannot cast this mesage data to this type of message");
        this._data = value;
      }
    }

    public int Channel
    {
      get
      {
        if (this.NetDeliveryMethod == NetDeliveryMethod.Unreliable || this.NetDeliveryMethod == NetDeliveryMethod.ReliableUnordered)
          return 0;
        return this.DefaultChannel <= 32 ? this.DefaultChannel : throw new Exception("Cannot set a channel above 32!");
      }
    }

    public abstract NetDeliveryMethod NetDeliveryMethod { get; }

    public virtual IMessageData GetMessageData(ushort subType) => this.SubTypeDictionary.ContainsKey(subType) ? MessageStore.GetMessageData(this.SubTypeDictionary[subType]) : throw new Exception("Subtype not defined in dictionary!");

    public bool VersionMismatch { get; set; }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      try
      {
        lidgrenMsg.Write(this.MessageTypeId);
        lidgrenMsg.Write(this.Data.SubType);
        lidgrenMsg.WritePadBits();
        this.Data.Serialize(lidgrenMsg);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Error serializing message! MsgDataType: {0} Exception: {1}", (object) this.Data.GetType(), (object) ex));
      }
    }

    public void Recycle() => MessageStore.RecycleMessage((IMessageBase) this);

    public int GetMessageSize() => 4 + this.Data.GetMessageSize();
  }
}
