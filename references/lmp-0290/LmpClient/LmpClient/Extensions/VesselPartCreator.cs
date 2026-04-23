// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.VesselPartCreator
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections.Generic;

namespace LmpClient.Extensions
{
  public static class VesselPartCreator
  {
    public static void CreateMissingPartsInCurrentProtoVessel(
      Vessel vessel,
      ProtoVessel protoVessel)
    {
      List<ProtoPartSnapshot> protoPartSnapshotList = new List<ProtoPartSnapshot>();
      foreach (ProtoPartSnapshot protoPartSnapshot in protoVessel.protoPartSnapshots)
      {
        Part part1;
        if (protoPartSnapshot.FindModule("ModuleDockingNode") != null && FlightGlobals.FindLoadedPart(protoPartSnapshot.persistentId, ref part1))
          part1.Die();
        Part part2;
        if (!FlightGlobals.FindLoadedPart(protoPartSnapshot.persistentId, ref part2))
        {
          Part part3 = protoPartSnapshot.Load(vessel, false);
          vessel.parts.Add(part3);
          protoPartSnapshotList.Add(protoPartSnapshot);
        }
      }
      foreach (ProtoPartSnapshot protoPartSnapshot in protoPartSnapshotList)
        protoPartSnapshot.Init(vessel);
      vessel.RebuildCrewList();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      MainSystem.Singleton.StartCoroutine(CallbackUtil.DelayedCallback(0.25f, VesselPartCreator.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (VesselPartCreator.\u003C\u003Ec.\u003C\u003E9__0_0 = new Callback((object) VesselPartCreator.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CCreateMissingPartsInCurrentProtoVessel\u003Eb__0_0)))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      MainSystem.Singleton.StartCoroutine(CallbackUtil.DelayedCallback(0.5f, VesselPartCreator.\u003C\u003Ec.\u003C\u003E9__0_1 ?? (VesselPartCreator.\u003C\u003Ec.\u003C\u003E9__0_1 = new Callback((object) VesselPartCreator.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CCreateMissingPartsInCurrentProtoVessel\u003Eb__0_1)))));
    }
  }
}
