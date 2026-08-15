using System.Collections.Generic;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class DomainModelTests
    {
        [Test]
        public void GivenAnimationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Animation model = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                CharacterColour = 8,
                GenderModel = 16,
                HasAttackFrames = true,
                HasFemaleFrames = true,
                SpriteIndex = 32,
            };

            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("RuneScape"));
            Assert.That(model.CharacterColour, Is.EqualTo(8));
            Assert.That(model.GenderModel, Is.EqualTo(16));
            Assert.That(model.HasAttackFrames);
            Assert.That(model.HasFemaleFrames);
            Assert.That(model.SpriteIndex, Is.EqualTo(32));
        }

        [Test]
        public void GivenElevationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Elevation model = new() { V1Id = 4, Roof = 8, Colour = 16 };

            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Roof, Is.EqualTo(8));
            Assert.That(model.Colour, Is.EqualTo(16));
        }

        [Test]
        public void GivenEntityValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            GameEntityInstance model = new() { Id = 4, Index = 8 };

            Assert.That(model.Id, Is.EqualTo(4));
            Assert.That(model.Index, Is.EqualTo(8));
        }

        [Test]
        public void GivenGameObjectLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Point2D expectedLocation = new(4, 8);
            GameObjectLocation model = new()
            {
                Location = expectedLocation,
                Direction = 16,
                Type = GameObjectType.WallObject,
            };

            Assert.That(model.Location, Is.EqualTo(expectedLocation));
            Assert.That(model.Direction, Is.EqualTo(16));
            Assert.That(model.Type, Is.EqualTo(GameObjectType.WallObject));
        }

        [Test]
        public void GivenTextureValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            GameTexture model = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                SubName = "Dark Souls III",
            };

            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("RuneScape"));
            Assert.That(model.SubName, Is.EqualTo("Dark Souls III"));
        }

        [Test]
        public void GivenInventoryItemValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            InventoryItem model = new()
            {
                Id = "RuneScape",
                Index = 4,
                Quantity = 8,
                IsEquipped = true,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.Index, Is.EqualTo(4));
            Assert.That(model.Quantity, Is.EqualTo(8));
            Assert.That(model.IsEquipped);
        }

        [Test]
        public void GivenItemValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Item model = new()
            {
                Id = "RuneScape",
                V1Id = 4,
                Name = "Dark Souls III",
                Description = "Minecraft",
                Command = "Terraria",
                BasePrice = 8,
                SpriteId = 16,
                InventoryPicture = 32,
                SpriteName = "Flappy Bird",
                PictureMask = 42,
                IsEquipable = 48,
                IsPremium = true,
                IsSpecial = true,
                IsStackable = true,
                IsUnused = true,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Description, Is.EqualTo("Minecraft"));
            Assert.That(model.Command, Is.EqualTo("Terraria"));
            Assert.That(model.BasePrice, Is.EqualTo(8));
            Assert.That(model.SpriteId, Is.EqualTo(16));
            Assert.That(model.InventoryPicture, Is.EqualTo(32));
            Assert.That(model.SpriteName, Is.EqualTo("Flappy Bird"));
            Assert.That(model.PictureMask, Is.EqualTo(42));
            Assert.That(model.IsEquipable, Is.EqualTo(48));
            Assert.That(model.IsPremium);
            Assert.That(model.IsSpecial);
            Assert.That(model.IsStackable);
            Assert.That(model.IsUnused);
        }

        [Test]
        public void GivenItemDropValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            ItemDrop model = new() { ItemId = "RuneScape", Amount = 4, Weight = 8 };

            Assert.That(model.ItemId, Is.EqualTo("RuneScape"));
            Assert.That(model.Amount, Is.EqualTo(4));
            Assert.That(model.Weight, Is.EqualTo(8));
        }

        [Test]
        public void GivenItemLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Point2D expectedCoordinates = new(4, 8);
            ItemLocation model = new()
            {
                Id = "RuneScape",
                Coordinates = expectedCoordinates,
                Amount = 16,
                RespawnTime = 32,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.Coordinates, Is.EqualTo(expectedCoordinates));
            Assert.That(model.Amount, Is.EqualTo(16));
            Assert.That(model.RespawnTime, Is.EqualTo(32));
        }

        [Test]
        public void GivenNpcLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Point2D initialCoordinates = new(4, 8);
            Point2D minimumCoordinates = new(16, 32);
            Point2D maximumCoordinates = new(42, 48);
            NpcLocation model = new()
            {
                Id = "RuneScape",
                InitialCoordinates = initialCoordinates,
                MinimumCoordinates = minimumCoordinates,
                MaximumCoordinates = maximumCoordinates,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.InitialCoordinates, Is.EqualTo(initialCoordinates));
            Assert.That(model.MinimumCoordinates, Is.EqualTo(minimumCoordinates));
            Assert.That(model.MaximumCoordinates, Is.EqualTo(maximumCoordinates));
        }

        [Test]
        public void GivenPrayerValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Prayer model = new()
            {
                Id = "RuneScape",
                V1Id = 4,
                Name = "Dark Souls III",
                Description = "Minecraft",
                RequiredLevel = 8,
                DrainRate = 16,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Description, Is.EqualTo("Minecraft"));
            Assert.That(model.RequiredLevel, Is.EqualTo(8));
            Assert.That(model.DrainRate, Is.EqualTo(16));
        }

        [Test]
        public void GivenQuestValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Quest model = new() { Id = "RuneScape", V1Id = 4, Name = "Dark Souls III", Stage = 8 };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Stage, Is.EqualTo(8));
        }

        [Test]
        public void GivenSkillValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Skill model = new()
            {
                Name = "RuneScape",
                CurrentLevel = 4,
                BaseLevel = 8,
                Experience = 16,
            };

            Assert.That(model.Name, Is.EqualTo("RuneScape"));
            Assert.That(model.CurrentLevel, Is.EqualTo(4));
            Assert.That(model.BaseLevel, Is.EqualTo(8));
            Assert.That(model.Experience, Is.EqualTo(16));
        }

        [Test]
        public void GivenSpellValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            IDictionary<int, int> requiredRunes = new Dictionary<int, int> { [4] = 8 };
            Spell model = new()
            {
                Id = "RuneScape",
                V1Id = 4,
                Name = "Dark Souls III",
                Description = "Minecraft",
                RequiredLevel = 8,
                Type = 16,
                RequiredRunes = requiredRunes,
                ExperienceGain = 32,
            };

            Assert.That(model.Id, Is.EqualTo("RuneScape"));
            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Description, Is.EqualTo("Minecraft"));
            Assert.That(model.RequiredLevel, Is.EqualTo(8));
            Assert.That(model.Type, Is.EqualTo(16));
            Assert.That(model.RequiredRunes, Is.SameAs(requiredRunes));
            Assert.That(model.ExperienceGain, Is.EqualTo(32));
        }

        [Test]
        public void GivenTileValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            Tile model = new() { V1Id = 4, Colour = 8, Unknown = 16, Type = 32 };

            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Colour, Is.EqualTo(8));
            Assert.That(model.Unknown, Is.EqualTo(16));
            Assert.That(model.Type, Is.EqualTo(32));
        }

        [Test]
        public void GivenTileValueValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            TileValue model = new() { MapValue = 4, ObjectValue = 8 };

            Assert.That(model.MapValue, Is.EqualTo(4));
            Assert.That(model.ObjectValue, Is.EqualTo(8));
        }

        [Test]
        public void GivenWallObjectValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            WallObject model = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                Command1 = "Minecraft",
                Command2 = "Terraria",
                Type = 8,
                FaceRenderMode = 16,
                ModelHeight = 32,
                ModelFaceBack = 42,
                ModelFaceFront = 48,
            };

            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Name, Is.EqualTo("RuneScape"));
            Assert.That(model.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Command1, Is.EqualTo("Minecraft"));
            Assert.That(model.Command2, Is.EqualTo("Terraria"));
            Assert.That(model.Type, Is.EqualTo(8));
            Assert.That(model.FaceRenderMode, Is.EqualTo(16));
            Assert.That(model.ModelHeight, Is.EqualTo(32));
            Assert.That(model.ModelFaceBack, Is.EqualTo(42));
            Assert.That(model.ModelFaceFront, Is.EqualTo(48));
        }

        [Test]
        public void GivenWorldObjectValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            WorldObject model = new()
            {
                Name = "RuneScape",
                Description = "Dark Souls III",
                Id = "Minecraft",
                V1Id = 4,
                Command1 = "Terraria",
                Command2 = "Flappy Bird",
                Type = 8,
                Width = 16,
                Height = 32,
                GroundItemElevationOffset = 42,
                ModelName = "Among Us",
                ModelIndex = 48,
            };

            Assert.That(model.Name, Is.EqualTo("RuneScape"));
            Assert.That(model.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(model.Id, Is.EqualTo("Minecraft"));
            Assert.That(model.V1Id, Is.EqualTo(4));
            Assert.That(model.Command1, Is.EqualTo("Terraria"));
            Assert.That(model.Command2, Is.EqualTo("Flappy Bird"));
            Assert.That(model.Type, Is.EqualTo(8));
            Assert.That(model.Width, Is.EqualTo(16));
            Assert.That(model.Height, Is.EqualTo(32));
            Assert.That(model.GroundItemElevationOffset, Is.EqualTo(42));
            Assert.That(model.ModelName, Is.EqualTo("Among Us"));
            Assert.That(model.ModelIndex, Is.EqualTo(48));
        }

        [Test]
        public void GivenWorldTileValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            WorldTile model = new()
            {
                Elevation = 4,
                Texture = 8,
                OverlayTextureId = 16,
                RoofTexture = 32,
                HorizontalWallId = 42,
                VerticalWallId = 48,
                DiagonalWallId = 64,
            };

            Assert.That(model.Elevation, Is.EqualTo(4));
            Assert.That(model.Texture, Is.EqualTo(8));
            Assert.That(model.OverlayTextureId, Is.EqualTo(16));
            Assert.That(model.RoofTexture, Is.EqualTo(32));
            Assert.That(model.HorizontalWallId, Is.EqualTo(42));
            Assert.That(model.VerticalWallId, Is.EqualTo(48));
            Assert.That(model.DiagonalWallId, Is.EqualTo(64));
        }
    }
}