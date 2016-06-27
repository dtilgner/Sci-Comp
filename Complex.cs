using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _340Final
{
    class Complex
    {
        private double real;
        private double imaginary;

        public Complex(double r, double i)
        {
            real = r;
            imaginary = i;
        }

        public Complex()
        {
            real = 0;
            imaginary = 0;
        }

        public Complex add(Complex comp)
        {
            return new Complex(this.real + comp.real, this.imaginary + comp.imaginary);
        }

        public Complex subtract(Complex comp)
        {
            return new Complex(this.real - comp.real, this.imaginary - comp.imaginary);
        }

        public Complex multiply(Complex comp)
        {
            return new Complex(this.real * comp.real - this.imaginary * comp.imaginary, this.imaginary * comp.real + this.real * comp.imaginary);
        }

        public Complex divide(Complex comp)
        {
            if(comp.getReal() == 0 && comp.getImaginary() == 0)
		{
			Console.WriteLine("Dividing by zero is bad, mmmmkay?");
			return null;
		}
		
		Complex conj = comp.conjugate();
		Complex num = this.multiply(conj);
		Complex mult = comp.multiply(conj);

        return new Complex(num.getReal() / mult.getReal(), num.getImaginary() / mult.getReal());
        }

        public Complex negate()
        {
            return new Complex(-real, -imaginary);
        }

        public Complex conjugate()
        {
            return new Complex(real, -imaginary);
        }

        public double magnitude()
        {
            return Math.Sqrt(real * real + imaginary * imaginary);
        }

        public double getReal()
        {
            return real;
        }

        public double getImaginary()
        {
            return imaginary;
        }

        public override string ToString()
        {
            if (Math.Abs(imaginary) < 1E-10)
            {
                return real.ToString();
            }

            if (imaginary < 0)
            {
                return real.ToString() + " - " + (-1 * imaginary).ToString() + "i";
            }

            return real.ToString() + " + " + imaginary.ToString() + "i";
        }
    }
}
