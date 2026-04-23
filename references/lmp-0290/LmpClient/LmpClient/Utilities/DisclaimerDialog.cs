// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.DisclaimerDialog
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization;
using UnityEngine;

namespace LmpClient.Utilities
{
  public class DisclaimerDialog
  {
    public static void SpawnDialog() => PopupDialog.SpawnPopupDialog(new MultiOptionDialog("DisclaimerWindow", LocalizationContainer.DisclaimerDialogText.Text, LocalizationContainer.DisclaimerDialogText.Title, HighLogic.UISkin, new Rect(0.5f, 0.5f, 425f, 150f), new DialogGUIBase[2]
    {
      (DialogGUIBase) new DialogGUIFlexibleSpace(),
      (DialogGUIBase) new DialogGUIVerticalLayout(new DialogGUIBase[1]
      {
        (DialogGUIBase) new DialogGUIHorizontalLayout(new DialogGUIBase[5]
        {
          (DialogGUIBase) new DialogGUIButton(LocalizationContainer.DisclaimerDialogText.Accept, DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_0 = new Callback((object) DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CSpawnDialog\u003Eb__0_0)))),
          (DialogGUIBase) new DialogGUIFlexibleSpace(),
          (DialogGUIBase) new DialogGUIButton(LocalizationContainer.DisclaimerDialogText.Decline, DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_1 ?? (DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_1 = new Callback((object) DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CSpawnDialog\u003Eb__0_1)))),
          (DialogGUIBase) new DialogGUIFlexibleSpace(),
          (DialogGUIBase) new DialogGUIButton(LocalizationContainer.GetCurrentLanguageAsText(), DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_2 ?? (DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9__0_2 = new Callback((object) DisclaimerDialog.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CSpawnDialog\u003Eb__0_2))))
        })
      })
    }), true, HighLogic.UISkin, true, "");
  }
}
