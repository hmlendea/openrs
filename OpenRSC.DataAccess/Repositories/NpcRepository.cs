using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// Npc repository implementation.
    /// </summary>
    public class NpcRepository : XmlRepository<NpcEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public NpcRepository(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Updates the specified npc.
        /// </summary>
        /// <param name="entity">Npc.</param>
        public override void Update(NpcEntity entity)
        {
            LoadEntitiesIfNeeded();

            NpcEntity entityToUpdate = Get(entity.Id);
            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(NpcEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.Command = entity.Command;
            entityToUpdate.Sprites = entity.Sprites;
            entityToUpdate.HairColour = entity.HairColour;
            entityToUpdate.TopColour = entity.TopColour;
            entityToUpdate.TrousersColour = entity.TrousersColour;
            entityToUpdate.SkinColour = entity.SkinColour;
            entityToUpdate.Camera1 = entity.Camera1;
            entityToUpdate.Camera2 = entity.Camera2;
            entityToUpdate.WalkModel = entity.WalkModel;
            entityToUpdate.CombatModel = entity.CombatModel;
            entityToUpdate.CombatSprite = entity.CombatSprite;
            entityToUpdate.HealthLevel = entity.HealthLevel;
            entityToUpdate.AttackLevel = entity.AttackLevel;
            entityToUpdate.DefenceLevel = entity.DefenceLevel;
            entityToUpdate.StrengthLevel = entity.StrengthLevel;
            entityToUpdate.RespawnTime = entity.RespawnTime;
            entityToUpdate.IsAttackable = entity.IsAttackable;
            entityToUpdate.IsAggressive = entity.IsAggressive;
            entityToUpdate.Drops = entity.Drops;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
