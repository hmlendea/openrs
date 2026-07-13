using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class WorldObjectEntity : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command1 { get; set; }
        public string Command2 { get; set; }
        public int Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int GroundItemVar { get; set; }
        public string ObjectModel { get; set; }
        public int ModelId { get; set; }
    }
}
