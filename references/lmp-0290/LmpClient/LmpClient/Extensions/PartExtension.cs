// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.PartExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using UnityEngine;

namespace LmpClient.Extensions
{
  public static class PartExtension
  {
    public static PartModule FindModuleInPart(Part part, string moduleName)
    {
      if (Object.op_Equality((Object) part, (Object) null))
        return (PartModule) null;
      for (int index = 0; index < part.Modules.Count; ++index)
      {
        if (part.Modules[index].moduleName == moduleName)
          return part.Modules[index];
      }
      return (PartModule) null;
    }

    public static PartResource FindResource(this Part part, string resourceName)
    {
      if (Object.op_Equality((Object) part, (Object) null))
        return (PartResource) null;
      for (int index = 0; index < part.Resources.Count; ++index)
      {
        if (part.Resources[index].resourceName == resourceName)
          return part.Resources[index];
      }
      return (PartResource) null;
    }

    public static void AddCrew(this Part part, ProtoCrewMember crew)
    {
      part.protoModuleCrew.Add(crew);
      crew.RegisterExperienceTraits(part);
      if (!Object.op_Inequality((Object) part.internalModel, (Object) null) || !Object.op_Inequality((Object) part.internalModel.GetNextAvailableSeat(), (Object) null))
      {
        crew.seatIdx = -1;
        crew.seat = (InternalSeat) null;
      }
      else
        part.internalModel.SitKerbalAt(crew, part.internalModel.GetNextAvailableSeat());
      if (!Object.op_Inequality((Object) part.vessel, (Object) null))
        return;
      part.vessel.CrewListSetDirty();
    }

    public static void RemoveCrew(this Part part, ProtoCrewMember crew)
    {
      part.RemoveCrewmember(crew);
      part.protoModuleCrew.Remove(crew);
      if (!Object.op_Inequality((Object) part.internalModel, (Object) null))
        return;
      part.internalModel.UnseatKerbal(crew);
    }

    public static void SetImmortal(this Part part, bool immortal)
    {
      if (Object.op_Equality((Object) part, (Object) null))
        return;
      part.gTolerance = immortal ? double.PositiveInfinity : part.partInfo.partPrefab.gTolerance;
      part.maxPressure = immortal ? double.PositiveInfinity : part.partInfo.partPrefab.maxPressure;
      part.crashTolerance = immortal ? float.PositiveInfinity : part.partInfo.partPrefab.crashTolerance;
      if (Object.op_Implicit((Object) part.rb))
      {
        PartBuoyancy component1 = ((Component) part).GetComponent<PartBuoyancy>();
        if (Object.op_Implicit((Object) component1))
          ((Behaviour) component1).enabled = !immortal;
        PQS_PartCollider component2 = ((Component) part).GetComponent<PQS_PartCollider>();
        if (Object.op_Implicit((Object) component2))
          ((Behaviour) component2).enabled = !immortal;
        CollisionEnhancer component3 = ((Component) part).GetComponent<CollisionEnhancer>();
        if (Object.op_Implicit((Object) component3))
          ((Behaviour) component3).enabled = !immortal;
      }
      if (!Object.op_Implicit((Object) part.attachJoint))
        return;
      if (immortal)
        part.attachJoint.SetUnbreakable(true, part.rigidAttachment);
      else
        part.ResetJoints();
    }
  }
}
