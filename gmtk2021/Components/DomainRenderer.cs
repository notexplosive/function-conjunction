using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class DomainRenderer : BaseComponent
    {
        private readonly Orientation orientation;
        private readonly DomainRange curveData;
        private readonly BoundingRect boundingRect;
        private BoundedTextRenderer minimumText;
        private BoundedTextRenderer maximumText;
        private BoundedTextRenderer middleText;

        public MinMax<float> Range
        {
            get
            {
                if (this.orientation == Orientation.Horizontal)
                {
                    return new MinMax<float>(-curveData.widthDomain, curveData.widthDomain);
                }
                else
                {
                    return new MinMax<float>(-curveData.heightDomain, curveData.heightDomain);
                }
            }
        }

        public DomainRenderer(Actor actor, DomainRange curveData, Orientation orientation) : base(actor)
        {
            this.orientation = orientation;
            this.curveData = curveData;
            this.boundingRect = RequireComponent<BoundingRect>();

            var font = MachinaGame.Assets.GetSpriteFont("DomainFont");

            if (orientation == Orientation.Vertical)
            {
                new LayoutGroup(this.actor, orientation)
                    .AddHorizontallyStretchedElement("TopText", font.LineSpacing, textActor =>
                    {
                        this.maximumText = new BoundedTextRenderer(textActor, "max", font, Color.White, HorizontalAlignment.Right, VerticalAlignment.Top, Overflow.Ignore);
                    })
                    .VerticallyStretchedSpacer()
                    .AddHorizontallyStretchedElement("ZeroText", font.LineSpacing, textActor =>
                    {
                        this.middleText = new BoundedTextRenderer(textActor, "middle", font, Color.White, HorizontalAlignment.Right, VerticalAlignment.Center, Overflow.Ignore);
                    })
                    .VerticallyStretchedSpacer()
                    .AddHorizontallyStretchedElement("BottomText", font.LineSpacing, textActor =>
                    {
                        this.minimumText = new BoundedTextRenderer(textActor, "minimum", font, Color.White, HorizontalAlignment.Right, VerticalAlignment.Bottom, Overflow.Ignore);
                    });
            }
            else
            {
                new LayoutGroup(this.actor, orientation)
                    .AddBothStretchedElement("RightText", textActor =>
                    {
                        this.minimumText = new BoundedTextRenderer(textActor, "minimum", font, Color.White, HorizontalAlignment.Left, VerticalAlignment.Top, Overflow.Ignore);
                    })
                    .AddBothStretchedElement("ZeroText", textActor =>
                    {
                        this.middleText = new BoundedTextRenderer(textActor, "middle", font, Color.White, HorizontalAlignment.Center, VerticalAlignment.Top, Overflow.Ignore);
                    })
                    .AddBothStretchedElement("LeftText", textActor =>
                    {
                        this.maximumText = new BoundedTextRenderer(textActor, "max", font, Color.White, HorizontalAlignment.Right, VerticalAlignment.Top, Overflow.Ignore);
                    });
            }

            UpdateTextToMatchDomain();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.orientation == Orientation.Vertical)
                spriteBatch.DrawLine(transform.Position + new Vector2(this.boundingRect.Width, 0), transform.Position + this.boundingRect.Size.ToVector2(), Color.White, 1f, transform.Depth);
            else
                spriteBatch.DrawLine(transform.Position, transform.Position + new Vector2(this.boundingRect.Width, 0), Color.White, 1f, transform.Depth);
        }

        public void UpdateTextToMatchDomain()
        {
            this.minimumText.Text = FloatAsString(Range.min);
            this.middleText.Text = FloatAsString(0);
            this.maximumText.Text = FloatAsString(Range.max);
        }

        public static string FloatAsString(float val)
        {
            if (((int) val) == val)
            {
                return val.ToString();
            }

            if (Math.Abs(Math.Abs(val) % (MathF.PI / 2)) < 0.1f)
            {
                return ((val / MathF.PI)).ToString() + " PI";
            }

            return val.ToString(".##");
        }

        private float Opposite(Vector2 position)
        {
            if (this.orientation == Orientation.Vertical)
            {
                return position.X;
            }
            else
            {
                return position.Y;
            }
        }

        private float Normal(Vector2 position)
        {
            if (this.orientation == Orientation.Horizontal)
            {
                return position.X;
            }
            else
            {
                return position.Y;
            }
        }
    }
}
