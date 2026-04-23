// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.FactoryBase
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LmpCommon.Message.Base
{
  public abstract class FactoryBase
  {
    private readonly Dictionary<uint, Type> _messageDictionary = new Dictionary<uint, Type>();

    protected FactoryBase()
    {
      foreach (Type type in ((IEnumerable<Type>) Assembly.GetExecutingAssembly().GetTypes()).Where<Type>((Func<Type, bool>) (t => t.IsClass && t.BaseType != (Type) null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == this.BaseMsgType)).ToArray<Type>())
      {
        ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
        object obj = !(constructor == (ConstructorInfo) null) ? constructor.Invoke((object[]) null) : throw new Exception("Message type " + type.FullName + " must have an internal parameter-less constructor");
        PropertyInfo property = type.GetProperty("MessageType", BindingFlags.Instance | BindingFlags.Public);
        if (property == (PropertyInfo) null)
          throw new Exception("Message type " + type.FullName + " must implement the MessageType property (uint)");
        this._messageDictionary.Add((uint) (int) property.GetValue(obj, (object[]) null), type);
      }
    }

    protected internal abstract Type BaseMsgType { get; }

    public IMessageBase Deserialize(NetIncomingMessage lidgrenMsg, long receiveTime)
    {
      ushort messageType = lidgrenMsg.LengthBytes >= 0 ? lidgrenMsg.ReadUInt16() : throw new Exception("Incorrect message length");
      ushort subType = lidgrenMsg.ReadUInt16();
      lidgrenMsg.SkipPadBits();
      IMessageBase messageByType = this.GetMessageByType(messageType);
      IMessageData messageData = messageByType.GetMessageData(subType);
      messageData.Deserialize(lidgrenMsg);
      messageByType.SetData(messageData);
      messageByType.Data.ReceiveTime = receiveTime;
      messageByType.VersionMismatch = !LmpVersioning.IsCompatible((int) messageByType.Data.MajorVersion, (int) messageByType.Data.MinorVersion, (int) messageByType.Data.BuildVersion);
      return messageByType;
    }

    protected internal abstract Type HandledMessageTypes { get; }

    public T CreateNew<T, TD>()
      where T : class, IMessageBase
      where TD : class, IMessageData
    {
      T message = MessageStore.GetMessage<T>();
      TD messageData = MessageStore.GetMessageData<TD>();
      message.SetData((IMessageData) messageData);
      return message;
    }

    public T CreateNew<T>(IMessageData data) where T : class, IMessageBase
    {
      T message = MessageStore.GetMessage<T>();
      message.SetData(data);
      return message;
    }

    public T CreateNewMessageData<T>() where T : class, IMessageData => MessageStore.GetMessageData<T>();

    private IMessageBase GetMessageByType(ushort messageType)
    {
      if (Enum.IsDefined(this.HandledMessageTypes, (object) (int) messageType) && this._messageDictionary.ContainsKey((uint) messageType))
        return MessageStore.GetMessage(this._messageDictionary[(uint) messageType]);
      throw new Exception("Cannot deserialize this type of message!");
    }
  }
}
