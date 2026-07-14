using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class CombatManager(InventoryManager inventoryManager)
    {
        public bool HasRequiredRunes(int itemId, int count)
        {
            if (itemId == (int)RuneElement.Air &&
                (inventoryManager.IsItemEquipped((int)ElementalStaff.Air) ||
                 inventoryManager.IsItemEquipped((int)ElementalBattlestaff.Air) ||
                 inventoryManager.IsItemEquipped((int)ElementalMysticStaff.Air)))
            {
                return true;
            }

            if (itemId == (int)RuneElement.Water &&
                (inventoryManager.IsItemEquipped((int)ElementalStaff.Water) ||
                 inventoryManager.IsItemEquipped((int)ElementalBattlestaff.Water) ||
                 inventoryManager.IsItemEquipped((int)ElementalMysticStaff.Water)))
            {
                return true;
            }

            if (itemId == (int)RuneElement.Earth &&
                (inventoryManager.IsItemEquipped((int)ElementalStaff.Earth) ||
                 inventoryManager.IsItemEquipped((int)ElementalBattlestaff.Earth) ||
                 inventoryManager.IsItemEquipped((int)ElementalMysticStaff.Earth)))
            {
                return true;
            }

            if (itemId == (int)RuneElement.Fire &&
                (inventoryManager.IsItemEquipped((int)ElementalStaff.Fire) ||
                 inventoryManager.IsItemEquipped((int)ElementalBattlestaff.Fire) ||
                 inventoryManager.IsItemEquipped((int)ElementalMysticStaff.Fire)))
            {
                return true;
            }

            return inventoryManager.GetItemTotalCount(itemId) >= count;
        }
    }
}
