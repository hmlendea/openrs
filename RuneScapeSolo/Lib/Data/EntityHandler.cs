using System.Collections.Generic;
using System.Linq;

using RuneScapeSolo.Models;

namespace RuneScapeSolo.Lib.Data
{
    public class EntityHandler
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

        static List<string> models = new List<string>();
        static int invPictureCount;

        // TODO: Properly handle those fields.
        public static int highestLoadedPicture;
        public static int overlayTextureCount;
        public static int akd;
        public static int ani;
        public static int[] aln;
        public static int[] aki;
        public static int[] roofs;
        public static int[] TileGroundOverlayTexture;
        public static int[] tileGroundOverlayTypes;
        public static string[] modelName = new string[5000];
        static int stringDataIndex;
        static int integerDataIndex;
        static sbyte[] stringData;
        static sbyte[] integerData;

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
        /// Gets the inv picture count.
        /// </summary>
        /// <value>The inv picture count.</value>
        public static int InvPictureCount => invPictureCount;

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
        public static int ModelCount => models.Count;

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

        // TODO: Private set?
        /// <summary>
        /// Gets or sets the spell projectile count.
        /// </summary>
        /// <value>The spell projectile count.</value>
        public static int SpellProjectileCount { get; set; }

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
            if (id < 0 || id >= models.Count)
            {
                return null;
            }

