using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace gmtk2021
{
    public class Game1 : MachinaGame
    {
        public Game1(string[] args) : base("Nested Function", args, new Point(1600, 900), new Point(1600, 900), ResizeBehavior.FillContent)
        {
        }

        protected override void OnGameLoad()
        {
            SceneLayers.BackgroundColor = Color.Black;

            CreateCard();
        }

        public void CreateCard()
        {
            new BoundingRect();
        }
    }
}
