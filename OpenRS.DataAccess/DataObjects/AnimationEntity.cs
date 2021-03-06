﻿using NuciXNA.DataAccess.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    /// <summary>
    /// Animation entity.
    /// </summary>
    public class AnimationEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the character colour.
        /// </summary>
        /// <value>The character colour.</value>
        public int CharacterColour { get; set; }

        /// <summary>
        /// Gets or sets the gender model.
        /// </summary>
        /// <value>The gender model.</value>
        public int GenderModel { get; set; }

        public int HasA { get; set; }

        public int HasF { get; set; }

        public int Number { get; set; }
    }
}
