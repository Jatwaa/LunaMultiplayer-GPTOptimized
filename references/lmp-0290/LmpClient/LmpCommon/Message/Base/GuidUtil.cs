// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.GuidUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace LmpCommon.Message.Base
{
  public static class GuidUtil
  {
    private static readonly ConcurrentBag<byte[]> ArrayPool = new ConcurrentBag<byte[]>();
    private static readonly FieldInfo _a = typeof (Guid).GetField(nameof (_a), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _b = typeof (Guid).GetField(nameof (_b), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _c = typeof (Guid).GetField(nameof (_c), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _d = typeof (Guid).GetField(nameof (_d), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _e = typeof (Guid).GetField(nameof (_e), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _f = typeof (Guid).GetField(nameof (_f), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _g = typeof (Guid).GetField(nameof (_g), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _h = typeof (Guid).GetField(nameof (_h), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _i = typeof (Guid).GetField(nameof (_i), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _j = typeof (Guid).GetField(nameof (_j), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _k = typeof (Guid).GetField(nameof (_k), BindingFlags.Instance | BindingFlags.NonPublic);

    public static int ByteSize => 16;

    public static void Serialize(Guid guid, NetOutgoingMessage lidgrenMsg)
    {
      byte[] result;
      if (!GuidUtil.ArrayPool.TryTake(out result))
        result = new byte[16];
      result[0] = (byte) (int) GuidUtil._a.GetValue((object) guid);
      result[1] = (byte) ((int) GuidUtil._a.GetValue((object) guid) >> 8);
      result[2] = (byte) ((int) GuidUtil._a.GetValue((object) guid) >> 16);
      result[3] = (byte) ((int) GuidUtil._a.GetValue((object) guid) >> 24);
      result[4] = (byte) (short) GuidUtil._b.GetValue((object) guid);
      result[5] = (byte) ((uint) (short) GuidUtil._b.GetValue((object) guid) >> 8);
      result[6] = (byte) (short) GuidUtil._c.GetValue((object) guid);
      result[7] = (byte) ((uint) (short) GuidUtil._c.GetValue((object) guid) >> 8);
      result[8] = (byte) GuidUtil._d.GetValue((object) guid);
      result[9] = (byte) GuidUtil._e.GetValue((object) guid);
      result[10] = (byte) GuidUtil._f.GetValue((object) guid);
      result[11] = (byte) GuidUtil._g.GetValue((object) guid);
      result[12] = (byte) GuidUtil._h.GetValue((object) guid);
      result[13] = (byte) GuidUtil._i.GetValue((object) guid);
      result[14] = (byte) GuidUtil._j.GetValue((object) guid);
      result[15] = (byte) GuidUtil._k.GetValue((object) guid);
      lidgrenMsg.Write(result, 0, 16);
      GuidUtil.ArrayPool.Add(result);
    }

    public static Guid Deserialize(NetIncomingMessage lidgrenMsg)
    {
      byte[] result;
      if (!GuidUtil.ArrayPool.TryTake(out result))
        result = new byte[16];
      lidgrenMsg.ReadBytes(result, 0, 16);
      Guid guid = new Guid(result);
      GuidUtil.ArrayPool.Add(result);
      return guid;
    }
  }
}
