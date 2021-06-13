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
        private Atmosphere atmosphere;
        private readonly Level[] levels =
        new Level[]
        {
            #if DEBUG
            new Level("Experiment")
            {
                Solution = new Function[] { Functions.Squared, Functions.MinConstant(1), Functions.ModConstant(1) },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.Squared, Functions.MinConstant(1), Functions.ModConstant(1) },
                LockedInCards = new Function[] {  },
            },
            #endif

            new Level("Round Peg")
            {
                Solution = new Function[] { Functions.Sin },
                Domain = MathF.PI,
                Range = 1,
            },

            new Level("Embiggen")
            {
                Solution = new Function[] { Functions.Sin, Functions.MultiplyConstant(2) },
                CardFunctions = new Function[] { Functions.MultiplyFraction(1, 2), Functions.MultiplyConstant(2) },
                Domain = MathF.PI * 2,
                Range = 2,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("A Little to the Left")
            {
                Solution = new Function[] { Functions.AddConstant(MathF.PI / 2), Functions.Sin },
                CardFunctions = new Function[] { Functions.MultiplyFraction(1, 2), Functions.AddConstant(-MathF.PI / 2), Functions.AddConstant(MathF.PI / 2) },
                Domain = MathF.PI * 2,
                Range = 2,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("Before and After")
            {
                Solution = new Function[] { Functions.AddConstant(2), Functions.Sin, Functions.AddConstant(1) },
                CardFunctions = new Function[] { Functions.AddConstant(-1), Functions.AddConstant(3) },
                Domain = MathF.PI * 2,
                Range = 2,
                LockedInCards = new Function[] { Functions.Sin }
            },

            new Level("Absolute Value")
            {
                Solution = new Function[] { Functions.ModConstant(1), Functions.Abs },
                CardFunctions = new Function[] { Functions.ModConstant(1), Functions.Abs },
                Domain = 3,
            },

            new Level("Reverse Arches")
            {
                Solution = new Function[] { Functions.Sin, Functions.Abs, Functions.MultiplyConstant(-1)},
            },

            new Level("Phase Stretching")
            {
                Solution = new Function[] { Functions.MultiplyFraction(1, 2), Functions.ModConstant(1), Functions.MultiplyConstant(2) },
                CardFunctions = new Function[] { Functions.ModConstant(1), Functions.MultiplyConstant(2), Functions.MultiplyFraction(1,2) },
                Domain = 3,
            },

            new Level("The Easy Way")
            {
                // The idea of this level: There would be more solutions if you had more slots
                Solution = new Function[] { Functions.MultiplyFraction(1,2), Functions.ModConstant(2) },
                CardFunctions = new Function[] { Functions.ModConstant(1), Functions.MultiplyConstant(2), Functions.ModConstant(2), Functions.MultiplyFraction(1,2) },
            },

            new Level("Flatline")
            {
                Solution = new Function[] { Functions.Ceiling, Functions.ModConstant(1) },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.ModConstant(2) },
                ForceShuffle = true
            },

            new Level("Blocky Sine")
            {
                Solution = new Function[] { Functions.Ceiling, Functions.Sin },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.Cos, Functions.ModConstant(2) },
                ForceShuffle = true
            },

            new Level("Triangle Wave")
            {
                Solution = new Function[] { Functions.ModConstant(1), Functions.Abs, Functions.MultiplyConstant(2), Functions.AddConstant(-0.5f) },
                ForceShuffle = true,
                CardFunctions = new Function[] { Functions.Floor },
            },

            new Level("Neon")
            {
                Solution = new Function[] { Functions.Sin, Functions.ModConstant(1), Functions.MinConstant(0) },
                CardFunctions = new Function[] { Functions.Floor, Functions.Ceiling, Functions.MinConstant(1) }
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

                this.tween.AppendPointTween(
                    new Point(0, 0),
                    1f, EaseFuncs.EaseInOutBack,
                    camPos);

                yield return new WaitUntil(this.tween.IsDone);
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
