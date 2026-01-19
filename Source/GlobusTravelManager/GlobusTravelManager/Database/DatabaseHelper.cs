using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Database
{
    public static class DatabaseHelper
    {
        // Строка подключения - МЕНЯЙ ЕСЛИ НУЖНО
        private static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GlobeTravelDB;Integrated Security=True;";

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}\n\nИзмени строку подключения в DatabaseHelper.cs",
                                "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static User AuthenticateUser(string login, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT UserID, Role, FullName FROM Users WHERE Login = @Login AND Password = @Password";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Role = reader.GetString(1),
                            FullName = reader.GetString(2),
                            Login = login
                        };
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public static List<Tour> GetAllTours()
        {
            var tours = new List<Tour>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT t.TourID, t.TourName, c.CountryName, t.DurationDays, 
                               t.StartDate, t.Price, bt.TypeName, t.Capacity, 
                               t.AvailableSeats, t.PhotoFileName
                        FROM Tours t
                        LEFT JOIN Countries c ON t.CountryID = c.CountryID
                        LEFT JOIN BusTypes bt ON t.BusTypeID = bt.BusTypeID
                        ORDER BY t.StartDate";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tours.Add(new Tour
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CountryName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            DurationDays = reader.GetInt32(3),
                            StartDate = reader.GetDateTime(4),
                            Price = reader.GetDecimal(5),
                            BusTypeName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Capacity = reader.GetInt32(7),
                            AvailableSeats = reader.GetInt32(8),
                            PhotoFileName = reader.IsDBNull(9) ? "" : reader.GetString(9)
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return tours;
        }
    }
}
