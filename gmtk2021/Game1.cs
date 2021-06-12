using gmtk2021.Components;
using gmtk2021.Data;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace gmtk2021
{
    public class Game1 : MachinaGame
    {
        public Point CardSize = new Point(150, 100);

        public Game1(string[] args) : base("Nested Functions", args, new Point(1600, 900), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
        }

        protected override void OnGameLoad()
        {
            SceneLayers.BackgroundColor = Color.Black;
            var gameScene = SceneLayers.AddNewScene();

            var dropZones = new List<CardDropZone>();
            CardDropZone startingDropZone = null;
            CurveRenderer curve = null;

            var gameLayoutActor = gameScene.AddActor("GameLayout");
            new BoundingRect(gameLayoutActor, SceneLayers.gameCanvas.ViewportSize);
            new LayoutGroup(gameLayoutActor, Orientation.Horizontal)
                .AddVerticallyStretchedElement("LeftColumn", CardSize.X + 20, leftColumnActor =>
                {
                    new LayoutGroup(leftColumnActor, Orientation.Vertical)
                        .AddBothStretchedElement("Extra CardDropZone", extraDropZoneActor =>
                        {
                            var dropZone = new CardDropZone(extraDropZoneActor);
                            startingDropZone = dropZone;
                            dropZones.Add(dropZone);
                            var group = new LayoutGroup(extraDropZoneActor, Orientation.Vertical)
                                .SetMargin(10)
                                .SetPaddingBetweenElements(10);

                            for (int i = 0; i < 6; i++)
                            {
                                group.AddElement("CardSlot", CardSize, cardSlotActor =>
                                {
                                    dropZone.AddCardSlot(cardSlotActor);
                                });
                            }
                        })
                        .SetPaddingBetweenElements(8);
                })
                .AddBothStretchedElement("RightColumn", rightColumnActor =>
                {
                    int rangeBarThickness = 64;
                    new LayoutGroup(rightColumnActor, Orientation.Vertical)
                        .AddBothStretchedElement("CurveWindow", curveWindowActor =>
                        {
                            curveWindowActor.transform.Depth += 5;



                            new LayoutGroup(curveWindowActor, Orientation.Horizontal)
                                .AddVerticallyStretchedElement("Range", rangeBarThickness, rangeActor =>
                                {
                                    new BoundingBoxFill(rangeActor, Color.White);
                                })
                                .AddBothStretchedElement("Curve", curveActor =>
                                {
                                    curve = new CurveRenderer(curveActor);
                                });
                        })
                        .AddHorizontallyStretchedElement("DomainContainer", 32, domainContainerActor =>
                        {
                            new LayoutGroup(domainContainerActor, Orientation.Horizontal)
                                .PixelSpacer(rangeBarThickness)
                                .AddBothStretchedElement("Domain", domainActor =>
                                {
                                    new BoundingBoxFill(domainActor, Color.White);
                                });
                        })
                        .AddHorizontallyStretchedElement("Chain CardDropZone", CardSize.Y + 20, dropZoneActor =>
                        {
                            var dropZone = new CardDropZone(dropZoneActor);
                            dropZones.Add(dropZone);

                            var group = new LayoutGroup(dropZoneActor, Orientation.Horizontal)
                                .SetMargin(10)
                                .SetPaddingBetweenElements(10);
                            for (int i = 0; i < 5; i++)
                            {
                                group.AddElement("CardSlot", CardSize, cardSlotActor =>
                                {
                                    dropZone.AddCardSlot(cardSlotActor);
                                });
                            }

                            var sequenceDropZone = new SequenceDropZone(dropZoneActor);
                            sequenceDropZone.FunctionUpdated += curve.OnFunctionUpdated;
                        });
                })
                .SetMargin(8)
                .transform.FlushBuffers(); // HACKY AF


            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.Sin), true);
            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.Sin), true);
            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.Abs), true);
            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.AddConstant(2)), true);
            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.TimeConstant(-1)), true);
            startingDropZone.Consume(CreateCard(gameScene, dropZones, Functions.TimesFraction(1, 2)), true);
        }

        public Card CreateCard(Scene scene, List<CardDropZone> dropZones, Function function)
        {
            var actor = scene.AddActor("Card");
            new BoundingRect(actor, CardSize);
            new Hoverable(actor);
            new Clickable(actor);
            new Draggable(actor);
            new MoveOnDrag(actor);
            new CardBackgroundRenderer(actor);

            var font = MachinaGame.Assets.GetSpriteFont("CardFont");

            new LayoutGroup(actor, Orientation.Horizontal)
                .PixelSpacer(8)
                .AddBothStretchedElement("CardInnerColumn", cardInnerColumn =>
                {
                    new LayoutGroup(cardInnerColumn, Orientation.Vertical)
                        .PixelSpacer(8)
                        .AddBothStretchedElement("CardText", cardTextActor =>
                        {
                            new BoundedTextRenderer(cardTextActor, function.name, font, Color.White, HorizontalAlignment.Center, VerticalAlignment.Center, Overflow.Ignore)
                                .EnableDropShadow(Color.Black);
                        })
                        .PixelSpacer(8);
                })
                .PixelSpacer(8);

            return new Card(actor, dropZones, function);
        }
    }
}
