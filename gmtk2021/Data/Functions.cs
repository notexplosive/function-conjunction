﻿using gmtk2021.Components;
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

        public static Function X = new Function("X", NoOp, new DomainRange(1, 1));

        public static Function Sin = new Function("Sine", MathF.Sin, new DomainRange(MathF.PI, 2));

        public static Function Squared = new Function("Squared", (float i) =>
        {
            return i * i;
        }, new DomainRange(MathF.PI, 2));

        public static Function Cubed = new Function("Cubed", (float i) =>
        {
            return i * i * i;
        }, new DomainRange(MathF.PI, 2));

        public static Function Cos = new Function("Cosine", MathF.Cos, new DomainRange(MathF.PI, 2));

        public static Function Abs = new Function("Abs", MathF.Abs, new DomainRange(2, 2));

        public static Function Sign = new Function("Sign", (float i) =>
        {
            return MathF.Sign(i);
        }, new DomainRange(MathF.PI, 2));

        public static Function OneOverX = new Function("1 / X", (float i) =>
        {
            if (i == 0)
            {
                return 1000;
            }
            return 1 / i;
        }, new DomainRange(MathF.PI, 2));

        public static Function AddConstant(float constant)
        {
            return new Function((constant > 0 ? "Add " : "Subtract ") + DomainRenderer.FloatAsString(MathF.Abs(constant)), (i) => i + constant, new DomainRange(4, 4));
        }

        public static Function MultiplyConstant(int constant)
        {
            return new Function(constant > 0 ? "Multiply " + constant : "Multiply (" + constant + ")", (i) => i * constant, new DomainRange(2, 2));
        }

        public static Function MultiplyFraction(int numerator, int denominator)
        {
            return new Function("Multiply " + numerator + " / " + denominator, (i) => i * (float) numerator / (float) denominator, new DomainRange(2, 2));
        }

        public static Function Floor = new Function("Floor", MathF.Floor, new DomainRange(MathF.PI, 2));
        public static Function Ceiling = new Function("Ceiling", MathF.Ceiling, new DomainRange(MathF.PI, 2));
        public static Function Sqrt = new Function("Sqrt", MathF.Sqrt, new DomainRange(MathF.PI, 2));
        public static Function Log10 = new Function("Log10", MathF.Log10, new DomainRange(MathF.PI, 2));
        public static Function Log2 = new Function("Log2", MathF.Log2, new DomainRange(MathF.PI, 2));
        public static Function Tan = new Function("Tan", MathF.Tan, new DomainRange(MathF.PI, 2));
        public static Function Atan = new Function("Atan", MathF.Atan, new DomainRange(MathF.PI, 2));

        public static Function MaxConstant(int constant)
        {
            return new Function("Max " + constant, (i) => MathF.Max(i, constant), new DomainRange(2, 2));
        }

        public static Function MinConstant(int constant)
        {
            return new Function("Min " + constant, (i) => MathF.Min(i, constant), new DomainRange(2, 2));
        }

        public static Function ModConstant(int constant)
        {
            return new Function("Mod " + constant, (i) => MathF.IEEERemainder(i, constant), new DomainRange(3, 3));
        }

        public static Func<float, float> Fold(Function[] functions)
        {
            Func<float, float> foldedFunction = NoOp;

            foreach (var function in functions)
            {
                var innerFunction = foldedFunction;
                foldedFunction = (i) => function.func(innerFunction(i));
            }

            return foldedFunction;
        }
    }

    public class Function
    {
        public readonly DomainRange domain;
        public readonly string name;
        public readonly Func<float, float> func;

        public override string ToString()
        {
            return name;
        }

        public Function(string name, Func<float, float> func, DomainRange domain)
        {
            this.name = name;
            this.func = func;
            this.domain = domain;
        }

        public override bool Equals(object obj)
        {
            if (obj is Function func)
            {
                return func.name == name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
