// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Screenshot.Screenshot
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using UnityEngine;

namespace LmpClient.Systems.Screenshot
{
  public class Screenshot
  {
    public long DateTaken;
    public int Width;
    public int Height;
    public byte[] Data = new byte[0];
    private Texture2D _texture;

    public Texture2D Texture
    {
      get
      {
        if (Object.op_Equality((Object) this._texture, (Object) null))
        {
          this._texture = new Texture2D(this.Width, this.Height);
          ImageConversion.LoadImage(this._texture, this.Data);
        }
        return this._texture;
      }
    }
  }
}
