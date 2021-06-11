using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    public class CardDropZone : BaseComponent
    {
        private readonly List<Slot> slots = new List<Slot>();
        private readonly List<Card> ownedCards = new List<Card>();
        private readonly BoundingRect boundingRect;
        private readonly TweenChain tween = new TweenChain();
        public Rectangle Rect => this.boundingRect.Rect;

        public CardDropZone(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.White, 1f, transform.Depth);

            foreach (var slot in this.slots)
            {
                spriteBatch.FillRectangle(slot.Rect, Color.Gray, transform.Depth + 10);
            }
        }

        public void AddCardSlot(Actor actor)
        {
            this.slots.Add(new Slot(actor));
        }

        public void Consume(Card card)
        {
            this.ownedCards.Remove(card);
            this.ownedCards.Add(card);
            ComputeLayout();
        }

        public void Detach(Card card)
        {
            this.ownedCards.Remove(card);
            ComputeLayout();
        }

        public void ComputeLayout()
        {
            this.tween.SkipToEnd();
            var multi = this.tween.AppendMulticastTween();

            for (int i = 0; i < this.slots.Count; i++)
            {
                if (i < this.ownedCards.Count)
                {
                    var channel = multi.AddChannel();
                    // We need to set `card` as its own variable so we don't mess up the captures
                    var card = this.ownedCards[i];
                    var pos = new TweenAccessors<Vector2>(() => card.transform.Position, val => card.transform.Position = val);
                    channel.AppendVectorTween(this.slots[i].Position, 0.25f, EaseFuncs.CubicEaseOut, pos);
                }
            }
        }

        private class Slot
        {
            private readonly Transform transform;
            private readonly BoundingRect boundingRect;

            public Rectangle Rect => this.boundingRect.Rect;
            public Vector2 Position => transform.Position;

            public Slot(Actor actor)
            {
                this.boundingRect = actor.GetComponent<BoundingRect>();
                this.transform = actor.transform;
            }
        }
    }
}
