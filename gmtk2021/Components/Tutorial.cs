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
    class Tutorial : BaseComponent
    {
        private readonly Rectangle startSlot;
        private readonly Rectangle destSlot;
        private Point travelingRectPos;
        private readonly TweenChain tween = new TweenChain();

        public Tutorial(Actor actor, CardDropZone startingDropZone, CardDropZone destinationZone) : base(actor)
        {
            this.startSlot = startingDropZone.SlotRectAt(0, true);
            this.destSlot = destinationZone.SlotRectAt(0, true);

            this.startSlot.Inflate(10, 10);
            this.destSlot.Inflate(10, 10);

            startingDropZone.CardLost += CancelTutorial;

            this.travelingRectPos = this.startSlot.Location;
            var pos = new TweenAccessors<Point>(() => this.travelingRectPos, val => this.travelingRectPos = val);

            this.tween
                .AppendCallback(() => { this.travelingRectPos = this.startSlot.Location; })
                .AppendWaitTween(0.5f)
                .AppendPointTween(new Point(this.destSlot.Location.X + 200, this.destSlot.Location.Y - 500), 0.5f, EaseFuncs.EaseInOutBack, pos)
                .AppendWaitTween(0.15f)
                .AppendPointTween(new Point(this.destSlot.Location.X, this.destSlot.Location.Y), 0.5f, EaseFuncs.EaseInOutBack, pos)
                .AppendWaitTween(0.5f)
                ;
        }

        private void CancelTutorial()
        {
            this.actor.Destroy();
        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);

            if (this.tween.IsDone())
            {
                this.tween.Refresh();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new Rectangle(this.travelingRectPos, this.destSlot.Size), Color.White, 5f, new Depth(0));
        }
    }
}
