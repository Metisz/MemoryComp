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
		//------------------------------------------------------------------------------------------------------
		Dictionary<string, int> MegyeToID;
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		private Account ActiveAccount;
		public MainWindow()
		{
			InitializeComponent();
			TabItems(false, false, false);
			string[] Megyek = new string[19] { "Bács-Kiskun", "Baranya", "Békés", "Borsod-Abaúj-Zemplén", "Csongrád-Csanád", "Fejér", "Győr-Moson-Sopron", "Hajdú-Bihar", "Heves", "Jász-Nagykun-Szolnok", "Komárom-Esztergom", "Nógrád", "Pest", "Somogy", "Szabolcs-Szatmár-Bereg", "Tolna", "Vas", "Veszprém", "Zala" };
			MegyeToID = new Dictionary<string, int>();
			for (int i = 0; i < 19; i++) { MegyeToID.Add(Megyek[i], i + 1); }
			rgstr_cb_megyek.ItemsSource = Megyek;
			ControlTemplate ct = rgstr_cb_megyek.Template;	
		}
		private void rgstr_cb_megyek_loaded(object sender, RoutedEventArgs e)
		{
			ControlTemplate ct = this.rgstr_cb_megyek.Template;
			Popup pop = ct.FindName("PART_Popup", this.rgstr_cb_megyek) as Popup;
			pop.Placement = PlacementMode.Top;
		}

		bool IsNameUnique(string NameInQuestion)
		{
			if (connect.State == ConnectionState.Closed) connect.Open();
			List<string> NameList = new List<string>();
			using (MySqlCommand GetUserNames = new MySqlCommand("SELECT felhnev FROM accounts;", connect))
			{
				using (MySqlDataReader reader = GetUserNames.ExecuteReader())
				{
					while (reader.Read())
					{
						NameList.Add(reader.GetString(0));
					}
				}
			}
			return !NameList.Contains(NameInQuestion);
		}
		void TabItems(bool rgstr, bool lgn, bool gms)
        {
			tabitem_register.IsEnabled = rgstr;
			tabitem_login.IsEnabled = lgn;
			tabitem_games.IsEnabled = gms;
        }
		//------------------------------------------------------------------------------------------------------
		#endregion

		private void btn_chimp_Start(object sender, RoutedEventArgs e)
		{
			Jatek Chimp_Test = new Jatek(ActiveAccount);
			this.Close();
			Chimp_Test.ShowDialog();
		}

		private void Register_Click(object sender, RoutedEventArgs e)
		{
			bool[] UsernameReqs = new bool[4] { false ,false, false, true }; //1. A Név egyedi-e| 2. Textbox üres-e | 3. Maximum 20 karakter | 4. Speciális karakterek			
			UsernameReqs[0] = IsNameUnique(rgstr_txtb_username.Text);
			if (rgstr_txtb_username.Text != null) { UsernameReqs[1] = true; }
			if (rgstr_txtb_username.Text.Length <= 20) { UsernameReqs[2] = true; }
			foreach (char item in @"()<>#&@{{}<łŁ€Í$ß\|ÄäđĐ[]") { if (rgstr_txtb_username.Text.Contains(item)) UsernameReqs[3] = false; };

			bool[] PasswordReqs = new bool[4]; //1. Textbox üres-e | 2. Minimum 8 karakter | 3. Kis- és Nagybetűk | 4. Szám van-e
			if (rgstr_txtb_password.Text != null) { PasswordReqs[0] = true; }
			if (rgstr_txtb_password.Text.Length >= 8) { PasswordReqs[1] = true; }
			if (rgstr_txtb_password.Text.Any(char.IsUpper)) { PasswordReqs[2] = true; }
			foreach (char item in "0123456789") { if (rgstr_txtb_password.Text.Contains(item)) PasswordReqs[3] = true; };

			if (UsernameReqs.All(x => x) && PasswordReqs.All(x => x) && rgstr_cb_megyek.SelectedItem != null)
			{
				try
				{
					if (connect.State == ConnectionState.Closed) connect.Open();
					MySqlCommand RegisterCMD = new MySqlCommand($"INSERT INTO Accounts (felhnev, jelszo, megyeid) " +
						$"VALUES ('{rgstr_txtb_username.Text}', '{rgstr_txtb_password.Text}', {MegyeToID[rgstr_cb_megyek.SelectedItem.ToString()]});", connect);
					RegisterCMD.CommandType = CommandType.Text;
					RegisterCMD.ExecuteNonQuery();
					tabctrl_menus.SelectedItem = tabitem_login;
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
				if (rgstr_cb_megyek.SelectedItem == null) { hibatext += "- Válasszon ki egy megyét!\n\n"; }
				MessageBox.Show(hibatext, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

			}
            //grd_lar_2.Width = new System.Windows.GridLength(0);
            //grd_lar.Width = 277.7;
            //btn_lar.Content = "Bejelentkezés";
    }

        

    //Követelmények
    private void rgstr_object_selected(object sender, RoutedEventArgs e)
		{
			if (sender == rgstr_txtb_username) { txt_req.Text = "- Minimum 8 karakterből álljon\n\n- Legyen benne kis- és nagybetű\n\n- Legyen benne szám"; }
			if (sender == rgstr_txtb_password) { txt_req.Text = "- Legyen egyedi\n\n- Maximum 20 karakterből álljon\n\n- Ne legyen benne speciális karakter ( )<>#&@{{}<łŁ€$ß\\|Ä )\n\n"; }
			if (sender == rgstr_cb_megyek) { txt_req.Text = "- Válasszon ki egy megyét a 19 lehetőség közül!"; }
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
      try
      {
				if (connect.State == ConnectionState.Closed) connect.Open();
				csatlakoz_teszt.Text = "Csatlakozott!";
        try
        {
					test_activeacc.Content = ActiveAccount.Username;
				}
        catch (Exception)
        {
					test_activeacc.Content = "nope";
				}
				connect.Close();
			}
      catch (Exception ex)
      {
				MessageBox.Show(ex.Message);
      }
		}

		private void MMenuButtonClick(object sender, RoutedEventArgs e)
		{
			if (sender == mmenu_account)
			{
				main_menu.Visibility = Visibility.Hidden;
				tabctrl_menus.Visibility = Visibility.Visible;
				TabItems(true, true, false);
				tabctrl_menus.SelectedItem = tabitem_login;
			}
			if (sender == mmenu_guest)
			{
				main_menu.Visibility = Visibility.Hidden;
				tabctrl_menus.Visibility = Visibility.Visible;
				TabItems(false, false, true);
				tabctrl_menus.SelectedItem = tabitem_games;
			}
			if (sender == mmenu_quit)
			{
				if (MessageBox.Show("Biztos ki szeretne lépni?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
				{
					this.Close();
				}
			}
		}

		private void MMenuButtonBackClick(object sender, RoutedEventArgs e)
		{
			TabItems(false, false, false);
			if (sender == games_back) { ActiveAccount = null; }
			main_menu.Visibility = Visibility.Visible;
			tabctrl_menus.Visibility = Visibility.Hidden;
		}

    private void lgn_btn_login_Click(object sender, RoutedEventArgs e)
    {
			if (connect.State == ConnectionState.Closed) connect.Open();
			using (MySqlCommand LoginAttempt = new MySqlCommand($"SELECT id, felhnev FROM accounts WHERE felhnev = '{lgn_txtb_username.Text}' AND jelszo = '{lgn_txtb_password.Text}';", connect))
			{
				using (MySqlDataReader reader = LoginAttempt.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						ActiveAccount = new Account(reader.GetInt32(0), reader.GetString(1));
						TabItems(false, false, true);
						tabctrl_menus.SelectedItem = tabitem_games;
					}
					else
					{
						MessageBox.Show("Hibás Felhasználónév/Jelszó kombináció!", "", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			connect.Close();
		}
    }
}
