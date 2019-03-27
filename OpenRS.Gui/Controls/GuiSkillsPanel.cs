using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public class GuiSkillsPanel : GuiControl
    {
        readonly GameClient client;

        GuiSkillCard attackCard;
        GuiSkillCard healthCard;
        GuiSkillCard miningCard;
        GuiSkillCard strengthCard;
        GuiSkillCard agilityCard;
        GuiSkillCard smithingCard;
        GuiSkillCard defenceCard;
        GuiSkillCard herbloreCard;
        GuiSkillCard fishingCard;
        GuiSkillCard rangedCard;
        GuiSkillCard thievingCard;
        GuiSkillCard cookingCard;
        GuiSkillCard prayerCard;
        GuiSkillCard craftingCard;
        GuiSkillCard firemakingCard;
        GuiSkillCard magicCard;
        GuiSkillCard fletchingCard;
        GuiSkillCard woodcuttingCard;

        public GuiSkillsPanel(GameClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
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
                woodcuttingCard);
            SetChildrenLocations();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {

        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenLocations();
            UpdateLevels();
        }

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }

        void SetChildrenLocations()
        {
            int spacingX = 1;
            int spacingY = 1;

            attackCard.Location = new Point2D(
                Location.X + (Size.Width - 3 * attackCard.Size.Width - 2 * spacingX) / 2,
                Location.Y + (Size.Width - 3 * attackCard.Size.Width - 2 * spacingX) / 2);

            healthCard.Location = new Point2D(
                attackCard.ClientRectangle.Right + spacingX,
                attackCard.ClientRectangle.Top);

            miningCard.Location = new Point2D(
                healthCard.ClientRectangle.Right + spacingX,
                healthCard.ClientRectangle.Top);

            strengthCard.Location = new Point2D(
                attackCard.ClientRectangle.Left,
                attackCard.ClientRectangle.Bottom + spacingY);

            agilityCard.Location = new Point2D(
                strengthCard.ClientRectangle.Right + spacingX,
                strengthCard.ClientRectangle.Top);

            smithingCard.Location = new Point2D(
                agilityCard.ClientRectangle.Right + spacingX,
                agilityCard.ClientRectangle.Top);

            defenceCard.Location = new Point2D(
                strengthCard.ClientRectangle.Left,
                strengthCard.ClientRectangle.Bottom + spacingY);

            herbloreCard.Location = new Point2D(
                defenceCard.ClientRectangle.Right + spacingX,
                defenceCard.ClientRectangle.Top);

            fishingCard.Location = new Point2D(
                herbloreCard.ClientRectangle.Right + spacingX,
                herbloreCard.ClientRectangle.Top);

            rangedCard.Location = new Point2D(
                defenceCard.ClientRectangle.Left,
                defenceCard.ClientRectangle.Bottom + spacingY);

            thievingCard.Location = new Point2D(
                rangedCard.ClientRectangle.Right + spacingX,
                rangedCard.ClientRectangle.Top);

            cookingCard.Location = new Point2D(
                thievingCard.ClientRectangle.Right + spacingX,
                thievingCard.ClientRectangle.Top);

            prayerCard.Location = new Point2D(
                rangedCard.ClientRectangle.Left,
                rangedCard.ClientRectangle.Bottom + spacingY);

            craftingCard.Location = new Point2D(
                prayerCard.ClientRectangle.Right + spacingX,
                prayerCard.ClientRectangle.Top);

            firemakingCard.Location = new Point2D(
                craftingCard.ClientRectangle.Right + spacingX,
                prayerCard.ClientRectangle.Top);

            magicCard.Location = new Point2D(
                prayerCard.ClientRectangle.Left,
                prayerCard.ClientRectangle.Bottom + spacingY);

            fletchingCard.Location = new Point2D(
                magicCard.ClientRectangle.Right + spacingX,
                magicCard.ClientRectangle.Top);

            woodcuttingCard.Location = new Point2D(
                fletchingCard.ClientRectangle.Right + spacingX,
                fletchingCard.ClientRectangle.Top);
        }

        void UpdateLevels()
        {
            attackCard.BaseLevel = client.Skills[0].BaseLevel;
            attackCard.CurrentLevel = client.Skills[0].CurrentLevel;
            attackCard.Experience = client.Skills[0].Experience;

            healthCard.BaseLevel = client.Skills[3].BaseLevel;
            healthCard.CurrentLevel = client.Skills[3].CurrentLevel;
            healthCard.Experience = client.Skills[3].Experience;

            miningCard.BaseLevel = client.Skills[14].BaseLevel;
            miningCard.CurrentLevel = client.Skills[14].CurrentLevel;
            miningCard.Experience = client.Skills[14].Experience;

            strengthCard.BaseLevel = client.Skills[2].BaseLevel;
            strengthCard.CurrentLevel = client.Skills[2].CurrentLevel;
            strengthCard.Experience = client.Skills[2].Experience;

            agilityCard.BaseLevel = client.Skills[16].BaseLevel;
            agilityCard.CurrentLevel = client.Skills[16].CurrentLevel;
            agilityCard.Experience = client.Skills[16].Experience;

            smithingCard.BaseLevel = client.Skills[13].BaseLevel;
            smithingCard.CurrentLevel = client.Skills[13].CurrentLevel;
            smithingCard.Experience = client.Skills[13].Experience;

            defenceCard.BaseLevel = client.Skills[1].BaseLevel;
            defenceCard.CurrentLevel = client.Skills[1].CurrentLevel;
            defenceCard.Experience = client.Skills[1].Experience;

            herbloreCard.BaseLevel = client.Skills[15].BaseLevel;
            herbloreCard.CurrentLevel = client.Skills[15].CurrentLevel;
            herbloreCard.Experience = client.Skills[15].Experience;

            fishingCard.BaseLevel = client.Skills[10].BaseLevel;
            fishingCard.CurrentLevel = client.Skills[10].CurrentLevel;
            fishingCard.Experience = client.Skills[10].Experience;

            rangedCard.BaseLevel = client.Skills[4].BaseLevel;
            rangedCard.CurrentLevel = client.Skills[4].CurrentLevel;
            rangedCard.Experience = client.Skills[4].Experience;

            thievingCard.BaseLevel = client.Skills[17].BaseLevel;
            thievingCard.CurrentLevel = client.Skills[17].CurrentLevel;
            thievingCard.Experience = client.Skills[17].Experience;

            cookingCard.BaseLevel = client.Skills[7].BaseLevel;
            cookingCard.CurrentLevel = client.Skills[7].CurrentLevel;
            cookingCard.Experience = client.Skills[7].Experience;

            prayerCard.BaseLevel = client.Skills[5].BaseLevel;
            prayerCard.CurrentLevel = client.Skills[5].CurrentLevel;
            prayerCard.Experience = client.Skills[5].Experience;

            craftingCard.BaseLevel = client.Skills[12].BaseLevel;
            craftingCard.CurrentLevel = client.Skills[12].CurrentLevel;
            craftingCard.Experience = client.Skills[12].Experience;

            firemakingCard.BaseLevel = client.Skills[11].BaseLevel;
            firemakingCard.CurrentLevel = client.Skills[11].CurrentLevel;
            firemakingCard.Experience = client.Skills[11].Experience;

            magicCard.BaseLevel = client.Skills[6].BaseLevel;
            magicCard.CurrentLevel = client.Skills[6].CurrentLevel;
            magicCard.Experience = client.Skills[6].Experience;

            fletchingCard.BaseLevel = client.Skills[9].BaseLevel;
            fletchingCard.CurrentLevel = client.Skills[9].CurrentLevel;
            fletchingCard.Experience = client.Skills[9].Experience;

            woodcuttingCard.BaseLevel = client.Skills[8].BaseLevel;
            woodcuttingCard.CurrentLevel = client.Skills[8].CurrentLevel;
            woodcuttingCard.Experience = client.Skills[8].Experience;
        }
    }
}
