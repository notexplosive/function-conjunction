﻿using gmtk2021.Data;
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
        private readonly CardDropZone destinationZone;
        private bool isDragging;
        private CardDropZone targetDropZone;
        private int? subzoneIndex;
        private bool isGrabbed;

        public Rectangle Rect => this.boundingRect.Rect;

        public Card(Actor actor, List<CardDropZone> dropZones, Function function, CardDropZone defaultDropZone, CardDropZone destinationZone) : base(actor)
        {
            this.dropZones = dropZones;
            this.boundingRect = RequireComponent<BoundingRect>();
            var doubleClickable = RequireComponent<DoubleClickable>();
            doubleClickable.DoubleClick += OnDoubleClick;

            var draggable = RequireComponent<Draggable>();
            draggable.DragEnd += OnDragEnd;
            draggable.DragStart += OnDragStart;
            draggable.Drag += DuringDrag;

            this.startingDepth = transform.Depth;
            this.function = function;
            this.defaultDropZone = defaultDropZone;
            this.destinationZone = destinationZone;

            this.defaultDropZone.Consume(this, skipAnimation: true);
        }

        private void OnDoubleClick()
        {
            var currentZone = FindCurrentZone();
            currentZone.Detach(this);

            if (currentZone == this.destinationZone)
            {
                this.defaultDropZone.Consume(this);
            }
            else
            {
                this.destinationZone.Consume(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isDragging && this.targetDropZone != null)
            {
                var isValid = IsValidDrop(FindCurrentZone());

                spriteBatch.DrawRectangle(this.targetDropZone.Rect, isValid ? Color.White : Color.DarkBlue, 3f, this.targetDropZone.transform.Depth);

                if (isValid)
                {
                    var subzoneIndex = this.subzoneIndex.GetValueOrDefault(-1);
                    var rect = this.targetDropZone.SlotRectAt(subzoneIndex);
                    spriteBatch.DrawRectangle(rect, Color.White, 3f, this.targetDropZone.transform.Depth);
                }
            }
        }

        private CardDropZone FindCurrentZone()
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

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (positionDelta.LengthSquared() > 0)
            {
                if (this.isGrabbed)
                {
                    this.isDragging = true;
                }
            }
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
            this.isGrabbed = true;
            MachinaGame.Assets.GetSoundEffectInstance("pickup").Play();
            transform.Depth = this.startingDepth - 50;
        }

        private bool IsValidDrop(CardDropZone originalDropZone)
        {
            return this.targetDropZone != null && (!this.targetDropZone.IsFull() || this.targetDropZone == originalDropZone);
        }

        private void OnDragEnd(Vector2 finalMousePosition)
        {
            this.isGrabbed = false;
            if (this.isDragging)
            {
                this.isDragging = false;
                MachinaGame.Assets.GetSoundEffectInstance("drop").Play();
                transform.Depth = this.startingDepth;
                var originalDropZone = FindCurrentZone();

                if (IsValidDrop(originalDropZone))
                {
                    originalDropZone?.Detach(this);
                    this.targetDropZone.Consume(this, this.subzoneIndex.GetValueOrDefault(-1));
                    this.targetDropZone.TweenCardsToLayout();
                }
                else
                {
                    originalDropZone?.Detach(this);
                    this.defaultDropZone.Consume(this);
                    this.defaultDropZone.TweenCardsToLayout();
                }



                this.targetDropZone = null;
            }
        }
    }
}
