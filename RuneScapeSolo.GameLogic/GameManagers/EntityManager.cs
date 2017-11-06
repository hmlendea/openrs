using System.Collections.Generic;
using System.IO;
using System.Linq;

using RuneScapeSolo.DataAccess.Repositories;
using RuneScapeSolo.GameLogic.Mapping;
using RuneScapeSolo.Models;
using RuneScapeSolo.Settings;

namespace RuneScapeSolo.GameLogic.GameManagers
{
    public class EntityManager
    {
        static Animation[] animations;
        static GameObject[] objects;
        static Elevation[] elevations;
        static Item[] items;
        static Npc[] npcs;
        static Prayer[] prayers;
        static Spell[] spells;
        static Texture[] textures;
        static Tile[] tiles;
        static WallObject[] wallObjects;

        /// <summary>
        /// Gets the animations count.
        /// </summary>
        /// <value>The animations count.</value>
        public static int AnimationCount => animations.Length;

        /// <summary>
        /// Gets the elevations count.
        /// </summary>
        /// <value>The elevations count.</value>
        public static int ElevationCount => elevations.Length;

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <value>The items count.</value>
        public static int ItemCount => items.Length;

        /// <summary>
        /// Gets the npc count.
        /// </summary>
        /// <value>The npc count.</value>
        public static int NpcCount => npcs.Length;

        /// <summary>
        /// Gets the models count.
        /// </summary>
        /// <value>The models count.</value>
        public static int ModelCount => ObjectCount;

        /// <summary>
        /// Gets the objects count.
        /// </summary>
        /// <value>The objects count.</value>
        public static int ObjectCount => objects.Length;

        /// <summary>
        /// Gets the prayers count.
        /// </summary>
        /// <value>The prayers count.</value>
        public static int PrayerCount => prayers.Length;

        /// <summary>
        /// Gets the spells count.
        /// </summary>
        /// <value>The spells count.</value>
        public static int SpellCount => spells.Length;

        /// <summary>
        /// Gets or sets the spell projectile count.
        /// </summary>
        /// <value>The spell projectile count.</value>
        public static int SpellProjectileCount { get; private set; }

        /// <summary>
        /// Gets the textures count.
        /// </summary>
        /// <value>The textures count.</value>
        public static int TextureCount => textures.Length;

        /// <summary>
        /// Gets the tiles count.
        /// </summary>
        /// <value>The tiles count.</value>
        public static int TileCount => tiles.Length;

        /// <summary>
        /// Gets the wall objects count.
        /// </summary>
        /// <value>The wall objects count.</value>
        public static int WallObjectCount => wallObjects.Length;

        public static int HighestLoadedPicture { get; private set; }

