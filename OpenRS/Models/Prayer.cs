namespace OpenRS.Models
{
    public sealed class Prayer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredLevel { get; set; }
        public int DrainRate { get; set; }
    }
}
