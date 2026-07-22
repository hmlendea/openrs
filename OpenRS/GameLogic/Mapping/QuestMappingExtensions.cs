using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class QuestMappingExtensions
    {
        internal static Quest ToServiceModel(this QuestEntity questEntity) => new()
        {
            Id = questEntity.Id,
            V1Id = questEntity.V1Id,
            Name = LocalisationManager.GetString(questEntity.Name)
        };

        internal static QuestEntity ToDataObject(this Quest quest) => new()
        {
            Id = quest.Id,
            V1Id = quest.V1Id,
            Name = quest.Name
        };

        internal static IEnumerable<Quest> ToServiceModels(
            this IEnumerable<QuestEntity> questEntities)
            => questEntities.Select(questEntity => questEntity.ToServiceModel());

        internal static IEnumerable<QuestEntity> ToDataObjects(
            this IEnumerable<Quest> quests)
            => quests.Select(quest => quest.ToDataObject());
    }
}
