using Microsoft.Xna.Framework;

using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Net.Client;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiSkillsPanel : GuiElement
    {
        GameClient client;

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

        public override void LoadContent()
        {
            attackCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/attack"
            };
            healthCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/health"
            };
            miningCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/mining"
            };
            strengthCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/strength"
            };
            agilityCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/agility"
            };
            smithingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/smithing"
            };
            defenceCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/defence"
            };
            herbloreCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/herblore"
            };
            fishingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/fishing"
            };
            rangedCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/ranged"
            };
            thievingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/thieving"
            };
            cookingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/cooking"
            };
            prayerCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/prayer"
            };
            craftingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/crafting"
            };
            firemakingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/firemaking"
            };
            magicCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/magic"
            };
            fletchingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/fletching"
            };
            woodcuttingCard = new GuiSkillCard
            {
                Size = new Size2D(64, 32),
                SkillIcon = "Icons/Skills/woodcutting"
            };

            Children.Add(attackCard);
            Children.Add(healthCard);
            Children.Add(miningCard);
            Children.Add(strengthCard);
            Children.Add(agilityCard);
            Children.Add(smithingCard);
            Children.Add(defenceCard);
            Children.Add(herbloreCard);
            Children.Add(fishingCard);
            Children.Add(rangedCard);
            Children.Add(thievingCard);
            Children.Add(cookingCard);
            Children.Add(prayerCard);
            Children.Add(craftingCard);
            Children.Add(firemakingCard);
            Children.Add(magicCard);
            Children.Add(fletchingCard);
            Children.Add(woodcuttingCard);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            attackCard.BaseLevel = client.Skills[0].BaseLevel;
            attackCard.CurrentLevel = client.Skills[0].CurrentLevel;

            healthCard.BaseLevel = client.Skills[3].BaseLevel;
            healthCard.CurrentLevel = client.Skills[3].CurrentLevel;

            miningCard.BaseLevel = client.Skills[14].BaseLevel;
            miningCard.CurrentLevel = client.Skills[14].CurrentLevel;

            strengthCard.BaseLevel = client.Skills[2].BaseLevel;
            strengthCard.CurrentLevel = client.Skills[2].CurrentLevel;

            agilityCard.BaseLevel = client.Skills[16].BaseLevel;
            agilityCard.CurrentLevel = client.Skills[16].CurrentLevel;

            smithingCard.BaseLevel = client.Skills[13].BaseLevel;
            smithingCard.CurrentLevel = client.Skills[13].CurrentLevel;

            defenceCard.BaseLevel = client.Skills[1].BaseLevel;
            defenceCard.CurrentLevel = client.Skills[1].CurrentLevel;

            herbloreCard.BaseLevel = client.Skills[15].BaseLevel;
            herbloreCard.CurrentLevel = client.Skills[15].CurrentLevel;

            fishingCard.BaseLevel = client.Skills[10].BaseLevel;
            fishingCard.CurrentLevel = client.Skills[10].CurrentLevel;

            rangedCard.BaseLevel = client.Skills[4].BaseLevel;
            rangedCard.CurrentLevel = client.Skills[4].CurrentLevel;

            thievingCard.BaseLevel = client.Skills[17].BaseLevel;
            thievingCard.CurrentLevel = client.Skills[17].CurrentLevel;

            cookingCard.BaseLevel = client.Skills[7].BaseLevel;
            cookingCard.CurrentLevel = client.Skills[7].CurrentLevel;

            prayerCard.BaseLevel = client.Skills[5].BaseLevel;
            prayerCard.CurrentLevel = client.Skills[5].CurrentLevel;

            craftingCard.BaseLevel = client.Skills[12].BaseLevel;
            craftingCard.CurrentLevel = client.Skills[12].CurrentLevel;

            firemakingCard.BaseLevel = client.Skills[11].BaseLevel;
            firemakingCard.CurrentLevel = client.Skills[11].CurrentLevel;

            magicCard.BaseLevel = client.Skills[6].BaseLevel;
            magicCard.CurrentLevel = client.Skills[6].CurrentLevel;

            fletchingCard.BaseLevel = client.Skills[9].BaseLevel;
            fletchingCard.CurrentLevel = client.Skills[9].CurrentLevel;

            woodcuttingCard.BaseLevel = client.Skills[8].BaseLevel;
            woodcuttingCard.CurrentLevel = client.Skills[8].CurrentLevel;

            base.Update(gameTime);
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.client = client;
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            int spacingX = (Size.Width - 3 * attackCard.Size.Width) / 4;
            int spacingY = (Size.Height - 6 * attackCard.Size.Height) / 7;

            attackCard.Location = new Point2D(
                Location.X + spacingX,
                Location.Y + spacingY);
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
    }
}
