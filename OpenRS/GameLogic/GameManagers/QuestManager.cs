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
        private static string QuestsFileName => "quests.json";

        private List<Quest> quests;

        public int QuestsCount => quests.Count;

        public int QuestPoints { get; set; }

        public QuestManager()
        {
            LoadQuests();
        }

        public Quest GetQuest(string id) => quests.FirstOrDefault(quest => quest.Id == id);

        public string[] GetNames() => [.. quests.Select(quest => quest.Name)];

        public void SetStage(string id, int stage) => GetQuest(id).Stage = stage;

        private void LoadQuests()
        {
            QuestRepository questRepository = new(
                Path.Combine(ApplicationPaths.DataDirectory, QuestsFileName));

            quests = [.. questRepository.GetAll().ToServiceModels()];
        }
    }
}

