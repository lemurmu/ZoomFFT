using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

namespace ZoomFFT
{
    public class Czt
    {

        public static double[] PerformCZT(double[] singal, double fs, double f1, double f2, int m) {
            int n = singal.Length;
            Complex[] x = new Complex[n];

            double avg = singal.Average();
            for (int i = 0; i < n; i++) {
                x[i] = new Complex(singal[i] - avg, 0);//去除直流分量
            }

            Complex w = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * (f2 - f1) / (fs * m));
            Complex a = Complex.Exp(Complex.ImaginaryOne * 2 * Math.PI * f1 / fs);

            Complex[] y = CZT(x, m, w, a);

            double[] powerSpectrumDBm = new double[m];
            //double[] frequencies = new double[m];

            // 计算频率分辨率
            //double df = (f2 - f1) / (m - 1);

            for (int i = 0; i < m; i++) {
                // 计算功率谱 (dBm)
                double magnitude = Complex.Abs(y[i]);
                double powerLinear = magnitude * magnitude / (50 * m); // 假设50欧姆阻抗
                powerSpectrumDBm[i] = 10 * Math.Log10(powerLinear * 1000); // 转换为dBm
            }

            return powerSpectrumDBm;
        }

        private static Complex[] CZT(Complex[] x, int M, Complex W, Complex A) {
            int N = x.Length;
            int i;
            int L;

            i = 1;
            do {
                i *= 2;
            } while (i < N + M - 1);
            L = i;

            Complex[] h = new Complex[L];
            Complex[] g = new Complex[L];
            Complex[] pComp = new Complex[L];
            Complex tmp, tmp1, tmp2;

            for (i = 0; i < N; i++) {
                tmp1 = Complex.Pow(A, -i);
                tmp2 = Complex.Pow(W, i * i / 2.0);
                tmp = Complex.Multiply(tmp1, tmp2);
                g[i] = Complex.Multiply(tmp, x[i]);
            }
            for (i = N; i < L; i++) {
                g[i] = Complex.Zero;
            }

            FFT.Fourier(g, WindowType.Hann);

            for (i = 0; i <= M - 1; i++) {
                h[i] = Complex.Pow(W, -i * i / 2.0);
            }
            for (i = M; i <= L - N; i++) {
                h[i] = Complex.Zero;
            }
            for (i = L - N + 1; i < L; i++) {
                h[i] = Complex.Pow(W, -(L - i) * (L - i) / 2.0);
            }

            FFT.Fourier(h, WindowType.Hann);

            for (i = 0; i < L; i++) {
                pComp[i] = Complex.Multiply(h[i], g[i]);
            }

            FFT.FourierInverse(pComp);

            Complex[] xCZT = new Complex[M];
            for (i = 0; i < M; i++) {
                tmp = Complex.Pow(W, i * i / 2.0);
                xCZT[i] = Complex.Multiply(tmp, pComp[i]);
            }

            return xCZT;
        }










    }
}
