// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.CompatibilityChecker
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization;
using LmpGlobal;
using System;
using UnityEngine;

namespace LmpClient.Utilities
{
  internal class CompatibilityChecker
  {
    private static readonly Version KspVersion = new Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision);

    public static bool IsCompatible() => CompatibilityChecker.KspVersion >= KspCompatible.MinKspVersion && CompatibilityChecker.KspVersion <= KspCompatible.MaxKspVersion;

    public static void SpawnDialog()
    {
      if (CompatibilityChecker.IsCompatible())
        return;
      PopupDialog.SpawnPopupDialog(new MultiOptionDialog("CompatibilityWindow", string.Empty, LocalizationContainer.CompatibleDialogText.Title, HighLogic.UISkin, new Rect(0.5f, 0.5f, 425f, 150f), new DialogGUIBase[1]
      {
        (DialogGUIBase) new DialogGUIVerticalLayout(new DialogGUIBase[4]
        {
          (DialogGUIBase) new DialogGUIFlexibleSpace(),
          (DialogGUIBase) new DialogGUILabel(LocalizationContainer.CompatibleDialogText.Text, false, false),
          (DialogGUIBase) new DialogGUIFlexibleSpace(),
          (DialogGUIBase) new DialogGUIHorizontalLayout(new DialogGUIBase[3]
          {
            (DialogGUIBase) new DialogGUIFlexibleSpace(),
            (DialogGUIBase) new DialogGUIButton(LocalizationContainer.CompatibleDialogText.Accept, (Callback) null),
            (DialogGUIBase) new DialogGUIFlexibleSpace()
          })
        })
      }), true, HighLogic.UISkin, true, "");
    }
  }
}
