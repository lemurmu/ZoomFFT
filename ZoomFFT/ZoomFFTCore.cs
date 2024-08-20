using System;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Filtering;
using MathNet.Filtering.FIR;
using System.Linq;

namespace ZoomFFT
{
    public class ZoomFFTCore
    {
        public static double[] PerformZoomFFT(double[] signal, double fs, double fStart, double fEnd, int zoomFactor) {
            int N = signal.Length;

            // 1. 应用窗函数
            double avg = signal.Average();
            var window = Window.Hann(N);
            var windowedSignal = signal.Zip(window, (s, w) => (s - avg) * w).ToArray();

            // 2. 计算中心频率
            double fc = (fStart + fEnd) / 2;

            // 3. 频率平移
            var shiftedSignal = FrequencyShift(windowedSignal, fs, fc);

            // 4. 低通滤波
            double cutoffFrequency = (fEnd - fStart) / 2;
            var filter = OnlineFirFilter.CreateLowpass(ImpulseResponse.Finite, fs, cutoffFrequency);
            var filteredSignal = filter.ProcessSamples(shiftedSignal);

            // 5. 降采样
            var downsampledSignal = Downsample(filteredSignal, zoomFactor);

            // 6. 执行最终的FFT
            var finalFFT = new Complex[downsampledSignal.Length];
            for (int i = 0; i < downsampledSignal.Length; i++) {
                finalFFT[i] = new Complex(downsampledSignal[i], 0);
            }
            Fourier.Forward(finalFFT, FourierOptions.Matlab);

            double[] power = new double[finalFFT.Length];
            for (int i = 0; i < power.Length; i++) {
                power[i] = 10 * Math.Log10((Math.Pow(finalFFT[i].Real, 2) + Math.Pow(finalFFT[i].Imaginary, 2)) / (50 * power.Length) * 1000);
            }
            return power;
        }

        private static double[] FrequencyShift(double[] signal, double fs, double fc) {
            var shiftedSignal = new Complex[signal.Length];
            for (int i = 0; i < signal.Length; i++) {
                double t = (double)i / fs;
                double realPart = signal[i] * Math.Cos(2 * Math.PI * fc * t);
                double imagPart = -signal[i] * Math.Sin(2 * Math.PI * fc * t);
                shiftedSignal[i] = new Complex(realPart, imagPart);
            }
            return shiftedSignal.Select(c => c.Real).ToArray();
        }

        private static double[] Downsample(double[] signal, int factor) {
            return signal.Where((value, index) => index % factor == 0).ToArray();
        }

    }
}
