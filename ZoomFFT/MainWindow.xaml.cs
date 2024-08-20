﻿using SciChart.Charting.Model.DataSeries;
using System;
using System.Numerics;
using System.Linq;
using SciChart.Data.Model;
using MathNet.Numerics.Interpolation;
using System.Windows.Media;

namespace ZoomFFT
{
    public partial class MainWindow
    {
        public MainWindow() {
            InitializeComponent();

            this.waveSeries.DataSeries = waveDataSeries;
            this.spectrumSeries.DataSeries = spectrumDataSeries;//fft
            this.spectrumSeries1.DataSeries = spectrumDataSeries1;//插值
            this.spectrumSeries2.DataSeries = spectrumDataSeries2;//zoomFFT
            this.spectrumSeries3.DataSeries = spectrumDataSeries3;//czt
            this.spectrumSeries4.DataSeries = spectrumDataSeries4;//三次样条插值

            ProductSin();
        }

        private double fs = 50e3;
        private double fc = 1e3;
        private double fc2 = 10e3;
        private const int N = 2000;
        private double[] waveData = new double[N];
        XyDataSeries<double, double> spectrumDataSeries = new XyDataSeries<double, double>() { SeriesName = "FFT" };
        XyDataSeries<double, double> spectrumDataSeries1 = new XyDataSeries<double, double>() { SeriesName = "插值" };
        XyDataSeries<double, double> spectrumDataSeries2 = new XyDataSeries<double, double>() { SeriesName = "ZoomFFT" };
        XyDataSeries<double, double> spectrumDataSeries3 = new XyDataSeries<double, double>() { SeriesName = "CZT" };
        XyDataSeries<double, double> spectrumDataSeries4 = new XyDataSeries<double, double>() { SeriesName = "三次样条插值" };

        XyDataSeries<double, double> waveDataSeries = new XyDataSeries<double, double>();

        private void sinBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            ProductSin();
        }

        void ProductSin() {
            fc = 1e3;
            double t = 1 / fs;

            waveDataSeries.Clear();
            for (int i = 0; i < N; i++) {
                waveData[i] = 100 * Math.Sin(2 * Math.PI * fc * i * t) + 200;
                waveDataSeries.Append(i, waveData[i]);
            }
        }

        private void fftBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            double[] real = new double[N];
            double[] imag = new double[N];
            double[] power = new double[N / 2];

            Array.Copy(waveData, 0, real, 0, real.Length);
            double avg = real.Average();
            for (int i = 0; i < N; i++) {
                real[i] -= avg;
            }

            FFT.Fourier(real, imag, WindowType.Hann);

            for (int i = 0; i < real.Length / 2; i++) {
                power[i] = 10 * Math.Log10((Math.Pow(real[i], 2) + Math.Pow(imag[i], 2)) / (50 * power.Length) * 1000);
            }
            this.spectrumAxis.VisibleRange = new DoubleRange(0, fs / 2);

