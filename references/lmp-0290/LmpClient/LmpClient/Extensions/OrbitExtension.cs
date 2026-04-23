// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.OrbitExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Globalization;

namespace LmpClient.Extensions
{
  public static class OrbitExtension
  {
    public static string PrintOrbitDataIndex(this Orbit orbit) => "INCLINATION;ECCENTRICITY;SEMIMAJORAXIS;LONGITUDEOFASCENDINGNODE;ARGUMENTOFPERIAPSIS;MEANANOMALYATEPOCH;EPOCH";

    public static string PrintOrbitData(this Orbit orbit) => orbit.inclination.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.eccentricity.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.semiMajorAxis.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.LAN.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.argumentOfPeriapsis.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.meanAnomalyAtEpoch.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ";" + orbit.epoch.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