            return models[id];
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
        public static GameObject GetObject(int id)
        {
            if (id < 0 || id >= objects.Length)
            {
                return null;
            }

            return objects[id];
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

        public static int GetModelNameIndex(string str)
        {
            if (str.ToLower().Equals("na"))
            {
                return 0;
            }

            for (int i = 0; i < ModelCount; i++)
            {
                if (models[i].ToLower().Equals(str))
                {
                    return i;
                }
            }

            models.Add(str);
            return models.Count - 1;
        }

        public static void Load(sbyte[] data)
        {
            stringData = DataOperations.loadData("string.dat", 0, data);
            stringDataIndex = 0;

            integerData = DataOperations.loadData("integer.dat", 0, data);
            integerDataIndex = 0;

            // The order of the loading is important
            LoadItems();
            LoadNpcs();
            LoadTextures();
            LoadAnimations();
            LoadObjects();
            LoadWallObjects();

            akd = ReadInt16();
            roofs = new int[akd];
            aln = new int[akd];
            for (int i = 0; i < akd; i++)
            {
                roofs[i] = ReadInt8();
            }

            for (int i = 0; i < akd; i++)
            {
                aln[i] = ReadInt8();
            }

            /*System.out.println("akd:");
            for(int i = 0; i < akd; i++) {
                System.out.println(i + ": " + alm[i] + " " + aln[i]);
            }*/

            overlayTextureCount = ReadInt16();
            TileGroundOverlayTexture = new int[overlayTextureCount];
            tileGroundOverlayTypes = new int[overlayTextureCount];
            aki = new int[overlayTextureCount];
            for (int i = 0; i < overlayTextureCount; i++)
            {
                TileGroundOverlayTexture[i] = ReadInt32();
            }

            for (int i = 0; i < overlayTextureCount; i++)
            {
                tileGroundOverlayTypes[i] = ReadInt8();
            }

            for (int i = 0; i < overlayTextureCount; i++)
            {
                aki[i] = ReadInt8();
            }

            /*System.out.println("overlayTextureCount:");
            for(int i = 0; i < overlayTextureCount; i++) {
                System.out.println(i + ": " + akg[i] + " " + akh[i] + " " + aki[i]);
            }*/

            LoadSpells();
            LoadPrayers();

            stringData = null;
            integerData = null;
        }

        static void LoadItems()
        {
            items = new Item[ReadInt16()];

            for (int i = 0; i < ItemCount; i++)
            {
                items[i] = new Item();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].Name = ReadString();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].Description = ReadString();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].Command = ReadString();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].InventoryPicture = ReadInt16();

                if (items[i].InventoryPicture + 1 > highestLoadedPicture)
                {
                    highestLoadedPicture = items[i].InventoryPicture + 1;
                }
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].BasePrice = ReadInt32();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].IsStackable = ReadInt8();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].IsUnused = ReadInt8();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].IsEquipable = ReadInt16();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].PictureMask = ReadInt32();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].IsSpecial = ReadInt8();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                items[i].IsPremium = ReadInt8();
            }

            for (int i = 0; i < ItemCount; i++)
            {
                if (!Configuration.PREMIUM_FEATURES && items[i].IsPremium == 0)
                {
                    items[i].Name = "Members object";
                    items[i].Description = "You need to be a premium user to use this object";
                    items[i].BasePrice = 0;
                    items[i].Command = "";
                    items[i].IsUnused = 0;
                    items[i].IsEquipable = 0;
                    items[i].IsSpecial = 1;
                }
            }
        }

        static void LoadNpcs()
        {
            npcs = new Npc[ReadInt16()];

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i] = new Npc();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].Name = ReadString();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].Description = ReadString();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].AttackLevel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].StrengthLevel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].HealthLevel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].DefenceLevel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].IsAttackable = ReadInt8();
            }

            for (int l4 = 0; l4 < NpcCount; l4++)
            {
                for (int i5 = 0; i5 < 12; i5++)
                {
                    int sprite = ReadInt8();

                    npcs[l4].Sprites[i5] = sprite;

                    if (sprite == 255)
                    {
                        npcs[l4].Sprites[i5] = -1;
                    }
                    else
                    {
                        npcs[l4].Sprites[i5] = sprite;
                    }
                }

            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].HairColour = ReadInt32();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].TopColour = ReadInt32();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].BottomColour = ReadInt32();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].SkinColour = ReadInt32();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].Camera1 = ReadInt16();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].Camera2 = ReadInt16();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].WalkModel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].CombatModel = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].CombatSprite = ReadInt8();
            }

            for (int i = 0; i < NpcCount; i++)
            {
                npcs[i].Command = ReadString();
            }
        }

        static void LoadTextures()
        {
            textures = new Texture[ReadInt16()];

            for (int i = 0; i < TextureCount; i++)
            {
                textures[i] = new Texture();
            }

            for (int i = 0; i < TextureCount; i++)
            {
                textures[i].Name = ReadString();
            }

            for (int i = 0; i < TextureCount; i++)
            {
                textures[i].SubName = ReadString();
            }
        }

        static void LoadAnimations()
        {
            animations = new Animation[ReadInt16()];

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i] = new Animation();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].Name = ReadString();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].CharacterColour = ReadInt32();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].GenderModel = ReadInt8();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].HasA = ReadInt8();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].HasF = ReadInt8();
            }

            for (int i = 0; i < AnimationCount; i++)
            {
                animations[i].Number = ReadInt8();
            }
        }

        static void LoadObjects()
        {
            objects = new GameObject[ReadInt16()];

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i] = new GameObject();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Name = ReadString();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Description = ReadString();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Command1 = ReadString();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Command2 = ReadString();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].ModelId = GetModelNameIndex(ReadString());
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Width = ReadInt8();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Height = ReadInt8();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].Type = ReadInt8();
            }

            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i].GroundItemVar = ReadInt8();
            }
        }

        static void LoadWallObjects()
        {
            wallObjects = new WallObject[ReadInt16()];

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i] = new WallObject();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Name = ReadString();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Description = ReadString();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Command1 = ReadString();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Command2 = ReadString();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].ModelHeight = ReadInt16();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].ModelFaceBack = ReadInt32();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].ModelFaceFront = ReadInt32();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Type = ReadInt8();
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                wallObjects[i].Unknown = ReadInt8();
            }
        }

        static void LoadSpells()
        {
            SpellProjectileCount = ReadInt16();

            spells = new Spell[ReadInt16()];

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i] = new Spell();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i].Name = ReadString();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i].Description = ReadString();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i].RequiredLevel = ReadInt8();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i].RuneCount = ReadInt8();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                spells[i].Type = ReadInt8();
            }

            for (int i = 0; i < SpellCount; i++)
            {
                int runeTypesCount = ReadInt8();

                spells[i].RequiredRunesIds = new int[runeTypesCount];

                for (int j = 0; j < runeTypesCount; j++)
                {
                    spells[i].RequiredRunesIds[j] = ReadInt16();
                }
            }

            for (int i = 0; i < SpellCount; i++)
            {
                int runeTypesCount = ReadInt8();

                spells[i].RequiredRunesCounts = new int[runeTypesCount];

                for (int j = 0; j < runeTypesCount; j++)
                {
                    spells[i].RequiredRunesCounts[j] = ReadInt8();
                }
            }
        }

        static void LoadPrayers()
        {
            prayers = new Prayer[ReadInt16()];

            for (int i = 0; i < PrayerCount; i++)
            {
                prayers[i] = new Prayer();
            }

            for (int i = 0; i < PrayerCount; i++)
            {
                prayers[i].Name = ReadString();
            }

            for (int i = 0; i < PrayerCount; i++)
            {
                prayers[i].Description = ReadString();
            }

            for (int i = 0; i < PrayerCount; i++)
            {
                prayers[i].RequiredLevel = ReadInt8();
            }

            for (int i = 0; i < PrayerCount; i++)
            {
                prayers[i].DrainRate = ReadInt8();
            }
        }

        static int StoreModel(string name)
        {
            if (name.ToLower().Equals("na"))
            {
                return 0;
            }

            int index = models.IndexOf(name);

            if (index < 0)
            {
                models.Add(name);
                return models.Count - 1;
            }

            return index;
        }

        static int ReadInt8()
        {
            int i = integerData[integerDataIndex] & 0xff;
            integerDataIndex++;

            return i;
        }

        static int ReadInt16()
        {
            int i = DataOperations.getShort(integerData, integerDataIndex);
            integerDataIndex += 2;

            return i;
        }

        static int ReadInt32()
        {
            int i = DataOperations.getInt(integerData, integerDataIndex);
            integerDataIndex += 4;

            if (i > 0x5f5e0ff)
            {
                i = 0x5f5e0ff - i;
            }

            return i;
        }

        static string ReadString()
        {
            string str = string.Empty;

            while (stringData[stringDataIndex] != 0)
            {
                str = str + (char)stringData[stringDataIndex];
                stringDataIndex += 1;
            }

            stringDataIndex++;

            return str;
        }
    }
}
