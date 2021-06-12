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
    class SequenceDropZone : BaseComponent
    {
        public event Action<Func<float, float>> FunctionUpdated;
        private readonly CardDropZone dropZone;
        private int pendingUp;

        public SequenceDropZone(Actor actor) : base(actor)
        {
            this.dropZone = RequireComponent<CardDropZone>();
            this.dropZone.CardGain += OnCardGain;
            this.dropZone.CardLost += OnCardLost;
        }

        public override void Start()
        {
            BuildFunction();
        }

        public override void Update(float dt)
        {
            if (this.pendingUp < 0)
            {
                MachinaGame.Assets.GetSoundEffectInstance("downbend" + MachinaGame.Random.DirtyRandom.Next(1, 4)).Play();
            }
            if (this.pendingUp > 0)
            {
                MachinaGame.Assets.GetSoundEffectInstance("upbend" + MachinaGame.Random.DirtyRandom.Next(1, 4)).Play();
            }
            this.pendingUp = 0;
        }

        public void LockAll()
        {
            foreach (var card in this.dropZone.OwnedCards)
            {
                if (!card.IsLocked)
                    card.Lock();
            }
        }

        private void OnCardLost()
        {
            this.pendingUp--;
            BuildFunction();
        }

        private void OnCardGain()
        {
            this.pendingUp++;
            BuildFunction();
        }

        private void BuildFunction()
        {
            Func<float, float> function = Functions.NoOp;

            var cards = this.dropZone.OwnedCards;
            foreach (var card in cards)
            {
                var innerFunction = function;
                function = (i) => card.function.func(innerFunction(i));
            }

            FunctionUpdated?.Invoke(function);
        }
    }
}
