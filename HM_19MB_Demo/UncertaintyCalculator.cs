using System;
using System.Linq;

namespace HM_19MB_Demo
{
    public static class UncertaintyCalculator
    {
        // Tính nhiệt độ trung bình của một kênh (t̄j)
        public static double CalculateMean(double[] values)
        {
            if (values == null || values.Length == 0)
                return 0;
            return values.Average();
        }
        // Tính độ lệch chuẩn Sj
        // 
        public static double CalculateStandardDeviation(double[] values, double mean)
        {
            if (values == null || values.Length <= 1)
                return 0;

            int n = values.Length;
            double sumSquares = values.Sum(t => Math.Pow(t - mean, 2));
            return Math.Sqrt(sumSquares / (n - 1));
        }

        // Tính độ không đảm bảo chuẩn loại A của kênh j
        public static double CalculateTypeAUncertainty(double standardDeviation, int n)
        {
            if (n <= 0)
                return 0;
            return standardDeviation / Math.Sqrt(n);
        }

        // Tính độ không đảm bảo chuẩn loại A tổng hợp
        public static double CalculateCombinedTypeA(double[] typeAUncertainties)
        {
            if (typeAUncertainties == null || typeAUncertainties.Length == 0)
                return 0;

            int j = typeAUncertainties.Length;
            double sumSquares = typeAUncertainties.Sum(u => u * u);
            return Math.Sqrt(sumSquares);
        }

        // Tính độ không đảm bảo loại B từ U
        public static double CalculateTypeBFromU(double uMax)
        {
            return uMax / 2.0;
        }

        // Tính độ không đảm bảo loại B từ ∂
        public static double CalculateTypeBFromDelta(double deltaMax)
        {
            return deltaMax / Math.Sqrt(3);
        }

        // Tính độ không đảm bảo chuẩn liên hợp theo công thức (12)
        public static double CalculateCombinedUncertainty(double uch1, double uch2)
        {
            return Math.Sqrt(uch1 * uch1 + uch2 * uch2);
        }
        /// Tính độ không đảm bảo mở rộng
        public static double CalculateExpandedUncertainty(double uc, double k = 2.0)
        {
            return k * uc;
        }

        // Tìm giá trị lớn nhất trong mảng
        public static double FindMax(double[] values)
        {
            if (values == null || values.Length == 0)
                return 0;
            return values.Max();
        }

        /// <summary>
        /// Tính t̄_j: trung bình của kênh j có tính số hiệu chính ∂t_j (Công thức 2)
        /// </summary>
        public static double CalculateCorrectedMean(double[] measurements, double correction)
        {
            if (measurements == null || measurements.Length == 0)
                return 0;

            int n = measurements.Length;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += measurements[i] + correction;
            }
            return sum / n;
        }

        /// <summary>
        /// Tính t̄_ch: trung bình tổng hợp từ k kênh đã hiệu chính (Công thức 1)
        /// Trả về tuple (double Tch, double[] ChannelCorrectedMeans)
        /// </summary>
        public static (double Tch, double[] ChannelCorrectedMeans) CalculateCorrectedTemperature(
            double[,] measurementData,
            double[] corrections)
        {
            int n = measurementData.GetLength(0);
            int k = measurementData.GetLength(1);

            if (corrections.Length != k)
                throw new ArgumentException($"corrections.Length ({corrections.Length}) != số kênh ({k})");

            double[] channelCorrectedMeans = new double[k];
            for (int j = 0; j < k; j++)
            {
                double[] channelData = new double[n];
                for (int i = 0; i < n; i++)
                {
                    channelData[i] = measurementData[i, j];
                }
                channelCorrectedMeans[j] = CalculateCorrectedMean(channelData, corrections[j]);
            }

            double tch = channelCorrectedMeans.Average();
            return (tch, channelCorrectedMeans);
        }

