using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    public class PrimaryCurve : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly DomainRange domain;
        private readonly Func<float, float> objectiveFunction;
        private readonly SoundEffectInstance objectiveTravelSound;
        private readonly SoundEffectInstance primaryTravelSound;
        private readonly TweenChain tween = new TweenChain();
        private readonly TweenChain introTween = new TweenChain();
        private CurvePoint[] points;
        private CurvePoint[] objectivePoints;
        private int objectiveLastDrawIndex;
        private int primaryLastDrawIndex;
        public event Action IntroFinished;

        public PrimaryCurve(Actor actor, DomainRange curveData, Function[] objective) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.domain = curveData;
            this.objectiveFunction = Functions.Fold(objective);

            this.objectiveTravelSound = MachinaGame.Assets.GetSoundEffectInstance("sin");
            this.objectiveTravelSound.Volume = 0.75f;
            this.objectiveTravelSound.IsLooped = true;

            this.primaryTravelSound = MachinaGame.Assets.GetSoundEffectInstance("saw");
            this.primaryTravelSound.Volume = 0.25f;
            this.primaryTravelSound.IsLooped = true;

            this.objectiveLastDrawIndex = 0;
            this.primaryLastDrawIndex = 0;
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
                this.objectivePoints[i].y = ApplyFunction(this.objectiveFunction, this.objectivePoints[i].x, this.domain, this.boundingRect);
            }

            this.introTween.Clear();
            this.introTween
                .AppendWaitTween(0.25f)
                .AppendCallback(() => { this.objectiveTravelSound.Play(); })
                .AppendIntTween(this.objectivePoints.Length - 1, 1, EaseFuncs.Linear, new TweenAccessors<int>(() => this.objectiveLastDrawIndex, val => this.objectiveLastDrawIndex = val))
                .AppendCallback(() => { this.objectiveTravelSound.Stop(); })
                .AppendWaitTween(0.25f)
                .AppendCallback(() => { this.primaryTravelSound.Play(); })
                .AppendIntTween(this.points.Length - 1, 1, EaseFuncs.Linear, new TweenAccessors<int>(() => this.primaryLastDrawIndex, val => this.primaryLastDrawIndex = val))
                .AppendCallback(() => { this.primaryTravelSound.Stop(); })
                .AppendCallback(() => { IntroFinished?.Invoke(); })
                ;
        }

        public override void Update(float dt)
        {
            this.introTween.Update(dt);
            this.tween.Update(dt);

            if (this.objectiveTravelSound.State == SoundState.Playing)
            {
                this.objectiveTravelSound.Pitch = this.objectivePoints[this.objectiveLastDrawIndex].Percent;
            }

            if (this.primaryTravelSound.State == SoundState.Playing)
            {
                this.primaryTravelSound.Pitch = this.points[this.primaryLastDrawIndex].Percent;
            }
        }

        public bool IsDoneTweening => this.tween.IsDone();

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPoints(spriteBatch, this.points, Color.Orange, Color.OrangeRed, transform.Depth, transform, this.boundingRect, 8f, 20, this.primaryLastDrawIndex);
            DrawPoints(spriteBatch, this.objectivePoints, Color.Cyan, Color.Teal, transform.Depth + 1, transform, this.boundingRect, 10f, 20, this.objectiveLastDrawIndex);

            // Draw zero lines
            var guidelineColor = new Color(100, 100, 100);
            var center = transform.Position + this.boundingRect.Rect.Size.ToVector2() / 2;
            spriteBatch.DrawLine(new Vector2(center.X, transform.Position.Y), new Vector2(center.X, transform.Position.Y + this.boundingRect.Height), guidelineColor, 1f, transform.Depth + 10);
            spriteBatch.DrawLine(new Vector2(transform.Position.X, center.Y), new Vector2(transform.Position.X + this.boundingRect.Width, center.Y), guidelineColor, 1f, transform.Depth + 10);
        }

        public static void DrawPoints(SpriteBatch spriteBatch, CurvePoint[] curvePoints, Color onColor, Color offColor, Depth depth, Transform transform, BoundingRect boundingRect, float thickness, int sizzleFactor, int lastDrawIndex)
        {
            // I'm doing this clever trick and I wanna write it down:
            // Intuitively you'd go for each point draw a line from prevPoint -> currPoint
            // Instead I do foreach point draw a line from prevPoint -> nextPoint
            // This is the same number of line segments as the former but draws a "fuller" line
            var prevPoint = curvePoints[1];
            for (int i = 2; i < Math.Min(curvePoints.Length - 2, lastDrawIndex); i += 3)
            {
                var adjustedPoint = Adjusted(curvePoints[i + 1].WorldPosition, transform, boundingRect);
                var adjustedPrevPoint = Adjusted(prevPoint.WorldPosition, transform, boundingRect);
                bool outOfBounds = (curvePoints[i + 1].WorldPosition != adjustedPoint);

                var sizzle = Vector2.Zero;

                if (sizzleFactor != 0 && i % 2 == 0)
                {
                    sizzle = new Vector2((float) MachinaGame.Random.DirtyRandom.Next(-sizzleFactor, sizzleFactor) * 2 / sizzleFactor, 0);
                }

                spriteBatch.DrawLine(adjustedPrevPoint + sizzle, adjustedPoint + sizzle, outOfBounds ? offColor : onColor, thickness, depth);
                prevPoint = curvePoints[i];
            }
        }

        public Func<bool> StartParticleAnimation()
        {
            var particle = new ParticleAnimation(this.actor.scene.AddActor("ParticleActor"), this.points, this.boundingRect);
            return particle.IsDone;
        }

        public static Vector2 Adjusted(Vector2 vec, Transform transform, BoundingRect boundingRect)
        {
            return new Vector2(vec.X, Math.Clamp(vec.Y, transform.Position.Y, transform.Position.Y + boundingRect.Height));
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
            if (Game1.DebugLevel >= DebugLevel.Passive && state == ButtonState.Pressed)
            {
                if (key == Keys.Q)
                {
                    OnFunctionUpdated((i) => i);
                    MachinaGame.Print("DBG: Curve reset");
                }
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
                var targetVal = ApplyFunction(function, this.points[i].x, this.domain, this.boundingRect);
                var point = this.points[i];
                var yAccessors = new TweenAccessors<int>(() => point.y, val => point.y = val);
                multiTween.AddChannel().AppendIntTween(targetVal, 0.25f, EaseFuncs.EaseOutBack, yAccessors);

                results[i] = targetVal; // for debugging
            }

            return; // for debugging
        }

        public static int ApplyFunction(Func<float, float> function, int x, DomainRange domain, BoundingRect boundingRect)
        {
            // Flip value because y is facing down
            var arg = ((float) x / boundingRect.Width - 0.5f) * domain.widthDomain * 2;
            var scalar = boundingRect.Height / 2 / domain.heightDomain;
            var rawOutput = function(arg);
            if (!float.IsNormal(rawOutput))
            {
                rawOutput = 0;
            }
            return -(int) (rawOutput * scalar);
        }
    }
}
