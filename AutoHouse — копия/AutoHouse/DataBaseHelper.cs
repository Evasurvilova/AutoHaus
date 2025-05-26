using System;
using System.Windows.Forms;
using Npgsql;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutohausManagementSystem
{
    public class DatabaseHelper
    {
        public string ConnectionString { get; private set; }

        public DatabaseHelper(string host, string database, string username, string password)
        {
            ConnectionString = $"Host={host};Database={database};Username={username};Password={password}";
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        // Создание таблицы clients
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS clients (
                                    client_id SERIAL PRIMARY KEY,
                                    first_name VARCHAR(50) NOT NULL,
                                    last_name VARCHAR(50) NOT NULL,
                                    phone VARCHAR(20) NOT NULL UNIQUE,
                                    email VARCHAR(100),
                                    license_number VARCHAR(30),
                                    registration_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                        cmd.ExecuteNonQuery();

                        // Создание таблицы employees
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS employees (
                                    employee_id SERIAL PRIMARY KEY,
                                    first_name VARCHAR(50) NOT NULL,
                                    last_name VARCHAR(50) NOT NULL,
                                    position VARCHAR(50) NOT NULL,
                                    phone VARCHAR(20) NOT NULL,
                                    email VARCHAR(100),
                                    hire_date DATE NOT NULL)";
                        cmd.ExecuteNonQuery();

                        // Создание таблицы car_brands
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS car_brands (
                                    brand_id SERIAL PRIMARY KEY,
                                    name VARCHAR(50) NOT NULL UNIQUE,
                                    country VARCHAR(50))";
                        cmd.ExecuteNonQuery();

                        // Создание таблицы car_models
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS car_models (
                                    model_id SERIAL PRIMARY KEY,
                                    brand_id INTEGER REFERENCES car_brands(brand_id),
                                    name VARCHAR(50) NOT NULL,
                                    year INTEGER NOT NULL,
                                    body_type VARCHAR(30))";
                        cmd.ExecuteNonQuery();

                        // Создание таблицы cars
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS cars (
                                    car_id SERIAL PRIMARY KEY,
                                    model_id INTEGER REFERENCES car_models(model_id),
                                    vin VARCHAR(17) UNIQUE,
                                    color VARCHAR(30) NOT NULL,
                                    production_date DATE,
                                    price DECIMAL(12, 2) NOT NULL,
                                    status VARCHAR(20) NOT NULL DEFAULT 'available' CHECK (status IN ('available', 'reserved', 'sold', 'service')),
                                    mileage INTEGER DEFAULT 0)";
                        cmd.ExecuteNonQuery();

                        // Создание таблицы test_drives
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS test_drives (
                                    test_drive_id SERIAL PRIMARY KEY,
                                    client_id INTEGER REFERENCES clients(client_id),
                                    car_id INTEGER REFERENCES cars(car_id),
                                    employee_id INTEGER REFERENCES employees(employee_id),
                                    scheduled_date TIMESTAMP NOT NULL,
                                    status VARCHAR(20) NOT NULL DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'completed', 'canceled')),
                                    notes TEXT)";
                        cmd.ExecuteNonQuery();

                        // Добавление тестовых данных
                        AddTestData(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации БД: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void AddTestData(NpgsqlCommand cmd)
        {
            // Добавление тестовых брендов (без ON CONFLICT)
            cmd.CommandText = @"INSERT INTO car_brands (name, country) 
                      SELECT 'Porsche', 'Germany'
                      WHERE NOT EXISTS (SELECT 1 FROM car_brands WHERE name = 'Porsche');
                      
                      INSERT INTO car_brands (name, country) 
                      SELECT 'Land Rover', 'UK'
                      WHERE NOT EXISTS (SELECT 1 FROM car_brands WHERE name = 'Land Rover');
                      
                      INSERT INTO car_brands (name, country) 
                      SELECT 'Lexus', 'Japan'
                      WHERE NOT EXISTS (SELECT 1 FROM car_brands WHERE name = 'Lexus')";
            cmd.ExecuteNonQuery();

            // Добавление тестовых моделей (без ON CONFLICT)
            cmd.CommandText = @"INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 1, '911', 2023, 'Coupe'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 1 AND name = '911' AND year = 2023);
                      
                      INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 1, 'Cayenne', 2023, 'SUV'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 1 AND name = 'Cayenne' AND year = 2023);
                      
                      INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 2, 'Range Rover', 2023, 'SUV'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 2 AND name = 'Range Rover' AND year = 2023);
                      
                      INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 2, 'Defender', 2023, 'SUV'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 2 AND name = 'Defender' AND year = 2023);
                      
                      INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 3, 'RX', 2023, 'SUV'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 3 AND name = 'RX' AND year = 2023);
                      
                      INSERT INTO car_models (brand_id, name, year, body_type)
                      SELECT 3, 'ES', 2023, 'Sedan'
                      WHERE NOT EXISTS (SELECT 1 FROM car_models WHERE brand_id = 3 AND name = 'ES' AND year = 2023)";
            cmd.ExecuteNonQuery();

            // Добавление тестовых автомобилей (без ON CONFLICT)
            cmd.CommandText = @"INSERT INTO cars (model_id, color, price, status)
                      SELECT 1, 'Red', 120000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 1 AND color = 'Red' AND price = 120000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 1, 'Black', 125000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 1 AND color = 'Black' AND price = 125000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 2, 'White', 85000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 2 AND color = 'White' AND price = 85000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 3, 'Silver', 95000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 3 AND color = 'Silver' AND price = 95000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 4, 'Green', 70000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 4 AND color = 'Green' AND price = 70000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 5, 'Blue', 65000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 5 AND color = 'Blue' AND price = 65000.00);
                      
                      INSERT INTO cars (model_id, color, price, status)
                      SELECT 6, 'Black', 55000.00, 'available'
                      WHERE NOT EXISTS (SELECT 1 FROM cars WHERE model_id = 6 AND color = 'Black' AND price = 55000.00)";
            cmd.ExecuteNonQuery();

            // Добавление тестового сотрудника (без ON CONFLICT)
            cmd.CommandText = @"INSERT INTO employees (first_name, last_name, position, phone, hire_date)
                      SELECT 'Иван', 'Иванов', 'Менеджер по продажам', '+79123456789', '2020-01-15'
                      WHERE NOT EXISTS (SELECT 1 FROM employees WHERE phone = '+79123456789')";
            cmd.ExecuteNonQuery();
        }

        public int AddClient(string firstName, string lastName, string phone, string? email = null, string? licenseNumber = null)
        {
            string sql = @"INSERT INTO clients (first_name, last_name, phone, email, license_number) 
                         VALUES (@firstName, @lastName, @phone, @email, @licenseNumber) 
                         RETURNING client_id";

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@email", email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@licenseNumber", licenseNumber ?? (object)DBNull.Value);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            // Удаляем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");

            // Приводим к формату 89*********
            if (digits.StartsWith("7") && digits.Length == 11)
                return "8" + digits.Substring(1);
            else if (digits.StartsWith("8") && digits.Length == 11)
                return digits;
            else if (digits.Length == 10)
                return "8" + digits;
            else
                return digits;
        }


        public int? GetClientIdByPhone(string phone)
        {
            // Приводим номер к единому формату
            phone = FormatPhoneNumber(phone);

            string sql = "SELECT client_id FROM clients WHERE phone = @phone OR phone = @altPhone";

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@phone", phone);
                        // Добавляем альтернативный вариант номера (+7 вместо 8)
                        cmd.Parameters.AddWithValue("@altPhone", phone.StartsWith("8") ? "+7" + phone.Substring(1) : phone);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : (int?)null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public int ScheduleTestDrive(int clientId, int carId, DateTime scheduledDate, int employeeId = 1)
        {
            string sql = @"INSERT INTO test_drives 
                         (client_id, car_id, scheduled_date, employee_id) 
                         VALUES (@clientId, @carId, @scheduledDate, @employeeId) 
                         RETURNING test_drive_id";

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        cmd.Parameters.AddWithValue("@carId", carId);
                        cmd.Parameters.AddWithValue("@scheduledDate", scheduledDate);
                        cmd.Parameters.AddWithValue("@employeeId", employeeId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи на тест-драйв: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public List<CarBrand> GetCarBrands()
        {
            var brands = new List<CarBrand>();
            string sql = "SELECT brand_id, name, country FROM car_brands ORDER BY name";

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            brands.Add(new CarBrand
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Country = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
                return brands;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке брендов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return brands;
            }
        }

        public List<CarModel> GetAvailableCars(int brandId)
        {
            var cars = new List<CarModel>();
            string sql = @"SELECT c.car_id, m.name, c.color, c.price 
                          FROM cars c
                          JOIN car_models m ON c.model_id = m.model_id
                          WHERE m.brand_id = @brandId AND c.status = 'available'";

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@brandId", brandId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cars.Add(new CarModel
                                {
                                    Id = reader.GetInt32(0),
                                    ModelName = reader.GetString(1),
                                    Color = reader.GetString(2),
                                    Price = reader.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
                return cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке автомобилей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return cars;
            }
        }
    }

    public class CarBrand
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Country { get; set; }
    }

    public class CarModel
    {
        public int Id { get; set; }
        public required string ModelName { get; set; }
        public required string Color { get; set; }
        public decimal Price { get; set; }
    }
}