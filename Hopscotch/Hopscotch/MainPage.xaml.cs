using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Hopscotch
{
	public partial class MainPage : PhoneApplicationPage
	{
		Random rnd = new Random();

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			int count = 20;

			while (count-- > 0)
			{
				AddItem();
			}
		}

		private void AddItem()
		{
			Border b = new Border()
			{
				Width = 100,
				Height = 100,
				Background = new SolidColorBrush(Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256))),
				BorderThickness = new Thickness(2),
				Margin = new Thickness(8)
			};

			b.BorderBrush = (SolidColorBrush)Resources["PhoneForegroundBrush"];

			GestureListener listener = GestureService.GetGestureListener(b);
			listener.Tap += new EventHandler<GestureEventArgs>(WrapPanelSample_Tap);

			wrapPanel.Children.Add(b);
		}

		void WrapPanelSample_Tap(object sender, GestureEventArgs e)
		{
			Border b = (Border)sender;
			wrapPanel.Children.Remove(b);
		}
	}
}