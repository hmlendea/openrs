namespace OpenRS.Models
{
    public sealed class Item
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Command { get; set; }

        public int BasePrice { get; set; }

        public int SpriteId { get; set; }

        public int InventoryPicture { get; set; }

        public string SpriteName { get; set; }

        public int PictureMask { get; set; }

        // Equipment type bitfield: 0 = not equippable, non-zero = equip slot (bits 3-4 indicate weapon type).
        public int IsEquipable { get; set; }

        public bool IsPremium { get; set; }

        public bool IsSpecial { get; set; }

        public bool IsStackable { get; set; }

        public bool IsUnused { get; set; }
    }
}
