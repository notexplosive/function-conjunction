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

        public static Function Sign = new Function("Sign", (float i) =>
        {
            return MathF.Sign(i);
        });

        public static Function AddConstant(int constant)
        {
            return new Function((constant > 0 ? "+ " : "- ") + constant.ToString(), (i) => i + constant);
        }

        public static Function MultiplyConstant(int constant)
        {
            return new Function(constant > 0 ? "* " + constant : "* (" + constant + ")", (i) => i * constant);
        }

        public static Function MultiplyFraction(int numerator, int denominator)
        {
            return new Function("* " + numerator + " / " + denominator, (i) => i * (float) numerator / (float) denominator);
        }

        public static Function Floor = new Function("Floor", MathF.Floor);
        public static Function Ceiling = new Function("Ceiling", MathF.Ceiling);
        public static Function Truncate = new Function("Trunc", MathF.Truncate);
        public static Function Sqrt = new Function("Sqrt", MathF.Sqrt);

        public static Function MaxConstant(int constant)
        {
            return new Function("Max " + constant, (i) => MathF.Max(i, constant));
        }

        public static Function MinConstant(int constant)
        {
            return new Function("Min " + constant, (i) => MathF.Min(i, constant));
        }

        public static Function ModConstant(int constant)
        {
            return new Function("Mod " + constant, (i) => MathF.IEEERemainder(i, constant));
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
