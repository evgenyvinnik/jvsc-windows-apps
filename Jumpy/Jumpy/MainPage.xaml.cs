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
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace Jumpy
{
	public partial class MainPage : PhoneApplicationPage
	{
		#region

		private int screen_height = 150;
		private int screen_width = 800;
		private int canvas_width = 800;
		private int time = 150;
		private int last_height = 800;
		private int unit = 50;
		private bool dead = true;
		private int[] heights;
		private int score = 0;
		private int i;
		private int x;
		private int y = 0;
		private Random random;
		private int plant_pos;
		private bool jump = false;
		private int speed_x = 0;
		private int speed_y = 9;
		private string scoreString;
		private int ground;
		private int player_index;
		//loop
		private DispatcherTimer _moverThread;
		#endregion
		// Constructor
		public MainPage()
		{
			InitializeComponent();
			random = new Random();
			heights = new int[0x1e4];
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			_moverThread = new DispatcherTimer();
			_moverThread.Interval = TimeSpan.FromMilliseconds(30);
			_moverThread.Tick += MoverThreadTick;
			_moverThread.Start();
		}

		void MoverThreadTick(object sender, EventArgs e)
		{
			if (dead)
			{
				i = 0;
				x = 405;
				score = 0;
				y = 0;
				while (i < 0x1e4)
				{
					var check = ( i < 9 ) | ( last_height < screen_width ) & ( random.NextDouble() < 0.3 );
					last_height = heights[i++] = (check ? screen_width : ( (int)(random.NextDouble() * unit) + 80 ) ) | 0;
				}
			}

			plant_pos = ++time % 99 - unit;
			plant_pos = plant_pos * plant_pos / 8 + 20;

			y += speed_y;
			x += y - heights[( x + speed_x ) / unit | 0] > 9 ? 0 : speed_x;
			player_index = x / unit | 0;
			ground = heights[player_index];

			var check1 = y < ground | speed_y < 0;
			if(check1)
			{
				speed_y = speed_y + 1;
			}
			else
			{
				y = ground;
				speed_y = jump ? -10 : 0;
			}

			for (i = 0; i < 21; i++ )
			{
				if(i < 7)
				{
					Arc(i % 6, screen_width / 2, 235, i ? 250 - 15 * i : screen_width);
				}

				if (dead)
					break;
			}

			if (y > screen_height)
			{
				dead = true;
			}
			else
			{
				dead = false;
			}
		}

		void Arc(int color, int x, int y, int radius)
		{
			//radius&&
		}
	}
}