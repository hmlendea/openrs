namespace RuneScapeSolo.Models
{
    /// <summary>
    /// Item drop.
    /// </summary>
    public class ItemDrop
    {
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public int Weight { get; set; }
    }
}
