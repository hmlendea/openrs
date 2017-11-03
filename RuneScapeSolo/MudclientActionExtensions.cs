using RuneScapeSolo.Enumerations;
using RuneScapeSolo.Lib;

namespace RuneScapeSolo
{
    public static class MudclientActionExtensions
    {
        public static void DisplayMessage(this mudclient mc, string message)
        {
            mc.displayMessage(message);
        }

        public static void DisplayMessage(this mudclient mc, string message, int type)
        {
            mc.displayMessage(message, type);
        }
        
        public static bool IsTradeWindowVisible(this mudclient mc, TradeAndDuelState state)
        {
            return state == TradeAndDuelState.Initial ? mc.showTradeBox : mc.showTradeConfirmBox;
        }
        public static void AcceptTrade(this mudclient mc, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Initial)
            {
            }
            else
            {
            }
        }
        public static void DeclineTrade(this mudclient mc)
        {

        }

        public static bool IsDuelWindowVisible(this mudclient mc, TradeAndDuelState state)
        {
            return state == TradeAndDuelState.Initial ? mc.showDuelBox : mc.showDuelConfirmBox;
        }
        public static void AcceptDuel(this mudclient mc, TradeAndDuelState state)
        {
            if (state == TradeAndDuelState.Confirm)
            {
                mc.duelConfirmOurAccepted = true;
                mc.StreamClass.CreatePacket(87);
                mc.StreamClass.FormatPacket();
            }
        }
        public static void DeclineDuel(this mudclient mc)
        {
            mc.showDuelConfirmBox = false;
            mc.StreamClass.CreatePacket(35);
            mc.StreamClass.FormatPacket();
        }
    }
}
