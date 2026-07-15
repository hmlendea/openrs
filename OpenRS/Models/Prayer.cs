namespace OpenRS.Models
{
    public sealed class Prayer
    {
        public static int MaximumCount => 14;

        public string Id { get; set; }

        public int V1Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int RequiredLevel { get; set; }

        public int DrainRate { get; set; }
    }
}
