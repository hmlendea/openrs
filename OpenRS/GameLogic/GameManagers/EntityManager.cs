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

        /// <summary>
        /// Gets the animations count.
        /// </summary>
        /// <value>The animations count.</value>
        public int AnimationCount => animations.Count;

        /// <summary>
        /// Gets the elevations count.
        /// </summary>
        /// <value>The elevations count.</value>
        public int ElevationCount => elevations.Count;

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <value>The items count.</value>
        public int ItemCount => items.Count;

        /// <summary>
        /// Gets the npc count.
        /// </summary>
        /// <value>The npc count.</value>
        public int NpcCount => npcs.Count;

        /// <summary>
        /// Gets the object models count.
        /// </summary>
        /// <value>The object models count.</value>
        public int ObjectModelCount { get; private set; }

        /// <summary>
        /// Gets the prayers count.
        /// </summary>
        /// <value>The prayers count.</value>
        public int PrayerCount => prayers.Count;

        /// <summary>
        /// Gets the spells count.
        /// </summary>
        /// <value>The spells count.</value>
        public int SpellCount => spells.Count;

        /// <summary>
        /// Gets or sets the spell projectile count.
        /// </summary>
        /// <value>The spell projectile count.</value>
        public int SpellProjectileCount { get; private set; }

        /// <summary>
        /// Gets the textures count.
        /// </summary>
        /// <value>The textures count.</value>
        public int TextureCount => textures.Count;

        /// <summary>
        /// Gets the tiles count.
        /// </summary>
        /// <value>The tiles count.</value>
        public int TileCount => tiles.Count;

        /// <summary>
        /// Gets the wall objects count.
        /// </summary>
        /// <value>The wall objects count.</value>
        public int WallObjectCount => wallObjects.Count;

        /// <summary>
        /// Gets the world objects count.
        /// </summary>
        /// <value>The world objects count.</value>
        public int WorldObjectCount => worldObjects.Count;

        public int HighestLoadedPicture { get; private set; }

        /// <summary>
        /// Loads the entities in memory.
        /// </summary>
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

        /// <summary>
        /// Gets the animation.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="index">Identifier.</param>
        public Animation GetAnimation(int index)
        {
            if (index < 0 || index >= AnimationCount)
            {
                return null;
            }

            return animations[index];
        }

        /// <summary>
        /// Gets the elevation.
        /// </summary>
        /// <returns>The elevation.</returns>
        /// <param name="index">Identifier.</param>
        public Elevation GetElevation(int index)
        {
            if (index < 0 || index >= ElevationCount)
            {
                return null;
            }

            return elevations[index];
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="index">Identifier.</param>
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

        /// <summary>
        /// Gets the npc.
        /// </summary>
        /// <returns>The npc.</returns>
        /// <param name="index">Identifier.</param>
        public Npc GetNpc(int index)
        {
            if (index < 0 || index >= NpcCount)
            {
                return null;
            }

            return npcs[index];
        }

        public string GetObjectModelName(int index) => modelName[index];

        /// <summary>
        /// Gets the prayer.
        /// </summary>
        /// <returns>The prayer.</returns>
        /// <param name="index">Identifier.</param>
        public Prayer GetPrayer(int index)
        {
            if (index < 0 || index >= PrayerCount)
            {
                return null;
            }

            return prayers[index];
        }

        /// <summary>
        /// Gets the spell.
        /// </summary>
        /// <returns>The spell.</returns>
        /// <param name="index">Identifier.</param>
        public Spell GetSpell(int index)
        {
            if (index < 0 || index >= SpellCount)
            {
                return null;
            }

            return spells[index];
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="index">Identifier.</param>
        public GameTexture GetTexture(int index)
        {
            if (index < 0 || index >= TextureCount)
            {
                return null;
            }

            return textures[index];
        }

        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <returns>The tile.</returns>
        /// <param name="index">Identifier.</param>
        public Tile GetTile(int index)
        {
            if (index < 0 || index >= TileCount)
            {
                return null;
            }

            return tiles[index];
        }

        /// <summary>
        /// Gets the wall object.
        /// </summary>
        /// <returns>The wall object.</returns>
        /// <param name="index">Identifier.</param>
        public WallObject GetWallObject(int index)
        {
            if (index < 0 || index >= WallObjectCount)
            {
                return null;
            }

            return wallObjects[index];
        }

        /// <summary>
        /// Gets the world object.
        /// </summary>
        /// <returns>The world object.</returns>
        /// <param name="index">Index.</param>
        public WorldObject GetWorldObject(int index)
        {
            if (index < 0 || index >= WorldObjectCount)
            {
                return null;
            }

            return worldObjects[index];
        }

        /// <summary>
        /// Gets the world object.
        /// </summary>
        /// <returns>The world object.</returns>
        /// <param name="id">Identifier.</param>
        public WorldObject GetWorldObject(string id) => worldObjects.FirstOrDefault(worldObject => worldObject.Id == id);
    }
}
