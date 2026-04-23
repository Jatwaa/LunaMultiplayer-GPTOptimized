// Decompiled with JetBrains decompiler
// Type: LmpClient.VesselUtilities.VesselSerializer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using System;

namespace LmpClient.VesselUtilities
{
  public class VesselSerializer
  {
    public static ProtoVessel DeserializeVessel(byte[] data, int numBytes)
    {
      try
      {
        ConfigNode configNode = data.DeserializeToConfigNode(numBytes);
        string g = configNode?.GetValue("pid");
        return VesselSerializer.CreateSafeProtoVesselFromConfigNode(configNode, new Guid(g));
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing vessel: {0}", (object) ex));
        return (ProtoVessel) null;
      }
    }

    public static byte[] SerializeVessel(ProtoVessel protoVessel)
    {
      ConfigNode configNode;
      return VesselSerializer.PreSerializationChecks(protoVessel, out configNode) ? configNode.Serialize() : new byte[0];
    }

    public static void SerializeVesselToArray(
      ProtoVessel protoVessel,
      byte[] data,
      out int numBytes)
    {
      ConfigNode configNode;
      if (VesselSerializer.PreSerializationChecks(protoVessel, out configNode))
        configNode.SerializeToArray(data, out numBytes);
      else
        numBytes = 0;
    }

    public static ProtoVessel CreateSafeProtoVesselFromConfigNode(
      ConfigNode inputNode,
      Guid protoVesselId)
    {
      try
      {
        return HighLogic.CurrentGame == null ? (ProtoVessel) null : new ProtoVessel(inputNode, HighLogic.CurrentGame);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Damaged vessel {0}, exception: {1}", (object) protoVesselId, (object) ex));
        return (ProtoVessel) null;
      }
    }

    private static bool PreSerializationChecks(ProtoVessel protoVessel, out ConfigNode configNode)
    {
      configNode = new ConfigNode();
      if (protoVessel == null)
      {
        LunaLog.LogError("[LMP]: Cannot serialize a null protovessel");
        return false;
      }
      try
      {
        protoVessel.Save(configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving vessel: {0}", (object) ex));
        return false;
      }
      Guid guid = new Guid(configNode.GetValue("pid"));
      if (configNode.VesselHasNaNPosition())
      {
        LunaLog.LogError(string.Format("[LMP]: Vessel {0} has NaN position", (object) guid));
        return false;
      }
      VesselSerializer.RemoveManeuverNodesFromProtoVessel(configNode);
      return true;
    }

    private static void RemoveManeuverNodesFromProtoVessel(ConfigNode vesselNode) => vesselNode?.GetNode("FLIGHTPLAN")?.ClearData();
  }
}