            double pfreq = fs / (N - 1);
            spectrumDataSeries.Clear();
            freqtxt.Text = string.Empty;
            freqtxt.Foreground = new SolidColorBrush(Colors.White);
            for (int i = 0; i < power.Length; i++) {
                spectrumDataSeries.Append(i * pfreq, power[i]);
            }
        }

        /// <summary>
        /// zoom FFT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomFftBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            int zoomFactor = 4; // 放大因子
            double fStart = 0;
            double fEnd = fs / 2;
            double[] power = ZoomFFTCore.PerformZoomFFT(waveData, fs, fStart, fEnd, zoomFactor);

            double fsNew = fs / zoomFactor;//倍数抽取后的采样率
            double nNew = N / zoomFactor;//倍数抽取后的数据长度
            double df = fsNew / nNew;  // 频率分辨率

            spectrumDataSeries2.Clear();
            this.spectrumAxis.VisibleRange = new DoubleRange(0, fsNew / 2);
            for (int i = 0; i < power.Length / 2; i++) {
                spectrumDataSeries2.Append(fStart + i * df, power[i]);
            }
            int peakIndex = power.Take(power.Length / 2).ToList().FindIndex(t => Math.Abs(t - power.Max()) < 0.001);
            double freq = fStart + df * peakIndex;
            freqtxt.Text = freq.ToString("F4");
            freqtxt.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }

        /// <summary>
        /// CZT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cztBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            // CZT参数
            double f1 = 0; // 起始频率
            double f2 = fs / 2; // 截止频率
            int m = 6000; // 细化倍数

            this.spectrumAxis.VisibleRange = new DoubleRange(f1, f2);
            spectrumDataSeries3.Clear();

            // 执行CZT
            var powerSpectrum = Czt.PerformCZT(waveData, fs, f1, f2, m);
            double df = (f2 - f1) / m;

            for (int i = 0; i < powerSpectrum.Length; i++) {
                spectrumDataSeries3.Append(f1 + i * df, powerSpectrum[i]);
            }

            // 查找峰值
            int peakIndex = powerSpectrum.ToList().FindIndex(t => Math.Abs(t - powerSpectrum.Max()) < 0.001);
            double freq = f1 + peakIndex * df;
            freqtxt.Text = freq.ToString("F4");
            freqtxt.Foreground = new SolidColorBrush(Colors.DeepSkyBlue);

        }

        /// <summary>
        /// 抛物线插值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chazhiBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            double[] power = ComputePowerSpectrum();
            CalFrequency(power);
            freqtxt.Foreground = new SolidColorBrush(Colors.MediumPurple);
        }

        private void threeChazhiBtn_Click(object sender, System.Windows.RoutedEventArgs e) {
            double[] power = ComputePowerSpectrum();
            double[] freqArr = new double[power.Length];
            double[] indexs = new double[power.Length];

            this.spectrumAxis.VisibleRange = new DoubleRange(0, fs / 2);

            int peakIndex = power.ToList().FindIndex(t => Math.Abs(t - power.Max()) < 0.001);

            double pfreq = fs / (N - 1);
            spectrumDataSeries4.Clear();
            for (int i = 0; i < power.Length; i++) {
                indexs[i] = i;
                freqArr[i] = i * pfreq;
                spectrumDataSeries4.Append(i * pfreq, power[i]);
            }
            var q = CubicSpline.InterpolateAkimaSorted(indexs, power);
            double val = q.Interpolate(peakIndex);
           

           // freqtxt.Text = freq.ToString("F4");
        }

        private static double InterpolateFrequency(double[] spectrum, int peakIndex, double sampleRate) {
            int N = spectrum.Length * 2;
            double alpha = spectrum[peakIndex - 1];
            double beta = spectrum[peakIndex];
            double gamma = spectrum[peakIndex + 1];

            double p = 0.5 * (alpha - gamma) / (alpha - 2 * beta + gamma);
            double interpolatedBin = peakIndex + p;

            return interpolatedBin * sampleRate / N;
        }


        // 计算功率谱（单位：dB）
        public double[] ComputePowerSpectrum() {
            double[] real = new double[N];
            double[] imag = new double[N];
            double[] power = new double[N / 2];

            Array.Copy(waveData, 0, real, 0, real.Length);
            double avg = real.Average();
            for (int i = 0; i < N; i++) {
                real[i] -= avg;
            }

            FFT.Fourier(real, imag, WindowType.Hann);

            for (int i = 0; i < real.Length / 2; i++) {
                power[i] = 10 * Math.Log10((Math.Pow(real[i], 2) + Math.Pow(imag[i], 2)) / Math.Pow(real.Length / 2, 2) / 50 * 1000);
            }

            return power;
        }

        private void CalFrequency(double[] power) {
            int peakIndex = power.ToList().FindIndex(t => Math.Abs(t - power.Max()) < 0.001);
            double freq = InterpolateFrequency(power, peakIndex, fs);

            freqtxt.Text = freq.ToString("F4");
            spectrumDataSeries1.Clear();
            this.spectrumAxis.VisibleRange = new DoubleRange(0, fs / 2);
            double pfreq = fs / (N - 1);
            for (int i = 0; i < power.Length; i++) {
                spectrumDataSeries1.Append(i * pfreq, power[i]);
            }
        }


    }
}