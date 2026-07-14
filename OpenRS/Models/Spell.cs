using System.Collections.Generic;

namespace OpenRS.Models
{
    public sealed class Spell
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredLevel { get; set; }
        public int Type { get; set; }
        public IDictionary<int, int> RequiredRunes { get; set; }
        public int ExperienceGain { get; set; }
    }
}
