using System;
using System.Collections.Generic;
using System.Data;
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
			//grd_lar_2.Width = new System.Windows.GridLength(0);
			//grd_lar.Width = 277.7;
			//btn_lar.Content = "Bejelentkezés";

			
			string specko = @")<>#&@{{}<łŁ€Í$ß\|Ä"; bool nincsspekco = false;
			if (txtb_username.Text != null && txtb_password.Text != null)
			{
				if (txtb_username.Text.Any(char.IsUpper))
				{
					foreach (char item in specko) { if (txtb_username.Text.Contains(item)) nincsspekco = true; };
					if (!nincsspekco)
					{
						string szamok = "0123456789"; bool vanszamok = false;
						foreach (char item in szamok) { if (txtb_username.Text.Contains(item)) vanszamok = true; }
					}
				}
			}




			try
			{
				if (connect.State == ConnectionState.Closed) connect.Open();
				MySqlCommand cmd = new MySqlCommand($"INSERT INTO Accounts (felhnev, jelszo) VALUES ('{txtb_username.Text}', '{txtb_password.Text}');", connect);

				cmd.CommandType = CommandType.Text;
				//valami oknál fogva nem megy, amúgy megelőzné az SQL Injectiont
				//cmd.Parameters.AddWithValue("@Name", txtb_username.Text);
				//cmd.Parameters.AddWithValue("@Password", txtb_password.Text);
				cmd.ExecuteNonQuery();
				MessageBox.Show("Faszának kéne hogy legyünk", "mayhaps", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				connect.Close();
			}
		}

		private void txtb_password_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Text = "- Minimum 8 karakterből álljon\n\n- Legyen benne kis- és nagybetű\n\n- Legyen benne szám";
		}

		private void txtb_username_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Text = "- Maximum 20 karakterből álljon\n\n- Ne legyen benne speciális karakter ( )<>#&@{{}<łŁ€Í$ß\\|Ä )\n\n";
			// - Legyen egyedi
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

		}
	}
}
