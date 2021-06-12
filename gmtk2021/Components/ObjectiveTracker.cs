using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class ObjectiveTracker : BaseComponent
    {
        private PrimaryCurve curve;
        private TweenChain tween = new TweenChain();
        private bool isWon;

        public ObjectiveTracker(Actor actor) : base(actor)
        {
            this.curve = RequireComponent<PrimaryCurve>();
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);

            if (this.curve.IsDoneTweening && this.curve.MatchWithObjective() && !this.isWon)
            {
                this.isWon = true;
                var camera = this.actor.scene.camera;
                this.tween.AppendWaitTween(0.5f);
                this.tween.AppendVectorTween(
                    new Point(0, -camera.UnscaledViewportSize.Y),
                    1f, EaseFuncs.EaseInOutBack,
                    new TweenAccessors<Point>(() => camera.ScaledPosition, val => camera.ScaledPosition = val));
            }
        }
    }
}
