using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MemoryComp
{
	public partial class Reakcio : Window
	{
		int jatekid = 2;
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		private bool HasAccount;
		private Account ActiveAccount;
		public Random rnd = new Random();
		private int pont = 0;
		public Dictionary<string, int> MegyeToID;
		private DispatcherTimer timer_cooldown;
		DateTime FigyelKezd; TimeSpan Kulonbseg;
		public int Pont
		{
			get
			{
				return pont;
			}
			set
			{
				pont = Convert.ToInt32(value);
			}
		}

		public Reakcio(Account ActiveAccount)
		{
			InitializeComponent();
			if (ActiveAccount == null) { HasAccount = false; }
			else
			{
				HasAccount = true;
				this.ActiveAccount = ActiveAccount;
				cb_megyek.SelectedIndex = ActiveAccount.Megyeid - 1;
			}
			MegyeToID = new Dictionary<string, int>();
			if (connect.State == ConnectionState.Closed) connect.Open();
			using (MySqlCommand GetMegyek = new MySqlCommand($"SELECT * FROM megyek", connect))
			{
				using (MySqlDataReader reader = GetMegyek.ExecuteReader())
				{
					while (reader.Read())
					{
						MegyeToID.Add(reader.GetString(1), reader.GetInt32(0));
					}
				}
			}
			connect.Close();
			cb_megyek.ItemsSource = MegyeToID.Keys.ToList();
			stckpnl_leaderboard.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Hidden;
			timer_cooldown = new DispatcherTimer();
			timer_cooldown.Tick += Dt_Tick;
			Start();
		}
		private void Dt_Tick(object sender, EventArgs e)
		{
			btn_game.Background = Brushes.Green;
			btn_game.Content = "Katt!";
			timer_cooldown.Stop();
			FigyelKezd = DateTime.Now;
		}

		public void Start()
		{
			timer_cooldown.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000,10000));
			btn_game.Content = "Várj...";
			timer_cooldown.Start();
		}

		public void Lose()
		{
			lbl_points_earned.Content = $"{Pont}";
			if (HasAccount)
			{
				btn_leaderboard.IsEnabled = false;
				if (connect.State == ConnectionState.Closed) connect.Open();
				using (MySqlCommand HasScore = new MySqlCommand($"SELECT felhid, rekordpont FROM pontok WHERE felhid = {ActiveAccount.Userid} AND jatekid = {jatekid};", connect))
				{
					using (MySqlDataReader reader = HasScore.ExecuteReader())
					{
						if (reader.HasRows)
						{
							reader.Read();
							if (Pont < reader.GetInt32(1))
							{
								lbl_newrecord.Visibility = Visibility.Visible;
								connect.Close(); connect.Open();
								MySqlCommand RegisterCMD = new MySqlCommand($"UPDATE pontok SET rekordpont = {Pont} WHERE felhid = {ActiveAccount.Userid} AND jatekid = {jatekid};", connect);
								RegisterCMD.CommandType = CommandType.Text;
								RegisterCMD.ExecuteNonQuery();
							}
						}
						else
						{
							connect.Close(); connect.Open();
							lbl_newrecord.Visibility = Visibility.Visible;
							MySqlCommand RegisterCMD = new MySqlCommand($"INSERT INTO pontok (felhid, jatekid ,rekordpont) VALUES ({ActiveAccount.Userid},{jatekid},{Pont});", connect);
							RegisterCMD.CommandType = CommandType.Text;
							RegisterCMD.ExecuteNonQuery();
							connect.Close();
						}
					}
				}
				connect.Close();
			}
			else btn_leaderboard.IsEnabled = false;
			btn_game.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Visible;
		}



		private void btn_restart_Click(object sender, RoutedEventArgs e)
		{
			lbl_newrecord.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Hidden;
			btn_game.Background = new SolidColorBrush(Color.FromRgb(211, 211, 211));
			btn_game.Visibility = Visibility.Visible;
			Pont = 0;

			Start();
		}

		private void btn_quit_Click(object sender, RoutedEventArgs e)
		{
			MainWindow Menu = new MainWindow();
			this.Close();
			Menu.ShowDialog();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            if (btn_game.Background == Brushes.Green)
            {
				Kulonbseg = DateTime.Now - FigyelKezd;
				Pont = Convert.ToInt32(Kulonbseg.TotalMilliseconds);
				Lose();
            }
		}

		private void btn_leaderboard_load(object sender, RoutedEventArgs e)
		{
			stckpnl_lose.Visibility = Visibility.Hidden;

			if (connect.State == ConnectionState.Closed) connect.Open();
			try
			{
				MySqlCommand cmd = new MySqlCommand("Select accounts.felhnev as 'Nevek', pontok.rekordpont as 'Pontok' from " +
					"megyek INNER JOIN (accounts INNER JOIN (pontok INNER JOIN jatekok ON pontok.jatekid = jatekok.id) ON accounts.id = pontok.felhid) ON megyek.id = accounts.megyeid " +
					$"WHERE jatekid = '{jatekid}' AND megyeid = '{MegyeToID[cb_megyek.SelectedItem.ToString()]}';", connect);
				MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds, "LoadDataBinding");
				dtgrd_players.DataContext = ds;
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				connect.Close();
			}
			connect.Close();

			stckpnl_leaderboard.Visibility = Visibility.Visible;

		}

		private void backtolose(object sender, RoutedEventArgs e)
		{
			stckpnl_leaderboard.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Visible;
		}

		private void MegyeChanged(object sender, SelectionChangedEventArgs e)
		{
			btn_leaderboard_load(sender, e);
		}
	}
}

