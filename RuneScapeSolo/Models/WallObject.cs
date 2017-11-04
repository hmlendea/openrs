using System;

namespace RuneScapeSolo.Models
{
    /// <summary>
    /// Wall object.
    /// </summary>
    public class WallObject : GameEntity
    {
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
        /// The type.
        /// </summary>
        public int Type { get; set; }

        public int Unknown { get; set; }

        public int ModelHeight { get; set; }

        public int ModelFaceBack { get; set; }

        public int ModelFaceFront { get; set; }
    }
}
