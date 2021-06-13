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
    class InBetweenRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public InBetweenRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = this.boundingRect.Rect;
            var center = rect.Center.ToVector2();
            var arrowHeight = rect.Height / 8;
            var arrowHead = center + new Vector2(rect.Width / 3, 0);
            var arrowBottom = center + new Vector2(0, arrowHeight);
            var arrowTop = center - new Vector2(0, arrowHeight);
            var arrowBack = center - new Vector2(rect.Width / 3, 0);

            spriteBatch.DrawLine(arrowHead, arrowTop, Color.White, 3f, transform.Depth);
            spriteBatch.DrawLine(arrowHead, arrowBottom, Color.White, 3f, transform.Depth);
            spriteBatch.DrawLine(arrowHead, arrowBack, Color.White, 3f, transform.Depth);
        }
    }
}
