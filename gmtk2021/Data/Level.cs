using gmtk2021.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    public class Level
    {
        public Level(string title)
        {
            Title = title;
        }

        public Function[] Solution
        {
            get; set;
        }

        public Function[] CardFunctions
        {
            get; set;
        }
        public float Domain
        {
            get;
            set;
        } = MathF.PI * 2;
        public float Range
        {
            get;
            set;
        } = 2;
        public int NumberOfSequenceSlots => Solution.Length;

        public string Title
        {
            get;
            set;
        }
    }
}

