using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenRS.DataAccess.Repositories;
using OpenRS.GameLogic.Mapping;
using OpenRS.Models;
using OpenRS.Net.Client.Data;
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

        private static int MaximumObjectModelCount => 5000;

        public int AnimationCount => animations.Count;
        public int ElevationCount => elevations.Count;
        public int ItemCount => items.Count;
        public int NpcCount => npcs.Count;
        public int ObjectModelCount => GameData.modelCount;
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
            string itemPath = Path.Combine(ApplicationPaths.DataDirectory, "items.json");
            ItemRepository itemRepository = new(itemPath);
            items = [.. itemRepository.GetAll().ToDomainModels().OrderBy(item => int.Parse(item.Id))];

            if (!Config.MembersFeatures)
            {
                foreach (Item item in items)
                {
                    if (item.IsPremium == 1)
                    {
                        item.Name = "Members object";
                        item.Description = "You need to be a member to use this object";
                        item.BasePrice = 0;
                        item.Command = "";
                        item.IsEquipable = 0;
                        item.IsSpecial = 1;
                    }
                }
            }

            HighestLoadedPicture = items.Count > 0 ? items.Max(i => i.InventoryPicture) + 1 : 0;
        }

        public void LoadNpcs()
        {
            string npcPath = Path.Combine(ApplicationPaths.DataDirectory, "npcs.json");
            NpcRepository npcRepository = new(npcPath);
            npcs = [.. npcRepository.GetAll().ToDomainModels().OrderBy(npc => int.Parse(npc.Id))];
        }

        public void LoadSpells(int projectileCount)
        {
            SpellProjectileCount = projectileCount;
            string spellPath = Path.Combine(ApplicationPaths.DataDirectory, "spells.json");
            SpellRepository spellRepository = new(spellPath);
            spells = [.. spellRepository.GetAll().ToDomainModels().OrderBy(spell => int.Parse(spell.Id))];
        }

        public void LoadAnimations()
        {
            string animationsPath = Path.Combine(ApplicationPaths.DataDirectory, "animations.json");
            AnimationRepository animationRepository = new(animationsPath);
            animations = [.. animationRepository.GetAll().OrderBy(animation => int.Parse(animation.Id)).ToDomainModels()];
        }

        public void LoadWallObjects()
        {
            string wallObjectPath = Path.Combine(ApplicationPaths.DataDirectory, "wall_objects.json");
            WallObjectRepository wallObjectRepository = new(wallObjectPath);
            wallObjects = [.. wallObjectRepository.GetAll().OrderBy(wallObject => int.Parse(wallObject.Id)).ToDomainModels()];
        }

        public void LoadWorldObjects()
        {
            string worldObjectPath = Path.Combine(ApplicationPaths.DataDirectory, "world_objects.json");
            WorldObjectRepository worldObjectRepository = new(worldObjectPath);
            worldObjects = [.. worldObjectRepository.GetAll().ToDomainModels().OrderBy(worldObject => int.Parse(worldObject.Id))];

            foreach (WorldObject worldObject in worldObjects)
            {
                worldObject.ModelId = GetModelIndex(worldObject.ObjectModel);
            }
        }

        public void LoadPrayers()
        {
            string prayerPath = Path.Combine(ApplicationPaths.DataDirectory, "prayers.json");
            PrayerRepository prayerRepository = new(prayerPath);
            prayers = [.. prayerRepository.GetAll().ToDomainModels()];
        }

        public void LoadTextures()
        {
            string texturePath = Path.Combine(ApplicationPaths.DataDirectory, "textures.json");
            GameTextureRepository textureRepository = new(texturePath);
            textures = [.. textureRepository.GetAll().OrderBy(texture => int.Parse(texture.Id)).ToDomainModels()];
        }

        public void LoadElevations()
        {
            string elevationPath = Path.Combine(ApplicationPaths.DataDirectory, "elevations.json");
            ElevationRepository elevationRepository = new(elevationPath);
            elevations = [.. elevationRepository.GetAll().OrderBy(elevation => int.Parse(elevation.Id)).ToDomainModels()];
        }

        public void LoadTiles()
        {
            string tilePath = Path.Combine(ApplicationPaths.DataDirectory, "tiles.json");
            TileRepository tileRepository = new(tilePath);
            tiles = [.. tileRepository.GetAll().OrderBy(tile => int.Parse(tile.Id)).ToDomainModels()];
        }

        public void LoadContent()
        {
            string animationsPath = Path.Combine(ApplicationPaths.DataDirectory, "animations.json");
            string elevationPath = Path.Combine(ApplicationPaths.DataDirectory, "elevations.json");
            string itemPath = Path.Combine(ApplicationPaths.DataDirectory, "items.json");
            string npcPath = Path.Combine(ApplicationPaths.DataDirectory, "npcs.json");
            string prayerPath = Path.Combine(ApplicationPaths.DataDirectory, "prayers.json");
            string spellPath = Path.Combine(ApplicationPaths.DataDirectory, "spells.json");
            string texturePath = Path.Combine(ApplicationPaths.DataDirectory, "textures.json");
            string tilePath = Path.Combine(ApplicationPaths.DataDirectory, "tiles.json");
            string wallObjectPath = Path.Combine(ApplicationPaths.DataDirectory, "wall_objects.json");
            string worldObjectPath = Path.Combine(ApplicationPaths.DataDirectory, "world_objects.json");

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

        public int GetModelIndex(string model) => GameData.GetModelNameIndex(model);

        public string GetObjectModelName(int index) => GameData.modelName[index];

        public Npc GetNpc(int index)
        {
            if (index < 0 || index >= NpcCount)
            {
                return null;
            }

            return npcs[index];
        }

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
