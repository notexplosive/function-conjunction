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
        private readonly Level[] levels =
        new Level[]
        {
            new Level("Absolute")
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

            new Level(null)
            {
                Solution = new Function[] { Functions.MultiplyConstant(2), Functions.Sin },
                CardFunctions = new Function[] {Functions.ModConstant(1), Functions.MultiplyConstant(2) },
            },
        };

        public Level CurrentLevel => this.levels[currentLevelIndex];

        public int CurrentLevelIndex => this.currentLevelIndex;

        public LevelTransition(Actor actor) : base(actor)
        {

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

        public void SetGameScene(Scene scene)
        {
            this.gameScene = scene;
        }

        private IEnumerator<ICoroutineAction> EndLevelCoroutine()
        {
            var camera = this.actor.scene.camera;
            var camPos = new TweenAccessors<Point>(() => camera.ScaledPosition, val => camera.ScaledPosition = val);

            yield return new WaitSeconds(1);

            this.tween.AppendVectorTween(
                new Point(0, -camera.UnscaledViewportSize.Y),
                1f, EaseFuncs.EaseInOutBack,
                camPos);

            yield return new WaitUntil(this.tween.IsDone);

            yield return new WaitUntil(HasClicked);
            IncrementLevel();
            if (HasLevel())
            {
                Game1.BuildGameScene(this.gameScene, this);

                this.tween.AppendVectorTween(
                    new Point(0, 0),
                    1f, EaseFuncs.EaseInOutBack,
                    camPos);

                yield return new WaitUntil(this.tween.IsDone);
            }

            yield return null;
        }

        public void LevelEnd()
        {
            this.actor.scene.StartCoroutine(EndLevelCoroutine());
        }

        public void IncrementLevel()
        {
            this.currentLevelIndex++;
        }

        public bool HasLevel()
        {
            return this.currentLevelIndex < this.levels.Length;
        }
    }
}
