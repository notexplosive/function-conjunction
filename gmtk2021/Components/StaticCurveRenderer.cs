using gmtk2021.Data;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            PrimaryCurve.DrawPoints(spriteBatch, this.points, OnColor, OffColor, transform.Depth - 1, transform, this.boundingRect, 3f, 1, this.points.Length);
        }
    }
}
