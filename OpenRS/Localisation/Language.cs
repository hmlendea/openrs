using System;
using System.Collections.Generic;

namespace OpenRS.Localisation
{
    public class Language : IEquatable<Language>
    {
        private static readonly Dictionary<string, Language> values = new()
        {
            { nameof(English), new Language(nameof(English), "en") }
        };

        public string Name { get; }

        public string Code { get; }

        private Language(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public static Language English => values[nameof(English)];

        public static Language[] GetValues() => [.. values.Values];

        public bool Equals(Language other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Language)obj);
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}
