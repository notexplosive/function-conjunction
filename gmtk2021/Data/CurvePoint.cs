using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    public class CurvePoint
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
