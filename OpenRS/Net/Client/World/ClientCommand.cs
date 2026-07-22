using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRS.Net.Client.World
{
    public class ClientCommand : IEquatable<ClientCommand>
    {
        private static readonly Dictionary<string, ClientCommand> values = new()
        {
            { "unknown", new ClientCommand("unknown") },
            { "closecon", new ClientCommand("closecon") },
            { "logout", new ClientCommand("logout") },
            { "lostcon", new ClientCommand("lostcon") },
            { "tell", new ClientCommand("tell") },
        };

        public string Name { get; }

        private ClientCommand(string name)
        {
            Name = name;
        }

        public static ClientCommand Unknown => values["unknown"];

        public static ClientCommand CloseConnection => values["closecon"];

        public static ClientCommand Logout => values["logout"];

        public static ClientCommand LostConnection => values["lostcon"];

        public static ClientCommand Tell => values["tell"];

        public static Array GetValues() => values.Values.ToArray();

        public static ClientCommand FromString(string name)
        {
            if (!values.ContainsKey(name))
            {
                return Unknown;
            }

            return values[name];
        }

        public bool Equals(ClientCommand other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name;
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

            return Equals((ClientCommand)obj);
        }

        public override int GetHashCode() => $"{nameof(ClientCommand)}:{Name}".GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(ClientCommand current, ClientCommand other) => current.Equals(other);

        public static bool operator !=(ClientCommand current, ClientCommand other) => !current.Equals(other);

        public static implicit operator string(ClientCommand command) => command.Name;
    }
}
