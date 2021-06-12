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
    class CurveRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly TweenChain tween = new TweenChain();
        private CurvePoint[] points;

        public CurveRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Start()
        {
            // Needs to be after boundingRect has been settled by UI
            this.points = new CurvePoint[this.boundingRect.Width];
            for (int i = 0; i < this.boundingRect.Width; i++)
            {
                this.points[i] = new CurvePoint(transform, boundingRect, i);
            }
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // I'm doing this clever trick and I wanna write it down:
            // Intuitively you'd go for each point draw a line from prevPoint -> currPoint
            // Instead I do foreach point draw a line from prevPoint -> nextPoint
            // This is the same number of line segments as the former but draws a "fuller" line
            var prevPoint = this.points[0];
            for (int i = 1; i < this.points.Length - 1; i++)
            {
                var adjustedPoint = Adjusted(this.points[i + 1].WorldPosition);
                var adjustedPrevPoint = Adjusted(prevPoint.WorldPosition);
                bool outOfBounds = (this.points[i + 1].WorldPosition != adjustedPoint);
                spriteBatch.DrawLine(adjustedPrevPoint, adjustedPoint, outOfBounds ? Color.OrangeRed : Color.Orange, 5f, transform.Depth);
                prevPoint = this.points[i];
            }
        }

        private Vector2 Adjusted(Vector2 vec)
        {
            return new Vector2(vec.X, Math.Clamp(vec.Y, transform.Position.Y, transform.Position.Y + this.boundingRect.Height));
        }

        public void OnFunctionUpdated(Func<float, float> function)
        {
            if (this.points == null)
            {
                return;
            }

            this.tween.SkipToEnd();
            this.tween.Clear();
            var multiTween = this.tween.AppendMulticastTween();
            for (int i = 0; i < this.points.Length; i++)
            {
                // Flip value because y is facing down

                var targetVal = ApplyFunction(function, this.points[i].x);
                var point = this.points[i];
                var accessors = new TweenAccessors<int>(() => point.y, val => point.y = val);
                multiTween.AddChannel().AppendIntTween(targetVal, 0.25f, EaseFuncs.CubicEaseOut, accessors);
            }
        }

        public int ApplyFunction(Func<float, float> function, int x)
        {
            float heightDomain = 2;
            float widthDomain = MathF.PI * 2;

            return -(int) (
                    function(
                        ((float) x / this.boundingRect.Width - 0.5f) * widthDomain * 2
                    )
                    * this.boundingRect.Height / 2 / heightDomain);
        }

        private class CurvePoint
        {
            public readonly int x;

            public int y;
            private readonly Transform parent;
            private readonly BoundingRect boundingRect;

            public CurvePoint(Transform parent, BoundingRect rect, int x)
            {
                this.parent = parent;
                this.boundingRect = rect;
                this.x = x;
                this.y = 0;
            }

            public Vector2 LocalPosition => new Vector2(this.x, this.y + this.boundingRect.Height / 2);
            public Vector2 WorldPosition => parent.Position + LocalPosition;
        }
    }
}
