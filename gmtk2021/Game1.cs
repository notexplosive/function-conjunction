using gmtk2021.Components;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace gmtk2021
{
    public class Game1 : MachinaGame
    {
        public Point CardSize = new Point(256, 128);

        public Game1(string[] args) : base("Nested Functions", args, new Point(1600, 900), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
        }

        protected override void OnGameLoad()
        {

            SceneLayers.BackgroundColor = Color.Black;
            var gameScene = SceneLayers.AddNewScene();

            CardDropZone dropZone = null;

            var gameLayoutActor = gameScene.AddActor("GameLayout");
            new BoundingRect(gameLayoutActor, SceneLayers.gameCanvas.ViewportSize);
            new LayoutGroup(gameLayoutActor, Orientation.Horizontal)
                .AddVerticallyStretchedElement("LeftColumn", 256, leftColumnActor =>
                {

                })
                .AddBothStretchedElement("RightColumn", rightColumnActor =>
                {
                    new LayoutGroup(rightColumnActor, Orientation.Vertical)
                        .SetMargin(8)
                        .VerticallyStretchedSpacer()
                        .AddHorizontallyStretchedElement("CardDropZone", CardSize.Y + 20, dropZoneActor =>
                        {
                            dropZone = new CardDropZone(dropZoneActor);
                            var group = new LayoutGroup(dropZoneActor, Orientation.Horizontal)
                                .SetMargin(10);
                            group.SetPaddingBetweenElements(10);
                            for (int i = 0; i < 5; i++)
                            {
                                group.AddElement("CardSlot", CardSize, cardSlotActor =>
                                {
                                    dropZone.AddCardSlot(cardSlotActor);
                                });
                            }
                        })
                        .SetPaddingBetweenElements(8);
                });



            CreateCard(gameScene, dropZone, "Card 1");
            CreateCard(gameScene, dropZone, "Card 2");
            CreateCard(gameScene, dropZone, "Card 3");
        }

        public void CreateCard(Scene scene, CardDropZone dropZone, string text)
        {
            var actor = scene.AddActor("Card");
            new BoundingRect(actor, CardSize);
            new Hoverable(actor);
            new Clickable(actor);
            new Draggable(actor);
            new MoveOnDrag(actor);
            new CardBackgroundRenderer(actor);
            new Card(actor, dropZone);

            var font = MachinaGame.Assets.GetSpriteFont("CardFont");

            new LayoutGroup(actor, Orientation.Horizontal)
                .PixelSpacer(8)
                .AddBothStretchedElement("CardInnerColumn", cardInnerColumn =>
                {
                    new LayoutGroup(cardInnerColumn, Orientation.Vertical)
                        .PixelSpacer(8)
                        .AddBothStretchedElement("CardText", cardTextActor =>
                        {
                            new BoundedTextRenderer(cardTextActor, text, font, Color.White, HorizontalAlignment.Center, VerticalAlignment.Center, Overflow.Ignore)
                                .EnableDropShadow(Color.Black);
                        })
                        .PixelSpacer(8);
                })
                .PixelSpacer(8);

        }
    }
}
