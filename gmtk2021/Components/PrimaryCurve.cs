using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class PrimaryCurve : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly CurveData curveData;
        private readonly Func<float, float> objectiveFunction;
        private readonly TweenChain tween = new TweenChain();
        private CurvePoint[] points;
        private CurvePoint[] objectivePoints;

        public PrimaryCurve(Actor actor, CurveData curveData, Function[] objective) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.curveData = curveData;
            this.objectiveFunction = Functions.Fold(objective);
        }

        public override void Start()
        {
            // Needs to be after boundingRect has been settled by UI
            this.points = new CurvePoint[this.boundingRect.Width];
            for (int i = 0; i < this.boundingRect.Width; i++)
            {
                this.points[i] = new CurvePoint(transform, boundingRect, i);
            }

            this.objectivePoints = new CurvePoint[this.boundingRect.Width];
            for (int i = 0; i < this.boundingRect.Width; i++)
            {
                this.objectivePoints[i] = new CurvePoint(transform, boundingRect, i);
                this.objectivePoints[i].y = ApplyFunction(this.objectiveFunction, this.objectivePoints[i].x);
            }
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
        }

        public bool IsDoneTweening => this.tween.IsDone();

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPoints(spriteBatch, this.points, Color.Orange, Color.OrangeRed, transform.Depth);
            DrawPoints(spriteBatch, this.objectivePoints, Color.Cyan, Color.Teal, transform.Depth + 1);

            // Draw zero lines
            var guidelineColor = new Color(100, 100, 100);
            var center = transform.Position + this.boundingRect.Rect.Size.ToVector2() / 2;
            spriteBatch.DrawLine(new Vector2(center.X, transform.Position.Y), new Vector2(center.X, transform.Position.Y + this.boundingRect.Height), guidelineColor, 1f, transform.Depth + 10);
            spriteBatch.DrawLine(new Vector2(transform.Position.X, center.Y), new Vector2(transform.Position.X + this.boundingRect.Width, center.Y), guidelineColor, 1f, transform.Depth + 10);
        }

        private void DrawPoints(SpriteBatch spriteBatch, CurvePoint[] curvePoints, Color onColor, Color offColor, Depth depth)
        {
            // I'm doing this clever trick and I wanna write it down:
            // Intuitively you'd go for each point draw a line from prevPoint -> currPoint
            // Instead I do foreach point draw a line from prevPoint -> nextPoint
            // This is the same number of line segments as the former but draws a "fuller" line
            var prevPoint = curvePoints[1];
            for (int i = 2; i < curvePoints.Length - 2; i += 3)
            {
                var adjustedPoint = Adjusted(curvePoints[i + 1].WorldPosition);
                var adjustedPrevPoint = Adjusted(prevPoint.WorldPosition);
                bool outOfBounds = (curvePoints[i + 1].WorldPosition != adjustedPoint);
                var sizzle = i % 2 == 0 ? new Vector2(MachinaGame.Random.DirtyRandom.Next(-20, 20), 0) / 10 : Vector2.Zero;

                spriteBatch.DrawLine(adjustedPrevPoint + sizzle, adjustedPoint + sizzle, outOfBounds ? offColor : onColor, 8f, depth);
                prevPoint = curvePoints[i];
            }
        }

        private Vector2 Adjusted(Vector2 vec)
        {
            return new Vector2(vec.X, Math.Clamp(vec.Y, transform.Position.Y, transform.Position.Y + this.boundingRect.Height));
        }

        public bool MatchWithObjective()
        {
            for (int i = 0; i < this.points.Length; i++)
            {
                if (this.points[i].y != this.objectivePoints[i].y)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Q && Game1.DebugLevel >= DebugLevel.Passive && state == ButtonState.Pressed)
            {
                OnFunctionUpdated((i) => i);
            }
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

            var results = new int[this.points.Length]; // for debugging

            for (int i = 0; i < this.points.Length; i++)
            {
                var targetVal = ApplyFunction(function, this.points[i].x);
                var point = this.points[i];
                var yAccessors = new TweenAccessors<int>(() => point.y, val => point.y = val);
                multiTween.AddChannel().AppendIntTween(targetVal, 0.25f, EaseFuncs.EaseOutBack, yAccessors);

                results[i] = targetVal; // for debugging
            }

            return;
        }

        public int ApplyFunction(Func<float, float> function, int x)
        {
            // Flip value because y is facing down
            var arg = ((float) x / this.boundingRect.Width - 0.5f) * this.curveData.widthDomain * 2;
            var scalar = this.boundingRect.Height / 2 / this.curveData.heightDomain;
            var rawOutput = function(arg);
            if (!float.IsNormal(rawOutput))
            {
                rawOutput = 0;
            }
            return -(int) (rawOutput * scalar);
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

            public override string ToString()
            {
                return "y: " + this.y;
            }
        }
    }
}
