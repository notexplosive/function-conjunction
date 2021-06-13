using gmtk2021.Data;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private int deltaCardCount;
        private bool wasCardChange;
        private SoundEffectInstance lockSound;

        public SequenceDropZone(Actor actor) : base(actor)
        {
            this.lockSound = MachinaGame.Assets.CreateSoundEffectInstance("drop");
            this.lockSound.Pitch = -1;
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
            if (this.wasCardChange)
            {
                if (this.deltaCardCount < 0)
                {
                    MachinaGame.Assets.GetSoundEffectInstance("downbend" + MachinaGame.Random.DirtyRandom.Next(1, 4)).Play();
                }

                if (this.deltaCardCount > 0)
                {
                    MachinaGame.Assets.GetSoundEffectInstance("upbend" + MachinaGame.Random.DirtyRandom.Next(1, 4)).Play();
                }

                if (this.deltaCardCount == 0)
                {
                    MachinaGame.Assets.GetSoundEffectInstance("middlebend" + MachinaGame.Random.DirtyRandom.Next(1, 4)).Play();
                }

                this.wasCardChange = false;
            }

            this.deltaCardCount = 0;
        }

        public void LockAll()
        {
            this.lockSound.Pitch = 1f;
            this.lockSound.Play();
            foreach (var card in this.dropZone.OwnedCards)
            {
                if (!card.IsLocked)
                    card.Lock();
            }
        }

        private void OnCardLost()
        {
            this.wasCardChange = true;
            this.deltaCardCount--;
            BuildFunction();
        }

        private void OnCardGain()
        {
            this.wasCardChange = true;
            this.deltaCardCount++;
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
