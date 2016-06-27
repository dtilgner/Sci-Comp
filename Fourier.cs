using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _340Final
{
    class Fourier
    {
        private Complex[] cArray;
        private Complex[][] c2DArray;

        public Fourier(String file)
        {
            read(file);
        }

        public Fourier(Complex[] data)
        {
            cArray = data;
        }

        public Fourier(Complex[][] data)
        {
            c2DArray = data;
        }

        public void read(String file)
        {           
            int numLines = 0;
            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                string newLine;
                while ((newLine = sr.ReadLine()) != null)
                {
                    numLines++;
                }
            }            

            cArray = new Complex[numLines];
            string line;
            string[] lineParsed;
            int i = 0;

            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lineParsed = line.Split(' ');
                    if (lineParsed.Length > 1)
                    {
                        if (lineParsed[1].Equals("-"))
                        {
                            lineParsed[2] = "-" + lineParsed[2];
                        }
                        cArray[i++] = new Complex(double.Parse(lineParsed[0]), double.Parse(lineParsed[2].Substring(0, lineParsed[2].Length - 1)));
                    }
                    else
                    {
                        cArray[i++] = new Complex(double.Parse(lineParsed[0]), 0);
                    }
                }
            }
        }

        public Fourier signalF(int s, int num)
        {
            double[] x = new double[num];
            for (int i = 1; i < num; i++)
            {
                x[i] = x[i - 1] + 1 / (double)num;
            }
            Complex[] fSignal = new Complex[num];
            if (s != Int32.MaxValue)
            {
                for (int i = 0; i < num; i++)
                {
                    double sum = 0;
                    for (int k = 1; k <= s; k++)
                    {
                        sum += Math.Sin(2 * Math.PI * (2 * k - 1) * x[i]) / (2 * k - 1);
                    }
                    fSignal[i] = new Complex(sum, 0);
                }
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    if (x[i] > 0 && x[i] < 0.5)
                    {
                        fSignal[i] = new Complex(Math.PI / 4, 0);
                    }
                    else if (x[i] > 0.5)
                    {
                        fSignal[i] = new Complex(-Math.PI / 4, 0);
                    }
                    else
                    {
                        fSignal[i] = new Complex();
                    }
                }
            }
            return new Fourier(fSignal);
        }

        public Fourier signalG(int s, int num)
        {
            double[] x = new double[num];
            for (int i = 1; i < num; i++)
            {
                x[i] = x[i - 1] + 1 / (double)num;
            }
            Complex[] gSignal = new Complex[num];
            if (s != Int32.MaxValue)
            {
                for (int i = 0; i < num; i++)
                {
                    double sum = 0;
                    for (int k = 1; k <= s; k++)
                    {
                        sum += Math.Sin(2 * Math.PI * (2 * k) * x[i]) / (2 * k);
                    }
                    gSignal[i] = new Complex(sum, 0);
                }
            }
            else
            {
                gSignal[0] = new Complex();
                gSignal[1] = new Complex(Math.PI / 4, 0);
                for (int i = 2; i < num; i++)
                {
                    if (x[i] > 0 && x[i] < 0.5)
                    {
                        gSignal[i] = new Complex(gSignal[i - 1].getReal() - Math.PI / (double)num, 0);
                    }
                    else if (x[i] > 0.5)
                    {
                        gSignal[i] = gSignal[i - num / 2];
                    }
                    else
                    {
                        gSignal[i] = new Complex();
                    }
                }
            }
            return new Fourier(gSignal);
        }

        public Fourier psd()
        {
            Complex[] newZ = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                newZ[i] = new Complex(Math.Pow(cArray[i].magnitude(), 2), 0);
            }
            return new Fourier(newZ);
        }

        public Fourier fft(int d)
        {
            int N = cArray.Length;
            double theta = -2 * Math.PI * d / N;
            int r = N / 2;
            Complex t;
            Complex u;
            Complex v;

            for (int i = 1; i <= N - 1; i *= 2)
            {
                t = new Complex(Math.Cos(i * theta), Math.Sin(i * theta));
                for (int k = 0; k <= N - 1; k += 2 * r)
                {
                    u = new Complex(1, 0);

                    for (int m = 0; m <= r - 1; m++)
                    {
                        v = cArray[k + m].subtract(cArray[k + m + r]);
                        cArray[k + m] = cArray[k + m].add(cArray[k + m + r]);
                        cArray[k + m + r] = v.multiply(u);
                        u = t.multiply(u);
                    }
                }
                r /= 2;
            }
            for (int i = 0; i <= N - 1; i++)
            {
                r = i;
                int k = 0;
                for (int m = 1; m <= N - 1; m++)
                {
                    k = 2 * k + r % 2;
                    r /= 2;
                    m *= 2;
                }
                if (k > i)
                {
                    v = cArray[i];
                    cArray[i] = cArray[k];
                    cArray[k] = v;
                }
            }
            if (d < 0)
            {
                for (int i = 0; i <= N - 1; i++)
                {
                    cArray[i] = cArray[i].divide(new Complex(N, 0));
                }
            }
            return new Fourier(cArray);
        }        
        
        public Fourier add(Fourier four)
        {
            if (cArray.Length != four.getZ().Length)
            {
                Console.WriteLine("Not equal length signals.");
                return null;
            }

            Complex[] newZ = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                newZ[i] = cArray[i].add(four.getZ()[i]);
            }

            return new Fourier(newZ);
        }

        public Fourier multiply(Fourier four)
        {
            if (cArray.Length != four.getZ().Length)
            {
                Console.WriteLine("Not equal length signals.");
                return null;
            }

            Complex[] newZ = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                newZ[i] = cArray[i].multiply(four.getZ()[i]);
            }

            return new Fourier(newZ);
        }

        public Fourier lowPassFilter(int f)
        {
            Complex[] filter = new Complex[cArray.Length];
            Complex[] newSignal = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                filter[i] = new Complex();
            }
            for (int i = 0; i < f; i++)
            {
                filter[i] = new Complex(1, 0);
            }
            for (int i = cArray.Length - 1; i > cArray.Length - f; i--)
            {
                filter[i] = new Complex(1, 0);
            }

            for (int i = 0; i < cArray.Length; i++)
            {
                newSignal[i] = cArray[i].multiply(filter[i]);
            }

            return new Fourier(newSignal);
        }

        public Fourier highPassFilter(int f)
        {
            Complex[] filter = new Complex[cArray.Length];
            Complex[] newSignal = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                filter[i] = new Complex(1, 0);
            }
            for (int i = 0; i < f; i++)
            {
                filter[i] = new Complex();
            }
            for (int i = cArray.Length - 1; i > cArray.Length - f; i--)
            {
                filter[i] = new Complex();
            }

            for (int i = 0; i < cArray.Length; i++)
            {
                newSignal[i] = cArray[i].multiply(filter[i]);
            }

            return new Fourier(newSignal);
        }

        public Fourier bandPassFilter(int f1, int f2)
        {
            Complex[] filter = new Complex[cArray.Length];
            Complex[] newSignal = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                if (i >= f1 && i <= f2)
                {
                    filter[i] = new Complex(1, 0);
                }
                else
                {
                    filter[i] = new Complex();
                }
            }

            for (int i = 0; i < cArray.Length; i++)
            {
                newSignal[i] = cArray[i].multiply(filter[i]);
            }

            return new Fourier(newSignal);
        }

        public Fourier notchFilter(int f1, int f2)
        {
            Complex[] filter = new Complex[cArray.Length];
            Complex[] newSignal = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                if (i >= f1 && i <= f2)
                {
                    filter[i] = new Complex();
                }
                else
                {
                    filter[i] = new Complex(1, 0);
                }
            }

            for (int i = 0; i < cArray.Length; i++)
            {
                newSignal[i] = cArray[i].multiply(filter[i]);
            }

            return new Fourier(newSignal);
        }

        public Fourier conjugate()
        {
            Complex[] newZ = new Complex[cArray.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                newZ[i] = cArray[i].conjugate();
            }

            return new Fourier(newZ);
        }        

        public Fourier correlation(Fourier four)
        {
            Fourier X = this.fft(1);
            Fourier Y_star = four.fft(1).conjugate();

            return X.multiply(Y_star).fft(-1);
        }

        public Fourier correlationNormalized(Fourier four)
        {
            Fourier X = this.fft(1);
            Fourier X_star = X.conjugate();
            Fourier Y = four.fft(1);
            Fourier Y_star = Y.conjugate();

            Fourier correlation = X.multiply(Y_star).fft(-1);
            Fourier autoCorrelationX = X.multiply(X_star).fft(-1);
            Fourier autoCorrelationY = Y.multiply(Y_star).fft(-1);

            Complex[] norm = new Complex[cArray.Length];
            for (int i = 0; i < cArray.Length; i++)
            {
                norm[i] = new Complex(correlation.getZ()[i].getReal() / Math.Sqrt(autoCorrelationX.getZ()[0].getReal() * autoCorrelationY.getZ()[0].getReal()), 0);
            }

            return new Fourier(norm);
        }

        public Fourier convolution(Fourier response)
        {
            Fourier U = this.fft(1);
            Fourier H = response.fft(1);
            Fourier convolution = H.multiply(U).fft(-1);

            return convolution;
        }

        public Fourier[] fft2d()
        {
            int N = c2DArray.Length;
            int M = c2DArray[0].Length;

            Fourier[] Y = new Fourier[N];
            Fourier[] Z = new Fourier[N];
            for (int k = 0; k <= M - 1; k++)
            {
                Complex[] col = new Complex[N];
                for (int i = 0; i < N; i++)
                {
                    col[i] = c2DArray[i][k];
                }
                Y[k] = new Fourier(col).fft(1);
            }
            for (int n = 0; n <= N - 1; n++)
            {
                Z[n] = Y[n].fft(1);
            }
            return Z;
        }

        public Complex[] getZ()
        {
            return cArray;
        }

        public Complex[][] getX()
        {
            return c2DArray;
        }
        
	    public override string ToString()
	    {
		    string str = "";
		
		    if(cArray.Length != 0)
		    {
			    for(int i = 0; i<cArray.Length; i++)
			    {
                    str += cArray[i] + Environment.NewLine;
			    }
		    }
		    else if(c2DArray.Length != 0)
		    {
			    for(int i = 0; i<c2DArray.Length; i++)
			    {
				    for(int j = 0; j<c2DArray[0].Length; j++)
				    {
					    str += c2DArray[i][j] + " ";
				    }
                    str += Environment.NewLine;
	            }
		    }
		
		    return str;
	    }
    }
}
