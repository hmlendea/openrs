using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class SilentProgressPacketHandler
    {
        internal bool TryHandlePacket(ServerCommand command)
            => command switch
            {
                ServerCommand.GuthixSpells or
                ServerCommand.ZamorakSpells or
                ServerCommand.SaradominSpells or
                ServerCommand.QuestPointsChange or
                ServerCommand.Deaths or
                ServerCommand.DruidicRitual or
                ServerCommand.ImpCatcher or
                ServerCommand.SheepShearer or
                ServerCommand.DoricQuest or
                ServerCommand.Kills or
                ServerCommand.RomeoAndJuliet or
                ServerCommand.WitchPotion or
                ServerCommand.CookAssistant or
                ServerCommand.TutorialChange or
                ServerCommand.DemonsSlayer or
                ServerCommand.TheRuthlessGhost or
                ServerCommand.PirateTreasure or
                ServerCommand.ErnestTheChicken or
                ServerCommand.PvpTournamentTimer or
                ServerCommand.WildernessModeTimer or
                ServerCommand.DropPartyTimer or
                ServerCommand.TaskPointsChange or
                ServerCommand.CompletedTasks or
                ServerCommand.Remaining or
                ServerCommand.MoneyTask or
                ServerCommand.TaskStatus or
                ServerCommand.TaskExperience or
                ServerCommand.TaskCash or
                ServerCommand.TaskItem => true,
                _ => false,
            };
    }
}