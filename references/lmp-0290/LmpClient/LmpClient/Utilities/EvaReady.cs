// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.EvaReady
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using System.Collections;
using UnityEngine;

namespace LmpClient.Utilities
{
  public class EvaReady
  {
    public static void FireOnCrewEvaReady(KerbalEVA eva) => MainSystem.Singleton.StartCoroutine(EvaReady.OnCrewEvaReady(eva));

    private static IEnumerator OnCrewEvaReady(KerbalEVA eva)
    {
      while (Object.op_Inequality((Object) eva, (Object) null) && !eva.Ready)
        yield return (object) null;
      if (Object.op_Inequality((Object) eva, (Object) null) && eva.Ready)
        EvaEvent.onCrewEvaReady.Fire(((PartModule) eva).vessel);
    }
  }
}
