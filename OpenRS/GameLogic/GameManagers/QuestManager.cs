using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenRS.DataAccess.Repositories;
using OpenRS.GameLogic.Mapping;
using OpenRS.Models;
using OpenRS.Settings;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class QuestManager
    {
        private List<Quest> quests;

        public int QuestsCount => quests.Count;

        public int QuestPoints { get; set; }

        public QuestManager()
        {
            LoadQuests();
        }

        public Quest GetQuest(string id) => quests.FirstOrDefault(quest => quest.Id == id);

        public string[] GetNames()
            => [.. quests.Select(quest => quest.Name)];

        public void SetStage(string id, int stage)
        {
            Quest quest = GetQuest(id);

            quest.Stage = stage;
        }

        private void LoadQuests()
        {
            string questRepositoryPath = Path.Combine(ApplicationPaths.DataDirectory, "quests.json");
            QuestRepository questRepository = new(questRepositoryPath);

            quests = [.. questRepository.GetAll().ToDomainModels()];
        }
    }
}
