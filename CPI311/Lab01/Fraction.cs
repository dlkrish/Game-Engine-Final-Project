using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab01
{
    struct Fraction
    {
        private int numerator;
        private int denominator;

        public int Numerator
        {
            get { return numerator; }
            set { numerator = value; Simplify(); }
        }
        public int Denominator
        {
            get { return denominator; }
            set { denominator = value; Simplify(); }
        }

        public Fraction(int n = 0, int d = 1)
        {
            numerator = n;
            if (d == 0) d = 1;
            denominator = d;
            Simplify();
        }

        public override string ToString()
        {
            if (denominator == 1) return numerator.ToString();
            return numerator + "/" + denominator;
        }

        private void Simplify()
        {
            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }
            int gcd = GCD(Math.Max(numerator, denominator), Math.Min(numerator, denominator));
            numerator /= gcd;
            denominator /= gcd;
        }
        public static int GCD(int larger, int smaller)
        {
            if (smaller == 0) return larger;
            return GCD(smaller, larger % smaller);
        }

        public static Fraction Multiply(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator, lhs.denominator * rhs.denominator);
        }
        public static Fraction Divide(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.denominator, lhs.denominator * rhs.numerator);
        }
        public static Fraction Add(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator + rhs.denominator) + (rhs.numerator * lhs.denominator), lhs.denominator * rhs.denominator);
        }
        public static Fraction Subtract(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator * rhs.denominator) - (rhs.numerator * lhs.denominator), lhs.denominator * rhs.denominator);
        }

        public static Fraction operator *(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator, lhs.denominator * rhs.denominator);
        }
        public static Fraction operator /(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.denominator, lhs.denominator * rhs.numerator);
        }
        public static Fraction operator +(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator * rhs.denominator) + (rhs.numerator * lhs.denominator), lhs.denominator * rhs.denominator);
        }
        public static Fraction operator -(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator * rhs.denominator) - (rhs.numerator * lhs.denominator), lhs.denominator * rhs.denominator);
        }
    }
}

