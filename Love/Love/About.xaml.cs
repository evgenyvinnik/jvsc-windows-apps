using System;
using System.Windows;

namespace Love
{
	public partial class About
	{
		public About()
		{
			InitializeComponent();
		}

		private void BackButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}
	}
}