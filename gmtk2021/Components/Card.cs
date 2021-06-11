using Machina.Components;
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

        public Card(Actor actor, List<CardDropZone> dropZones) : base(actor)
        {
            this.dropZones = dropZones;
            this.boundingRect = RequireComponent<BoundingRect>();
            var draggable = RequireComponent<Draggable>();
            draggable.DragEnd += OnDragEnd;
            draggable.DragStart += OnDragStart;
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
        }

        private void OnDragEnd(Vector2 finalMousePosition)
        {
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
