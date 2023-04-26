using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace MemoryComp
{
	/// <summary>
	/// Interaction logic for Csimpanz.xaml
	/// </summary>
	public partial class Csimpanz : Window
	{
		int jatekid = 1;
		MySqlConnection connect = new MySqlConnection(
			"server = localhost; userid = root; password = ; database = MemoryComp"
			);
		private bool HasAccount;
		public List<Button> gombok = new List<Button>();
		private Account ActiveAccount;
		public Random rnd = new Random();
		private int pont = 0;
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

		public void Lose()
		{
			Grid.Children.Clear();
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
			stckpnl_lose.Visibility = Visibility.Visible;
		}

		void AddButton(int i) // i = a ponttal amit előtte elért
		{
			//Csinál egy listát, amiben i + 1 mennyiségű gomb van
			gombok = new List<Button>();
			for (int j = 0; j < i + 1; j++)
			{
				gombok.Add(new Button());
				gombok[j] = new Button()
				{
					Content = Convert.ToString(j + 1),
					//A gombok tagje egyenlő a listabeli indexükkel
					Tag = j
				};
				//Mindegyik a Button_Click eseményt használja
				gombok[j].Click += new RoutedEventHandler(Button_Click);
				gombok[j].Width = 50;
				gombok[j].Height = 50;
				gombok[j].VerticalAlignment = VerticalAlignment.Top; gombok[j].HorizontalAlignment = HorizontalAlignment.Left;
				//Random bal és felső margóval lehet random helyet adni nekik
				gombok[j].Margin = new Thickness(rnd.Next(1, 750), rnd.Next(1, 550), 0, 0);
				//Fel is rakja a gridre
				this.Grid.Children.Add(gombok[j]);
			}
		}


		public Csimpanz(Account ActiveAccount)
		{ 
			InitializeComponent();
			if (ActiveAccount == null) { HasAccount = false; }
            else {
				HasAccount = true;
				this.ActiveAccount = ActiveAccount;	
            }
			DataContext = this;
			stckpnl_lose.Visibility = Visibility.Hidden;
			AddButton(pont+1);
		}


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).IsEnabled = false;
			if ((sender as Button) != gombok.First())
			{
				if (gombok[int.Parse((sender as Button).Tag.ToString()) - 1].IsEnabled == true)
				{
					Lose();

					//this.Close();
					//MessageBox.Show($"Összpont: {pont}", "Aztacsuhajjamegamindenségit");
				}
			}
			else
			{
				foreach (var item in gombok)
				{
					item.Content = "";
				}
			}
			if ((sender as Button) == gombok.Last() && gombok[int.Parse((sender as Button).Tag.ToString()) - 1].IsEnabled == false)
			{
				pont++;
				Grid.Children.Clear();
				AddButton(pont+1);
			}
		}

		private void btn_restart_Click(object sender, RoutedEventArgs e)
		{
			lbl_newrecord.Visibility = Visibility.Hidden;
			stckpnl_lose.Visibility = Visibility.Hidden;
			Pont = 0;
			AddButton(pont + 1);
		}

		private void btn_quit_Click(object sender, RoutedEventArgs e)
		{
			MainWindow Menu = new MainWindow();
			this.Close();
			Menu.ShowDialog();
		}

	}
}
