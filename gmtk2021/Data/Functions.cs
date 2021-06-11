using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    public static class Functions
    {
        public static float NoOp(float i)
        {
            return i;
        }

        public static Function Sin = new Function("Sine", (float i) =>
        {
            return MathF.Sin(i);
        });

        public static Function Square = new Function("Square", (float i) =>
        {
            return i * i;
        });

        public static Function Cos = new Function("Cosine", (float i) =>
        {
            return MathF.Cos(i);
        });

        public static Function Abs = new Function("Abs", (float i) =>
        {
            return MathF.Abs(i);
        });

        public static Function AddConstant(int constant)
        {
            return new Function((constant > 0 ? "+ " : "- ") + constant.ToString(), (i) => i + constant);
        }

        public static Function TimeConstant(int constant)
        {
            return new Function("* " + constant, (i) => i * constant);
        }

        public static Function TimesFraction(int numerator, int denominator)
        {
            return new Function("* " + numerator + " / " + denominator, (i) => i * (float) numerator / (float) denominator);
        }
    }

    public class Function
    {
        public readonly string name;
        public readonly Func<float, float> func;

        public Function(string name, Func<float, float> func)
        {
            this.name = name;
            this.func = func;
        }
    }
}