        public static void Load(sbyte[] data)
        {
            string animationsPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "animations.xml");
            string elevationPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "elevations.xml");
            string itemPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "items.xml");
            string npcPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "npcs.xml");
            string objectPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "objects.xml");
            string prayerPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "prayers.xml");
            string spellPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "spells.xml");
            string texturePath = Path.Combine(ApplicationPaths.EntitiesDirectory, "textures.xml");
            string tilePath = Path.Combine(ApplicationPaths.EntitiesDirectory, "tiles.xml");
            string wallObjectPath = Path.Combine(ApplicationPaths.EntitiesDirectory, "wall_objects.xml");

            AnimationRepository animationRepository = new AnimationRepository(animationsPath);
            ElevationRepository elevationRepository = new ElevationRepository(elevationPath);
            ItemRepository itemRepository = new ItemRepository(itemPath);
            NpcRepository npcRepository = new NpcRepository(npcPath);
            GameObjectRepository objectRepository = new GameObjectRepository(objectPath);
            PrayerRepository prayerRepository = new PrayerRepository(prayerPath);
            SpellRepository spellRepository = new SpellRepository(spellPath);
            TextureRepository textureRepository = new TextureRepository(texturePath);
            TileRepository tileRepository = new TileRepository(tilePath);
            WallObjectRepository wallObjectRepository = new WallObjectRepository(wallObjectPath);

            animations = animationRepository.GetAll().ToDomainModels().ToArray();
            elevations = elevationRepository.GetAll().ToDomainModels().ToArray();
            items = itemRepository.GetAll().ToDomainModels().ToArray();
            npcs = npcRepository.GetAll().ToDomainModels().ToArray();
            objects = objectRepository.GetAll().ToDomainModels().ToArray();
            prayers = prayerRepository.GetAll().ToDomainModels().ToArray();
            spells = spellRepository.GetAll().ToDomainModels().ToArray();
            textures = textureRepository.GetAll().ToDomainModels().ToArray();
            tiles = tileRepository.GetAll().ToDomainModels().ToArray();
            wallObjects = wallObjectRepository.GetAll().ToDomainModels().ToArray();
        }

        /// <summary>
        /// Gets the animation.
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="id">Identifier.</param>
        public static Animation GetAnimation(int id)
        {
            if (id < 0 || id >= animations.Length)
            {
                return null;
            }

            return animations[id];
        }

        /// <summary>
        /// Gets the elevation.
        /// </summary>
        /// <returns>The elevation.</returns>
        /// <param name="id">Identifier.</param>
        public static Elevation GetElevation(int id)
        {
            if (id < 0 || id >= elevations.Length)
            {
                return null;
            }

            return elevations[id];
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="id">Identifier.</param>
        public static Item GetItem(int id)
        {
            if (id < 0 || id >= items.Length)
            {
                return null;
            }

            return items[id];
        }

        /// <summary>
        /// Gets the spell.
        /// </summary>
        /// <returns>The spell.</returns>
        /// <param name="id">Identifier.</param>
        public static Spell GetSpell(int id)
        {
            if (id < 0 || id >= spells.Length)
            {
                return null;
            }

            return spells[id];
        }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <returns>The model name.</returns>
        /// <param name="id">Identifier.</param>
        public static string GetModelName(int id)
        {
            if (id < 0 || id >= ObjectCount)
            {
                return null;
            }

            return objects[id].Id;
        }

        public static int GetModelNameIndex(string id)
        {
            return objects.ToList().FindIndex(x => x.Id == id);
        }

        /// <summary>
        /// Gets the npc.
        /// </summary>
        /// <returns>The npc.</returns>
        /// <param name="id">Identifier.</param>
        public static Npc GetNpc(int id)
        {
            if (id < 0 || id >= npcs.Length)
            {
                return null;
            }

            return npcs[id];
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="id">Identifier.</param>
        public static GameObject GetObject(string id)
        {
            return objects.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="index">Index.</param>
        public static GameObject GetObject(int index)
        {
            if (index < 0 || index >= objects.Length)
            {
                return null;
            }

            return objects[index];
        }

        /// <summary>
        /// Gets the prayer.
        /// </summary>
        /// <returns>The prayer.</returns>
        /// <param name="id">Identifier.</param>
        public static Prayer GetPrayer(int id)
        {
            if (id < 0 || id >= prayers.Length)
            {
                return null;
            }

            return prayers[id];
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="id">Identifier.</param>
        public static Texture GetTexture(int id)
        {
            if (id < 0 || id >= textures.Length)
            {
                return null;
            }

            return textures[id];
        }

        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <returns>The tile.</returns>
        /// <param name="id">Identifier.</param>
        public static Tile GetTile(int id)
        {
            if (id < 0 || id >= tiles.Length)
            {
                return null;
            }

            return tiles[id];
        }

        /// <summary>
        /// Gets the wall object.
        /// </summary>
        /// <returns>The wall object.</returns>
        /// <param name="id">Identifier.</param>
        public static WallObject GetWallObject(int id)
        {
            if (id < 0 || id >= wallObjects.Length)
            {
                return null;
            }

            return wallObjects[id];
        }
    }
}
