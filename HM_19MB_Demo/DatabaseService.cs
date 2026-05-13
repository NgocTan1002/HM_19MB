using HM_19MB_Demo;
using Npgsql;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace HM_19MB_Demo
{
    public class SessionMetadata
    {
        public string DeviceName { get; set; } = "";
        public string DeviceCode { get; set; } = "";
        public string DeviceNumber { get; set; } = "";
        public string SealNumber { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string ManufactureYear { get; set; } = "";
        public string UsingUnit { get; set; } = "";
        public string Method { get; set; } = "";
        public string EnvTemperature { get; set; } = "";
        public string EnvHumidity { get; set; } = "";
        public string TechnicalSpecs { get; set; } = "";
        public string MeasuringDevices { get; set; } = "";
        public int CalibrationDay { get; set; }
        public int CalibrationMonth { get; set; }
        public int CalibrationYear { get; set; }
    }

    public static class DatabaseService
    {
        private static string ConnectionString =>
            ConfigurationManager.AppSettings["PostgresConnectionString"]
            ?? throw new InvalidOperationException("PostgresConnectionString not found in app.config.");

        public static async Task EnsureSchemaAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            string sql = @"
                CREATE TABLE IF NOT EXISTS calibration_sessions (
                    id SERIAL PRIMARY KEY,
                    device_name VARCHAR(255),
                    device_code VARCHAR(100),
                    device_number VARCHAR(100),
                    seal_number VARCHAR(100),
                    manufacturer VARCHAR(255),
                    manufacture_year VARCHAR(10),
                    using_unit VARCHAR(255),
                    method VARCHAR(255),
                    env_temperature VARCHAR(50),
                    env_humidity VARCHAR(50),
                    technical_specs TEXT,
                    measuring_devices TEXT,
                    calibration_day INT,
                    calibration_month INT,
                    calibration_year INT,
                    created_at TIMESTAMP DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS measurement_records (
                    id SERIAL PRIMARY KEY,
                    session_id INT REFERENCES calibration_sessions(id),
                    received_at TIMESTAMP,
                    device_id VARCHAR(20),
                    avg_temperature FLOAT,
                    avg_humidity FLOAT,
                    uniformity_temp FLOAT,
                    uniformity_humidity FLOAT,
                    stability_raw VARCHAR(50),
                    created_at TIMESTAMP DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS probe_measurements (
                    id SERIAL PRIMARY KEY,
                    measurement_record_id INT REFERENCES measurement_records(id) ON DELETE CASCADE,
                    probe_number INT NOT NULL,
                    temperature FLOAT NOT NULL,
                    humidity FLOAT NOT NULL,
                    measured_at TIMESTAMP NOT NULL,
                    CONSTRAINT probe_number_check CHECK (probe_number >= 1 AND probe_number <= 10)
                );

                CREATE INDEX IF NOT EXISTS idx_probe_measurements_record 
                    ON probe_measurements(measurement_record_id);
                CREATE INDEX IF NOT EXISTS idx_probe_measurements_probe 
                    ON probe_measurements(probe_number);";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Inserts a new calibration session and returns its generated ID.
        /// </summary>
        public static async Task<int> InsertSessionAsync(SessionMetadata meta)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO calibration_sessions
                    (device_name, device_code, device_number, seal_number, manufacturer, manufacture_year,
                     using_unit, method, env_temperature, env_humidity, technical_specs, measuring_devices,
                     calibration_day, calibration_month, calibration_year)
                VALUES
                    (@dn, @dc, @dnum, @seal, @mfr, @mfry, @unit, @meth, @etemp, @ehum, @specs, @mdev,
                     @cday, @cmonth, @cyear)
                RETURNING id;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dn", meta.DeviceName);
            cmd.Parameters.AddWithValue("@dc", meta.DeviceCode);
            cmd.Parameters.AddWithValue("@dnum", meta.DeviceNumber);
            cmd.Parameters.AddWithValue("@seal", meta.SealNumber);
            cmd.Parameters.AddWithValue("@mfr", meta.Manufacturer);
            cmd.Parameters.AddWithValue("@mfry", meta.ManufactureYear);
            cmd.Parameters.AddWithValue("@unit", meta.UsingUnit);
            cmd.Parameters.AddWithValue("@meth", meta.Method);
            cmd.Parameters.AddWithValue("@etemp", meta.EnvTemperature);
            cmd.Parameters.AddWithValue("@ehum", meta.EnvHumidity);
            cmd.Parameters.AddWithValue("@specs", meta.TechnicalSpecs);
            cmd.Parameters.AddWithValue("@mdev", meta.MeasuringDevices);
            cmd.Parameters.AddWithValue("@cday", meta.CalibrationDay);
            cmd.Parameters.AddWithValue("@cmonth", meta.CalibrationMonth);
            cmd.Parameters.AddWithValue("@cyear", meta.CalibrationYear);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Inserts a measurement record linked to a session and returns its ID.
        /// Then inserts individual probe measurements.
        /// </summary>
        public static async Task<int> InsertMeasurementRecordAsync(int sessionId, MeasurementBlock block)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Insert main measurement record
                string sqlRecord = @"
                    INSERT INTO measurement_records
                        (session_id, received_at, device_id, avg_temperature, avg_humidity, 
                         uniformity_temp, uniformity_humidity, stability_raw)
                    VALUES
                        (@sid, @rat, @did, @avgT, @avgH, @uniT, @uniH, @stab)
                    RETURNING id;";

                int recordId;
                await using (var cmd = new NpgsqlCommand(sqlRecord, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.Parameters.AddWithValue("@rat", block.Timestamp);
                    cmd.Parameters.AddWithValue("@did", block.DeviceId);
                    cmd.Parameters.AddWithValue("@avgT", block.AvgTemperature);
                    cmd.Parameters.AddWithValue("@avgH", block.AvgHumidity);
                    cmd.Parameters.AddWithValue("@uniT", block.UniformityTemp);
                    cmd.Parameters.AddWithValue("@uniH", block.UniformityHumidity);
                    cmd.Parameters.AddWithValue("@stab", block.StabilityRaw);

                    var result = await cmd.ExecuteScalarAsync();
                    recordId = Convert.ToInt32(result);
                }

                // Insert individual probe measurements
                string sqlProbe = @"
                    INSERT INTO probe_measurements
                        (measurement_record_id, probe_number, temperature, humidity, measured_at)
                    VALUES
                        (@rid, @pnum, @temp, @hum, @mat);";

                for (int i = 0; i < block.ProbeCount && i < 10; i++)
                {
                    if (float.IsNaN(block.ProbeTemperatures[i]) || float.IsNaN(block.ProbeHumidities[i]))
                        continue;

                    await using var cmdProbe = new NpgsqlCommand(sqlProbe, conn, transaction);
                    cmdProbe.Parameters.AddWithValue("@rid", recordId);
                    cmdProbe.Parameters.AddWithValue("@pnum", i + 1); // Probe 1-10
                    cmdProbe.Parameters.AddWithValue("@temp", block.ProbeTemperatures[i]);
                    cmdProbe.Parameters.AddWithValue("@hum", block.ProbeHumidities[i]);
                    cmdProbe.Parameters.AddWithValue("@mat", block.Timestamp);

                    await cmdProbe.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return recordId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
