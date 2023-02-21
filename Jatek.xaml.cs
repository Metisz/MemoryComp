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
	public partial class Jatek : Window, INotifyPropertyChange
	{

		public List<Button> gombok = new List<Button>();
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
				pont = value;
				RaisePropertyChange("Pont");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;



		void AddButton(int i)
		{
			gombok = new List<Button>();
			for (int j = 0; j < i + 1; j++)
			{
				gombok.Add(new Button());
				gombok[j] = new Button()
				{
					Content = Convert.ToString(j + 1),
					Tag = j
				};
				gombok[j].Click += new RoutedEventHandler(Button_Click);
				gombok[j].Width = 50;
				gombok[j].Height = 50;
				gombok[j].VerticalAlignment = VerticalAlignment.Top; gombok[j].HorizontalAlignment = HorizontalAlignment.Left;
				gombok[j].Margin = new Thickness(rnd.Next(1, 550), rnd.Next(1, 550), 0, 0);
				this.Grid.Children.Add(gombok[j]);
			}
		}


		public Jatek()
		{
			InitializeComponent();
			AddButton(pont);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).IsEnabled = false;
			if ((sender as Button) != gombok.First())
			{
				if (gombok[int.Parse((sender as Button).Tag.ToString()) - 1].IsEnabled == true)
				{
					this.Close();
					MessageBox.Show($"Összpont: {pont}", "Aztacsuhajjamegamindenségit");
				}
			}
			if ((sender as Button) == gombok.Last())
			{
				pont++;
				Grid.Children.Clear();
				AddButton(pont);
			}
		}


		public void RaisePropertyChange(string propertyname)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
			}
		}

	}
	internal interface INotifyPropertyChange
	{
	}
}
