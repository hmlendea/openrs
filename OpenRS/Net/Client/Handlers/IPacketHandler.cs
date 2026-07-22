namespace OpenRS.Net.Client.Handlers
{
    public interface IPacketHandler
    {
        public void HandlePacket(int packetID, int packetLength, sbyte[] packetData);
    }
}
