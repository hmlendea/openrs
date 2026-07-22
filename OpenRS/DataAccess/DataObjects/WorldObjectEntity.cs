using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class WorldObjectEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Command1 { get; set; }

        public string Command2 { get; set; }

        public int Type { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int GroundItemElevationOffset { get; set; }

        public string ModelName { get; set; }

        public int ModelIndex { get; set; }
    }
}
