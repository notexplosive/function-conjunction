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
    public interface IZone
    {
        public Rectangle Rect
        {
            get;
        }
        public int Index
        {
            get;
        }
        public float PercentInZone(Rectangle rect);
    }

    public class CardDropZone : BaseComponent, IZone
    {
        public int Index => 0;
        private readonly List<Slot> slots = new List<Slot>();
        private readonly List<InBetween> inBetweens = new List<InBetween>();
        private readonly List<Card> ownedCards = new List<Card>();
        private readonly BoundingRect boundingRect;
        private readonly bool isSequencer;
        private readonly TweenChain tween = new TweenChain();
        public Rectangle Rect => this.boundingRect.Rect;

        public List<Card> OwnedCards => new List<Card>(this.ownedCards);

        public event Action CardGain;
        public event Action CardLost;

        public CardDropZone(Actor actor, bool isSequencer) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.isSequencer = isSequencer;
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.White, 2f, transform.Depth);

            foreach (var slot in this.slots)
            {
                spriteBatch.FillRectangle(slot.Rect, slot.Index < this.ownedCards.Count ? Color.Teal : Color.Gray, transform.Depth + 10);
            }
        }

        public bool IsFull()
        {
            return this.ownedCards.Count == this.slots.Count;
        }

        public Rectangle SlotRectAt(int subzoneIndex, bool forceActualRect = false)
        {
            var max = this.ownedCards.Count;

            if (IsFull())
            {
                max = max - 1;
            }

            List<Zone> arr = new List<Zone>(this.slots);

            if (subzoneIndex >= max)
            {
                forceActualRect = true;
            }

            if (!forceActualRect)
            {
                if (this.isSequencer)
                {
                    arr = new List<Zone>(this.inBetweens);
                }
            }

            if (subzoneIndex < 0)
            {
                return arr[max].Rect;
            }
            return arr[Math.Min(subzoneIndex, max)].Rect;
        }

        public void AddCardSlot(Actor actor)
        {
            this.slots.Add(new Slot(actor, this.slots.Count));
        }

        public void Consume(Card card, int index = -1, bool skipAnimation = false)
        {
            if (IsFull())
            {
                return;
            }

            this.ownedCards.Remove(card);

            if (index == -1)
            {
                this.ownedCards.Add(card);
            }
            else
            {
                if (index < this.ownedCards.Count)
                    this.ownedCards.Insert(index, card);
                else
                    this.ownedCards.Add(card);
            }
            TweenCardsToLayout(skipAnimation);
            CardGain?.Invoke();
        }

        public float PercentInZone(Rectangle card)
        {
            // DUPLICATE CODE ALERT
            var intersect = GetIntersection(card, Rect);

            float intersectArea = intersect.Width * intersect.Height;
            float cardArea = card.Width * card.Height;
            return intersectArea / cardArea;
        }

        public static Rectangle GetIntersection(Rectangle a, Rectangle b)
        {
            var left = Math.Max(a.X, b.X);
            var right = Math.Min(a.X + a.Width, b.X + b.Width);
            var top = Math.Max(a.Y, b.Y);
            var bottom = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (left < right && top < bottom)
            {
                return new Rectangle(left, top, right - left, bottom - top);
            }
            else
            {
                return new Rectangle();
            }
        }

        public List<Zone> SubZones()
        {
            var list = new List<Zone>(this.inBetweens);
            list.AddRange(this.slots);
            return list;
        }

        public void Detach(Card card)
        {
            bool hadCard = this.ownedCards.Remove(card);
            if (hadCard)
            {
                CardLost?.Invoke();
            }

            TweenCardsToLayout();

        }

        public void TweenCardsToLayout(bool skipAnimation = false)
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
                    var slot = this.slots[i];
                    var pos = new TweenAccessors<Vector2>(() => card.transform.Position, val => card.transform.Position = val);
                    channel.AppendVectorTween(slot.Position, 0.25f, EaseFuncs.CubicEaseOut, pos);

                    if (skipAnimation)
                    {
                        card.transform.Position = slot.Position;
                    }
                }
            }
        }

        public bool OwnsThisCard(Card card)
        {
            return this.ownedCards.Contains(card);
        }

        public abstract class Zone : IZone
        {
            private readonly BoundingRect boundingRect;
            public Rectangle Rect => this.boundingRect.Rect;
            public int Index => this.index;
            private readonly int index;

            protected Zone(Actor actor, int index)
            {
                this.boundingRect = actor.GetComponent<BoundingRect>();
                this.index = index;
            }

            public float PercentInZone(Rectangle card)
            {
                // DUPLICATE CODE ALERT
                var intersect = GetIntersection(card, Rect);

                float intersectArea = intersect.Width * intersect.Height;
                float cardArea = card.Width * card.Height;
                return intersectArea / cardArea;
            }

            public static T FindBestZone<T>(Rectangle activeRect, List<T> zones) where T : class, IZone
            {
                T best = null;
                foreach (var currentSubzone in zones)
                {
                    if (currentSubzone.PercentInZone(activeRect) > 0)
                    {
                        if (best == null)
                        {
                            best = currentSubzone;
                        }
                        else
                        {
                            if (currentSubzone.PercentInZone(activeRect) > best.PercentInZone(activeRect))
                            {
                                best = currentSubzone;
                            }
                        }
                    }
                }
                return best;
            }
        }

        private class Slot : Zone
        {
            private readonly Transform transform;
            public Vector2 Position => transform.Position;

            public Slot(Actor actor, int index) : base(actor, index)
            {
                this.transform = actor.transform;
            }
        }

        public void AddInBetween(Actor cardInsertion, int i)
        {
            this.inBetweens.Add(new InBetween(cardInsertion, i));
        }

        private class InBetween : Zone
        {
            public InBetween(Actor actor, int index) : base(actor, index)
            {
            }
        }
    }
}
