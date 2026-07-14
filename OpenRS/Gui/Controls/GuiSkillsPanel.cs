using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using OpenRS.Models;
using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSkillsPanel(GameClient client) : GuiControl
    {
        private static string SkillIconPathPrefix => "Icons/Skills/";

        private static int CardSpacing => 1;
        private static int ColumnCount => 3;

        private static int AttackSkillIndex => 0;
        private static int DefenceSkillIndex => 1;
        private static int StrengthSkillIndex => 2;
        private static int HealthSkillIndex => 3;
        private static int RangedSkillIndex => 4;
        private static int PrayerSkillIndex => 5;
        private static int MagicSkillIndex => 6;
        private static int CookingSkillIndex => 7;
        private static int WoodcuttingSkillIndex => 8;
        private static int FletchingSkillIndex => 9;
        private static int FishingSkillIndex => 10;
        private static int FiremakingSkillIndex => 11;
        private static int CraftingSkillIndex => 12;
        private static int SmithingSkillIndex => 13;
        private static int MiningSkillIndex => 14;
        private static int HerbloreSkillIndex => 15;
        private static int AgilitySkillIndex => 16;
        private static int ThievingSkillIndex => 17;

        private GuiSkillCard attackCard;
        private GuiSkillCard healthCard;
        private GuiSkillCard miningCard;
        private GuiSkillCard strengthCard;
        private GuiSkillCard agilityCard;
        private GuiSkillCard smithingCard;
        private GuiSkillCard defenceCard;
        private GuiSkillCard herbloreCard;
        private GuiSkillCard fishingCard;
        private GuiSkillCard rangedCard;
        private GuiSkillCard thievingCard;
        private GuiSkillCard cookingCard;
        private GuiSkillCard prayerCard;
        private GuiSkillCard craftingCard;
        private GuiSkillCard firemakingCard;
        private GuiSkillCard magicCard;
        private GuiSkillCard fletchingCard;
        private GuiSkillCard woodcuttingCard;

        protected override void DoLoadContent()
        {
            attackCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "attack" };
            healthCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "health" };
            miningCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "mining" };
            strengthCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "strength" };
            agilityCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "agility" };
            smithingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "smithing" };
            defenceCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "defence" };
            herbloreCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "herblore" };
            fishingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "fishing" };
            rangedCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "ranged" };
            thievingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "thieving" };
            cookingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "cooking" };
            prayerCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "prayer" };
            craftingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "crafting" };
            firemakingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "firemaking" };
            magicCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "magic" };
            fletchingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "fletching" };
            woodcuttingCard = new GuiSkillCard { SkillIcon = SkillIconPathPrefix + "woodcutting" };

            RegisterChildren(
                attackCard,
                healthCard,
                miningCard,
                strengthCard,
                agilityCard,
                smithingCard,
                defenceCard,
                herbloreCard,
                fishingCard,
                rangedCard,
                thievingCard,
                cookingCard,
                prayerCard,
                craftingCard,
                firemakingCard,
                magicCard,
                fletchingCard,
                woodcuttingCard);
            SetChildrenLocations();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenLocations();
            UpdateLevels();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenLocations()
        {
            int gridLeft =
                (Size.Width - ColumnCount * attackCard.Size.Width -
                    (ColumnCount - 1) * CardSpacing) / 2;
            int gridTop = gridLeft;

            attackCard.Location = new Point2D(gridLeft, gridTop);

            healthCard.Location = new Point2D(
                attackCard.ClientRectangle.Right + CardSpacing,
                attackCard.ClientRectangle.Top);

            miningCard.Location = new Point2D(
                healthCard.ClientRectangle.Right + CardSpacing,
                healthCard.ClientRectangle.Top);

            strengthCard.Location = new Point2D(
                attackCard.ClientRectangle.Left,
                attackCard.ClientRectangle.Bottom + CardSpacing);

            agilityCard.Location = new Point2D(
                strengthCard.ClientRectangle.Right + CardSpacing,
                strengthCard.ClientRectangle.Top);

            smithingCard.Location = new Point2D(
                agilityCard.ClientRectangle.Right + CardSpacing,
                agilityCard.ClientRectangle.Top);

            defenceCard.Location = new Point2D(
                strengthCard.ClientRectangle.Left,
                strengthCard.ClientRectangle.Bottom + CardSpacing);

            herbloreCard.Location = new Point2D(
                defenceCard.ClientRectangle.Right + CardSpacing,
                defenceCard.ClientRectangle.Top);

            fishingCard.Location = new Point2D(
                herbloreCard.ClientRectangle.Right + CardSpacing,
                herbloreCard.ClientRectangle.Top);

            rangedCard.Location = new Point2D(
                defenceCard.ClientRectangle.Left,
                defenceCard.ClientRectangle.Bottom + CardSpacing);

            thievingCard.Location = new Point2D(
                rangedCard.ClientRectangle.Right + CardSpacing,
                rangedCard.ClientRectangle.Top);

            cookingCard.Location = new Point2D(
                thievingCard.ClientRectangle.Right + CardSpacing,
                thievingCard.ClientRectangle.Top);

            prayerCard.Location = new Point2D(
                rangedCard.ClientRectangle.Left,
                rangedCard.ClientRectangle.Bottom + CardSpacing);

            craftingCard.Location = new Point2D(
                prayerCard.ClientRectangle.Right + CardSpacing,
                prayerCard.ClientRectangle.Top);

            firemakingCard.Location = new Point2D(
                craftingCard.ClientRectangle.Right + CardSpacing,
                prayerCard.ClientRectangle.Top);

            magicCard.Location = new Point2D(
                prayerCard.ClientRectangle.Left,
                prayerCard.ClientRectangle.Bottom + CardSpacing);

            fletchingCard.Location = new Point2D(
                magicCard.ClientRectangle.Right + CardSpacing,
                magicCard.ClientRectangle.Top);

            woodcuttingCard.Location = new Point2D(
                fletchingCard.ClientRectangle.Right + CardSpacing,
                fletchingCard.ClientRectangle.Top);
        }

        private void UpdateLevels()
        {
            Skill[] skills = client.Skills;

            if (skills.Length == 0)
            {
                return;
            }

            AssignSkillCard(attackCard, skills[AttackSkillIndex]);
            AssignSkillCard(healthCard, skills[HealthSkillIndex]);
            AssignSkillCard(miningCard, skills[MiningSkillIndex]);
            AssignSkillCard(strengthCard, skills[StrengthSkillIndex]);
            AssignSkillCard(agilityCard, skills[AgilitySkillIndex]);
            AssignSkillCard(smithingCard, skills[SmithingSkillIndex]);
            AssignSkillCard(defenceCard, skills[DefenceSkillIndex]);
            AssignSkillCard(herbloreCard, skills[HerbloreSkillIndex]);
            AssignSkillCard(fishingCard, skills[FishingSkillIndex]);
            AssignSkillCard(rangedCard, skills[RangedSkillIndex]);
            AssignSkillCard(thievingCard, skills[ThievingSkillIndex]);
            AssignSkillCard(cookingCard, skills[CookingSkillIndex]);
            AssignSkillCard(prayerCard, skills[PrayerSkillIndex]);
            AssignSkillCard(craftingCard, skills[CraftingSkillIndex]);
            AssignSkillCard(firemakingCard, skills[FiremakingSkillIndex]);
            AssignSkillCard(magicCard, skills[MagicSkillIndex]);
            AssignSkillCard(fletchingCard, skills[FletchingSkillIndex]);
            AssignSkillCard(woodcuttingCard, skills[WoodcuttingSkillIndex]);
        }

        private static void AssignSkillCard(GuiSkillCard card, Skill skill)
        {
            card.BaseLevel = skill.BaseLevel;
            card.CurrentLevel = skill.CurrentLevel;
            card.Experience = skill.Experience;
        }
    }
}
