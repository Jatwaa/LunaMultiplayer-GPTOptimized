// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.ProtoVesselExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.Chat;
using LmpClient.Systems.Flag;
using LmpClient.Systems.Mod;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Extensions
{
  public static class ProtoVesselExtension
  {
    public static ProtoPartSnapshot GetProtoPart(
      this ProtoVessel protoVessel,
      uint partFlightId)
    {
      if (protoVessel == null)
        return (ProtoPartSnapshot) null;
      for (int index = 0; index < protoVessel.protoPartSnapshots.Count; ++index)
      {
        if ((int) protoVessel.protoPartSnapshots[index].flightID == (int) partFlightId)
          return protoVessel.protoPartSnapshots[index];
      }
      return (ProtoPartSnapshot) null;
    }

    public static bool HasInvalidParts(this ProtoVessel pv, bool verboseErrors)
    {
      foreach (ProtoPartSnapshot protoPartSnapshot in pv.protoPartSnapshots)
      {
        if (LmpClient.Base.System<ModSystem>.Singleton.ModControl && !LmpClient.Base.System<ModSystem>.Singleton.AllowedParts.Contains(protoPartSnapshot.partName))
        {
          if (verboseErrors)
          {
            string message = string.Format("Protovessel {0} ({1}) contains the BANNED PART '{2}'. Skipping load.", (object) pv.vesselID, (object) pv.vesselName, (object) protoPartSnapshot.partName);
            LunaLog.LogWarning(message);
            LmpClient.Base.System<ChatSystem>.Singleton.PmMessageServer(message);
          }
          return true;
        }
        string[] array = ((IEnumerable<ProtoPartResourceSnapshot>) protoPartSnapshot.resources).Select<ProtoPartResourceSnapshot, string>((Func<ProtoPartResourceSnapshot, string>) (r => r.resourceName)).Except<string>((IEnumerable<string>) LmpClient.Base.System<ModSystem>.Singleton.AllowedResources).ToArray<string>();
        if (LmpClient.Base.System<ModSystem>.Singleton.ModControl && ((IEnumerable<string>) array).Any<string>())
        {
          if (verboseErrors)
          {
            string message = string.Format("Protovessel {0} ({1}) contains the BANNED RESOURCE/S '{2}'. Skipping load.", (object) pv.vesselID, (object) pv.vesselName, (object) string.Join(", ", array));
            LunaLog.LogWarning(message);
            LmpClient.Base.System<ChatSystem>.Singleton.PmMessageServer(message);
          }
          return true;
        }
        if (protoPartSnapshot.partInfo == null)
        {
          if (verboseErrors)
          {
            LunaLog.LogWarning(string.Format("Protovessel {0} ({1}) contains the MISSING PART '{2}'. Skipping load.", (object) pv.vesselID, (object) pv.vesselName, (object) protoPartSnapshot.partName));
            LunaScreenMsg.PostScreenMessage("Cannot load '" + pv.vesselName + "' - missing part: " + protoPartSnapshot.partName, 10f, (ScreenMessageStyle) 0);
          }
          return true;
        }
        ProtoPartResourceSnapshot resourceSnapshot = ((IEnumerable<ProtoPartResourceSnapshot>) protoPartSnapshot.resources).FirstOrDefault<ProtoPartResourceSnapshot>((Func<ProtoPartResourceSnapshot, bool>) (r => !PartResourceLibrary.Instance.resourceDefinitions.Contains(r.resourceName)));
        if (resourceSnapshot != null & verboseErrors)
        {
          string message = string.Format("Protovessel {0} ({1}) contains the MISSING RESOURCE '{2}'.", (object) pv.vesselID, (object) pv.vesselName, (object) resourceSnapshot.resourceName);
          LunaLog.LogWarning(message);
          LmpClient.Base.System<ChatSystem>.Singleton.PmMessageServer(message);
          LunaScreenMsg.PostScreenMessage("Vessel '" + pv.vesselName + "' contains the modded RESOURCE: " + protoPartSnapshot.partName, 10f, (ScreenMessageStyle) 0);
        }
      }
      return false;
    }

    public static bool IsCometOrAsteroid(this ProtoVessel protoVessel) => protoVessel.IsComet() || protoVessel.IsAsteroid();

    public static bool IsComet(this ProtoVessel protoVessel)
    {
      if (protoVessel == null)
        return false;
      return (protoVessel.protoPartSnapshots == null || protoVessel.protoPartSnapshots.Count == 0) && protoVessel.vesselName.StartsWith("Ast.") || protoVessel.protoPartSnapshots != null && protoVessel.protoPartSnapshots.Count == 1 && protoVessel.protoPartSnapshots[0].partName == "PotatoComet";
    }

    public static bool IsAsteroid(this ProtoVessel protoVessel)
    {
      if (protoVessel == null)
        return false;
      return (protoVessel.protoPartSnapshots == null || protoVessel.protoPartSnapshots.Count == 0) && protoVessel.vesselName.StartsWith("Ast.") || protoVessel.protoPartSnapshots != null && protoVessel.protoPartSnapshots.Count == 1 && protoVessel.protoPartSnapshots[0].partName == "PotatoRoid";
    }

    public static bool Validate(this ProtoVessel protoVessel)
    {
      if (protoVessel == null)
      {
        LunaLog.LogError("[LMP]: protoVessel is null!");
        return false;
      }
      if (protoVessel.vesselID == Guid.Empty)
      {
        LunaLog.LogError("[LMP]: protoVessel id is null!");
        return false;
      }
      if (protoVessel.situation == 8)
      {
        if (protoVessel.orbitSnapShot == null)
        {
          LunaLog.LogWarning("[LMP]: Skipping flying vessel load - Protovessel does not have an orbit snapshot");
          return false;
        }
        if (FlightGlobals.Bodies == null || FlightGlobals.Bodies.Count < protoVessel.orbitSnapShot.ReferenceBodyIndex)
        {
          LunaLog.LogWarning(string.Format("[LMP]: Skipping flying vessel load - Could not find celestial body index {0}", (object) protoVessel.orbitSnapShot.ReferenceBodyIndex));
          return false;
        }
      }
      foreach (ProtoPartSnapshot protoPartSnapshot in ((IEnumerable<ProtoPartSnapshot>) protoVessel.protoPartSnapshots).Where<ProtoPartSnapshot>((Func<ProtoPartSnapshot, bool>) (p => !string.IsNullOrEmpty(p.flagURL))))
      {
        if (!LmpClient.Base.System<FlagSystem>.Singleton.FlagExists(protoPartSnapshot.flagURL))
        {
          LunaLog.Log("[LMP]: Flag '" + protoPartSnapshot.flagURL + "' doesn't exist, setting to default!");
          protoPartSnapshot.flagURL = "Squad/Flags/default";
        }
      }
      return true;
    }
  }
}
