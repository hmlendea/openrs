using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Quest mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class QuestMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="questEntity">Quest entity.</param>
        internal static Quest ToDomainModel(this QuestEntity questEntity) => new()
        {
            Id = questEntity.Id,
            Name = questEntity.Name
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="quest">Quest.</param>
        internal static QuestEntity ToDataObject(this Quest quest) => new()
        {
            Id = quest.Id,
            Name = quest.Name
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="questEntities">Quest entities.</param>
        internal static IEnumerable<Quest> ToDomainModels(
            this IEnumerable<QuestEntity> questEntities)
            => questEntities.Select(questEntity => questEntity.ToDomainModel());

        internal static IEnumerable<QuestEntity> ToDataObjects(
            this IEnumerable<Quest> quests)
            => quests.Select(quest => quest.ToDataObject());
    }
}
