// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.Interface.IWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Base.Interface
{
  public interface IWindow
  {
    string WindowName { get; }

    void Update();

    void OnGui();

    void RemoveWindowLock();

    void CheckWindowLock();

    void SetStyles();
  }
}
