// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.StringUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Text;

namespace LmpCommon.Message.Base
{
  public static class StringUtil
  {
    public static int GetByteCount(this string[] array, int length)
    {
      int byteCount = 0;
      for (int index = 0; index < length; ++index)
        byteCount += array[index].GetByteCount();
      return byteCount;
    }

    public static int GetByteCount(this string stringToCheck) => string.IsNullOrEmpty(stringToCheck) ? 4 : Encoding.UTF8.GetByteCount(stringToCheck) + 4;
  }
}
