// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.MessageStore
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LmpCommon.Message.Base
{
  public static class MessageStore
  {
    internal static ConcurrentDictionary<string, ConcurrentBag<IMessageData>> MessageDataDictionary = new ConcurrentDictionary<string, ConcurrentBag<IMessageData>>();
    internal static ConcurrentDictionary<string, ConcurrentBag<IMessageBase>> MessageDictionary = new ConcurrentDictionary<string, ConcurrentBag<IMessageBase>>();
    private static readonly ConcurrentDictionary<Type, ConstructorInfo> MessageDataConstructorDictionary = new ConcurrentDictionary<Type, ConstructorInfo>();
    private static readonly ConcurrentDictionary<Type, ConstructorInfo> MessageConstructorDictionary = new ConcurrentDictionary<Type, ConstructorInfo>();

    internal static void RecycleMessage(IMessageBase message)
    {
      ConcurrentBag<IMessageData> concurrentBag1;
      if (!MessageStore.MessageDataDictionary.TryGetValue(message.Data.ClassName, out concurrentBag1))
      {
        concurrentBag1 = new ConcurrentBag<IMessageData>();
        MessageStore.MessageDataDictionary.TryAdd(message.Data.ClassName, concurrentBag1);
      }
      concurrentBag1.Add(message.Data);
      message.SetData((IMessageData) null);
      ConcurrentBag<IMessageBase> concurrentBag2;
      if (!MessageStore.MessageDictionary.TryGetValue(message.ClassName, out concurrentBag2))
      {
        concurrentBag2 = new ConcurrentBag<IMessageBase>();
        MessageStore.MessageDictionary.TryAdd(message.ClassName, concurrentBag2);
      }
      concurrentBag2.Add(message);
    }

    internal static T GetMessageData<T>() where T : class, IMessageData
    {
      ConcurrentBag<IMessageData> concurrentBag;
      IMessageData result;
      return MessageStore.MessageDataDictionary.TryGetValue(typeof (T).Name, out concurrentBag) && concurrentBag.TryTake(out result) ? result as T : MessageStore.CreateNewInstance<T>();
    }

    internal static IMessageData GetMessageData(Type messageDataType)
    {
      ConcurrentBag<IMessageData> concurrentBag;
      IMessageData result;
      return MessageStore.MessageDataDictionary.TryGetValue(messageDataType.Name, out concurrentBag) && concurrentBag.TryTake(out result) ? result : MessageStore.CreateNewMessageDataInstance(messageDataType);
    }

    internal static T GetMessage<T>() where T : class, IMessageBase
    {
      ConcurrentBag<IMessageBase> concurrentBag;
      IMessageBase result;
      if (!MessageStore.MessageDictionary.TryGetValue(typeof (T).Name, out concurrentBag) || !concurrentBag.TryTake(out result))
        return MessageStore.CreateNewInstance<T>();
      result.SetData((IMessageData) null);
      return result as T;
    }

    internal static IMessageBase GetMessage(Type type)
    {
      ConcurrentBag<IMessageBase> concurrentBag;
      IMessageBase result;
      return MessageStore.MessageDictionary.TryGetValue(type.Name, out concurrentBag) && concurrentBag.TryTake(out result) ? result : MessageStore.CreateNewMessageInstance(type);
    }

    public static int GetMessageCount(Type type)
    {
      ConcurrentBag<IMessageBase> concurrentBag;
      return type == (Type) null ? MessageStore.MessageDictionary.SelectMany<KeyValuePair<string, ConcurrentBag<IMessageBase>>, IMessageBase>((Func<KeyValuePair<string, ConcurrentBag<IMessageBase>>, IEnumerable<IMessageBase>>) (v => (IEnumerable<IMessageBase>) v.Value)).Count<IMessageBase>() : (MessageStore.MessageDictionary.TryGetValue(type.Name, out concurrentBag) ? concurrentBag.Count : 0);
    }

    public static int GetMessageDataCount(Type type)
    {
      ConcurrentBag<IMessageData> concurrentBag;
      return type == (Type) null ? MessageStore.MessageDataDictionary.SelectMany<KeyValuePair<string, ConcurrentBag<IMessageData>>, IMessageData>((Func<KeyValuePair<string, ConcurrentBag<IMessageData>>, IEnumerable<IMessageData>>) (v => (IEnumerable<IMessageData>) v.Value)).Count<IMessageData>() : (MessageStore.MessageDataDictionary.TryGetValue(type.Name, out concurrentBag) ? concurrentBag.Count : 0);
    }

    private static T CreateNewInstance<T>() where T : class
    {
      if (typeof (IMessageData).IsAssignableFrom(typeof (T)))
      {
        ConstructorInfo constructorInfo;
        if (!MessageStore.MessageDataConstructorDictionary.TryGetValue(typeof (T), out constructorInfo))
        {
          constructorInfo = ((IEnumerable<ConstructorInfo>) typeof (T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).First<ConstructorInfo>();
          MessageStore.MessageDataConstructorDictionary.TryAdd(typeof (T), constructorInfo);
        }
        return constructorInfo.Invoke((object[]) null) as T;
      }
      if (!typeof (IMessageBase).IsAssignableFrom(typeof (T)))
        throw new Exception("Cannot implement this object!");
      ConstructorInfo constructorInfo1;
      if (!MessageStore.MessageConstructorDictionary.TryGetValue(typeof (T), out constructorInfo1))
      {
        constructorInfo1 = ((IEnumerable<ConstructorInfo>) typeof (T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).First<ConstructorInfo>();
        MessageStore.MessageConstructorDictionary.TryAdd(typeof (T), constructorInfo1);
      }
      return constructorInfo1.Invoke((object[]) null) as T;
    }

    private static IMessageBase CreateNewMessageInstance(Type type)
    {
      if (!typeof (IMessageBase).IsAssignableFrom(type))
        throw new Exception("Cannot implement this object!");
      ConstructorInfo constructorInfo;
      if (!MessageStore.MessageConstructorDictionary.TryGetValue(type, out constructorInfo))
      {
        constructorInfo = ((IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).First<ConstructorInfo>();
        MessageStore.MessageConstructorDictionary.TryAdd(type, constructorInfo);
      }
      return constructorInfo.Invoke((object[]) null) as IMessageBase;
    }

    private static IMessageData CreateNewMessageDataInstance(Type type)
    {
      if (!typeof (IMessageData).IsAssignableFrom(type))
        throw new Exception("Cannot implement this object!");
      ConstructorInfo constructorInfo;
      if (!MessageStore.MessageDataConstructorDictionary.TryGetValue(type, out constructorInfo))
      {
        constructorInfo = ((IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).First<ConstructorInfo>();
        MessageStore.MessageDataConstructorDictionary.TryAdd(type, constructorInfo);
      }
      return constructorInfo.Invoke((object[]) null) as IMessageData;
    }
  }
}
