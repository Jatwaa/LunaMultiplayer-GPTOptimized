// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.ConfigNodeSerializer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LmpClient.Extensions
{
  public static class ConfigNodeSerializer
  {
    static ConfigNodeSerializer()
    {
      Type type = typeof (ConfigNode);
      MethodInfo method1 = type.GetMethod("WriteNode", BindingFlags.Instance | BindingFlags.NonPublic);
      if (method1 == (MethodInfo) null)
        return;
      ConfigNodeSerializer.WriteNodeThunk = (ConfigNodeSerializer.WriteNodeDelegate) Delegate.CreateDelegate(typeof (ConfigNodeSerializer.WriteNodeDelegate), (object) null, method1);
      MethodInfo method2 = type.GetMethod("PreFormatConfig", BindingFlags.Static | BindingFlags.NonPublic);
      if (method2 == (MethodInfo) null)
        return;
      ConfigNodeSerializer.PreFormatConfigThunk = (ConfigNodeSerializer.PreFormatConfigDelegate) Delegate.CreateDelegate(typeof (ConfigNodeSerializer.PreFormatConfigDelegate), (object) null, method2);
      MethodInfo method3 = type.GetMethod("RecurseFormat", BindingFlags.Static | BindingFlags.NonPublic, (Binder) null, new Type[1]
      {
        typeof (List<string[]>)
      }, (ParameterModifier[]) null);
      if (method3 == (MethodInfo) null)
        return;
      ConfigNodeSerializer.RecurseFormatThunk = (ConfigNodeSerializer.RecurseFormatDelegate) Delegate.CreateDelegate(typeof (ConfigNodeSerializer.RecurseFormatDelegate), (object) null, method3);
    }

    private static ConfigNodeSerializer.WriteNodeDelegate WriteNodeThunk { get; }

    private static ConfigNodeSerializer.PreFormatConfigDelegate PreFormatConfigThunk { get; }

    private static ConfigNodeSerializer.RecurseFormatDelegate RecurseFormatThunk { get; }

    public static byte[] Serialize(this ConfigNode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter writer = new StreamWriter((Stream) memoryStream))
        {
          ConfigNodeSerializer.WriteNodeThunk(node, writer);
          return memoryStream.ToArray();
        }
      }
    }

    public static void SerializeToArray(this ConfigNode node, byte[] data, out int numBytes)
    {
      try
      {
        if (node == null)
          throw new ArgumentNullException(nameof (node));
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
          using (StreamWriter writer = new StreamWriter((Stream) memoryStream))
          {
            ConfigNodeSerializer.WriteNodeThunk(node, writer);
            numBytes = (int) memoryStream.Position;
          }
        }
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error serializing vessel! Details {0}", (object) ex));
        numBytes = 0;
      }
    }

    public static ConfigNode DeserializeToConfigNode(this byte[] data, int numBytes)
    {
      if (data == null || data.Length == 0 || ((IEnumerable<byte>) data).All<byte>((Func<byte, bool>) (b => b == (byte) 0)))
        return (ConfigNode) null;
      using (MemoryStream memoryStream = new MemoryStream(data, 0, numBytes))
      {
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
        {
          List<string> stringList = new List<string>();
          while (!streamReader.EndOfStream)
            stringList.Add(streamReader.ReadLine());
          return ConfigNodeSerializer.RecurseFormatThunk(ConfigNodeSerializer.PreFormatConfigThunk(stringList.ToArray()));
        }
      }
    }

    private delegate void WriteNodeDelegate(ConfigNode configNode, StreamWriter writer);

    private delegate List<string[]> PreFormatConfigDelegate(string[] cfgData);

    private delegate ConfigNode RecurseFormatDelegate(List<string[]> cfg);
  }
}
