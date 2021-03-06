﻿using NuciXNA.DataAccess.DataObjects;

namespace OpenRS.DataAccess.DataObjects
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
