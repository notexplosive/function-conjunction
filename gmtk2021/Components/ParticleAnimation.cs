using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class ParticleAnimation : BaseComponent
    {
        private readonly TweenChain tween = new TweenChain();
        private int x;
        private float size;
        private readonly SoundEffectInstance travelSound;
        private readonly CurvePoint[] points;
        private readonly BoundingRect curveBoundingRect;

        public ParticleAnimation(Actor actor, CurvePoint[] points, BoundingRect curveBoundingRect) : base(actor)
        {
            this.x = 1;
            var xAcc = new TweenAccessors<int>(() => this.x, val => this.x = val);
            var sizeAcc = new TweenAccessors<float>(() => this.size, val => this.size = val);
            this.travelSound = MachinaGame.Assets.GetSoundEffectInstance("particle_travel");
            this.travelSound.IsLooped = true;
            this.points = points;
            this.curveBoundingRect = curveBoundingRect;
            this.tween
                .AppendCallback(() => { MachinaGame.Assets.GetSoundEffectInstance("particle_appear").Play(); })
                .AppendFloatTween(1, 0.35f, EaseFuncs.EaseOutBack, sizeAcc)
                .AppendCallback(() => { travelSound.Play(); })
                .AppendIntTween(points.Length - 1, 1, EaseFuncs.Linear, xAcc)
                .AppendCallback(() => { MachinaGame.Assets.GetSoundEffectInstance("particle_disappear").Play(); })
                .AppendCallback(() => { travelSound.Stop(); })
                .AppendFloatTween(0, 0.35f, EaseFuncs.EaseInBack, sizeAcc)
                .AppendWaitTween(0.15f)
                ;
        }

        public override void Update(float dt)
        {
            var heightPercent = this.points[this.x].Percent;
            this.travelSound.Pitch = Math.Clamp(heightPercent, -1f, 1f);
            this.tween.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = PrimaryCurve.Adjusted(this.points[this.x].WorldPosition, curveBoundingRect.transform, curveBoundingRect);
            var random = MachinaGame.Random.DirtyRandom;
            spriteBatch.DrawCircle(new CircleF(pos, 10 * this.size + random.Next(0, 10)), 10, Color.OrangeRed, 3f, transform.Depth - 20);
            spriteBatch.DrawCircle(new CircleF(pos, 24 * this.size + random.Next(0, 10)), 10, Color.Orange, 3f, transform.Depth - 20);
            spriteBatch.DrawCircle(new CircleF(pos, 36 * this.size + random.Next(0, 10)), 10, Color.Cyan, 3f, transform.Depth - 20);
        }

        public bool IsDone()
        {
            return this.tween.IsDone();
        }
    }
}
