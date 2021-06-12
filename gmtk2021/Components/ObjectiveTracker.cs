using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class ObjectiveTracker : BaseComponent
    {
        private readonly PrimaryCurve curve;
        private readonly LevelTransition transition;
        private bool isWon;
        public event Action Win;

        public ObjectiveTracker(Actor actor, LevelTransition transition) : base(actor)
        {
            this.curve = RequireComponent<PrimaryCurve>();
            this.transition = transition;

            this.transition.SetGameScene(this.actor.scene, this.curve);
        }

        public override void Update(float dt)
        {


            if (this.curve.IsDoneTweening && this.curve.MatchWithObjective() && !this.isWon)
            {
                Win?.Invoke();
                this.isWon = true;
                this.transition.FinishLevel();
            }
        }
    }
}
