using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class WallObjectEntity : EntityBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Command1 { get; set; }

        public string Command2 { get; set; }

        public int Type { get; set; }

        public int Unknown { get; set; }

        public int ModelHeight { get; set; }

        public int ModelFaceBack { get; set; }

        public int ModelFaceFront { get; set; }
    }
}
