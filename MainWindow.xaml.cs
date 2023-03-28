using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
		#region Kötelező dolgok amiket nem tudom hova kéne rakni
		Dictionary<string, int> MegyeToID;
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		public MainWindow()
		{
			InitializeComponent();
			string[] Megyek = new string[19] { "Bács-Kiskun", "Baranya", "Békés", "Borsod-Abaúj-Zemplén", "Csongrád-Csanád", "Fejér", "Győr-Moson-Sopron", "Hajdú-Bihar", "Heves", "Jász-Nagykun-Szolnok", "Komárom-Esztergom", "Nógrád", "Pest", "Somogy", "Szabolcs-Szatmár-Bereg", "Tolna", "Vas", "Veszprém", "Zala" };
			MegyeToID = new Dictionary<string, int>();
			for (int i = 0; i < 19; i++) { MegyeToID.Add(Megyek[i], i + 1); }
			cb_megyek.ItemsSource = Megyek;
			ControlTemplate ct = cb_megyek.Template;

			
		}
		private void cb_megyek_loaded(object sender, RoutedEventArgs e)
		{
			ControlTemplate ct = this.cb_megyek.Template;
			Popup pop = ct.FindName("PART_Popup", this.cb_megyek) as Popup;
			pop.Placement = PlacementMode.Top;
		}
		#endregion
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

            /*
			if (txtb_username.Text != null && txtb_password.Text != null)
            {
                if (txtb_username.Text.Any(char.IsUpper))
                {
					bool nincsspekco = false;
					foreach (char item in @")<>#&@{{}<łŁ€Í$ß\|Ä") { if (txtb_username.Text.Contains(item)) nincsspekco = true; };
                    if (!nincsspekco)
                    {
						bool vanszamok = false;
                        foreach (char item in "0123456789") { if (txtb_username.Text.Contains(item)) vanszamok = true; }
                    }
                }
            }
			*/



            if (cb_megyek.SelectedItem != null)
            {
				try
				{
					if (connect.State == ConnectionState.Closed) connect.Open();
					MySqlCommand cmd = new MySqlCommand($"INSERT INTO Accounts (felhnev, jelszo, megyeid) VALUES ('{txtb_username.Text}', '{txtb_password.Text}', {MegyeToID[cb_megyek.SelectedItem.ToString()]});", connect);

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
		}

        #region Követelmények
        private void txtb_password_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Text = "- Minimum 8 karakterből álljon\n\n- Legyen benne kis- és nagybetű\n\n- Legyen benne szám";
		}

		private void txtb_username_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Text = "- Maximum 20 karakterből álljon\n\n- Ne legyen benne speciális karakter ( )<>#&@{{}<łŁ€Í$ß\\|Ä )\n\n";
			// - Legyen egyedi
		}
        private void cb_megyek_selected(object sender, RoutedEventArgs e)
        {
			txt_req.Text = "- Válasszon ki egy megyét a 19 lehetőség közül!";
        }
		#endregion
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            try
            {
				connect.Open();
				anyad.Text = "heheahahahehoho";
			}
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }
		}

    }
}
