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
        public static Point CardSize = new Point(200, 100);

        public Game1(string[] args) : base("Nested Functions", args, new Point(1920, 1080), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
        }

        protected override void OnGameLoad()
        {
            Window.Title = "Nested Function";
            SceneLayers.SamplerState = SamplerState.LinearWrap;
            SamplerState = SamplerState.LinearWrap;

            SceneLayers.BackgroundColor = Color.Black;

            var titleFont = Assets.GetSpriteFont("TitleFont");
            var subtitleFont = Assets.GetSpriteFont("CardFont");
            var uiFont = Assets.GetSpriteFont("UIFont");


            var atmos = Assets.GetSoundEffectInstance("static_atmos");
            atmos.IsLooped = true;
            atmos.Play();

            var menuScene = SceneLayers.AddNewScene();
            var menuLayoutActor = menuScene.AddActor("GameLayout");
            new BoundingRect(menuLayoutActor, menuScene.camera.UnscaledViewportSize);
            var fade = new Fade(menuLayoutActor, true);
            fade.Finish += () =>
            {
                BuildFirstGameScene(SceneLayers);
                SceneLayers.RemoveScene(menuScene);
            };

            new LayoutGroup(menuLayoutActor, Orientation.Horizontal)
                .HorizontallyStretchedSpacer()
                .AddBothStretchedElement("InnerColumn", menuInnerColumnActor =>
                {
                    new LayoutGroup(menuInnerColumnActor, Orientation.Vertical)
                        .VerticallyStretchedSpacer()
                        .AddBothStretchedElement("MenuContent", menuContentActor =>
                        {
                            new LayoutGroup(menuContentActor, Orientation.Vertical)
                                .AddHorizontallyStretchedElement("Title", 80, titleActor =>
                                {
                                    new BoundedTextRenderer(titleActor, Window.Title, titleFont, Color.White, HorizontalAlignment.Center, VerticalAlignment.Top, Overflow.Ignore);
                                })
                                .AddHorizontallyStretchedElement("Subtitle", 80, subtitleActor =>
                                {
                                    new BoundedTextRenderer(subtitleActor, "Programmed & Designed by NotExplosive\nSound Design by Ryan Yoshikami", subtitleFont, Color.White, HorizontalAlignment.Center, VerticalAlignment.Top, Overflow.Ignore);
                                })
                                .PixelSpacer(32)
                                .AddHorizontallyStretchedElement("StartGameButton", 64, startGameButton =>
                                {
                                    new Hoverable(startGameButton);
                                    new Clickable(startGameButton).onClick += (mouseButton) => { if (mouseButton == MouseButton.Left) fade.Activate(); };
                                    new ButtonRenderer(startGameButton);
                                    new BoundedTextRenderer(startGameButton, "Start", uiFont, Color.Black, HorizontalAlignment.Center, VerticalAlignment.Center, Overflow.Ignore, new Machina.Data.Depth(-3))
                                        .EnableDropShadow(Color.DarkGray);
                                })
                                .PixelSpacer(16)
                                .AddHorizontallyStretchedElement("FullscreenButton", 64, fullscreenButton =>
                                 {
                                     new Hoverable(fullscreenButton);
                                     new Clickable(fullscreenButton).onClick += (mouseButton) => { Fullscreen = !Fullscreen; };
                                     new ButtonRenderer(fullscreenButton);
                                     new BoundedTextRenderer(fullscreenButton, "Toggle Fullscreen", uiFont, Color.Black, HorizontalAlignment.Center, VerticalAlignment.Center, Overflow.Ignore, new Machina.Data.Depth(-3))
                                        .EnableDropShadow(Color.DarkGray);
                                 });
                        })
                        .VerticallyStretchedSpacer();
                })
                .HorizontallyStretchedSpacer();
        }

        public static void BuildFirstGameScene(SceneLayers sceneLayers)
        {
            var bgScene = sceneLayers.AddNewScene();

            var font = Assets.GetSpriteFont("UIFont");

            var viewSize = bgScene.camera.UnscaledViewportSize;
            var bgRoot = bgScene.AddActor("BGRoot", new Vector2(0, -viewSize.Y));
            new BoundingRect(bgRoot, viewSize.X, viewSize.Y * 2);
            new LayoutGroup(bgRoot, Orientation.Vertical)
                .AddBothStretchedElement("BGTop", bgTopActor =>
                {
                    new BoundedTextRenderer(bgTopActor, "Level Complete!", font, Color.White, HorizontalAlignment.Center, VerticalAlignment.Center, Overflow.Ignore);
                })
                .VerticallyStretchedSpacer();
            var levelTransition = new LevelTransition(bgRoot);

            if (DebugLevel >= DebugLevel.Passive)
                new PanAndZoomCamera(bgRoot, Keys.LeftControl);

            var gameScene = sceneLayers.AddNewScene();

            BuildGameScene(gameScene, levelTransition);
        }

        public static void BuildGameScene(Scene gameScene, LevelTransition levelTransition)
        {
            foreach (var actor in gameScene.GetAllActors())
            {
                actor.Delete();
            }

            var currentLevel = levelTransition.CurrentLevel;
            var dropZones = new List<CardDropZone>();
            CardDropZone startingDropZone = null;
            CardDropZone destinationZone = null;
            DomainRange curveData = new DomainRange(currentLevel.Domain, currentLevel.Range);
            PrimaryCurve curve = null;
            SequenceDropZone sequenceDropZone = null;

            var titleFont = MachinaGame.Assets.GetSpriteFont("UIFont");

            var gameLayoutActor = gameScene.AddActor("GameLayout");
            new Fade(gameLayoutActor, false).Activate();
            new BoundingRect(gameLayoutActor, gameScene.camera.UnscaledViewportSize);
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

                            for (int i = 0; i < currentLevel.CardFunctions.Length; i++)
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
                        .AddHorizontallyStretchedElement("TitleBar", 64, titleBarActor =>
                         {
                             new LayoutGroup(titleBarActor, Orientation.Horizontal)
                                .PixelSpacer(64)
                                .AddBothStretchedElement("Title", titleActor =>
                                {
                                    new BoundedTextRenderer(titleActor, "Level " + (levelTransition.CurrentLevelIndex + 1) +
                                        (currentLevel.Title != null ? ": " + currentLevel.Title : ""),
                                        titleFont, Color.White);
                                })
                                .AddBothStretchedElement("URL", urlActor =>
                                {
                                    new BoundedTextRenderer(urlActor, "// notexplosive.net", titleFont, Color.White, HorizontalAlignment.Right);
                                });
                         })
                        .AddBothStretchedElement("CurveWindow", curveWindowActor =>
                        {
                            curveWindowActor.transform.Depth += 5;

                            new LayoutGroup(curveWindowActor, Orientation.Horizontal)
                                .AddVerticallyStretchedElement("Range", rangeBarThickness, rangeActor =>
                                {
                                    new DomainRenderer(rangeActor, curveData, Orientation.Vertical);
                                })
                                .AddBothStretchedElement("Curve", curveActor =>
                                {
                                    curve = new PrimaryCurve(curveActor, curveData, levelTransition.CurrentLevel.Solution);
                                    var objective = new ObjectiveTracker(curveActor, levelTransition);
                                    objective.Win += () => sequenceDropZone.LockAll();
                                });
                        })
                        .AddHorizontallyStretchedElement("DomainContainer", 32, domainContainerActor =>
                        {
                            new LayoutGroup(domainContainerActor, Orientation.Horizontal)
                                .PixelSpacer(rangeBarThickness)
                                .AddBothStretchedElement("Domain", domainActor =>
                                {
                                    new DomainRenderer(domainActor, curveData, Orientation.Horizontal);
                                });
                        })
                        .AddHorizontallyStretchedElement("Chain CardDropZone", CardSize.Y + 20, dropZoneActor =>
                        {
                            var dropZone = new CardDropZone(dropZoneActor);
                            destinationZone = dropZone;
                            dropZones.Add(dropZone);

                            var group = new LayoutGroup(dropZoneActor, Orientation.Horizontal)
                                .SetMargin(10);

                            for (int i = 0; i < currentLevel.NumberOfSequenceSlots; i++)
                            {

                                group.AddElement("CardInBetween", new Point(10, CardSize.Y), cardInsertion =>
                                {
                                    dropZone.AddInBetween(cardInsertion, i);
                                });

                                group.AddElement("CardSlot", CardSize, cardSlotActor =>
                                {
                                    dropZone.AddCardSlot(cardSlotActor);
                                });
                            }

                            sequenceDropZone = new SequenceDropZone(dropZoneActor);
                            sequenceDropZone.FunctionUpdated += curve.OnFunctionUpdated;
                        });
                })
                .SetMargin(8)
                .transform.FlushBuffers(); // HACKY AF


            foreach (var function in currentLevel.CardFunctions)
            {
                CreateCard(gameScene, dropZones, function, startingDropZone, destinationZone);
            }

            foreach (var function in currentLevel.LockedInCards)
            {
                var card = CreateCard(gameScene, dropZones, function, destinationZone, destinationZone);
                card.Lock();
            }
        }

        public static Card CreateCard(Scene scene, List<CardDropZone> dropZones, Function function, CardDropZone startingDropZone, CardDropZone destinationZone)
        {
            var actor = scene.AddActor("Card");
            new BoundingRect(actor, CardSize);
            new Hoverable(actor);
            new Clickable(actor);
            new DoubleClickable(actor);
            new Draggable(actor);
            new MoveOnDrag(actor);


            var font = MachinaGame.Assets.GetSpriteFont("CardFont");
            StaticCurveRenderer curveRenderer = null;

            new LayoutGroup(actor, Orientation.Horizontal)
                .PixelSpacer(8)
                .AddBothStretchedElement("CardInnerColumn", cardInnerColumn =>
                {
                    new LayoutGroup(cardInnerColumn, Orientation.Vertical)
                        .PixelSpacer(8)
                        .AddBothStretchedElement("CardText", cardTextActor =>
                        {
                            curveRenderer = new StaticCurveRenderer(cardTextActor, function);
                        })
                        .PixelSpacer(8);
                })
                .PixelSpacer(8);

            var card = new Card(actor, dropZones, function, startingDropZone, destinationZone);
            new CardBackgroundRenderer(actor, curveRenderer);

            return card;
        }
    }
}
