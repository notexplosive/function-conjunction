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
        private readonly Card card;
        private readonly BoundingRect boundingRect;
        private readonly StaticCurveRenderer curve;

        public CardBackgroundRenderer(Actor actor, StaticCurveRenderer curve) : base(actor)
        {
            this.card = RequireComponent<Card>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.curve = curve;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(this.boundingRect.Rect, this.card.IsLocked ? new Color(180, 100, 20) : Color.Orange, transform.Depth);

            if (this.card.IsLocked)
            {
                this.curve.OnColor = Color.White;
                this.curve.OffColor = Color.Black;
            }
            else
            {
                this.curve.OnColor = Color.Black;
                this.curve.OffColor = Color.Gray;
            }
        }
    }
}
