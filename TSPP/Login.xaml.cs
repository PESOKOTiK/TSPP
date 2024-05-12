using BCrypt.Net;
using MySql.Data.MySqlClient;
using System.Windows;
namespace TSPP
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        string connectionString = "SERVER=152.67.71.178;PORT=3306;DATABASE=University;UID=oleksii;PASSWORD=20032004Alexey1;";
        public Login()
        {
            InitializeComponent();
        }

        public bool IsGuest { get; private set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            string query = $"SELECT passhash FROM Users WHERE username = {username}";
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
                                string hashedPassword = reader.GetString("passhash");
                                if (VerifyPassword(password, hashedPassword))
                                {
                                    IsGuest = false;
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect password");
                                    connection.Close();
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("User not found");
                                connection.Close();
                                return;
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

            MainWindow mainWindow = new MainWindow(IsGuest);
            mainWindow.Show();
            this.Close();
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(true);
            mainWindow.Show();
            this.Close();
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }
    }
}
