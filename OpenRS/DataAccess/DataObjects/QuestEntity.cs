using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class QuestEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }
    }
}
