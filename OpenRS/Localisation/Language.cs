using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRS.Localisation
{
    public class Language : IEquatable<Language>
    {
        private static readonly Dictionary<string, Language> values = new()
        {
            { nameof(English), new Language(nameof(English), "en") },
        };

        public string Name { get; }

        public string Code { get; }

        private Language(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public static Language English => values[nameof(English)];

        public static Array GetValues() => values.Values.ToArray();

        public static Language FromString(string name)
        {
            if (!values.ContainsKey(name))
            {
                return English;
            }

            return values[name];
        }

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

        public override int GetHashCode() => $"{nameof(Language)}:{Name}".GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(Language current, Language other) => current.Equals(other);

        public static bool operator !=(Language current, Language other) => !current.Equals(other);

        public static implicit operator string(Language language) => language.Name;
    }
}
