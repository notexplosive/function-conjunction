using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    public class Card : BaseComponent
    {
        private readonly List<CardDropZone> dropZones;
        private readonly BoundingRect boundingRect;
        private readonly Depth startingDepth;
        public readonly Function function;

        public Card(Actor actor, List<CardDropZone> dropZones, Function function) : base(actor)
        {
            this.dropZones = dropZones;
            this.boundingRect = RequireComponent<BoundingRect>();
            var draggable = RequireComponent<Draggable>();
            draggable.DragEnd += OnDragEnd;
            draggable.DragStart += OnDragStart;

            this.startingDepth = transform.Depth;
            this.function = function;
        }

        private CardDropZone FindOwner()
        {
            foreach (var dropZone in this.dropZones)
            {
                if (dropZone.OwnsThisCard(this))
                {
                    return dropZone;
                }
            }
            return null;
        }

        private void OnDragStart(Vector2 obj)
        {
            MachinaGame.Assets.GetSoundEffectInstance("pickup").Play();
            transform.Depth = this.startingDepth - 50;
        }

        private void OnDragEnd(Vector2 finalMousePosition)
        {
            MachinaGame.Assets.GetSoundEffectInstance("drop").Play();
            transform.Depth = this.startingDepth;
            var wasConsumed = false;
            var originalOwner = FindOwner();
            foreach (var dropZone in this.dropZones)
            {
                if (this.boundingRect.Rect.Intersects(dropZone.Rect) && !wasConsumed)
                {
                    dropZone.Consume(this);
                    wasConsumed = true;
                }
                else
                {
                    dropZone.Detach(this);
                }
            }

            if (FindOwner() == null)
            {
                originalOwner.Consume(this);
            }
        }
    }
}
