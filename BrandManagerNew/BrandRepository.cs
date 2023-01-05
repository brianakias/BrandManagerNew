
using Npgsql;
using System;
using System.Collections.Generic;

namespace BrandManagerNew
{
    public class BrandRepository : IDataAccess
    {
        private static string connectionString = Environment.GetEnvironmentVariable("postgresConnectionString");

        public void CreateRecord(Brand brand)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO brands (name, is_enabled) VALUES (@name, @is_enabled)";
                    command.Parameters.AddWithValue("@name", brand.Name);
                    command.Parameters.AddWithValue("@is_enabled", brand.IsEnabled);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public List<Brand> ReadRecords()
        {
            List<Brand> brands = new List<Brand>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM brands ORDER BY id", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Brand brand = new Brand();
                            brand.Id = reader.GetInt32(0);
                            brand.Name = reader.GetString(1);
                            brand.IsEnabled = reader.GetBoolean(2);
                            brands.Add(brand);
                        }
                    }
                }
                connection.Close();
                return brands;
            }
        }

        public List<string> ReadBrandNames()
        {
            List<string> brandNames = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT name FROM brands", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            brandNames.Add(reader.GetString(0));
                        }
                    }
                }
                connection.Close();
                return brandNames;
            }
        }

        public List<int> ReadIDs()
        {
            List<int> ids = new List<int>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT id FROM brands", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt16(0));
                        }
                    }
                }
                connection.Close();
                return ids;
            }

        }

        public void UpdateRecord(Brand brand)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE brands SET name = @name, is_enabled = @is_enabled WHERE id = @id";
                    command.Parameters.AddWithValue("id", brand.Id);
                    command.Parameters.AddWithValue("@name", brand.Name);
                    command.Parameters.AddWithValue("@is_enabled", brand.IsEnabled);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void DeleteRecord(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM brands WHERE id = @id";
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void CreateTableIfNotExists(string tableName)
        {

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName} (id smallserial PRIMARY KEY, name varchar(255) NOT NULL, is_enabled boolean NOT NULL)";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

        }
    }
}
