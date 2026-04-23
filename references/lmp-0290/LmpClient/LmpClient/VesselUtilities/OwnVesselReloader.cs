// Decompiled with JetBrains decompiler
// Type: LmpClient.VesselUtilities.OwnVesselReloader
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.VesselUtilities
{
  public class OwnVesselReloader
  {
    public static bool ReloadOwnVessel(ProtoVessel protoVessel)
    {
      foreach (uint partFlightId in ((IEnumerable<Part>) FlightGlobals.ActiveVessel.parts).Select<Part, uint>((Func<Part, uint>) (p => p.flightID)).Except<uint>(((IEnumerable<ProtoPartSnapshot>) protoVessel.protoPartSnapshots).Select<ProtoPartSnapshot, uint>((Func<ProtoPartSnapshot, uint>) (pp => pp.flightID))).ToArray<uint>())
        FlightGlobals.ActiveVessel.FindPart(partFlightId)?.Die();
      uint[] array1 = ((IEnumerable<ProtoPartSnapshot>) protoVessel.protoPartSnapshots).Select<ProtoPartSnapshot, uint>((Func<ProtoPartSnapshot, uint>) (pp => pp.flightID)).Except<uint>(((IEnumerable<Part>) FlightGlobals.ActiveVessel.parts).Select<Part, uint>((Func<Part, uint>) (p => p.flightID))).ToArray<uint>();
      List<ProtoPartSnapshot> protoPartSnapshotList = new List<ProtoPartSnapshot>();
      foreach (uint partFlightId in array1)
      {
        ProtoPartSnapshot protoPart = protoVessel.GetProtoPart(partFlightId);
        FlightGlobals.ActiveVessel.parts.Add(protoPart.Load(FlightGlobals.ActiveVessel, false));
        protoPartSnapshotList.Add(protoPart);
      }
      foreach (ProtoPartSnapshot protoPartSnapshot in protoPartSnapshotList)
        protoPartSnapshot.Init(FlightGlobals.ActiveVessel);
      \u003C\u003Ef__AnonymousType0<int, uint, Part, List<ProtoCrewMember>>[] array2 = ((IEnumerable<Part>) FlightGlobals.ActiveVessel.parts).Where<Part>((Func<Part, bool>) (p => ((IEnumerable<ProtoCrewMember>) p.protoModuleCrew).Any<ProtoCrewMember>())).Select(p => new
      {
        Count = p.protoModuleCrew.Count,
        flightID = p.flightID,
        p = p,
        protoModuleCrew = p.protoModuleCrew
      }).ToArray();
      \u003C\u003Ef__AnonymousType0<int, uint, ProtoPartSnapshot, List<ProtoCrewMember>>[] array3 = ((IEnumerable<ProtoPartSnapshot>) protoVessel.protoPartSnapshots).Where<ProtoPartSnapshot>((Func<ProtoPartSnapshot, bool>) (p => ((IEnumerable<ProtoCrewMember>) p.protoModuleCrew).Any<ProtoCrewMember>())).Select(p => new
      {
        Count = p.protoModuleCrew.Count,
        flightID = p.flightID,
        p = p,
        protoModuleCrew = p.protoModuleCrew
      }).ToArray();
      foreach (var data1 in array2)
      {
        var crewedPart = data1;
        var data2 = array3.FirstOrDefault(pp => (int) pp.flightID == (int) crewedPart.flightID);
        if (data2 != null)
        {
          if (data2.Count > crewedPart.Count)
          {
            foreach (string str in ((IEnumerable<ProtoCrewMember>) data2.protoModuleCrew).Select<ProtoCrewMember, string>((Func<ProtoCrewMember, string>) (c => c.name)).Except<string>(((IEnumerable<ProtoCrewMember>) crewedPart.protoModuleCrew).Select<ProtoCrewMember, string>((Func<ProtoCrewMember, string>) (c => c.name))).ToArray<string>())
            {
              string crewMember = str;
              crewedPart.p.AddCrew(((IEnumerable<ProtoCrewMember>) data2.protoModuleCrew).First<ProtoCrewMember>((Func<ProtoCrewMember, bool>) (m => m.name == crewMember)));
            }
          }
          else if (data2.Count < crewedPart.Count)
          {
            foreach (string str in ((IEnumerable<ProtoCrewMember>) crewedPart.protoModuleCrew).Select<ProtoCrewMember, string>((Func<ProtoCrewMember, string>) (c => c.name)).Except<string>(((IEnumerable<ProtoCrewMember>) data2.protoModuleCrew).Select<ProtoCrewMember, string>((Func<ProtoCrewMember, string>) (c => c.name))).ToArray<string>())
            {
              string crewMember = str;
              crewedPart.p.RemoveCrew(((IEnumerable<ProtoCrewMember>) crewedPart.protoModuleCrew).First<ProtoCrewMember>((Func<ProtoCrewMember, bool>) (m => m.name == crewMember)));
            }
          }
        }
      }
      FlightGlobals.ActiveVessel.RebuildCrewList();
      return true;
    }
  }
}
