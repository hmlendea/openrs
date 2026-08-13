namespace OpenRS.GameLogic.GameManagers
{
    public sealed class CombatManager(InventoryManager inventoryManager)
    {
        private readonly ElementalRuneStaffCoverageChecker runeStaffCoverageChecker =
            new(inventoryManager);

        public bool HasRequiredRunes(int itemId, int count)
        {
            if (runeStaffCoverageChecker.IsCovered(itemId))
            {
                return true;
            }

            return inventoryManager.GetItemTotalCount(itemId) >= count;
        }
    }
}
