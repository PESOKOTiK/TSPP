using MySql.Data.MySqlClient;
using System.Windows;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;

namespace TSPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        const int MAXDBENTRY = 10;
        bool isMaxEntry;
        List<UniWorker> uniWorkers;
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
                exitbuttun.Visibility = Visibility.Collapsed;
                editcol.Width = new GridLength(0);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            updateTable();
        }

        private void updateTable()
        {
            nametxtbx.Text = string.Empty;
            kafedratxtbx.Text = string.Empty;
            birthtxtbx.Text = string.Empty;
            workyeartxtbx.Text = string.Empty;
            ranktxtbx.Text= string.Empty;
            sciranktxtbx.Text= string.Empty;
            selectedId = 0;
            MySql.Data.MySqlClient.MySqlConnection connection;
            using (connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM UniWorkers", connection);
                    uniWorkers = new List<UniWorker>();
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
                    MessageBox.Show("УВАГА!\nДосягнуто максимальну кількість записів");
                    return;
                }
                else
                {
                    try
                    {
                        tmp.Name = nametxtbx.Text;
                        tmp.Kafedra = kafedratxtbx.Text;
                        tmp.BirthYear = Convert.ToInt32(birthtxtbx.Text);
                        tmp.WorkYear = Convert.ToInt32(workyeartxtbx.Text);
                        tmp.Rank = ranktxtbx.Text;
                    }
                    catch
                    {
                        MessageBox.Show("Values error");
                        return;
                    }
                   
                        if (sciranktxtbx.Text == "")
                            tmp.ScienceRank = "none";
                        else
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
            if (nametxtbx.Text == "" || kafedratxtbx.Text == "" || birthtxtbx.Text == "" || workyeartxtbx.Text == "" || ranktxtbx.Text == "")
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
        }

        private void editbtn_Click(object sender, RoutedEventArgs e)
        {
            if(selectedId == 0)
            {
                MessageBox.Show("User not selected");
                return;
            }
            if (CheckForEmptyFields())
            {
                UniWorker tmp = new();
                tmp.Id = selectedId;
                tmp.Name = nametxtbx.Text;
                tmp.Kafedra = kafedratxtbx.Text;
                tmp.BirthYear = Convert.ToInt32(birthtxtbx.Text);
                tmp.WorkYear = Convert.ToInt32(workyeartxtbx.Text);
                tmp.Rank = ranktxtbx.Text;
                if (sciranktxtbx.Text == "")
                    tmp.ScienceRank = "none";
                else
                    tmp.ScienceRank = sciranktxtbx.Text;
                EditUniWorker(tmp.Id, tmp.Name, tmp.Kafedra, tmp.BirthYear, tmp.WorkYear, tmp.Rank, tmp.ScienceRank);
                updateTable();
            }
        }

        private void DeleteUniWorker(int id)
        {
            if (id == 0)
                return;
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
            if (!(selectedId==0))
            {
                MessageBoxResult result = MessageBox.Show("Точно видалити вибраного співробітника?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteUniWorker(selectedId);
                    selectedId = 0;
                    updateTable();
                }
            }
            else
            {
                MessageBox.Show("User not selected");
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
                        if(!isGuest)
                            MessageBox.Show("УВАГА\nДосягнуто максимальну кількість записів");
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

        private void scirankbtn_Click(object sender, RoutedEventArgs e)
        {
            updateTable();
            if(uniWorkers.Count != 0)
            {
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add();
                string find = scirankfind.Text == "" ? "none" : scirankfind.Text;
                Paragraph paragraph = doc.Paragraphs.Add();
                paragraph.Range.Text = $"Workers with science rank {find}\n";
                List<UniWorker> finded = new();
                foreach (var item in uniWorkers)
                {
                    if (item.ScienceRank == find)
                    {
                        finded.Add(item);
                        paragraph = doc.Paragraphs.Add();
                        paragraph.Range.Text = $"Worker {item.Name} has science rank {item.ScienceRank}\n";
                    }
                }
                dataGrid.DataContext = finded;
                object fileName = GetSaveFileName();
                if (fileName!= "")
                {
                    try
                    {
                        doc.SaveAs2(ref fileName);
                        doc.Close();
                        wordApp.Quit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Operation canceled. Document not saved.");
                }
            }
            
        }

        private void seniorsbtn_Click(object sender, RoutedEventArgs e)
        {
            updateTable();

            if (uniWorkers.Count != 0)
            {
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add();
                Paragraph paragraph = doc.Paragraphs.Add();
                paragraph.Range.Text = "Workers older than 60 years\n";
                List<UniWorker> finded = new();
                foreach (var item in uniWorkers)
                {
                    if ((DateTime.Now.Year - item.BirthYear) > 60)
                    {
                        finded.Add(item);
                        paragraph = doc.Paragraphs.Add();
                        paragraph.Range.Text = $"Worker {item.Name} with birth year {item.BirthYear} is older than 60 years and have worked for {DateTime.Now.Year - item.WorkYear} years.\n";
                    }
                }
                dataGrid.DataContext = finded;
                object fileName = GetSaveFileName();
                if (fileName != "")
                {
                    doc.SaveAs2(ref fileName);
                    doc.Close();
                    wordApp.Quit();
                }
                else
                {
                    MessageBox.Show("Operation canceled. Document not saved.");
                }
            }
        }
        private string GetSaveFileName()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Word Documents|*.docx";
            saveFileDialog.Title = "Save Output as Word Document";
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        private void exitbuttun_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void refreshbtn_Click(object sender, RoutedEventArgs e)
        {
            updateTable();
        }
    }
}