using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Components
{
    class DoubleClickable : BaseComponent
    {
        private readonly Clickable clickable;
        private int doubleClickCounter;
        private float doubleClickTimer;
        public event Action DoubleClick;

        public DoubleClickable(Actor actor) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.onClick += OnClick;
        }

        private void OnClick(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                this.doubleClickCounter++;
                this.doubleClickTimer = 0.300f;

                if (this.doubleClickCounter >= 2)
                {
                    this.doubleClickCounter = 0;
                    DoubleClick?.Invoke();
                }
            }
        }
        public override void Update(float dt)
        {
            if (this.doubleClickTimer > 0)
            {
                this.doubleClickTimer -= dt;
            }
            else
            {
                this.doubleClickCounter = 0;
            }
        }

    }
}
