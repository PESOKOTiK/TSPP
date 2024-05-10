using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace TSPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int MAXDBENTRY = 10;
        bool isMaxEntry;
        public bool isGuest { get; private set; }
        string connectionString = "SERVER=152.67.71.178;PORT=3306;DATABASE=University;UID=oleksii;PASSWORD=20032004Alexey1;";
        public MainWindow(bool _isGuest)
        {
            InitializeComponent();
            updateTable();
            this.isGuest = _isGuest;
            if(isGuest)
            {
                addbtn.IsEnabled = false;
                editbtn.IsEnabled = false;
                deletebtn.IsEnabled = false;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            updateTable();
        }

        private void updateTable()
        {
            MySql.Data.MySqlClient.MySqlConnection connection;
            using (connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM UniWorkers", connection);
                    List<UniWorker> uniWorkers = new List<UniWorker>();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        uniWorkers.Add(new UniWorker
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Kafedra = reader.GetString(2),
                            BirthYear = reader.GetInt32(3),
                            WorkYear = reader.GetInt32(4),
                            Rank = reader.GetString(5),
                            ScienceRank = reader.GetString(6)
                        });
                    }
                    connection.Close();
                    if (uniWorkers.Count == 0)
                        MessageBox.Show("таблиця пуста");
                    else
                        dataGrid.DataContext = uniWorkers;
                }
                catch
                {
                    MessageBox.Show("Помилка при підключенні до бази даних");
                }
            }
            checkForMaxEntry();
        }
        int selectedId;
        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UniWorker editable = dataGrid.SelectedItem as UniWorker;
            try
            {
                selectedId = editable.Id;
                nametxtbx.Text = editable.Name;
                kafedratxtbx.Text = editable.Kafedra;
                birthtxtbx.Text = editable.BirthYear.ToString();
                workyeartxtbx.Text = editable.WorkYear.ToString();
                ranktxtbx.Text = editable.Rank;
                sciranktxtbx.Text = editable.ScienceRank;
            }
            catch
            {

            }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            UniWorker tmp = new();
            if (CheckForEmptyFields())
            {
                if(isMaxEntry)
                {
                    MessageBox.Show("Досягнуто максимальну кількість записів");
                    return;
                }
                else
                {
                    tmp.Name = nametxtbx.Text;
                    tmp.Kafedra = kafedratxtbx.Text;
                    tmp.BirthYear = Convert.ToInt32(birthtxtbx.Text);
                    tmp.WorkYear = Convert.ToInt32(workyeartxtbx.Text);
                    tmp.Rank = ranktxtbx.Text;
                    tmp.ScienceRank = sciranktxtbx.Text;
                    AddUniWorker(tmp.Name, tmp.Kafedra, tmp.BirthYear, tmp.WorkYear, tmp.Rank, tmp.ScienceRank);
                }
            }
            else
            {
                MessageBox.Show("Заповніть всі поля");
            }
        }

        private bool CheckForEmptyFields()
        {
            if (nametxtbx.Text == "" || kafedratxtbx.Text == "" || birthtxtbx.Text == "" || workyeartxtbx.Text == "" || ranktxtbx.Text == "" || sciranktxtbx.Text == "")
            {
                MessageBox.Show("Заповніть всі поля");
                return false;
            }
            return true;
        }

        public void AddUniWorker(string name, string kafedra, int birthYear, int workYear, string rank, string scienceRank)
        {
            string sqlQuery = "INSERT INTO UniWorkers (`Name`, `Kafedra`, `BirthYear`, `WorkYear`, `Rank`, `ScienceRank`) " +
                              "VALUES (@Name, @Kafedra, @BirthYear, @WorkYear, @Rank, @ScienceRank)";



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Kafedra", kafedra);
                    cmd.Parameters.AddWithValue("@BirthYear", birthYear);
                    cmd.Parameters.AddWithValue("@WorkYear", workYear);
                    cmd.Parameters.AddWithValue("@Rank", rank);
                    cmd.Parameters.AddWithValue("@ScienceRank", scienceRank);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            updateTable();
        }

        private void EditUniWorker(int id, string name, string kafedra, int birthYear, int workYear, string rank, string scienceRank)
        {
            string sqlQuery = "UPDATE UniWorkers SET `Name` = @Name, `Kafedra` = @Kafedra, `BirthYear` = @BirthYear, `WorkYear` = @WorkYear, `Rank` = @Rank, `ScienceRank` = @ScienceRank WHERE `Id` = @Id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Kafedra", kafedra);
                    cmd.Parameters.AddWithValue("@BirthYear", birthYear);
                    cmd.Parameters.AddWithValue("@WorkYear", workYear);
                    cmd.Parameters.AddWithValue("@Rank", rank);
                    cmd.Parameters.AddWithValue("@ScienceRank", scienceRank);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            updateTable();
        }

        private void editbtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForEmptyFields())
            {
                UniWorker tmp = new();
                tmp.Id = selectedId;
                tmp.Name = nametxtbx.Text;
                tmp.Kafedra = kafedratxtbx.Text;
                tmp.BirthYear = Convert.ToInt32(birthtxtbx.Text);
                tmp.WorkYear = Convert.ToInt32(workyeartxtbx.Text);
                tmp.Rank = ranktxtbx.Text;
                tmp.ScienceRank = sciranktxtbx.Text;
                EditUniWorker(tmp.Id, tmp.Name, tmp.Kafedra, tmp.BirthYear, tmp.WorkYear, tmp.Rank, tmp.ScienceRank);
                updateTable();
            }
        }

        private void DeleteUniWorker(int id)
        {
            string sqlQuery = "DELETE FROM UniWorkers WHERE `Id` = @Id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            updateTable();
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForEmptyFields())
            {
                MessageBoxResult result = MessageBox.Show("Точно видалити вибраного співробітника?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteUniWorker(selectedId);
                    updateTable();
                }
            }
            
            
        }

        public void checkForMaxEntry()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM UniWorkers", connection);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count >= MAXDBENTRY)
                    {
                        MessageBox.Show("Досягнуто максимальну кількість записів");
                        isMaxEntry = true;
                    }
                    else
                    {
                        isMaxEntry = false;
                    }
                }
                catch
                {
                    MessageBox.Show("Помилка при підключенні до бази даних");
                }
            }
        }
    }
}