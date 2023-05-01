using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MemoryComp
{
	/// <summary>
	/// Interaction logic for Szekvencia.xaml
	/// </summary>
	public partial class Szekvencia : Window
	{
		int jatekid = 3;
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		private bool HasAccount;
		private Account ActiveAccount;
		public Random rnd = new Random();
		private int pont = 0;
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

		public Szekvencia(Account ActiveAccount)
		{
			InitializeComponent();
			if (ActiveAccount == null) { HasAccount = false; }
			else
			{
				HasAccount = true;
				this.ActiveAccount = ActiveAccount;
			}
			stckpnl_lose.Visibility = Visibility.Hidden;
			timer = new DispatcherTimer();
			timer.Tick += Dt_Tick;
			NewNumber(Pont, true);
		}
		private void Dt_Tick(object sender, EventArgs e)
		{
			
		}

		public void Lose()
		{
			lbl_points_earned.Content = $"{Pont}";
			if (HasAccount)
			{
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
							MessageBox.Show("jej");
						}
					}
				}
				connect.Close();
			}
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
	}
}
