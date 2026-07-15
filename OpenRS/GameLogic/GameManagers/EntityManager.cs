using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenRS.DataAccess.Repositories;
using OpenRS.GameLogic.Mapping;
using OpenRS.Localisation;
using OpenRS.Models;
using OpenRS.Net.Client.Data;
using OpenRS.Settings;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class EntityManager
    {
        private static string AnimationsFileName => "animations.json";
        private static string ElevationsFileName => "elevations.json";
        private static string ItemsFileName => "items.json";
        private static string NpcsFileName => "npcs.json";
        private static string PrayersFileName => "prayers.json";
        private static string SpellsFileName => "spells.json";
        private static string TexturesFileName => "textures.json";
        private static string TilesFileName => "tiles.json";
        private static string WallObjectsFileName => "wall_objects.json";
        private static string WorldObjectsFileName => "world_objects.json";

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

        public int AnimationCount => animations.Count;

        public int ElevationCount => elevations.Count;

        public int ItemCount => items.Count;

        public int NpcCount => npcs.Count;

        public int ObjectModelCount => GameData.ModelCount;

        public int PrayerCount => prayers.Count;

        public int SpellCount => spells.Count;

        public int SpellProjectileCount { get; private set; }

        public int TextureCount => textures.Count;

        public int TileCount => tiles.Count;

        public int WallObjectCount => wallObjects.Count;

        public int WorldObjectCount => worldObjects.Count;

        public int HighestLoadedPicture { get; private set; }

        public void LoadItems()
        {
            ItemRepository itemRepository = new(GetDataFilePath(ItemsFileName));
            items = [.. itemRepository.GetAll()
                .ToServiceModels()
                .OrderBy(item => item.V1Id)];

            if (!Config.MembersFeatures)
            {
                foreach (Item item in items)
                {
                    if (item.IsPremium)
                    {
                        item.Name = LocalisationManager.GetString(
                            "entity_manager.members_object");
                        item.Description = LocalisationManager.GetString(
                            "entity_manager.members_object_description");
                        item.BasePrice = 0;
                        item.Command = "";
                        item.IsEquipable = 0;
                        item.IsSpecial = true;
                    }
                }
            }

            HighestLoadedPicture = 0;

            if (items.Count > 0)
            {
                HighestLoadedPicture = items.Max(item => item.InventoryPicture) + 1;
            }
        }

        public void LoadNpcs()
        {
            NpcRepository npcRepository = new(GetDataFilePath(NpcsFileName));
            npcs = [.. npcRepository.GetAll()
                .ToServiceModels()
                .OrderBy(npc => int.Parse(npc.Id))];
        }

        public void LoadSpells(int projectileCount)
        {
            SpellProjectileCount = projectileCount;
            SpellRepository spellRepository = new(GetDataFilePath(SpellsFileName));
            spells = [.. spellRepository.GetAll()
                .ToServiceModels()
                .OrderBy(spell => int.Parse(spell.Id))];
        }

        public void LoadAnimations()
        {
            AnimationRepository animationRepository = new(GetDataFilePath(AnimationsFileName));
            animations = [.. animationRepository.GetAll()
                .OrderBy(animation => int.Parse(animation.Id))
                .ToServiceModels()];
        }

        public void LoadWallObjects()
        {
            WallObjectRepository wallObjectRepository = new(
                GetDataFilePath(WallObjectsFileName));
            wallObjects = [.. wallObjectRepository.GetAll()
                .OrderBy(wallObject => int.Parse(wallObject.Id))
                .ToServiceModels()];
        }

        public void LoadWorldObjects()
        {
            WorldObjectRepository worldObjectRepository = new(
                GetDataFilePath(WorldObjectsFileName));
            worldObjects = [.. worldObjectRepository.GetAll()
                .ToServiceModels()
                .OrderBy(worldObject => int.Parse(worldObject.Id))];

            AssignWorldObjectModelIds();
        }

        public void LoadPrayers()
        {
            PrayerRepository prayerRepository = new(GetDataFilePath(PrayersFileName));
            prayers = [.. prayerRepository.GetAll().ToServiceModels()];
        }

        public void LoadTextures()
        {
            GameTextureRepository textureRepository = new(GetDataFilePath(TexturesFileName));
            textures = [.. textureRepository.GetAll()
                .OrderBy(texture => int.Parse(texture.Id))
                .ToServiceModels()];
        }

        public void LoadElevations()
        {
            ElevationRepository elevationRepository = new(GetDataFilePath(ElevationsFileName));
            elevations = [.. elevationRepository.GetAll()
                .OrderBy(elevation => int.Parse(elevation.Id))
                .ToServiceModels()];
        }

        public void LoadTiles()
        {
            TileRepository tileRepository = new(GetDataFilePath(TilesFileName));
            tiles = [.. tileRepository.GetAll()
                .OrderBy(tile => int.Parse(tile.Id))
                .ToServiceModels()];
        }

        public void LoadContent()
        {
            AnimationRepository animationRepository = new(GetDataFilePath(AnimationsFileName));
            ElevationRepository elevationRepository = new(GetDataFilePath(ElevationsFileName));
            ItemRepository itemRepository = new(GetDataFilePath(ItemsFileName));
            NpcRepository npcRepository = new(GetDataFilePath(NpcsFileName));
            PrayerRepository prayerRepository = new(GetDataFilePath(PrayersFileName));
            SpellRepository spellRepository = new(GetDataFilePath(SpellsFileName));
            GameTextureRepository textureRepository = new(GetDataFilePath(TexturesFileName));
            TileRepository tileRepository = new(GetDataFilePath(TilesFileName));
            WallObjectRepository wallObjectRepository = new(
                GetDataFilePath(WallObjectsFileName));
            WorldObjectRepository worldObjectRepository = new(
                GetDataFilePath(WorldObjectsFileName));

            animations = [.. animationRepository.GetAll().ToServiceModels()];
            elevations = [.. elevationRepository.GetAll().ToServiceModels()];
            items = [.. itemRepository.GetAll().ToServiceModels()];
            npcs = [.. npcRepository.GetAll().ToServiceModels()];
            prayers = [.. prayerRepository.GetAll().ToServiceModels()];
            spells = [.. spellRepository.GetAll().ToServiceModels()];
            textures = [.. textureRepository.GetAll().ToServiceModels()];
            tiles = [.. tileRepository.GetAll().ToServiceModels()];
            wallObjects = [.. wallObjectRepository.GetAll().ToServiceModels()];
            worldObjects = [.. worldObjectRepository.GetAll().ToServiceModels()];

            AssignWorldObjectModelIds();
        }

        public Animation GetAnimation(int index) => GetByIndex(animations, index);

        public Elevation GetElevation(int index) => GetByIndex(elevations, index);

        public Item GetItem(int index) => GetByIndex(items, index);

        public int GetModelIndex(string model) => GameData.GetModelNameIndex(model);

        public string GetObjectModelName(int index) => GameData.ModelNames[index];

        public Npc GetNpc(int index) => GetByIndex(npcs, index);

        public Prayer GetPrayer(int index) => GetByIndex(prayers, index);

        public Spell GetSpell(int index) => GetByIndex(spells, index);

        public GameTexture GetTexture(int index) => GetByIndex(textures, index);

        public Tile GetTile(int index) => GetByIndex(tiles, index);

        public WallObject GetWallObject(int index) => GetByIndex(wallObjects, index);

        public WorldObject GetWorldObject(int index) => GetByIndex(worldObjects, index);

        public WorldObject GetWorldObject(string id)
            => worldObjects.FirstOrDefault(worldObject => worldObject.Id == id);

        private static string GetDataFilePath(string fileName)
            => Path.Combine(ApplicationPaths.DataDirectory, fileName);

        private static T GetByIndex<T>(List<T> list, int index) where T : class
        {
            if (index < 0 || index >= list.Count)
            {
                return null;
            }

            return list[index];
        }

        private void AssignWorldObjectModelIds()
        {
            foreach (WorldObject worldObject in worldObjects)
            {
                worldObject.ModelIndex = GetModelIndex(worldObject.ModelName);
            }
        }
    }
}
