using NUnit.Framework;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.UnitTests.DataAccess.DataObjects
{
    [TestFixture]
    public sealed class DataObjectTests
    {
        [Test]
        public void GivenAnimationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            AnimationEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                CharacterColour = 8,
                GenderModel = 16,
                HasAttackFrames = 32,
                HasFemaleFrames = 42,
                SpriteIndex = 48,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.CharacterColour, Is.EqualTo(8));
            Assert.That(entity.GenderModel, Is.EqualTo(16));
            Assert.That(entity.HasAttackFrames, Is.EqualTo(32));
            Assert.That(entity.HasFemaleFrames, Is.EqualTo(42));
            Assert.That(entity.SpriteIndex, Is.EqualTo(48));
        }

        [Test]
        public void GivenElevationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            ElevationEntity entity = new() { V1Id = 4, Roof = 8, Colour = 16 };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Roof, Is.EqualTo(8));
            Assert.That(entity.Colour, Is.EqualTo(16));
        }

        [Test]
        public void GivenGameObjectLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            GameObjectLocationEntity entity = new()
            {
                XCoordinate = 4,
                YCoordinate = 8,
                Direction = 16,
                Type = 32,
            };

            Assert.That(entity.XCoordinate, Is.EqualTo(4));
            Assert.That(entity.YCoordinate, Is.EqualTo(8));
            Assert.That(entity.Direction, Is.EqualTo(16));
            Assert.That(entity.Type, Is.EqualTo(32));
        }

        [Test]
        public void GivenTextureValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            GameTextureEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                SubName = "Dark Souls III",
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.SubName, Is.EqualTo("Dark Souls III"));
        }

        [Test]
        public void GivenItemDropValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            ItemDropEntity entity = new() { ItemId = "RuneScape", Amount = 4, Weight = 8 };

            Assert.That(entity.ItemId, Is.EqualTo("RuneScape"));
            Assert.That(entity.Amount, Is.EqualTo(4));
            Assert.That(entity.Weight, Is.EqualTo(8));
        }

        [Test]
        public void GivenItemValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            ItemEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                Command = "Minecraft",
                BasePrice = 8,
                SpriteId = 16,
                InventoryPicture = 32,
                SpriteName = "Terraria",
                PictureMask = 42,
                IsEquipable = 48,
                IsPremium = 64,
                IsSpecial = 96,
                IsStackable = 128,
                IsUnused = 256,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.Command, Is.EqualTo("Minecraft"));
            Assert.That(entity.BasePrice, Is.EqualTo(8));
            Assert.That(entity.SpriteId, Is.EqualTo(16));
            Assert.That(entity.InventoryPicture, Is.EqualTo(32));
            Assert.That(entity.SpriteName, Is.EqualTo("Terraria"));
            Assert.That(entity.PictureMask, Is.EqualTo(42));
            Assert.That(entity.IsEquipable, Is.EqualTo(48));
            Assert.That(entity.IsPremium, Is.EqualTo(64));
            Assert.That(entity.IsSpecial, Is.EqualTo(96));
            Assert.That(entity.IsStackable, Is.EqualTo(128));
            Assert.That(entity.IsUnused, Is.EqualTo(256));
        }

        [Test]
        public void GivenItemLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            ItemLocationEntity entity = new()
            {
                XCoordinate = 4,
                YCoordinate = 8,
                Amount = 16,
                RespawnTime = 32,
            };

            Assert.That(entity.XCoordinate, Is.EqualTo(4));
            Assert.That(entity.YCoordinate, Is.EqualTo(8));
            Assert.That(entity.Amount, Is.EqualTo(16));
            Assert.That(entity.RespawnTime, Is.EqualTo(32));
        }

        [Test]
        public void GivenANewNpcEntity_WhenReadingSprites_ThenAnIndependentCompatibleArrayExists()
        {
            NpcEntity firstEntity = new();
            NpcEntity secondEntity = new();

            Assert.That(firstEntity.Sprites, Has.Length.EqualTo(Npc.SpriteCount));
            Assert.That(firstEntity.Sprites, Is.Not.SameAs(secondEntity.Sprites));
        }

        [Test]
        public void GivenNpcValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            int[] sprites = [4, 8, 16];
            ItemDropEntity[] drops = [new() { ItemId = "RuneScape" }];
            NpcEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                Command = "Minecraft",
                Sprites = sprites,
                HairColour = 8,
                TopColour = 16,
                TrousersColour = 32,
                SkinColour = 42,
                SpriteWidth = 48,
                SpriteHeight = 64,
                WalkAnimationSpeed = 96,
                CombatAnimationSpeed = 128,
                CombatSwingOffset = 256,
                HealthLevel = 4,
                AttackLevel = 8,
                DefenceLevel = 16,
                StrengthLevel = 32,
                RespawnTime = 42,
                IsAttackable = 48,
                IsAggressive = 64,
                Drops = drops,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.Command, Is.EqualTo("Minecraft"));
            Assert.That(entity.Sprites, Is.SameAs(sprites));
            Assert.That(entity.HairColour, Is.EqualTo(8));
            Assert.That(entity.TopColour, Is.EqualTo(16));
            Assert.That(entity.TrousersColour, Is.EqualTo(32));
            Assert.That(entity.SkinColour, Is.EqualTo(42));
            Assert.That(entity.SpriteWidth, Is.EqualTo(48));
            Assert.That(entity.SpriteHeight, Is.EqualTo(64));
            Assert.That(entity.WalkAnimationSpeed, Is.EqualTo(96));
            Assert.That(entity.CombatAnimationSpeed, Is.EqualTo(128));
            Assert.That(entity.CombatSwingOffset, Is.EqualTo(256));
            Assert.That(entity.HealthLevel, Is.EqualTo(4));
            Assert.That(entity.AttackLevel, Is.EqualTo(8));
            Assert.That(entity.DefenceLevel, Is.EqualTo(16));
            Assert.That(entity.StrengthLevel, Is.EqualTo(32));
            Assert.That(entity.RespawnTime, Is.EqualTo(42));
            Assert.That(entity.IsAttackable, Is.EqualTo(48));
            Assert.That(entity.IsAggressive, Is.EqualTo(64));
            Assert.That(entity.Drops, Is.SameAs(drops));
        }

        [Test]
        public void GivenNpcLocationValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            NpcLocationEntity entity = new()
            {
                InitialXCoordinate = 4,
                InitialYCoordinate = 8,
                MinimumXCoordinate = 16,
                MinimumYCoordinate = 32,
                MaximumXCoordinate = 42,
                MaximumYCoordinate = 48,
            };

            Assert.That(entity.InitialXCoordinate, Is.EqualTo(4));
            Assert.That(entity.InitialYCoordinate, Is.EqualTo(8));
            Assert.That(entity.MinimumXCoordinate, Is.EqualTo(16));
            Assert.That(entity.MinimumYCoordinate, Is.EqualTo(32));
            Assert.That(entity.MaximumXCoordinate, Is.EqualTo(42));
            Assert.That(entity.MaximumYCoordinate, Is.EqualTo(48));
        }

        [Test]
        public void GivenPrayerValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            PrayerEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                RequiredLevel = 8,
                DrainRate = 16,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.RequiredLevel, Is.EqualTo(8));
            Assert.That(entity.DrainRate, Is.EqualTo(16));
        }

        [Test]
        public void GivenQuestValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            QuestEntity entity = new() { V1Id = 4, Name = "RuneScape" };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
        }

        [Test]
        public void GivenSpellValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            int[] runeIds = [4, 8];
            int[] runeCounts = [16, 32];
            SpellEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                RequiredLevel = 8,
                Type = 16,
                RuneCount = 32,
                RequiredRunesIds = runeIds,
                RequiredRunesCounts = runeCounts,
                ExperienceGain = 42,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.RequiredLevel, Is.EqualTo(8));
            Assert.That(entity.Type, Is.EqualTo(16));
            Assert.That(entity.RuneCount, Is.EqualTo(32));
            Assert.That(entity.RequiredRunesIds, Is.SameAs(runeIds));
            Assert.That(entity.RequiredRunesCounts, Is.SameAs(runeCounts));
            Assert.That(entity.ExperienceGain, Is.EqualTo(42));
        }

        [Test]
        public void GivenTileValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            TileEntity entity = new() { V1Id = 4, Colour = 8, Unknown = 16, Type = 32 };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Colour, Is.EqualTo(8));
            Assert.That(entity.Unknown, Is.EqualTo(16));
            Assert.That(entity.Type, Is.EqualTo(32));
        }

        [Test]
        public void GivenWallObjectValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            WallObjectEntity entity = new()
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

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.Command1, Is.EqualTo("Minecraft"));
            Assert.That(entity.Command2, Is.EqualTo("Terraria"));
            Assert.That(entity.Type, Is.EqualTo(8));
            Assert.That(entity.FaceRenderMode, Is.EqualTo(16));
            Assert.That(entity.ModelHeight, Is.EqualTo(32));
            Assert.That(entity.ModelFaceBack, Is.EqualTo(42));
            Assert.That(entity.ModelFaceFront, Is.EqualTo(48));
        }

        [Test]
        public void GivenWorldObjectValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            WorldObjectEntity entity = new()
            {
                V1Id = 4,
                Name = "RuneScape",
                Description = "Dark Souls III",
                Command1 = "Minecraft",
                Command2 = "Terraria",
                Type = 8,
                Width = 16,
                Height = 32,
                GroundItemElevationOffset = 42,
                ModelName = "Flappy Bird",
                ModelIndex = 48,
            };

            Assert.That(entity.V1Id, Is.EqualTo(4));
            Assert.That(entity.Name, Is.EqualTo("RuneScape"));
            Assert.That(entity.Description, Is.EqualTo("Dark Souls III"));
            Assert.That(entity.Command1, Is.EqualTo("Minecraft"));
            Assert.That(entity.Command2, Is.EqualTo("Terraria"));
            Assert.That(entity.Type, Is.EqualTo(8));
            Assert.That(entity.Width, Is.EqualTo(16));
            Assert.That(entity.Height, Is.EqualTo(32));
            Assert.That(entity.GroundItemElevationOffset, Is.EqualTo(42));
            Assert.That(entity.ModelName, Is.EqualTo("Flappy Bird"));
            Assert.That(entity.ModelIndex, Is.EqualTo(48));
        }
    }
}