        /// <summary>
        /// Công thức 5: δt_od = ±½(t_max,j − t_min,j), lấy max qua k kênh
        /// </summary>
        public static double CalculateStability(double[,] measurementData)
        {
            int n = measurementData.GetLength(0);
            int k = measurementData.GetLength(1);

            double maxDelta = 0;
            for (int j = 0; j < k; j++)
            {
                double min = double.MaxValue, max = double.MinValue;
                for (int i = 0; i < n; i++)
                {
                    double v = measurementData[i, j];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                double delta = 0.5 * (max - min);
                if (delta > maxDelta) maxDelta = delta;
            }
            return maxDelta;
        }

        /// <summary>
        /// Công thức 6: δt_dd = ±½(max(t̄j) − min(t̄j))
        /// </summary>
        public static double CalculateUniformity(double[] channelCorrectedMeans)
        {
            if (channelCorrectedMeans == null || channelCorrectedMeans.Length == 0)
                return 0;
            return 0.5 * (channelCorrectedMeans.Max() - channelCorrectedMeans.Min());
        }

        /// <summary>
        /// Công thức 3: t̄_tn = trung bình chỉ thị tủ từ 2 bộ chỉ thị
        /// </summary>
        public static double CalculateMeanIndicatorTemperature(double[] ttn1, double[] ttn2)
        {
            if (ttn1 == null || ttn2 == null || ttn1.Length == 0)
                return 0;
            int n = ttn1.Length;
            double sum = 0;
            for (int i = 0; i < n; i++)
                sum += (ttn1[i] + ttn2[i]) / 2.0;
            return sum / n;
        }

        /// <summary>
        /// Công thức 4: Δt = t̄_ch − t̄_tn
        /// </summary>
        public static double CalculateCorrection(double tch, double ttn)
        {
            return tch - ttn;
        }

        /// <summary>
        /// Công thức 13-14: ubk1 = S/√n
        /// tᵢ = trung bình k kênh tại lần đo i (từ measurementData)
        /// t̄ = trung bình của tᵢ qua n lần đo
        /// </summary>
        public static double CalculateIndicatorTypeA(double[,] measurementData)
        {
            int n = measurementData.GetLength(0);
            int k = measurementData.GetLength(1);

            if (n <= 1 || k == 0) return 0;

            double[] ti = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < k; j++)
                    sum += measurementData[i, j];
                ti[i] = sum / k;
            }

