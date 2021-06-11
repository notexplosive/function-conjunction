using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    static class Functions
    {
        public static float NoOp(float i)
        {
            return i;
        }

        public static float Flat(float i)
        {
            return 0;
        }

        public static float Sin(float i)
        {
            return MathF.Sin(i);
        }

        public static float Square(float i)
        {
            return i * i;
        }

        public static float Cos(float i)
        {
            return MathF.Cos(i);
        }
    }
}
