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
    class CardBackgroundRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public CardBackgroundRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(this.boundingRect.Rect, Color.Orange, transform.Depth);
        }
    }
}
