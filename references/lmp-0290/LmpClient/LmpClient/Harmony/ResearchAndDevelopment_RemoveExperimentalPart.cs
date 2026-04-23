// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ResearchAndDevelopment_RemoveExperimentalPart
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ResearchAndDevelopment))]
  [HarmonyPatch("RemoveExperimentalPart")]
  public class ResearchAndDevelopment_RemoveExperimentalPart
  {
    [HarmonyPrefix]
    private static void PrefixRemoveExperimentalPart(AvailablePart ap, out int __state)
    {
      __state = -1;
      if (Object.op_Equality((Object) ResearchAndDevelopment.Instance, (Object) null))
        return;
      Dictionary<AvailablePart, int> dictionary = Traverse.Create((object) ResearchAndDevelopment.Instance).Field<Dictionary<AvailablePart, int>>("experimentalPartsStock").Value;
      if (dictionary == null)
        return;
      __state = dictionary.ContainsKey(ap) ? dictionary[ap] : 0;
    }

    [HarmonyPostfix]
    private static void PostfixRemoveExperimentalPart(AvailablePart ap, int __state)
    {
      if (Object.op_Equality((Object) ResearchAndDevelopment.Instance, (Object) null))
        return;
      Dictionary<AvailablePart, int> dictionary = Traverse.Create((object) ResearchAndDevelopment.Instance).Field<Dictionary<AvailablePart, int>>("experimentalPartsStock").Value;
      if (dictionary == null)
        return;
      int num = dictionary.ContainsKey(ap) ? dictionary[ap] : 0;
      if (num != __state)
        ExperimentalPartEvent.onExperimentalPartRemoved.Fire(ap, num);
    }
  }
}
