using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    public class LevelTransition : BaseComponent
    {
        private readonly TweenChain tween = new TweenChain();
        private bool clickDown;
        private Scene gameScene;
        private int currentLevelIndex = 0;
        private PrimaryCurve primaryCurve;
        public readonly Atmosphere atmosphere;
        private readonly Level[] levels =
        new Level[]
        {
            #if DEBUG
            new Level("Experiment")
            {
                Solution = new Function[] { Functions.ModConstant(1), Functions.Floor, Functions.Squared, Functions.AddConstant(-0.5f) },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.Squared, Functions.MinConstant(1), Functions.ModConstant(2), Functions.Sign },
                LockedInCards = new Function[] { Functions.ModConstant(1) },
                AdditionalSequenceSlots = 5,
            },
            #endif

            new Level("Round Peg")
            {
                // Teach how to drag cards
                Solution = new Function[] { Functions.Sin },
                Domain = MathF.PI,
                Range = 1,
            },

            new Level("Embiggen")
            {
                // Teach how multiply works
                Solution = new Function[] { Functions.Sin, Functions.MultiplyConstant(2) },
                CardFunctions = new Function[] { Functions.AddConstant(2), Functions.MultiplyConstant(2) },
                Domain = MathF.PI * 2,
                Range = 3,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("A Little to the Left")
            {
                // Teach how to interact with locked cards
                Solution = new Function[] { Functions.AddConstant(MathF.PI / 2), Functions.Sin },
                Domain = MathF.PI * 2,
                Range = 2,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("Before and After")
            {
                // Teach how add works at different times
                Solution = new Function[] { Functions.AddConstant(2), Functions.Sin, Functions.AddConstant(1) },
                CardFunctions = new Function[] { Functions.AddConstant(-1), Functions.AddConstant(3) },
                Domain = MathF.PI * 2,
                Range = 2,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("Phase Stretching")
            {
                // Teach how multiply works at different times
                Solution = new Function[] { Functions.MultiplyFraction(1, 2), Functions.Sin, Functions.MultiplyConstant(2) },
                Domain = 2 * MathF.PI,
            },

            new Level("Reverse Arches")
            {
                // Introduce absolute value
                Solution = new Function[] { Functions.Sin, Functions.Abs, Functions.MultiplyConstant(-1)},
            },

            new Level("Rolling Hills")
            {
                // (medium) Test knowledge on multiply/add at different times
                Solution = new Function[] { Functions.Sin, Functions.MultiplyConstant(2), Functions.AddConstant(-1), Functions.Abs },
                CardFunctions = new Function[] { Functions.AddConstant(1) },
                Range = 4,
            },

            new Level("Square Wave")
            {
                Solution = new Function[] { Functions.ModConstant(1), Functions.Floor, Functions.Squared, Functions.AddConstant(-0.5f) },
                CardFunctions = new Function[] { Functions.Sign },
            },

            new Level("Flatline")
            {
                Solution = new Function[] { Functions.Sin, Functions.Squared, Functions.Ceiling, Functions.AddConstant(-1) },
                ForceShuffle = true
            },

            new Level("Spikes")
            {
                // Intermediate skill check
                Solution = new Function[] { Functions.Sin, Functions.ModConstant(1), Functions.Squared, Functions.MultiplyConstant(4), Functions.AddConstant(-1) },
            },

            new Level("Bitcrunch")
            {
                // Introduce ceiling/floor
                Solution = new Function[] { Functions.Floor, Functions.Sin },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.ModConstant(2) },
                ForceShuffle = true
            },

            new Level("Triangle Wave")
            {
                // Skill check with absolute value (and phase manipulation)
                Solution = new Function[] { Functions.ModConstant(1), Functions.Abs, Functions.MultiplyConstant(2), Functions.AddConstant(-0.5f) },
                ForceShuffle = true,
                CardFunctions = new Function[] { Functions.Floor },
            },

            new Level("W")
            {
                // Skill check for squared
                Solution = new Function[] { Functions.Squared, Functions.AddConstant(-2), Functions.Squared, Functions.AddConstant(-1) },
                CardFunctions = new Function[] { /*The validator won't notice we need to 2 square funcs:*/ Functions.Squared, Functions.Squared, /*Red harrings:*/ Functions.MultiplyConstant(-1) },
            },

            new Level("Neon")
            {
                // Introduce Min
                Solution = new Function[] { Functions.Sin, Functions.ModConstant(1), Functions.MinConstant(0) },
                CardFunctions = new Function[] { Functions.Floor, Functions.MinConstant(1) }
            },

            new Level("Fizzled Out")
            {
                Solution = new Function[] { Functions.MinConstant(0), Functions.Squared, Functions.MaxConstant(0), Functions.Sin, Functions.MultiplyConstant(-1) },
            },

            new Level("Mesa")
            {
                // Tick: Cubed -> Abs -> Sub 1 -> Multiply -1 -> Add 1 ALMOST works, but you need one more slot for the min
                Solution = new Function[] { Functions.Cubed, Functions.Abs, Functions.MultiplyConstant(-1), Functions.AddConstant(2), Functions.MinConstant(0) },
                CardFunctions = new Function[] { Functions.AddConstant(-1), Functions.AddConstant(1), Functions.Squared },
                LockedInCards = new Function[] { Functions.MultiplyConstant(-1) }
            },

            /*
            new Level("Repeat")
            {
                //Broken?
                Solution = new Function[] { Functions.Sin, Functions.MaxConstant(1), Functions.Squared, Functions.ModConstant(1) },
                LockedInCards = new Function[] { Functions.Sin },
            },
            */

            new Level("Cityscape")
            {
                Solution = new Function[] { Functions.Ceiling, Functions.Squared, Functions.AddConstant(1), Functions.Sin },
                Range = 4,
                Domain = MathF.PI * 2
            },

            new Level("Morshu Wave")
            {
                Solution = new Function[] { Functions.Squared, Functions.MinConstant(1), Functions.ModConstant(1) },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.Squared, Functions.MinConstant(1) },
                LockedInCards = new Function[] { Functions.ModConstant(1) },
                CustomSound = MachinaGame.Assets.GetSoundEffectInstance("morshu"),
            },

            new Level("Lockout")
            {
                Solution = new Function[] { Functions.Sin, Functions.Abs, Functions.AddConstant(-1), Functions.MinConstant(0), Functions.AddConstant(2) },
                LockedInCards = new Function[] { Functions.MinConstant(0) },
                CardFunctions = new Function[] { Functions.MultiplyConstant(-1) },
            }
        };

        public Level CurrentLevel => this.levels[currentLevelIndex];

        public int CurrentLevelIndex => this.currentLevelIndex;

        public LevelTransition(Actor actor, Atmosphere atmos) : base(actor)
        {
            this.atmosphere = atmos;
            foreach (var level in levels)
            {
                level.Validate();
            }
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
            this.gameScene.camera.UnscaledPosition = this.actor.scene.camera.UnscaledPosition;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Escape && modifiers.None && MachinaGame.Fullscreen)
            {
                MachinaGame.Fullscreen = false;
            }

            if (key == Keys.Space && modifiers.None)
            {
                this.clickDown = state == ButtonState.Pressed;
            }

            if (Game1.DebugLevel >= DebugLevel.Passive && state == ButtonState.Pressed)
            {
                if (key == Keys.W)
                {
                    SkipLevel();
                }
            }

            if (state == ButtonState.Pressed && modifiers.ControlAlt && key == Keys.T)
            {
                SkipLevel();
            }
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Left)
            {
                this.clickDown = state == ButtonState.Pressed;
            }
        }

        public bool HasClicked()
        {
            return this.clickDown;
        }

        public void SetGameScene(Scene scene, PrimaryCurve primaryCurve)
        {
            this.gameScene = scene;
            this.primaryCurve = primaryCurve;
        }

        private IEnumerator<ICoroutineAction> EndLevelCoroutine()
        {
            var camera = this.actor.scene.camera;
            var camPos = new TweenAccessors<Point>(() => camera.ScaledPosition, val => camera.ScaledPosition = val);

            yield return new WaitSeconds(1);

            var isDone = this.primaryCurve.StartParticleAnimation();
            yield return new WaitUntil(isDone);

            var fade = new Fade(this.gameScene.AddActor("FadeOut"), true).Activate();
            var isFadeFinished = false;
            fade.Finish += () => { isFadeFinished = true; };

            yield return new WaitUntil(() => isFadeFinished);

            IncrementLevel();
            if (HasNextLevel())
            {
                Game1.BuildGameScene(this.gameScene, this);
            }
            else
            {
                foreach (var actor in gameScene.GetAllActors())
                {
                    actor.Delete();
                }

                var creditsRoot = this.gameScene.AddActor("CreditsRoot");
                new Fade(creditsRoot, false).Activate();
                new BoundingRect(creditsRoot, camera.UnscaledViewportSize);
                new BoundedTextRenderer(creditsRoot, "Thanks for playing!\n\nProgrammed & Designed by NotExplosive\nSound Design by Ryan Yoshikami\nTested by lectvs and soomy\nPlayed by you <3\n\nMade with MonoGame in 48 hours\nGraphics drawn entirely with MonoGame.Extended Primitives", MachinaGame.Assets.GetSpriteFont("UIFont"), Color.White, HorizontalAlignment.Center, VerticalAlignment.Center);

                // Make sure music plays during the credits
                atmosphere.music.Play();
            }

            yield return null;
        }

        public void SkipLevel()
        {
            IncrementLevel();
            if (HasNextLevel())
            {
                Game1.BuildGameScene(this.gameScene, this);
            }
            else
            {
                FinishLevel();
            }
        }

        public void FinishLevel()
        {
            this.actor.scene.StartCoroutine(EndLevelCoroutine());
        }

        public void IncrementLevel()
        {
            this.currentLevelIndex++;
            this.atmosphere.PlayNext();
        }

        public bool HasNextLevel()
        {
            return this.currentLevelIndex < this.levels.Length;
        }
    }
}
