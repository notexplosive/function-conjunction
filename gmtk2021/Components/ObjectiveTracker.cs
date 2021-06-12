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

        public ObjectiveTracker(Actor actor, LevelTransition transition) : base(actor)
        {
            this.curve = RequireComponent<PrimaryCurve>();
            this.transition = transition;

            this.transition.SetGameScene(this.actor.scene);
        }

        public override void Update(float dt)
        {


            if (this.curve.IsDoneTweening && this.curve.MatchWithObjective() && !this.isWon)
            {
                this.isWon = true;
                this.transition.FinishLevel();
            }
        }
    }
}
