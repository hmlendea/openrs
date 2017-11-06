﻿namespace RuneScapeSolo.Models
{
    /// <summary>
    /// Model.
    /// </summary>
    public class GameModel : GameEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the first command.
        /// </summary>
        /// <value>The first command.</value>
        public string Command1 { get; set; }

        /// <summary>
        /// Gets or sets the second command.
        /// </summary>
        /// <value>The second command.</value>
        public string Command2 { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the ground item variable.
        /// </summary>
        /// <value>The ground item variable.</value>
        public int GroundItemVar { get; set; }

        /// <summary>
        /// Gets or sets the object model.
        /// </summary>
        /// <value>The object model.</value>
        public string ObjectModel { get; set; }

        /// <summary>
        /// Gets or sets the model identifier.
        /// </summary>
        /// <value>The model identifier.</value>
        public int ModelId { get; set; }
    }
}