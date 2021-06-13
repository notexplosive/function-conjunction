using gmtk2021.Data;
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
    class StaticCurveRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly DomainRange domain;
        private readonly Func<float, float> composedFunction;
        private TweenChain tween;
        private CurvePoint[] points;
        private CurvePoint[] baseCurve;

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
            this.baseCurve = new CurvePoint[this.boundingRect.Width];
            for (int i = 0; i < this.boundingRect.Width; i++)
            {
                this.points[i] = new CurvePoint(transform, boundingRect, i);
            }

            for (int i = 0; i < this.points.Length; i++)
            {
                var point = this.points[i];
                point.y = PrimaryCurve.ApplyFunction(this.composedFunction, point.x, this.domain, this.boundingRect);
            }

            for (int i = 0; i < this.points.Length; i++)
            {
                this.baseCurve[i] = new CurvePoint(transform, boundingRect, i);
                var point = this.baseCurve[i];
                point.y = PrimaryCurve.ApplyFunction(Functions.NoOp, point.x, this.domain, this.boundingRect);
            }

            this.tween = BuildTween();
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw curve
            DrawCurve(spriteBatch, this.points, transform, transform.Depth - 15, OnColor, OffColor, boundingRect, 3f);
            DrawCurve(spriteBatch, this.baseCurve, transform, transform.Depth - 10, new Color(OnColor, 0.15f), new Color(OffColor, 0.15f), boundingRect, 1.5f);

            // draw axis lines
            var center = this.boundingRect.TopLeft + this.boundingRect.Size.ToVector2() / 2;
            var topLeft = this.boundingRect.TopLeft;
            var lineColor = new Color(Color.Black, 0.5f);
            spriteBatch.DrawLine(new Vector2(center.X, topLeft.Y), new Vector2(center.X, topLeft.Y + this.boundingRect.Height), lineColor, 3f, transform.Depth - 1);
            spriteBatch.DrawLine(new Vector2(topLeft.X, center.Y), new Vector2(topLeft.X + this.boundingRect.Width, center.Y), lineColor, 3f, transform.Depth - 1);
        }

        private static void DrawCurve(SpriteBatch spriteBatch, CurvePoint[] points, Transform transform, Depth depth, Color onColor, Color offColor, BoundingRect boundingRect, float thickness)
        {
            var prevPoint = points[1];
            for (int i = 2; i < points.Length - 2; i++)
            {
                var adjustedPoint = PrimaryCurve.Adjusted(points[i + 1].WorldPosition, transform, boundingRect);
                var adjustedPrevPoint = PrimaryCurve.Adjusted(prevPoint.WorldPosition, transform, boundingRect);
                bool outOfBounds = (points[i + 1].WorldPosition != adjustedPoint);

                spriteBatch.DrawLine(adjustedPrevPoint, adjustedPoint, outOfBounds ? offColor : onColor, thickness, depth);
                prevPoint = points[i];
            }
        }

        private TweenChain BuildTween()
        {
            var tween = new TweenChain();
            var multi = tween.AppendMulticastTween();

            foreach (var point in this.points)
            {
                var channel = multi.AddChannel();
                var destY = point.y;
                var baseY = PrimaryCurve.ApplyFunction(Functions.NoOp, point.x, this.domain, this.boundingRect);

                var yAcc = new TweenAccessors<int>(() => point.y, val => point.y = val);
                channel.AppendIntTween(baseY, 0.1f, EaseFuncs.CubicEaseIn, yAcc);
                channel.AppendWaitTween(0.15f);
                channel.AppendIntTween(destY, 0.25f, EaseFuncs.CubicEaseOut, yAcc);
            }

            return tween;
        }

        public void PlayTween()
        {
            this.tween.Refresh();
        }

        public void StopTween()
        {
            this.tween.FinishRestOfTween_Dangerous();
        }
    }
}
