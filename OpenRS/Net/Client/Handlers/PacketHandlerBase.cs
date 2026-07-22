using System.Linq;

namespace OpenRS.Net.Client.Handlers
{
    internal abstract class PacketHandlerBase(GameClient client)
    {
        protected GameClient Client => client;

        protected static string DecodePacketString(
            sbyte[] packetData,
            int startIndex,
            int length)
            => new([.. packetData.Select(byteValue => (char)byteValue)], startIndex, length);
    }
}
