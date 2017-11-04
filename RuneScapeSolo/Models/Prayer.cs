namespace RuneScapeSolo.Models
{
    /// <summary>
    /// Prayer.
    /// </summary>
    public class Prayer : GameEntity
    {
        /// <summary>
        /// Gets or sets the required level to use this <see cref="Prayer"/>.
        /// </summary>
        /// <value>The required level.</value>
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how fast this <see cref="Prayer"/> drains.
        /// </summary>
        /// <value>The drain rate.</value>
        public int DrainRate { get; set; }
    }
}
