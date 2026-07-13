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
        public int PictureMask { get; set; }

        // TODO: Convert to bool.
        public int IsEquipable { get; set; }

        // TODO: Convert to bool.
        public int IsPremium { get; set; }

        // TODO: Convert to bool.
        public int IsSpecial { get; set; }

        // TODO: Convert to bool.
        public int IsStackable { get; set; }

        // TODO: Convert to bool.
        public int IsUnused { get; set; }
    }
}
