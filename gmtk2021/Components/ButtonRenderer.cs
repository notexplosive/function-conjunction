using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class ButtonRenderer : BaseComponent
    {
        private readonly Hoverable hoverable;
        private readonly Clickable clickable;
        private readonly BoundingRect boundingRect;

        public ButtonRenderer(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
            this.clickable = RequireComponent<Clickable>();

            this.hoverable.OnHoverStart += PlayerHoverSound;
            this.clickable.onClick += OnClick;
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        private void OnClick(MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                var sfx = MachinaGame.Assets.GetSoundEffectInstance("menu_click");
                sfx.Pitch = 0f;
                sfx.Volume = 0.5f;
                sfx.Stop();
                sfx.Play();
            }
        }

        private void PlayerHoverSound()
        {
            var sfx = MachinaGame.Assets.GetSoundEffectInstance("menu_click");
            sfx.Pitch = 0.5f;
            sfx.Volume = 0.5f;
            sfx.Stop();
            sfx.Play();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(this.boundingRect.Rect, this.hoverable.IsHovered ? Color.LightBlue : Color.Orange, transform.Depth);
            spriteBatch.DrawRectangle(this.boundingRect.Rect, this.hoverable.IsHovered ? Color.Teal : Color.OrangeRed, this.hoverable.IsHovered ? 2f : 4f, transform.Depth - 1);
        }
    }
}
