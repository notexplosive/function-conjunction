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
    class CurveRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
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

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var prevPoint = this.points[0];
            for (int i = 1; i < this.points.Length; i++)
            {
                spriteBatch.DrawLine(prevPoint.WorldPosition, this.points[i].WorldPosition, Color.Orange, 3f, transform.Depth);
                prevPoint = this.points[i];
            }
        }

        public void OnFunctionUpdated(Func<float, float> function)
        {
            for (int i = 0; i < this.points.Length; i++)
            {
                this.points[i].SetY((int) (function(this.points[i].x / 50f) * this.boundingRect.Height / 2));
            }
        }

        private struct CurvePoint
        {
            public readonly int x;

            private int y;
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

            public void SetY(int val)
            {
                // Flip value because y is facing down
                this.y = -val;
            }
        }
    }
}
