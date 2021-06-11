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
        private readonly CardDropZone dropZone;
        private readonly BoundingRect boundingRect;

        public Card(Actor actor, CardDropZone dropZone) : base(actor)
        {
            this.dropZone = dropZone;
            this.boundingRect = RequireComponent<BoundingRect>();
            var draggable = RequireComponent<Draggable>();
            draggable.DragEnd += OnDragEnd;
            draggable.DragStart += OnDragStart;
        }

        private void OnDragStart(Vector2 obj)
        {
        }

        private void OnDragEnd(Vector2 finalMousePosition)
        {
            if (this.boundingRect.Rect.Intersects(this.dropZone.Rect))
            {
                this.dropZone.Consume(this);
            }
            else
            {
                this.dropZone.Detach(this);
            }
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
