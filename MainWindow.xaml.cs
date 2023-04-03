using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
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
			txt_req.Foreground = Brushes.Black;
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

		bool IsNameUnique(string NameInQuestion)
        {
			if (connect.State == ConnectionState.Closed) connect.Open();
			List<string> test = new List<string>();
			using (MySqlCommand GetUserNames = new MySqlCommand("SELECT felhnev FROM accounts", connect))
			{
				using (MySqlDataReader reader = GetUserNames.ExecuteReader())
				{
					while (reader.Read())
					{
						test.Add(reader.GetString(0));
					}
				}
			}
			return !test.Contains(NameInQuestion);
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
			bool[] UsernameReqs = new bool[4] { false ,false, false, true }; //1. A Név egyedi-e| 1. Textbox üres-e | 2. Maximum 20 karakter | 3. Speciális karakterek			
			UsernameReqs[0] = IsNameUnique(txtb_username.Text);
			if (txtb_username.Text != null) { UsernameReqs[1] = true; }
			if (txtb_username.Text.Length <= 20) { UsernameReqs[2] = true; }
			foreach (char item in @"()<>#&@{{}<łŁ€Í$ß\|ÄäđĐ[]") { if (txtb_username.Text.Contains(item)) UsernameReqs[3] = false; };

			bool[] PasswordReqs = new bool[4]; //1. Textbox üres-e | 2. Minimum 8 karakter | 3. Kis- és Nagybetűk | 4. Szám van-e
			if (txtb_password.Text != null) { PasswordReqs[0] = true; }
			if (txtb_password.Text.Length >= 8) { PasswordReqs[1] = true; }
			if (txtb_password.Text.Any(char.IsUpper)) { PasswordReqs[2] = true; }
			foreach (char item in "0123456789") { if (txtb_password.Text.Contains(item)) PasswordReqs[3] = true; };

			if (UsernameReqs.All(x => x) && PasswordReqs.All(x => x) && cb_megyek.SelectedItem != null)
            {
				try
				{
					if (connect.State == ConnectionState.Closed) connect.Open();
					MySqlCommand RegisterCMD = new MySqlCommand($"INSERT INTO Accounts (felhnev, jelszo, megyeid) VALUES ('{txtb_username.Text}', '{txtb_password.Text}', {MegyeToID[cb_megyek.SelectedItem.ToString()]});", connect);

					RegisterCMD.CommandType = CommandType.Text;
					//valami oknál fogva nem megy, amúgy megelőzné az SQL Injectiont
					//RegisterCMD.Parameters.AddWithValue("@Name", txtb_username.Text);
					//RegisterCMD.Parameters.AddWithValue("@Password", txtb_password.Text);
					RegisterCMD.ExecuteNonQuery();
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
            else
            {
				string hibatext = "";
				if (!UsernameReqs[0]) { hibatext += "- Legyen a Felhasználónév egyedi!\n\n"; }
				if (!UsernameReqs[1]) { hibatext += "- Ne legyen üres a Felhasználónév mező!\n\n"; }
				if (!UsernameReqs[2]) { hibatext += "- Ne legyen a Felhasználónév 20 karakternél hosszabb!\n\n"; }
				if (!UsernameReqs[3]) { hibatext += "- Ne legyen a Felhasználónévben speciális karakter!\n\n"; }
				if (!PasswordReqs[0]) { hibatext += "- Ne legyen üres a Jelszó mező!\n\n"; }
				if (!PasswordReqs[1]) { hibatext += "- A Jelszó minimum 8 karakter hosszú legyen!\n\n"; }
				if (!PasswordReqs[2]) { hibatext += "- A Jelszóban legyen kis- és nagybetű is!\n\n"; }
				if (!PasswordReqs[3]) { hibatext += "- A Jelszóban legyen legalább egy szám!\n\n"; }
				if (cb_megyek.SelectedItem == null) { hibatext += "- Válasszon ki egy megyét!\n\n"; }
				MessageBox.Show(hibatext, "Hiba",MessageBoxButton.OK,MessageBoxImage.Error);
				
			}
            //grd_lar_2.Width = new System.Windows.GridLength(0);
            //grd_lar.Width = 277.7;
            //btn_lar.Content = "Bejelentkezés";
        }

        

        #region Követelmények
        private void txtb_password_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Foreground = Brushes.Black;
			txt_req.Text = "- Minimum 8 karakterből álljon\n\n- Legyen benne kis- és nagybetű\n\n- Legyen benne szám";
		}

		private void txtb_username_selected(object sender, RoutedEventArgs e)
		{
			txt_req.Foreground = Brushes.Black;
			txt_req.Text = "- Legyen egyedi\n\n- Maximum 20 karakterből álljon\n\n- Ne legyen benne speciális karakter ( )<>#&@{{}<łŁ€$ß\\|Ä )\n\n";
			// - Legyen egyedi
		}
        private void cb_megyek_selected(object sender, RoutedEventArgs e)
        {
			txt_req.Foreground = Brushes.Black;
			txt_req.Text = "- Válasszon ki egy megyét a 19 lehetőség közül!";
        }
		#endregion
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            try
            {
				if (connect.State == ConnectionState.Closed) connect.Open();
				csatlakoz_teszt.Text = "Csatlakozott!";
				connect.Close();
			}
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }
		}

    }
}
