using MySql.Data.MySqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con;
            MySqlCommand cmd;
            MySqlDataReader dr;
            string path = "Data Source=localhost;Initial Catalog=Prescripshun;Integrated Security=True";
            string sql = "Select * from Users";
            con = new MySqlConnection(path);
            try
            {
                con.Open();
                cmd = new MySqlCommand(sql, con);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MessageBox.Show(dr.GetString(0) + " " + dr.GetString(1) + " " + dr.GetString(2));
                }
                dr.Close();
                cmd.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection!");
            }
        }
    }
}