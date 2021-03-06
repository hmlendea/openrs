﻿using NuciXNA.DataAccess.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    /// <summary>
    /// Non-player character entity.
    /// </summary>
    public class NpcEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the sprites.
        /// </summary>
        /// <value>The sprites.</value>
        public int[] Sprites { get; set; }

        /// <summary>
        /// Gets or sets the hair colour.
        /// </summary>
        /// <value>The hair colour.</value>
        public int HairColour { get; set; }

        /// <summary>
        /// Gets or sets the top colour.
        /// </summary>
        /// <value>The top colour.</value>
        public int TopColour { get; set; }

        /// <summary>
        /// Gets or sets the bottom colour.
        /// </summary>
        /// <value>The bottom colour.</value>
        public int TrousersColour { get; set; }

        /// <summary>
        /// Gets or sets the skin colour.
        /// </summary>
        /// <value>The skin colour.</value>
        public int SkinColour { get; set; }

        public int Camera1 { get; set; }

        public int Camera2 { get; set; }

        /// <summary>
        /// Gets or sets the walk model.
        /// </summary>
        /// <value>The walk model.</value>
        public int WalkModel { get; set; }

        /// <summary>
        /// Gets or sets the combat model.
        /// </summary>
        /// <value>The combat model.</value>
        public int CombatModel { get; set; }

        /// <summary>
        /// Gets or sets the combat sprite.
        /// </summary>
        /// <value>The combat sprite.</value>
        public int CombatSprite { get; set; }

        /// <summary>
        /// Gets or sets the health level.
        /// </summary>
        /// <value>The health level.</value>
        public int HealthLevel { get; set; }

        /// <summary>
        /// Gets or sets the attack level.
        /// </summary>
        /// <value>The attack level.</value>
        public int AttackLevel { get; set; }

        /// <summary>
        /// Gets or sets the defence level.
        /// </summary>
        /// <value>The defence level.</value>
        public int DefenceLevel { get; set; }

        /// <summary>
        /// Gets or sets the strength level.
        /// </summary>
        /// <value>The strength level.</value>
        public int StrengthLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long it takes for this <see cref="NpcEntity"/> to respawn.
        /// </summary>
        /// <value>The respawn time.</value>
        public int RespawnTime { get; set; }

        // TODO: Convert to bool
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NpcEntity"/> is attackable.
        /// </summary>
        /// <value><c>true</c> if it is attackable; otherwise, <c>false</c>.</value>
        public int IsAttackable { get; set; }

        // TODO: Convert to bool
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NpcEntity"/> is aggressive.
        /// </summary>
        /// <value><c>true</c> if it is aggressive; otherwise, <c>false</c>.</value>
        public int IsAggressive { get; set; }

        /// <summary>
        /// Gets or sets the drops.
        /// </summary>
        /// <value>The drops.</value>
        public ItemDropEntity[] Drops { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcEntity"/> class.
        /// </summary>
        public NpcEntity()
        {
            Sprites = new int[12];
        }
    }
}
