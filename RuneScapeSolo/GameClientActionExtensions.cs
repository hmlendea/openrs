using RuneScapeSolo.Enumerations;
using RuneScapeSolo.Lib;

namespace RuneScapeSolo
{
    public static class GameClientActionExtensions
    {
        public static void DisplayMessage(this GameClient client, string message)
        {
            client.displayMessage(message);
        }

        public static void DisplayMessage(this GameClient client, string message, int type)
        {
            client.displayMessage(message, type);
        }

        public static bool IsTradeWindowVisible(this GameClient client, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Initial)
            {
                return client.showTradeBox;
            }

            return client.showTradeConfirmBox;
        }

        public static void AcceptTrade(this GameClient client, TradeAndDuelState state)
        {

        }

        public static void DeclineTrade(this GameClient client)
        {

        }

        public static bool IsDuelWindowVisible(this GameClient client, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Initial)
            {
                return client.ShowDuelBox;
            }

            return client.ShowDuelConfirmBox;
        }

        public static void AcceptDuel(this GameClient client, TradeAndDuelState state)
        {
            if (state != TradeAndDuelState.Confirm)
            {
                return;
            }

            client.duelConfirmOurAccepted = true;
            client.StreamClass.CreatePacket(87);
            client.StreamClass.FormatPacket();
        }

        public static void DeclineDuel(this GameClient client)
        {
            client.ShowDuelConfirmBox = false;
            client.StreamClass.CreatePacket(35);
            client.StreamClass.FormatPacket();
        }
    }
}
