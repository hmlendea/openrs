using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class CombatManager(InventoryManager inventoryManager)
    {
        public bool HasRequiredRunes(int itemId, int count)
        {
            if (IsRuneCoveredByEquippedStaff(itemId))
            {
                return true;
            }

            return inventoryManager.GetItemTotalCount(itemId) >= count;
        }

        private bool IsRuneCoveredByEquippedStaff(int runeItemId)
            => (RuneElement)runeItemId switch
            {
                RuneElement.Air => IsAnyElementalStaffEquipped(
                    (int)ElementalStaff.Air,
                    (int)ElementalBattlestaff.Air,
                    (int)ElementalMysticStaff.Air),
                RuneElement.Water => IsAnyElementalStaffEquipped(
                    (int)ElementalStaff.Water,
                    (int)ElementalBattlestaff.Water,
                    (int)ElementalMysticStaff.Water),
                RuneElement.Earth => IsAnyElementalStaffEquipped(
                    (int)ElementalStaff.Earth,
                    (int)ElementalBattlestaff.Earth,
                    (int)ElementalMysticStaff.Earth),
                RuneElement.Fire => IsAnyElementalStaffEquipped(
                    (int)ElementalStaff.Fire,
                    (int)ElementalBattlestaff.Fire,
                    (int)ElementalMysticStaff.Fire),
                _ => false
            };

        private bool IsAnyElementalStaffEquipped(
            int staffItemId,
            int battlestaffItemId,
            int mysticStaffItemId)
        {
            return
                inventoryManager.IsItemEquipped(staffItemId) ||
                inventoryManager.IsItemEquipped(battlestaffItemId) ||
                inventoryManager.IsItemEquipped(mysticStaffItemId);
        }
    }
}
