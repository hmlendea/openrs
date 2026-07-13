using OpenRS.Net.Client;

namespace OpenRS
{
    public static class GameClientExtensions
    {
        public static void ShowMessage(this GameClient mc, string message, int type)
            => mc.DisplayMessage(message, type);

        public static bool IsTradeWindowVisible(this GameClient mc, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Initial)
            {
                return mc.showTradeBox;
            }

            return mc.showTradeConfirmBox;
        }

        public static void AcceptTrade(this GameClient mc, TradeAndDuelState state)
        {
        }

        public static void DeclineTrade(this GameClient mc)
        {
        }

        public static bool IsDuelWindowVisible(this GameClient mc, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Initial)
            {
                return mc.showDuelBox;
            }

            return mc.showDuelConfirmBox;
        }

        public static void AcceptDuel(this GameClient mc, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Confirm)
            {
                mc.duelConfirmOurAccepted = true;
                mc.streamClass.CreatePacket(87);
                mc.streamClass.FormatPacket();
            }
        }

        public static void DeclineDuel(this GameClient mc)
        {
            mc.showDuelConfirmBox = false;
            mc.streamClass.CreatePacket(35);
            mc.streamClass.FormatPacket();
        }
    }
}
