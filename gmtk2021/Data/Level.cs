using gmtk2021.Components;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        } = Array.Empty<Function>();

        public Function[] LockedInCards
        {
            get; set;
        } = Array.Empty<Function>();

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

        public static void CleanShuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = MachinaGame.Random.CleanRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void Validate()
        {
            Debug.Assert(Solution != null);

            var lockedIn = new List<Function>(LockedInCards);
            var deck = new List<Function>(CardFunctions);

            var shouldShuffle = CardFunctions.Length == 0;

            foreach (var function in Solution)
            {
                if (!lockedIn.Contains(function) && !deck.Contains(function))
                {
                    deck.Add(function);
                }
            }

            if (shouldShuffle)
            {
                CleanShuffle(deck);
            }

            CardFunctions = deck.ToArray();
        }
    }
}

