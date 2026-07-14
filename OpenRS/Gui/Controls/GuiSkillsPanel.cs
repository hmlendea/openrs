using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;

using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSkillsPanel(GameClient client) : GuiControl
    {
        private readonly GameClient client = client;

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
            attackCard = new GuiSkillCard { SkillIcon = "Icons/Skills/attack" };
            healthCard = new GuiSkillCard { SkillIcon = "Icons/Skills/health" };
            miningCard = new GuiSkillCard { SkillIcon = "Icons/Skills/mining" };
            strengthCard = new GuiSkillCard { SkillIcon = "Icons/Skills/strength" };
            agilityCard = new GuiSkillCard { SkillIcon = "Icons/Skills/agility" };
            smithingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/smithing" };
            defenceCard = new GuiSkillCard { SkillIcon = "Icons/Skills/defence" };
            herbloreCard = new GuiSkillCard { SkillIcon = "Icons/Skills/herblore" };
            fishingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/fishing" };
            rangedCard = new GuiSkillCard { SkillIcon = "Icons/Skills/ranged" };
            thievingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/thieving" };
            cookingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/cooking" };
            prayerCard = new GuiSkillCard { SkillIcon = "Icons/Skills/prayer" };
            craftingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/crafting" };
            firemakingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/firemaking" };
            magicCard = new GuiSkillCard { SkillIcon = "Icons/Skills/magic" };
            fletchingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/fletching" };
            woodcuttingCard = new GuiSkillCard { SkillIcon = "Icons/Skills/woodcutting" };

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

        protected override void DoUnloadContent()
        {
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenLocations();
            UpdateLevels();
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void SetChildrenLocations()
        {
            int horizontalSpacing = 1;
            int verticalSpacing = 1;

            attackCard.Location = new(
                (Size.Width - 3 * attackCard.Size.Width - 2 * horizontalSpacing) / 2,
                (Size.Width - 3 * attackCard.Size.Width - 2 * horizontalSpacing) / 2);

            healthCard.Location = new(
                attackCard.ClientRectangle.Right + horizontalSpacing,
                attackCard.ClientRectangle.Top);

            miningCard.Location = new(
                healthCard.ClientRectangle.Right + horizontalSpacing,
                healthCard.ClientRectangle.Top);

            strengthCard.Location = new(
                attackCard.ClientRectangle.Left,
                attackCard.ClientRectangle.Bottom + verticalSpacing);

            agilityCard.Location = new(
                strengthCard.ClientRectangle.Right + horizontalSpacing,
                strengthCard.ClientRectangle.Top);

            smithingCard.Location = new(
                agilityCard.ClientRectangle.Right + horizontalSpacing,
                agilityCard.ClientRectangle.Top);

            defenceCard.Location = new(
                strengthCard.ClientRectangle.Left,
                strengthCard.ClientRectangle.Bottom + verticalSpacing);

            herbloreCard.Location = new(
                defenceCard.ClientRectangle.Right + horizontalSpacing,
                defenceCard.ClientRectangle.Top);

            fishingCard.Location = new(
                herbloreCard.ClientRectangle.Right + horizontalSpacing,
                herbloreCard.ClientRectangle.Top);

            rangedCard.Location = new(
                defenceCard.ClientRectangle.Left,
                defenceCard.ClientRectangle.Bottom + verticalSpacing);

            thievingCard.Location = new(
                rangedCard.ClientRectangle.Right + horizontalSpacing,
                rangedCard.ClientRectangle.Top);

            cookingCard.Location = new(
                thievingCard.ClientRectangle.Right + horizontalSpacing,
                thievingCard.ClientRectangle.Top);

            prayerCard.Location = new(
                rangedCard.ClientRectangle.Left,
                rangedCard.ClientRectangle.Bottom + verticalSpacing);

            craftingCard.Location = new(
                prayerCard.ClientRectangle.Right + horizontalSpacing,
                prayerCard.ClientRectangle.Top);

            firemakingCard.Location = new(
                craftingCard.ClientRectangle.Right + horizontalSpacing,
                prayerCard.ClientRectangle.Top);

            magicCard.Location = new(
                prayerCard.ClientRectangle.Left,
                prayerCard.ClientRectangle.Bottom + verticalSpacing);

            fletchingCard.Location = new(
                magicCard.ClientRectangle.Right + horizontalSpacing,
                magicCard.ClientRectangle.Top);

            woodcuttingCard.Location = new(
                fletchingCard.ClientRectangle.Right + horizontalSpacing,
                fletchingCard.ClientRectangle.Top);
        }

        private void UpdateLevels()
        {
            Models.Skill[] skills = client.Skills;

            if (skills.Length == 0)
            {
                return;
            }

            attackCard.BaseLevel = skills[0].BaseLevel;
            attackCard.CurrentLevel = skills[0].CurrentLevel;
            attackCard.Experience = skills[0].Experience;

            healthCard.BaseLevel = skills[3].BaseLevel;
            healthCard.CurrentLevel = skills[3].CurrentLevel;
            healthCard.Experience = skills[3].Experience;

            miningCard.BaseLevel = skills[14].BaseLevel;
            miningCard.CurrentLevel = skills[14].CurrentLevel;
            miningCard.Experience = skills[14].Experience;

            strengthCard.BaseLevel = skills[2].BaseLevel;
            strengthCard.CurrentLevel = skills[2].CurrentLevel;
            strengthCard.Experience = skills[2].Experience;

            agilityCard.BaseLevel = skills[16].BaseLevel;
            agilityCard.CurrentLevel = skills[16].CurrentLevel;
            agilityCard.Experience = skills[16].Experience;

            smithingCard.BaseLevel = skills[13].BaseLevel;
            smithingCard.CurrentLevel = skills[13].CurrentLevel;
            smithingCard.Experience = skills[13].Experience;

            defenceCard.BaseLevel = skills[1].BaseLevel;
            defenceCard.CurrentLevel = skills[1].CurrentLevel;
            defenceCard.Experience = skills[1].Experience;

            herbloreCard.BaseLevel = skills[15].BaseLevel;
            herbloreCard.CurrentLevel = skills[15].CurrentLevel;
            herbloreCard.Experience = skills[15].Experience;

            fishingCard.BaseLevel = skills[10].BaseLevel;
            fishingCard.CurrentLevel = skills[10].CurrentLevel;
            fishingCard.Experience = skills[10].Experience;

            rangedCard.BaseLevel = skills[4].BaseLevel;
            rangedCard.CurrentLevel = skills[4].CurrentLevel;
            rangedCard.Experience = skills[4].Experience;

            thievingCard.BaseLevel = skills[17].BaseLevel;
            thievingCard.CurrentLevel = skills[17].CurrentLevel;
            thievingCard.Experience = skills[17].Experience;

            cookingCard.BaseLevel = skills[7].BaseLevel;
            cookingCard.CurrentLevel = skills[7].CurrentLevel;
            cookingCard.Experience = skills[7].Experience;

            prayerCard.BaseLevel = skills[5].BaseLevel;
            prayerCard.CurrentLevel = skills[5].CurrentLevel;
            prayerCard.Experience = skills[5].Experience;

            craftingCard.BaseLevel = skills[12].BaseLevel;
            craftingCard.CurrentLevel = skills[12].CurrentLevel;
            craftingCard.Experience = skills[12].Experience;

            firemakingCard.BaseLevel = skills[11].BaseLevel;
            firemakingCard.CurrentLevel = skills[11].CurrentLevel;
            firemakingCard.Experience = skills[11].Experience;

            magicCard.BaseLevel = skills[6].BaseLevel;
            magicCard.CurrentLevel = skills[6].CurrentLevel;
            magicCard.Experience = skills[6].Experience;

            fletchingCard.BaseLevel = skills[9].BaseLevel;
            fletchingCard.CurrentLevel = skills[9].CurrentLevel;
            fletchingCard.Experience = skills[9].Experience;

            woodcuttingCard.BaseLevel = skills[8].BaseLevel;
            woodcuttingCard.CurrentLevel = skills[8].CurrentLevel;
            woodcuttingCard.Experience = skills[8].Experience;
        }
    }
}
