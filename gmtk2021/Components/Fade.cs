using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class Fade : BaseComponent
    {
        private readonly TweenChain tween;
        private float opacityVal;
        private bool isActive;
        public event Action Finish;

        public Fade(Actor actor, bool fadeOut) : base(actor)
        {
            this.tween = new TweenChain();
            this.opacityVal = 0f;
            var opacity = new TweenAccessors<float>(() => this.opacityVal, val => this.opacityVal = val);
            var zoom = new TweenAccessors<float>(() => this.actor.scene.camera.Zoom, val => this.actor.scene.camera.Zoom = val);
            float duration = 0.5f;
            var multi = this.tween.AppendMulticastTween();

            if (fadeOut)
            {
                multi.AddChannel().AppendFloatTween(1f, duration, EaseFuncs.CubicEaseIn, opacity);
                multi.AddChannel().AppendFloatTween(0.5f, duration, EaseFuncs.CubicEaseIn, zoom);
            }
            else
            {
                opacity.setter(1);
                zoom.setter(1.5f);

                multi.AddChannel().AppendFloatTween(0f, duration, EaseFuncs.CubicEaseOut, opacity);
                multi.AddChannel().AppendFloatTween(1f, duration, EaseFuncs.CubicEaseOut, zoom);
            }
        }

        public Fade Activate()
        {
            this.isActive = true;
            return this;
        }

        public override void Update(float dt)
        {
            if (this.isActive)
            {
                this.tween.Update(dt);

                if (this.tween.IsDone())
                {
                    Finish?.Invoke();
                    this.isActive = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var fadeOverlay = new Color(Color.Black, this.opacityVal);
            // Draw rect in absolute foreground
            spriteBatch.FillRectangle(new Rectangle(Point.Zero, this.actor.scene.camera.UnscaledViewportSize), fadeOverlay, new Depth(0));
        }
    }
}
