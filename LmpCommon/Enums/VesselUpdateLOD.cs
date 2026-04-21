namespace LmpCommon.Enums
{
    public enum VesselUpdateLOD
    {
        /// <summary>Full update: position, velocity, rotation, orbit, all fields.</summary>
        FULL = 0,
        /// <summary>Position only: lat/lon/alt, no velocity or rotation.</summary>
        POS_ONLY = 1,
        /// <summary>No update: vessel is outside interest radius or suppressed.</summary>
        NONE = 2
    }
}