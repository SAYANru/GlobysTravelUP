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
        private static string connectionString = @"Data Source=KYARO;Initial Catalog=GlobusTravelDB;Integrated Security=True;";

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
        public static List<string> GetAllCountries()
        {
            var countries = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT CountryName FROM Countries ORDER BY CountryName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        countries.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стран: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return countries;
        }

        public static bool DeleteTour(int tourId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем, есть ли заявки на этот тур
                    string checkQuery = "SELECT COUNT(*) FROM Bookings WHERE TourID = @TourID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@TourID", tourId);
                    int bookingCount = (int)checkCmd.ExecuteScalar();

                    if (bookingCount > 0)
                    {
                        MessageBox.Show("Нельзя удалить тур, на который есть заявки", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    // Удаляем тур
                    string deleteQuery = "DELETE FROM Tours WHERE TourID = @TourID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@TourID", tourId);

                    int result = deleteCmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления тура: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static List<Booking> GetAllBookings()
        {
            var bookings = new List<Booking>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT b.BookingID, t.TourName, u.FullName, b.BookingDate, 
                       b.Status, b.PeopleCount, b.TotalPrice, b.Comment
                FROM Bookings b
                JOIN Tours t ON b.TourID = t.TourID
                JOIN Users u ON b.UserID = u.UserID
                ORDER BY b.BookingDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        bookings.Add(new Booking
                        {
                            Id = reader.GetInt32(0),
                            TourName = reader.GetString(1),
                            ClientName = reader.GetString(2),
                            BookingDate = reader.GetDateTime(3),
                            Status = reader.GetString(4),
                            PeopleCount = reader.GetInt32(5),
                            TotalPrice = reader.GetDecimal(6),
                            Comment = reader.IsDBNull(7) ? "" : reader.GetString(7)
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return bookings;
        }

        public static List<string> GetBookingStatuses()
        {
            return new List<string>
    {
        "Все",
        "Новая",
        "Подтверждена",
        "Отменена",
        "В обработке"
    };
        }

        public static bool UpdateBookingStatus(int bookingId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем, можно ли подтвердить заявку
                    if (newStatus == "Подтверждена")
                    {
                        // Получаем информацию о туре
                        string checkQuery = @"
                    SELECT t.AvailableSeats, b.PeopleCount
                    FROM Bookings b
                    JOIN Tours t ON b.TourID = t.TourID
                    WHERE b.BookingID = @BookingID";

                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@BookingID", bookingId);

                        SqlDataReader reader = checkCmd.ExecuteReader();
                        if (reader.Read())
                        {
                            int availableSeats = reader.GetInt32(0);
                            int peopleCount = reader.GetInt32(1);

                            if (availableSeats < peopleCount)
                            {
                                reader.Close();
                                MessageBox.Show($"Недостаточно свободных мест! Доступно: {availableSeats}, требуется: {peopleCount}",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        }
                        reader.Close();
                    }

                    // Обновляем статус
                    string updateQuery = "UPDATE Bookings SET Status = @Status WHERE BookingID = @BookingID";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Status", newStatus);
                    updateCmd.Parameters.AddWithValue("@BookingID", bookingId);

                    int result = updateCmd.ExecuteNonQuery();

                    // Если подтверждаем заявку, уменьшаем количество свободных мест
                    if (newStatus == "Подтверждена" && result > 0)
                    {
                        string updateSeatsQuery = @"
                    UPDATE Tours 
                    SET AvailableSeats = AvailableSeats - (
                        SELECT PeopleCount FROM Bookings WHERE BookingID = @BookingID
                    )
                    WHERE TourID = (
                        SELECT TourID FROM Bookings WHERE BookingID = @BookingID
                    )";

                        SqlCommand updateSeatsCmd = new SqlCommand(updateSeatsQuery, conn);
                        updateSeatsCmd.Parameters.AddWithValue("@BookingID", bookingId);
                        updateSeatsCmd.ExecuteNonQuery();
                    }

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static List<User> GetAllClients()
        {
            var clients = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT UserID, FullName, Login FROM Users WHERE Role = 'Авторизированный клиент' ORDER BY FullName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        clients.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Login = reader.GetString(2)
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return clients;
        }

        public static bool CreateBooking(int tourId, int userId, int peopleCount, decimal totalPrice, string comment)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем доступность мест
                    string checkQuery = "SELECT AvailableSeats FROM Tours WHERE TourID = @TourID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@TourID", tourId);

                    int availableSeats = (int)checkCmd.ExecuteScalar();

                    if (availableSeats < peopleCount)
                    {
                        MessageBox.Show($"Недостаточно свободных мест! Доступно: {availableSeats}, требуется: {peopleCount}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    // Создаем заявку
                    string insertQuery = @"
                INSERT INTO Bookings (TourID, UserID, PeopleCount, TotalPrice, Comment, Status)
                VALUES (@TourID, @UserID, @PeopleCount, @TotalPrice, @Comment, 'Новая')";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@TourID", tourId);
                    insertCmd.Parameters.AddWithValue("@UserID", userId);
                    insertCmd.Parameters.AddWithValue("@PeopleCount", peopleCount);
                    insertCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    insertCmd.Parameters.AddWithValue("@Comment", string.IsNullOrEmpty(comment) ? DBNull.Value : (object)comment);

                    int result = insertCmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания заявки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        // Автобусы
        public static List<BusType> GetAllBusTypes()
        {
            var busTypes = new List<BusType>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT BusTypeID, TypeName, Description, Capacity FROM BusTypes ORDER BY TypeName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        busTypes.Add(new BusType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Capacity = reader.GetInt32(3)
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов автобусов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return busTypes;
        }

        public static bool AddBusType(string name, string description, int capacity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO BusTypes (TypeName, Description, Capacity) VALUES (@Name, @Description, @Capacity)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description);
                    cmd.Parameters.AddWithValue("@Capacity", capacity);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления типа автобуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool UpdateBusType(int id, string name, string description, int capacity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE BusTypes SET TypeName = @Name, Description = @Description, Capacity = @Capacity WHERE BusTypeID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description);
                    cmd.Parameters.AddWithValue("@Capacity", capacity);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления типа автобуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool DeleteBusType(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем, используется ли тип автобуса в турах
                    string checkQuery = "SELECT COUNT(*) FROM Tours WHERE BusTypeID = @ID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@ID", id);

                    int usageCount = (int)checkCmd.ExecuteScalar();
                    if (usageCount > 0)
                    {
                        MessageBox.Show("Нельзя удалить тип автобуса, который используется в турах", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    string query = "DELETE FROM BusTypes WHERE BusTypeID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления типа автобуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Клиенты
        public static bool UpdateClient(int id, string fullName, string login, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Users SET FullName = @FullName, Login = @Login, Password = @Password WHERE UserID = @ID AND Role = 'Авторизированный клиент'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления клиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool DeleteClient(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем, есть ли заявки у клиента
                    string checkQuery = "SELECT COUNT(*) FROM Bookings WHERE UserID = @ID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@ID", id);

                    int bookingCount = (int)checkCmd.ExecuteScalar();
                    if (bookingCount > 0)
                    {
                        MessageBox.Show("Нельзя удалить клиента, у которого есть заявки", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    string query = "DELETE FROM Users WHERE UserID = @ID AND Role = 'Авторизированный клиент'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления клиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Статистика
        public static Dictionary<string, int> GetStatistics()
        {
            var stats = new Dictionary<string, int>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Количество туров
                    string toursQuery = "SELECT COUNT(*) FROM Tours";
                    SqlCommand toursCmd = new SqlCommand(toursQuery, conn);
                    stats["Tours"] = (int)toursCmd.ExecuteScalar();

                    // Количество заявок
                    string bookingsQuery = "SELECT COUNT(*) FROM Bookings";
                    SqlCommand bookingsCmd = new SqlCommand(bookingsQuery, conn);
                    stats["Bookings"] = (int)bookingsCmd.ExecuteScalar();

                    // Количество клиентов
                    string clientsQuery = "SELECT COUNT(*) FROM Users WHERE Role = 'Авторизированный клиент'";
                    SqlCommand clientsCmd = new SqlCommand(clientsQuery, conn);
                    stats["Clients"] = (int)clientsCmd.ExecuteScalar();

                    // Доход (сумма подтвержденных заявок)
                    string revenueQuery = "SELECT ISNULL(SUM(TotalPrice), 0) FROM Bookings WHERE Status = 'Подтверждена'";
                    SqlCommand revenueCmd = new SqlCommand(revenueQuery, conn);
                    stats["Revenue"] = (int)revenueCmd.ExecuteScalar();

                    // Свободные места
                    string seatsQuery = "SELECT ISNULL(SUM(AvailableSeats), 0) FROM Tours";
                    SqlCommand seatsCmd = new SqlCommand(seatsQuery, conn);
                    stats["AvailableSeats"] = (int)seatsCmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return stats;
        }
    }
}
