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

        public SequenceDropZone(Actor actor) : base(actor)
        {
            this.dropZone = RequireComponent<CardDropZone>();
            this.dropZone.CardGain += OnCardGain;
            this.dropZone.CardLost += OnCardLost;
        }

        private void OnCardLost()
        {
            BuildFunction();
        }

        private float NoOp(float i)
        {
            return i;
        }

        private float Flat(float i)
        {
            return 0;
        }

        private float Sin(float i)
        {
            return MathF.Sin(i);
        }

        private void OnCardGain()
        {
            BuildFunction();
        }

        private void BuildFunction()
        {
            Func<float, float> function = NoOp;

            if (this.dropZone.OwnedCards.Count == 0)
            {
                function = Flat;
            }

            foreach (var card in this.dropZone.OwnedCards)
            {
                var innerFunction = function;
                function = (i) => Sin(innerFunction(i));
            }

            FunctionUpdated?.Invoke(function);
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
