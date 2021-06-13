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
    class DropZoneBackgroundRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public DropZoneBackgroundRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, new Color(30, 30, 30), 2f, transform.Depth);
            spriteBatch.FillRectangle(this.boundingRect.Rect, new Color(150, 150, 150), transform.Depth + 100);
            spriteBatch.FillRectangle(
                this.boundingRect.TopLeft + new Point(0, this.boundingRect.Size.Y / 2).ToVector2(),
                new Point(this.boundingRect.Size.X, this.boundingRect.Size.Y / 2),
                new Color(100, 100, 100), transform.Depth + 99);
        }
    }
}
