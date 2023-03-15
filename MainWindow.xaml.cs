using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace MemoryComp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		public MainWindow()
		{
			InitializeComponent();
			
		}

		private void btn_chimp_Start(object sender, RoutedEventArgs e)
		{
			Jatek Chimp_Test = new Jatek();
			this.Close();
			Chimp_Test.ShowDialog();
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			grd_lar_2.Width = new System.Windows.GridLength(0);
			grd_lar.Width = 277.7;
			btn_lar.Content = "Bejelentkezés";

		}

        private void txtb_password_selected(object sender, RoutedEventArgs e)
        {
			txt_req.Text = "- Minimum 8 karakterből álljon\n\n- Legyen benne kis- és nagybetű\n\n- Legyen benne szám";
		}

        private void txtb_username_selected(object sender, RoutedEventArgs e)
        {
			txt_req.Text = "- Maximum 20 karakterből álljon\n\n- Ne legyen benne speciális karakter ()<>#&@{{}<łŁ€Í$ß\\|Ä)\n\n- Legyen egyedi";
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
				connect.Open();
				anyad.Text = "zsíroskenyér";
            }
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }
        }
    }
}
