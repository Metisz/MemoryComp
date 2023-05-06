using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MemoryComp
{
	/// <summary>
	/// Interaction logic for Szammemoria.xaml
	/// </summary>
	public partial class Szammemoria : Window
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
		private DispatcherTimer timer;
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
		
		public Szammemoria(Account ActiveAccount)
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
			timer = new DispatcherTimer();
			timer.Tick += Dt_Tick;
			NewNumber(Pont, true);
		}
		private void Dt_Tick(object sender, EventArgs e)
		{
			if (prgrss_timer.Value != 0) prgrss_timer.Value -= 1;
			else
			{
				timer.Stop();
				vwbx_Number.Visibility = Visibility.Collapsed;
				txtb_answer.IsEnabled = true;
				txtb_answer.Visibility = Visibility.Visible;
				txtb_answer.Focus();
				prgrss_timer.Visibility = Visibility.Hidden;
			}
		}

		public void NewNumber(int pont, bool newgame)
		{
			NumGame.Visibility = Visibility.Visible;
			if (newgame) timer.Interval = TimeSpan.FromMilliseconds(15);
			Number.Text = "";
			for (int i = 0; i < pont+1; i++)
			{
				Number.Text += rnd.Next(0, 10).ToString();
			}
			vwbx_Number.Visibility = Visibility.Visible;
			txtb_answer.IsEnabled = false;
			txtb_answer.Visibility = Visibility.Collapsed;
			prgrss_timer.Visibility = Visibility.Visible;
			prgrss_timer.Value = prgrss_timer.Maximum;
			timer.Start();
		}

		public void Lose()
		{
			lbl_points_earned.Content = $"{Pont}";
			if (HasAccount)
			{
				btn_leaderboard.IsEnabled = true;
				if (connect.State == ConnectionState.Closed) connect.Open();
				using (MySqlCommand HasScore = new MySqlCommand($"SELECT felhid, rekordpont FROM pontok WHERE felhid = {ActiveAccount.Userid} AND jatekid = {jatekid};", connect))
				{
					using (MySqlDataReader reader = HasScore.ExecuteReader())
					{
						if (reader.HasRows)
						{
							reader.Read();
							if (Pont > reader.GetInt32(1))
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
			NumGame.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Visible;
		}

		

		private void btn_restart_Click(object sender, RoutedEventArgs e)
		{
			lbl_newrecord.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Hidden;
			Pont = 0;
			NewNumber(pont, true);
		}

		private void btn_quit_Click(object sender, RoutedEventArgs e)
		{
			MainWindow Menu = new MainWindow();
			this.Close();
			Menu.ShowDialog();
		}

		private void PressedEnter(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (txtb_answer.Text == Number.Text)
				{
					++Pont;
					timer.Interval = TimeSpan.FromMilliseconds(15 + Pont * 2);
					NewNumber(Pont, false);
				}
				else Lose();
				txtb_answer.Text = "";
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

