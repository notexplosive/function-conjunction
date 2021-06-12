using gmtk2021.Data;
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
    class StaticCurveRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly DomainRange domain;
        private readonly Func<float, float> composedFunction;
        private CurvePoint[] points;
        public Color OnColor
        {
            get; set;
        }

        public Color OffColor
        {
            get; set;
        }

        public StaticCurveRenderer(Actor actor, Function function) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.domain = function.domain;
            this.composedFunction = function.func;
        }

        public override void Start()
        {
            this.points = new CurvePoint[this.boundingRect.Width];
            for (int i = 0; i < this.boundingRect.Width; i++)
            {
                this.points[i] = new CurvePoint(transform, boundingRect, i);
            }

            for (int i = 0; i < this.points.Length; i++)
            {
                var point = this.points[i];
                point.y = PrimaryCurve.ApplyFunction(this.composedFunction, this.points[i].x, this.domain, this.boundingRect);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw curve
            var prevPoint = this.points[1];
            for (int i = 2; i < this.points.Length - 2; i++)
            {
                var adjustedPoint = PrimaryCurve.Adjusted(this.points[i + 1].WorldPosition, transform, boundingRect);
                var adjustedPrevPoint = PrimaryCurve.Adjusted(prevPoint.WorldPosition, transform, boundingRect);
                bool outOfBounds = (this.points[i + 1].WorldPosition != adjustedPoint);

                spriteBatch.DrawLine(adjustedPrevPoint, adjustedPoint, outOfBounds ? OffColor : OnColor, 3f, transform.Depth - 10);
                prevPoint = this.points[i];
            }

            // draw axis lines
            var center = this.boundingRect.TopLeft + this.boundingRect.Size.ToVector2() / 2;
            var topLeft = this.boundingRect.TopLeft;
            var lineColor = new Color(Color.Black, 0.5f);
            spriteBatch.DrawLine(new Vector2(center.X, topLeft.Y), new Vector2(center.X, topLeft.Y + this.boundingRect.Height), lineColor, 3f, transform.Depth - 1);
            spriteBatch.DrawLine(new Vector2(topLeft.X, center.Y), new Vector2(topLeft.X + this.boundingRect.Width, center.Y), lineColor, 3f, transform.Depth - 1);
        }
    }
}
