using System;
using System.Threading;
using System.Threading.Tasks;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Tạo dữ liệu giả lập để test chương trình mà không cần thiết bị thật
    /// </summary>
    public class FakeDataGenerator : IDisposable
    {
        private readonly Random _random = new Random();
        private CancellationTokenSource? _cts;
        private Task? _generatorTask;
        private bool _isRunning;

        // Cấu hình nhiệt độ và độ ẩm cơ bản
        private float _baseTemperature = 25.0f;
        private float _baseHumidity = 60.0f;
        private float _temperatureVariation = 2.0f;
        private float _humidityVariation = 5.0f;

        public event EventHandler<MeasurementBlock>? BlockReceived;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsRunning => _isRunning;

        /// <summary>
        /// Thiết lập nhiệt độ và độ ẩm cơ bản
        /// </summary>
        public void SetBaseValues(float temperature, float humidity, float tempVariation = 2.0f, float humVariation = 5.0f)
        {
            _baseTemperature = temperature;
            _baseHumidity = humidity;
            _temperatureVariation = tempVariation;
            _humidityVariation = humVariation;
        }

        /// <summary>
        /// Bắt đầu sinh dữ liệu giả với chu kỳ nhất định
        /// </summary>
        public void Start(int intervalMilliseconds = 2000)
        {
            if (_isRunning)
                throw new InvalidOperationException("Fake data generator is already running.");

            _isRunning = true;
            _cts = new CancellationTokenSource();

            _generatorTask = Task.Run(async () =>
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var block = GenerateBlock();
                        BlockReceived?.Invoke(this, block);

                        await Task.Delay(intervalMilliseconds, _cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Normal cancellation
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Fake data error: {ex.Message}");
                }
                finally
                {
                    _isRunning = false;
                }
            }, _cts.Token);
        }

        /// <summary>
        /// Dừng sinh dữ liệu giả
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;

            _cts?.Cancel();
            _generatorTask?.Wait(1000);
            _isRunning = false;
        }

        /// <summary>
        /// Sinh một block dữ liệu giả ngẫu nhiên
        /// </summary>
        public MeasurementBlock GenerateBlock()
        {
            var block = new MeasurementBlock
            {
                DeviceId = "u99",
                Timestamp = DateTime.Now,
                ProbeCount = 10
            };

            float sumTemp = 0;
            float sumHum = 0;
            float minTemp = float.MaxValue;
            float maxTemp = float.MinValue;
            float minHum = float.MaxValue;
            float maxHum = float.MinValue;

            // Sinh dữ liệu cho 10 đầu đo
            for (int i = 0; i < 10; i++)
            {
                // Nhiệt độ dao động quanh giá trị cơ bản
                float temp = _baseTemperature + (float)(_random.NextDouble() * 2 - 1) * _temperatureVariation;
                temp = (float)Math.Round(temp, 1);
                block.ProbeTemperatures[i] = temp;

                // Độ ẩm dao động quanh giá trị cơ bản
                float hum = _baseHumidity + (float)(_random.NextDouble() * 2 - 1) * _humidityVariation;
                hum = Math.Max(0, Math.Min(100, hum)); // Giới hạn 0-100%
                hum = (float)Math.Round(hum, 1);
                block.ProbeHumidities[i] = hum;

                sumTemp += temp;
                sumHum += hum;

                if (temp < minTemp) minTemp = temp;
                if (temp > maxTemp) maxTemp = temp;
                if (hum < minHum) minHum = hum;
                if (hum > maxHum) maxHum = hum;
            }

            // Tính trung bình
            block.AvgTemperature = (float)Math.Round(sumTemp / 10, 1);
            block.AvgHumidity = (float)Math.Round(sumHum / 10, 1);

            // Tính độ đồng đều (max - min)
            block.UniformityTemp = (float)Math.Round(maxTemp - minTemp, 1);
            block.UniformityHumidity = (float)Math.Round(maxHum - minHum, 1);

            // Độ ổn định (giả lập)
            float stabilityTemp = (float)Math.Round(_random.NextDouble() * 0.5, 1);
            float stabilityHum = (float)Math.Round(_random.NextDouble() * 2.0, 1);
            block.StabilityTemperature = $"{stabilityTemp:F1}";
            block.StabilityHumidity = $"{stabilityHum:F1}";
            block.StabilityRaw = $"{block.StabilityTemperature} / {block.StabilityHumidity}";

            return block;
        }

        /// <summary>
        /// Sinh một block dữ liệu với giá trị cụ thể (để test các trường hợp đặc biệt)
        /// </summary>
        public MeasurementBlock GenerateBlockWithValues(float temperature, float humidity)
        {
            var block = new MeasurementBlock
            {
                DeviceId = "u99",
                Timestamp = DateTime.Now,
                ProbeCount = 10
            };

            for (int i = 0; i < 10; i++)
            {
                block.ProbeTemperatures[i] = temperature;
                block.ProbeHumidities[i] = humidity;
            }

            block.AvgTemperature = temperature;
            block.AvgHumidity = humidity;
            block.UniformityTemp = 0.0f;
            block.UniformityHumidity = 0.0f;
            block.StabilityTemperature = "0.0";
            block.StabilityHumidity = "0.0";
            block.StabilityRaw = $"{block.StabilityTemperature} / {block.StabilityHumidity}";

            return block;
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}
