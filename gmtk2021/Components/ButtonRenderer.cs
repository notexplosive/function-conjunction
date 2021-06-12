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
        private readonly BoundingRect boundingRect;

        public ButtonRenderer(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
            this.boundingRect = RequireComponent<BoundingRect>();
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
