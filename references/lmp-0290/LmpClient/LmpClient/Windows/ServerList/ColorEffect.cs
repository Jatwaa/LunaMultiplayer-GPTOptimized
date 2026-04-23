// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.ColorEffect
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon;
using UnityEngine;

namespace LmpClient.Windows
{
  [KSPAddon]
  public class ColorEffect : MonoBehaviour
  {
    private static Color _defaultContentColor;
    private static readonly Color[] Colors = new Color[6]
    {
      Color.red,
      Color.magenta,
      Color.blue,
      Color.cyan,
      Color.green,
      Color.yellow
    };
    private static Color _lerpedColor = ColorEffect.Colors[0];
    private int _currentColorIndex = 0;
    private float _colorTime = 0.0f;

    public void Awake()
    {
      ColorEffect._defaultContentColor = GUI.contentColor;
      Object.DontDestroyOnLoad((Object) this);
    }

    public void OnGUI()
    {
      if ((double) this._colorTime <= 1.0)
      {
        ColorEffect._lerpedColor = Color.Lerp(ColorEffect.Colors[this._currentColorIndex], ColorEffect.Colors[this._currentColorIndex == ColorEffect.Colors.Length - 1 ? 0 : this._currentColorIndex + 1], this._colorTime);
        this._colorTime += 0.015f;
      }
      else
      {
        this._colorTime = 0.0f;
        if (this._currentColorIndex == ColorEffect.Colors.Length - 1)
          this._currentColorIndex = 0;
        else
          ++this._currentColorIndex;
      }
    }

    public static void StartPaintingServer(ServerInfo server)
    {
      if (!server.DedicatedServer)
        return;
      if (server.RainbowEffect)
        ColorEffect.StartRainbowEffect();
      else
        GUI.contentColor = new Color((float) server.Color[0] / (float) byte.MaxValue, (float) server.Color[1] / (float) byte.MaxValue, (float) server.Color[2] / (float) byte.MaxValue);
    }

    public static void StopPaintingServer() => GUI.contentColor = ColorEffect._defaultContentColor;

    private static void StartRainbowEffect() => GUI.contentColor = ColorEffect._lerpedColor;
  }
}
