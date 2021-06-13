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
        private readonly Level[] levels =
        new Level[]
        {
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
                CardFunctions = new Function[] { Functions.AddConstant(1), Functions.AddConstant(2) },
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

            new Level(null)
            {
                Solution = new Function[] { Functions.MultiplyConstant(2), Functions.Sin },
                CardFunctions = new Function[] { Functions.ModConstant(1), Functions.MultiplyConstant(2) },
            },
        };

        public Level CurrentLevel => this.levels[currentLevelIndex];

        public int CurrentLevelIndex => this.currentLevelIndex;

        public LevelTransition(Actor actor) : base(actor)
        {
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
                    this.FinishLevel();
                    MachinaGame.Print("DBG: Skip Level");
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

        public void FinishLevel()
        {
            this.actor.scene.StartCoroutine(EndLevelCoroutine());
        }

        public void IncrementLevel()
        {
            this.currentLevelIndex++;
        }

        public bool HasNextLevel()
        {
            return this.currentLevelIndex < this.levels.Length;
        }
    }
}
