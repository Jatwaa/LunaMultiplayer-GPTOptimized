// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.ConfigNodeExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Extensions
{
  public static class ConfigNodeExtension
  {
    public static bool VesselHasNaNPosition(this ConfigNode vesselNode)
    {
      if (vesselNode.GetValue("landed") == "True" || vesselNode.GetValue("splashed") == "True")
      {
        double result1;
        double result2;
        double result3;
        if (double.TryParse(vesselNode.values.GetValue("lat"), out result1) && (double.IsNaN(result1) || double.IsInfinity(result1)) || double.TryParse(vesselNode.values.GetValue("lon"), out result2) && (double.IsNaN(result2) || double.IsInfinity(result2)) || double.TryParse(vesselNode.values.GetValue("alt"), out result3) && (double.IsNaN(result3) || double.IsInfinity(result3)))
          return true;
      }
      else
      {
        ConfigNode orbitNode = vesselNode.GetNode("ORBIT");
        if (orbitNode != null)
          return ((IEnumerable<string>) orbitNode.values.DistinctNames()).Select<string, string>((Func<string, string>) (v => orbitNode.GetValue(v))).Take<string>(7).All<string>((Func<string, bool>) (v => v == "0")) || ((IEnumerable<string>) orbitNode.values.DistinctNames()).Select<string, string>((Func<string, string>) (v => orbitNode.GetValue(v))).Any<string>((Func<string, bool>) (val =>
          {
            double result;
            if (!double.TryParse(val, out result))
              return false;
            return double.IsNaN(result) || double.IsInfinity(result);
          }));
      }
      return false;
    }
  }
}
