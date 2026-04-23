// Decompiled with JetBrains decompiler
// Type: LmpCommon.GithubCertification
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LmpCommon
{
  public static class GithubCertification
  {
    public static bool MyRemoteCertificateValidationCallback(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      bool flag = true;
      if ((uint) sslPolicyErrors > 0U)
      {
        foreach (X509ChainStatus chainStatu in chain.ChainStatus)
        {
          if (chainStatu.Status != X509ChainStatusFlags.RevocationStatusUnknown)
          {
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            if (!chain.Build((X509Certificate2) certificate))
              flag = false;
          }
        }
      }
      return flag;
    }
  }
}
