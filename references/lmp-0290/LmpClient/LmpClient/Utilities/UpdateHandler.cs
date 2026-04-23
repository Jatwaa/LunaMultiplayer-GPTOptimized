// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.UpdateHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Windows.Update;
using LmpCommon;
using LmpGlobal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace LmpClient.Utilities
{
  public static class UpdateHandler
  {
    public static IEnumerator CheckForUpdates()
    {
      using (UnityWebRequest www = UnityWebRequest.Get(RepoConstants.ApiLatestGithubReleaseUrl))
      {
        yield return (object) www.SendWebRequest();
        if (!www.isNetworkError || !www.isHttpError)
        {
          if (!(Json.Deserialize(www.downloadHandler.text) is Dictionary<string, object> data))
            ;
          else
          {
            Version latestVersion = new Version(data["tag_name"].ToString());
            LunaLog.Log(string.Format("Latest version: {0}", (object) latestVersion));
            if (latestVersion > LmpVersioning.CurrentVersion)
            {
              using (UnityWebRequest www2 = new UnityWebRequest(data["url"].ToString()))
              {
                yield return (object) www2.SendWebRequest();
                if (!www2.isNetworkError)
                {
                  string changelog = data["body"].ToString();
                  UpdateWindow.LatestVersion = latestVersion;
                  UpdateWindow.Changelog = changelog;
                  Window<UpdateWindow>.Singleton.Display = true;
                  changelog = (string) null;
                }
              }
            }
            data = (Dictionary<string, object>) null;
            latestVersion = (Version) null;
          }
        }
        else
          LunaLog.Log("Could not check for latest version. Error: " + www.error);
      }
    }
  }
}
