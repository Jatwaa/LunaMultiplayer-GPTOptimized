// Decompiled with JetBrains decompiler
// Type: LmpCommon.ContentChecker
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.IO;
using System.Text;

namespace LmpCommon
{
  public static class ContentChecker
  {
    private const int BytesToRead = 8;

    public static bool ContentsAreEqual(string contents, string pathToFile)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(contents);
      return ContentChecker.ContentsAreEqual(bytes, bytes.Length, pathToFile);
    }

    public static bool ContentsAreEqual(byte[] contents, int numBytes, string pathToFile)
    {
      if (!File.Exists(pathToFile))
        return false;
      FileInfo fileInfo = new FileInfo(pathToFile);
      if ((long) numBytes != fileInfo.Length)
        return false;
      int num = (int) Math.Ceiling((double) numBytes / 8.0);
      using (MemoryStream memoryStream = new MemoryStream(contents, 0, numBytes))
      {
        using (FileStream fileStream = File.OpenRead(pathToFile))
        {
          byte[] buffer1 = new byte[8];
          byte[] buffer2 = new byte[8];
          for (int index = 0; index < num; ++index)
          {
            memoryStream.Read(buffer1, 0, 8);
            fileStream.Read(buffer2, 0, 8);
            if (BitConverter.ToInt64(buffer1, 0) != BitConverter.ToInt64(buffer2, 0))
              return false;
          }
        }
      }
      return true;
    }
  }
}
