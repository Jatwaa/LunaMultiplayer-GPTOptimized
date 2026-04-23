// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.WindowUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.IO;
using UnityEngine;

namespace LmpClient.Windows
{
  public class WindowUtil
  {
    public static Texture2D LoadIcon(string path, int width, int height)
    {
      Texture2D texture2D = new Texture2D(width, height);
      if (File.Exists(path))
      {
        byte[] numArray = File.ReadAllBytes(path);
        ImageConversion.LoadImage(texture2D, numArray);
      }
      return texture2D;
    }
  }
}
