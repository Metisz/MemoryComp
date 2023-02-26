using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for Jatek.xaml
	/// </summary>
	public partial class Jatek : Window
	{

		public List<Button> gombok = new List<Button>();
		public Random rnd = new Random();
		private int pont = 0;
		public string Pont
		{
			get
			{
				return pont.ToString();
			}
			set
			{
				pont = Convert.ToInt32(value);
			}
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


		public Jatek()
		{
			InitializeComponent();
			DataContext = this;
			gbox_lose.Visibility = Visibility.Hidden;
			AddButton(pont+1);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).IsEnabled = false;
			if ((sender as Button) != gombok.First())
			{
				if (gombok[int.Parse((sender as Button).Tag.ToString()) - 1].IsEnabled == true)
				{
					Grid.Children.Clear();
					lbl_points_earned.Content = $"{Pont}";
					gbox_lose.Visibility = Visibility.Visible;

					//this.Close();
					MessageBox.Show($"Összpont: {pont}", "Aztacsuhajjamegamindenségit");
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

		//	//Az INotifyPropertyChange-s dolgok
	
		//	public event PropertyChangedEventHandler PropertyChanged;
		//	public void RaisePropertyChange(string propertyname)
		//	{
		//		if (PropertyChanged != null)
		//		{
		//			PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
		//		}
		//	}
	
		//}
		//internal interface INotifyPropertyChange
		//{
	}
}
