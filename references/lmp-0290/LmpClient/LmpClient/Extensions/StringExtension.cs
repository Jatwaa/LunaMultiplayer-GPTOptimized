// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.StringExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Globalization;

namespace LmpClient.Extensions
{
  public static class StringExtension
  {
    public static string FormatModuleValue(this string str)
    {
      Decimal result;
      return Decimal.TryParse(str, out result) ? Math.Round(result, 2).ToString((IFormatProvider) CultureInfo.InvariantCulture) : str;
    }

    public static string ToInvariantString(this object obj)
    {
      string invariantString;
      string str;
      switch (obj)
      {
        case IConvertible convertible:
          invariantString = convertible.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          goto label_5;
        case IFormattable formattable:
          str = formattable.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        default:
          str = (string) null;
          break;
      }
      invariantString = str ?? obj.ToString();
label_5:
      return invariantString;
    }
  }
}
