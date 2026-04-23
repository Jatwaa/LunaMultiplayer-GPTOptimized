// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Toolbar.ToolbarEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.Base;
using UnityEngine;

namespace LmpClient.Systems.Toolbar
{
  public class ToolbarEvents : SubSystem<ToolbarSystem>
  {
    public void EnableToolBar()
    {
      Texture2D texture = GameDatabase.Instance.GetTexture("LunaMultiplayer/Button/LMPButton", false);
      // ISSUE: method pointer
      GameEvents.onGUIApplicationLauncherReady.Remove(new EventVoid.OnEvent((object) this, __methodptr(EnableToolBar)));
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ApplicationLauncher.Instance.AddModApplication(new Callback((object) SubSystem<ToolbarSystem>.System, __methodptr(HandleButtonClick)), new Callback((object) SubSystem<ToolbarSystem>.System, __methodptr(HandleButtonClick)), ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_0 = new Callback((object) ToolbarEvents.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CEnableToolBar\u003Eb__0_0))), ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_1 ?? (ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_1 = new Callback((object) ToolbarEvents.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CEnableToolBar\u003Eb__0_1))), ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_2 ?? (ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_2 = new Callback((object) ToolbarEvents.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CEnableToolBar\u003Eb__0_2))), ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_3 ?? (ToolbarEvents.\u003C\u003Ec.\u003C\u003E9__0_3 = new Callback((object) ToolbarEvents.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CEnableToolBar\u003Eb__0_3))), (ApplicationLauncher.AppScenes) -1, (Texture) texture);
    }
  }
}
