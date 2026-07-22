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

        private EntityLookup<Animation> animations;
        private EntityLookup<Elevation> elevations;
        private EntityLookup<Item> items;
        private EntityLookup<Npc> npcs;
        private EntityLookup<Prayer> prayers;
        private EntityLookup<Spell> spells;
        private EntityLookup<GameTexture> textures;
        private EntityLookup<Tile> tiles;
        private EntityLookup<WallObject> wallObjects;
        private EntityLookup<WorldObject> worldObjects;

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
            items = new EntityLookup<Item>(
                itemRepository.GetAll().ToServiceModels(),
                item => item.V1Id,
                item => item.Id);

            if (!Config.MembersFeatures)
            {
                foreach (Item item in items.Values)
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
                HighestLoadedPicture = items.Values.Max(item => item.InventoryPicture) + 1;
            }
        }

        public void LoadNpcs()
        {
            NpcRepository npcRepository = new(GetDataFilePath(NpcsFileName));
            npcs = new EntityLookup<Npc>(
                npcRepository.GetAll().ToServiceModels(),
                npc => npc.V1Id,
                npc => npc.Id);
        }

        public void LoadSpells(int projectileCount)
        {
            SpellProjectileCount = projectileCount;
            SpellRepository spellRepository = new(GetDataFilePath(SpellsFileName));
            spells = new EntityLookup<Spell>(
                spellRepository.GetAll().ToServiceModels(),
                spell => spell.V1Id,
                spell => spell.Id);
        }

        public void LoadAnimations()
        {
            AnimationRepository animationRepository = new(GetDataFilePath(AnimationsFileName));
            animations = new EntityLookup<Animation>(
                animationRepository.GetAll().ToServiceModels(),
                animation => animation.V1Id);
        }

        public void LoadWallObjects()
        {
            WallObjectRepository wallObjectRepository = new(
                GetDataFilePath(WallObjectsFileName));
            wallObjects = new EntityLookup<WallObject>(
                wallObjectRepository.GetAll().ToServiceModels(),
                wallObject => wallObject.V1Id);
        }

        public void LoadWorldObjects()
        {
            WorldObjectRepository worldObjectRepository = new(
                GetDataFilePath(WorldObjectsFileName));
            worldObjects = new EntityLookup<WorldObject>(
                worldObjectRepository.GetAll().ToServiceModels(),
                worldObject => worldObject.V1Id,
                worldObject => worldObject.Id);

            AssignWorldObjectModelIds();
        }

        public void LoadPrayers()
        {
            PrayerRepository prayerRepository = new(GetDataFilePath(PrayersFileName));
            prayers = new EntityLookup<Prayer>(
                prayerRepository.GetAll().ToServiceModels(),
                prayer => prayer.V1Id,
                prayer => prayer.Id);
        }

        public void LoadTextures()
        {
            GameTextureRepository textureRepository = new(GetDataFilePath(TexturesFileName));
            textures = new EntityLookup<GameTexture>(
                textureRepository.GetAll().ToServiceModels(),
                texture => texture.V1Id);
        }

        public void LoadElevations()
        {
            ElevationRepository elevationRepository = new(GetDataFilePath(ElevationsFileName));
            elevations = new EntityLookup<Elevation>(
                elevationRepository.GetAll().ToServiceModels(),
                elevation => elevation.V1Id);
        }

        public void LoadTiles()
        {
            TileRepository tileRepository = new(GetDataFilePath(TilesFileName));
            tiles = new EntityLookup<Tile>(
                tileRepository.GetAll().ToServiceModels(),
                tile => tile.V1Id);
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

            animations = new EntityLookup<Animation>(
                animationRepository.GetAll().ToServiceModels(),
                animation => animation.V1Id);
            elevations = new EntityLookup<Elevation>(
                elevationRepository.GetAll().ToServiceModels(),
                elevation => elevation.V1Id);
            items = new EntityLookup<Item>(
                itemRepository.GetAll().ToServiceModels(),
                item => item.V1Id,
                item => item.Id);
            npcs = new EntityLookup<Npc>(
                npcRepository.GetAll().ToServiceModels(),
                npc => npc.V1Id,
                npc => npc.Id);
            prayers = new EntityLookup<Prayer>(
                prayerRepository.GetAll().ToServiceModels(),
                prayer => prayer.V1Id,
                prayer => prayer.Id);
            spells = new EntityLookup<Spell>(
                spellRepository.GetAll().ToServiceModels(),
                spell => spell.V1Id,
                spell => spell.Id);
            textures = new EntityLookup<GameTexture>(
                textureRepository.GetAll().ToServiceModels(),
                texture => texture.V1Id);
            tiles = new EntityLookup<Tile>(
                tileRepository.GetAll().ToServiceModels(),
                tile => tile.V1Id);
            wallObjects = new EntityLookup<WallObject>(
                wallObjectRepository.GetAll().ToServiceModels(),
                wallObject => wallObject.V1Id);
            worldObjects = new EntityLookup<WorldObject>(
                worldObjectRepository.GetAll().ToServiceModels(),
                worldObject => worldObject.V1Id,
                worldObject => worldObject.Id);

            AssignWorldObjectModelIds();
        }

        public Animation GetAnimation(int v1Id) => animations.GetByV1Id(v1Id);

        public Elevation GetElevation(int v1Id) => elevations.GetByV1Id(v1Id);

        public Item GetItem(int v1Id) => items.GetByV1Id(v1Id);

        public Item GetItem(string id) => items.GetById(id);

        public int GetModelIndex(string model) => GameData.GetModelNameIndex(model);

        public string GetObjectModelName(int index) => GameData.ModelNames[index];

        public Npc GetNpc(int v1Id) => npcs.GetByV1Id(v1Id);

        public Npc GetNpc(string id) => npcs.GetById(id);

        public Prayer GetPrayer(int v1Id) => prayers.GetByV1Id(v1Id);

        public Prayer GetPrayer(string id) => prayers.GetById(id);

        public Spell GetSpell(int v1Id) => spells.GetByV1Id(v1Id);

        public Spell GetSpell(string id) => spells.GetById(id);

        public GameTexture GetTexture(int v1Id) => textures.GetByV1Id(v1Id);

        public Tile GetTile(int v1Id) => tiles.GetByV1Id(v1Id);

        public WallObject GetWallObject(int v1Id) => wallObjects.GetByV1Id(v1Id);

        public WorldObject GetWorldObject(int v1Id) => worldObjects.GetByV1Id(v1Id);

        public WorldObject GetWorldObject(string id) => worldObjects.GetById(id);

        private static string GetDataFilePath(string fileName)
            => Path.Combine(ApplicationPaths.DataDirectory, fileName);

        private void AssignWorldObjectModelIds()
        {
            foreach (WorldObject worldObject in worldObjects.Values)
            {
                worldObject.ModelIndex = GetModelIndex(worldObject.ModelName);
            }
        }
    }
}
