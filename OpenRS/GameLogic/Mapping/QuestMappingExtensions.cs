using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class QuestMappingExtensions
    {
        internal static Quest ToDomainModel(this QuestEntity questEntity) => new()
        {
            Id = questEntity.Id,
            Name = questEntity.Name
        };
        internal static QuestEntity ToDataObject(this Quest quest) => new()
        {
            Id = quest.Id,
            Name = quest.Name
        };
        internal static IEnumerable<Quest> ToDomainModels(
            this IEnumerable<QuestEntity> questEntities)
            => questEntities.Select(questEntity => questEntity.ToDomainModel());

        internal static IEnumerable<QuestEntity> ToDataObjects(
            this IEnumerable<Quest> quests)
            => quests.Select(quest => quest.ToDataObject());
    }
}
