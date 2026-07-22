using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class GameTextureEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public string SubName { get; set; }
    }
}
