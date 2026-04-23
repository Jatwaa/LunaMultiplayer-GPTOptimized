// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Label.LabelEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using LmpClient.Base;
using LmpClient.Systems.Lock;
using TMPro;
using UnityEngine;

namespace LmpClient.Systems.Label
{
  public class LabelEvents : SubSystem<LabelSystem>
  {
    public void OnLabelProcessed(BaseLabel label)
    {
      if (!(label is VesselLabel vesselLabel))
        return;
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(vesselLabel.vessel.id);
      if (!string.IsNullOrEmpty(controlLockOwner))
        ((TMP_Text) label.text).text = controlLockOwner + "\n" + ((TMP_Text) label.text).text;
    }

    public void OnMapLabelProcessed(Vessel vessel, MapNode.CaptionData label)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(vessel.id);
      if (string.IsNullOrEmpty(controlLockOwner))
        return;
      label.Header = controlLockOwner + "\n" + label.Header;
    }

    public void OnMapWidgetTextProcessed(TrackingStationWidget widget)
    {
      if (Object.op_Equality((Object) widget.vessel, (Object) null))
        return;
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(widget.vessel.id);
      if (string.IsNullOrEmpty(controlLockOwner))
        return;
      ((TMP_Text) widget.textName).text = "(" + controlLockOwner + ") " + ((TMP_Text) widget.textName).text;
    }
  }
}
