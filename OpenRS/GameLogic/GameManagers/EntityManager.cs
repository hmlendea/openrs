using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenRS.DataAccess.Repositories;
using OpenRS.GameLogic.Mapping;
using OpenRS.Models;
using OpenRS.Settings;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class EntityManager
    {
        private List<Animation> animations;
        private List<Elevation> elevations;
        private List<Item> items;
        private List<Npc> npcs;
        private List<Prayer> prayers;
        private List<Spell> spells;
        private List<GameTexture> textures;
        private List<Tile> tiles;
        private List<WallObject> wallObjects;
        private List<WorldObject> worldObjects;

        private readonly string[] modelName = new string[5000];
        public int AnimationCount => animations.Count;
        public int ElevationCount => elevations.Count;
        public int ItemCount => items.Count;
        public int NpcCount => npcs.Count;
        public int ObjectModelCount { get; private set; }
        public int PrayerCount => prayers.Count;
        public int SpellCount => spells.Count;
        public int SpellProjectileCount { get; private set; }
        public int TextureCount => textures.Count;
        public int TileCount => tiles.Count;
        public int WallObjectCount => wallObjects.Count;
        public int WorldObjectCount => worldObjects.Count;

        public int HighestLoadedPicture { get; private set; }
        public void LoadContent()
        {
            string animationsPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "animations.xml");
            string elevationPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "elevations.xml");
            string itemPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "items.xml");
            string npcPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "npcs.xml");
            string prayerPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "prayers.xml");
            string spellPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "spells.xml");
            string texturePath = Path.Combine(ApplicationPaths.EntitiesDirectory, "textures.xml");
            string tilePath = Path.Combine(ApplicationPaths.EntitiesDirectory, "tiles.xml");
            string wallObjectPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "wall_objects.xml");
            string worldObjectPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "world_objects.xml");

            AnimationRepository animationRepository = new(animationsPath);
            ElevationRepository elevationRepository = new(elevationPath);
            ItemRepository itemRepository = new(itemPath);
            NpcRepository npcRepository = new(npcPath);
            PrayerRepository prayerRepository = new(prayerPath);
            SpellRepository spellRepository = new(spellPath);
            GameTextureRepository textureRepository = new(texturePath);
            TileRepository tileRepository = new(tilePath);
            WallObjectRepository wallObjectRepository = new(wallObjectPath);
            WorldObjectRepository worldObjectRepository = new(worldObjectPath);

            animations = [.. animationRepository.GetAll().ToDomainModels()];
            elevations = [.. elevationRepository.GetAll().ToDomainModels()];
            items = [.. itemRepository.GetAll().ToDomainModels()];
            npcs = [.. npcRepository.GetAll().ToDomainModels()];
            prayers = [.. prayerRepository.GetAll().ToDomainModels()];
            spells = [.. spellRepository.GetAll().ToDomainModels()];
            textures = [.. textureRepository.GetAll().ToDomainModels()];
            tiles = [.. tileRepository.GetAll().ToDomainModels()];
            wallObjects = [.. wallObjectRepository.GetAll().ToDomainModels()];
            worldObjects = [.. worldObjectRepository.GetAll().ToDomainModels()];

            foreach (WorldObject worldObject in worldObjects)
            {
                worldObject.ModelId = GetModelIndex(worldObject.ObjectModel);
            }
        }
        public Animation GetAnimation(int index)
        {
            if (index < 0 || index >= AnimationCount)
            {
                return null;
            }

            return animations[index];
        }
        public Elevation GetElevation(int index)
        {
            if (index < 0 || index >= ElevationCount)
            {
                return null;
            }

            return elevations[index];
        }
        public Item GetItem(int index)
        {
            if (index < 0 || index >= ItemCount)
            {
                return null;
            }

            return items[index];
        }

        public int GetModelIndex(string model)
        {
            if (model.ToLower() == "na")
            {
                return 0;
            }

            for (int i = 0; i < ObjectModelCount; i += 1)
            {
                if (modelName[i].ToLower() == model)
                {
                    return i;
                }
            }

            modelName[ObjectModelCount] = model;
            ObjectModelCount += 1;

            return ObjectModelCount - 1;
        }
        public Npc GetNpc(int index)
        {
            if (index < 0 || index >= NpcCount)
            {
                return null;
            }

            return npcs[index];
        }

        public string GetObjectModelName(int index) => modelName[index];
        public Prayer GetPrayer(int index)
        {
            if (index < 0 || index >= PrayerCount)
            {
                return null;
            }

            return prayers[index];
        }
        public Spell GetSpell(int index)
        {
            if (index < 0 || index >= SpellCount)
            {
                return null;
            }

            return spells[index];
        }
        public GameTexture GetTexture(int index)
        {
            if (index < 0 || index >= TextureCount)
            {
                return null;
            }

            return textures[index];
        }
        public Tile GetTile(int index)
        {
            if (index < 0 || index >= TileCount)
            {
                return null;
            }

            return tiles[index];
        }
        public WallObject GetWallObject(int index)
        {
            if (index < 0 || index >= WallObjectCount)
            {
                return null;
            }

            return wallObjects[index];
        }
        public WorldObject GetWorldObject(int index)
        {
            if (index < 0 || index >= WorldObjectCount)
            {
                return null;
            }

            return worldObjects[index];
        }
        public WorldObject GetWorldObject(string id) => worldObjects.FirstOrDefault(worldObject => worldObject.Id == id);
    }
}
