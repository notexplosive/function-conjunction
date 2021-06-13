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
        private readonly ICard card;
        private readonly BoundingRect boundingRect;
        private readonly StaticCurveRenderer curve;
        public Color CustomBGColor
        {
            get; set;
        }

        public CardBackgroundRenderer(Actor actor, StaticCurveRenderer curve) : base(actor)
        {
            this.card = this.actor.GetComponentUnsafe<ICard>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.curve = curve;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var bgColor = this.card.IsLocked || this.card.IsPartialLocked ? new Color(50, 50, 50) : Color.Black;

            if (this.card.UseCustomColor)
            {
                bgColor = this.CustomBGColor;
            }
            else
            {
                if (this.card.IsLocked || this.card.IsPartialLocked)
                {
                    this.curve.OnColor = Color.DarkOrange;
                    this.curve.OffColor = Color.OrangeRed;
                }
                else
                {
                    this.curve.OnColor = Color.Orange;
                    this.curve.OffColor = Color.OrangeRed;
                }
            }

            spriteBatch.FillRectangle(this.boundingRect.Rect, bgColor, transform.Depth);
            spriteBatch.DrawRectangle(this.boundingRect.Rect, new Color(200, 200, 200), 5, transform.Depth - 1);


        }
    }
}
