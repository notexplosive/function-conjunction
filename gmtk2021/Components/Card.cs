using gmtk2021.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;
using static gmtk2021.Components.CardDropZone;

namespace gmtk2021.Components
{
    public class Card : BaseComponent
    {
        private readonly List<CardDropZone> dropZones;
        private readonly Depth startingDepth;
        private readonly BoundingRect boundingRect;
        public readonly Function function;
        private readonly CardDropZone defaultDropZone;
        private bool isDragging;
        private CardDropZone targetDropZone;
        private int? subzoneIndex;

        public Rectangle Rect => this.boundingRect.Rect;

        public Card(Actor actor, List<CardDropZone> dropZones, Function function, CardDropZone defaultDropZone) : base(actor)
        {
            this.dropZones = dropZones;
            this.boundingRect = RequireComponent<BoundingRect>();
            var draggable = RequireComponent<Draggable>();
            draggable.DragEnd += OnDragEnd;
            draggable.DragStart += OnDragStart;
            draggable.Drag += DuringDrag;

            this.startingDepth = transform.Depth;
            this.function = function;
            this.defaultDropZone = defaultDropZone;

            this.defaultDropZone.Consume(this, skipAnimation: true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isDragging && this.targetDropZone != null)
            {
                spriteBatch.DrawRectangle(this.targetDropZone.Rect, Color.White, 3f, this.targetDropZone.transform.Depth);

                if (this.subzoneIndex.HasValue)
                {
                    var rect = this.targetDropZone.SlotRectAt(this.subzoneIndex.Value);
                    spriteBatch.DrawRectangle(rect, Color.White, 3f, this.targetDropZone.transform.Depth);
                }
            }
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

        private void DuringDrag(Vector2 obj)
        {
            this.targetDropZone = null;
            this.subzoneIndex = null;
            CardDropZone bestZone = Zone.FindBestZone(Rect, this.dropZones);

            if (bestZone != null)
            {
                Zone bestSubZone = Zone.FindBestZone(Rect, bestZone.SubZones());
                if (bestSubZone != null)
                {
                    this.subzoneIndex = bestSubZone.Index;
                }
            }

            this.targetDropZone = bestZone;
        }

        private void OnDragStart(Vector2 obj)
        {
            this.isDragging = true;
            MachinaGame.Assets.GetSoundEffectInstance("pickup").Play();
            transform.Depth = this.startingDepth - 50;
        }

        private void OnDragEnd(Vector2 finalMousePosition)
        {
            this.isDragging = false;
            MachinaGame.Assets.GetSoundEffectInstance("drop").Play();
            transform.Depth = this.startingDepth;
            var originalDropZone = FindOwner();

            if (this.targetDropZone != null)
            {
                if (this.targetDropZone != originalDropZone)
                {
                    originalDropZone.Detach(this);
                }
                this.targetDropZone.Consume(this, this.subzoneIndex.GetValueOrDefault(-1));
            }

            originalDropZone.TweenCardsToLayout();

            this.targetDropZone = null;

        }
    }
}
