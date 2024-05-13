using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace TSPP
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        string connectionString = "SERVER=152.67.71.178;PORT=3306;DATABASE=University;UID=oleksii;PASSWORD=20032004Alexey1;";
        public Register()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(true);
            mainWindow.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void RegisterButton(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string query = $"Select * from Users where username = '{username}'";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MessageBox.Show("User already exists");
                                return;
                            }
                            else
                            {
                                string hashedPassword = HashPassword(password);
                                string insertQuery = $"INSERT INTO Users (username, passhash) VALUES ('{username}', '{hashedPassword}')";
                                connection.Close();
                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    connection.Open();
                                    insertCommand.ExecuteNonQuery();
                                }
                                MessageBox.Show("User registered");
                                MainWindow mainWindow = new MainWindow(false);
                                mainWindow.Show();
                                this.Close();
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
