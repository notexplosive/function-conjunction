using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class FakeCard : BaseComponent, ICard
    {
        public bool IsLocked => false;

        public bool IsPartialLocked => false;

        public bool UseCustomColor
        {
            get => true;
        }

        public FakeCard(Actor actor) : base(actor)
        {
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
