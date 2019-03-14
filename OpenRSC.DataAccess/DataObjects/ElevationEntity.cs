using NuciXNA.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.DataObjects
{
    /// <summary>
    /// Elevation entity.
    /// </summary>
    public class ElevationEntity : EntityBase
    {
        public int Roof { get; set; }

        public int Unknown { get; set; }
    }
}