            double tBar = ti.Average();
            double sumSq = ti.Sum(t => Math.Pow(t - tBar, 2));
            double S = Math.Sqrt(sumSq / (n - 1));
            return S / Math.Sqrt(n);
        }

        /// <summary>
        /// Công thức 15: ubk2 = δt_od / √3
        /// </summary>
        public static double CalculateUbk2(double deltaOd)
        {
            return deltaOd / Math.Sqrt(3);
        }

        /// <summary>
        /// Công thức 16: ubk3 = δt_dd / √3
        /// </summary>
        public static double CalculateUbk3(double deltaDd)
        {
            return deltaDd / Math.Sqrt(3);
        }

        /// <summary>
        /// Công thức 17: ubk4 = A×d / √3
        /// </summary>
        public static double CalculateUbk4(double A, double d)
        {
            return A * d / Math.Sqrt(3);
        }

        /// <summary>
        /// Công thức 18: ubk = √(ubk1² + ubk2² + ubk3² + ubk4²)
        /// </summary>
        public static double CalculateCombinedUbk(double ubk1, double ubk2, double ubk3, double ubk4)
        {
            return Math.Sqrt(ubk1 * ubk1 + ubk2 * ubk2 + ubk3 * ubk3 + ubk4 * ubk4);
        }

        /// <summary>
        /// Công thức 19: U = 2 × √(uch² + ubk²)
        /// </summary>
        public static double CalculateFinalExpandedUncertainty(double uch, double ubk)
        {
            return 2.0 * Math.Sqrt(uch * uch + ubk * ubk);
        }

        // Kết quả tính toán đầy đủ cho một phép đo
        public class UncertaintyResult
        {
            // Dữ liệu đầu vào
            public int NumberOfChannels { get; set; }  // j
            public int NumberOfMeasurements { get; set; }  // n
            public double[,] MeasurementData { get; set; }  // ti,j (n × j)
            public double[] UValues { get; set; }  // U1...Uj
            public double[] DeltaValues { get; set; }  // ∂1...∂j

            // Kết quả trung gian cho từng kênh
            public double[] ChannelMeans { get; set; }  // t̄j
            public double[] ChannelStdDevs { get; set; }  // Sj
            public double[] ChannelTypeAUncertainties { get; set; }  // uch1,j

            // Kết quả tổng hợp
            public double Uch1 { get; set; }  // Độ không đảm bảo loại A
            public double UMax { get; set; }  // Max(U1...Uj)
            public double DeltaMax { get; set; }  // Max(∂1...∂j)
            public double Uch2FromU { get; set; }  // Từ U
            public double Uch2FromDelta { get; set; }  // Từ ∂
            public double Uch2 { get; set; }  // Độ không đảm bảo loại B (chọn từ U hoặc ∂)
            public double Uc { get; set; }  // Độ không đảm bảo chuẩn liên hợp
            public double U { get; set; }  // Độ không đảm bảo mở rộng (k=2)

            // Metadata
            public string CalculationMethod { get; set; }  // "U" hoặc "Delta"
            public DateTime CalculatedAt { get; set; }
        }

        // Tính toán đầy đủ độ không đảm bảo đo
        /// <param name="measurementData">Mảng dữ liệu đo n×j (hàng=lần đo, cột=kênh)</param>
        /// <param name="uValues">Mảng U1...Uj</param>
        /// <param name="deltaValues">Mảng ∂1...∂j</param>
        /// <param name="useUMethod">true=dùng U, false=dùng ∂</param>
        public static UncertaintyResult CalculateFull(
            double[,] measurementData,
            double[] uValues,
            double[] deltaValues,
            bool useUMethod)
        {
            int n = measurementData.GetLength(0);  // Số lần đo
            int j = measurementData.GetLength(1);  // Số kênh

            var result = new UncertaintyResult
            {
                NumberOfChannels = j,
                NumberOfMeasurements = n,
                MeasurementData = measurementData,
                UValues = uValues,
                DeltaValues = deltaValues,
                CalculatedAt = DateTime.Now
            };

            // Bước 1: Tính toán cho từng kênh
            result.ChannelMeans = new double[j];
            result.ChannelStdDevs = new double[j];
            result.ChannelTypeAUncertainties = new double[j];

            for (int channelIdx = 0; channelIdx < j; channelIdx++)
            {
                // Lấy dữ liệu của kênh này
                double[] channelData = new double[n];
                for (int i = 0; i < n; i++)
                {
                    channelData[i] = measurementData[i, channelIdx];
                }

                // Tính t̄j, Sj, uch1,j
                result.ChannelMeans[channelIdx] = CalculateMean(channelData);
                result.ChannelStdDevs[channelIdx] = CalculateStandardDeviation(
                    channelData, result.ChannelMeans[channelIdx]);
                result.ChannelTypeAUncertainties[channelIdx] = CalculateTypeAUncertainty(
                    result.ChannelStdDevs[channelIdx], n);
            }

            // Bước 2: Tính uch1 (loại A tổng hợp)
            result.Uch1 = CalculateCombinedTypeA(result.ChannelTypeAUncertainties);

            // Bước 3: Tính uch2 (loại B)
            result.UMax = FindMax(uValues);
            result.DeltaMax = FindMax(deltaValues);
            result.Uch2FromU = CalculateTypeBFromU(result.UMax);
            result.Uch2FromDelta = CalculateTypeBFromDelta(result.DeltaMax);

            if (useUMethod)
            {
                result.Uch2 = result.Uch2FromU;
                result.CalculationMethod = "U";
            }
            else
            {
                result.Uch2 = result.Uch2FromDelta;
                result.CalculationMethod = "Delta";
            }

            // Bước 4: Tính uc và U
            result.Uc = CalculateCombinedUncertainty(result.Uch1, result.Uch2);
            result.U = CalculateExpandedUncertainty(result.Uc);

            return result;
        }
    }
}
