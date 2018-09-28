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

namespace Dodge
{
	public partial class MainPage : PhoneApplicationPage
	{
		#region variables

		//private Point position;

		//private string level;
		//private string score;
		//private string time;

		//private DateTime startTime;

		//private bool t = false;

		#region point class
		public class SinPoint
		{
			public double x;
			public double y;

			public SinPoint(double h, double r)
			{
				x = h;
				y = r;
			}

			public double distanceTo(SinPoint point)
			{
				var r = point.x - x;
				var h = point.y - y;
				return Math.Sqrt(r * r + h * h);
			}

			public void clonePosition(ref SinPoint point)
			{
				point.x = x;
				point.y = y;
			}
		}
		#endregion

		#region region class
		public class Region
		{
			public int top;
			public int left;
			public int bottom;
			public int right;

			public Region()
			{
				reset();
			}

			public void reset()
			{
				top = left = int.MaxValue;
				bottom = right = 0;
			}

			public void inflate (int h, int r)
			{
				left = Math.Min(left, h);
				top = Math.Min(top, r);
				right = Math.Max(right, h);
				bottom = Math.Max(bottom, r);
			}
		}
		#endregion

		#region SinuousWorld class
		public class SinuousWorld
		{
			public int pa;
			public int qa;
			public int E;
			public double sa;
			public int ha;
			public int ca;
			public int ta;

			public bool t;

			public FClass[] F;

			public mClass m;
			public int s;

			public SinuousWorld()
			{
				pa = 800;
				qa = 480;
				E = 60;
				sa = 0.25;
				ha = 2;
				ca = 3;
				ta = 120;

				F = new FClass[9];
				F[0] = new FClass(1.2, 100, 0.5);
				F[1] = new FClass(1.4, 200, 0.6);
				F[2] = new FClass(1.6, 300, 0.7);
				F[3] = new FClass(1.8, 450, 0.8);
				F[4] = new FClass(2.0, 600, 0.1);
				F[5] = new FClass(2.4, 800, 1.1);
				F[6] = new FClass(2.9, 1000, 1.3);
				F[7] = new FClass(3.5, 1300, 1.7);
				F[8] = new FClass(4.8, 2000, 2);

				m = new mClass(1, 1, false);
				s = 1;

				t = false;
			}

			void Ga()
			{
				if (t == false)
				{
					t = true;

				}
			}

			//saves parameters
			void h()
			{
				//save m.unlockedLevels;
				//save m.selectedLevel;
				//save m.mute;
			}

			void r()
			{
				if (m.mute)
				{
					//mute
				}
				else
				{
					//unmute
				}
			}

			void W()
			{
				m.mute = !m.mute;
				r();
				h();
			}

			void Da()
			{
				//set local storage unlocked levels to null
				//set local storage selected level to null
				m.unlockedLevels = 1;
				m.selectedLevel = 1;
				s = 1;
				//TODO L();
			}

			#region q class
			class qClass
			{
				public 
			}
			#endregion
			#region m class
			public class mClass
			{
				public int unlockedLevels;
				public int selectedLevel;
				public bool mute;

				public mClass(int unlockedLevels, int selectedLevel, bool mute)
				{
					this.unlockedLevels = unlockedLevels;
					this.selectedLevel = selectedLevel;
					this.mute = mute;
				}
			}
			#endregion
			#region F class
			public class FClass
			{
				public double factor;
				public double duration;
				public double multiplier;

				public FClass(double factor, double duration, double multiplier)
				{
					this.factor = factor;
					this.duration = duration;
					this.multiplier = multiplier;
				}
			}
			#endregion

			#region ga class
			public class ga
			{
				public SinPoint gaPosition;
				public int[] trail;
				public int size;
				public int shield;
				public int lives;
				public int flicker;
				public bool gravitywarped;
				public int gravityfactor;
				public bool timewarped;
				public int timefactor;
				public bool sizewarped;
				public int sizefactor;

				public ga()
				{
					gaPosition = new SinPoint(0, 0);
					size = 8;
					shield = 0;
					lives = 2;
					gravitywarped = false;
					gravityfactor = 0;
					timewarped = false;
					timefactor = 0;
					sizewarped = false;
					sizefactor = 0;
				}

				public bool isBoosted()
				{
					return shield != 0 || gravityfactor != 0;
				}
			}
			#endregion

			#region wa class
			public class wa
			{
				public SinPoint position;
				public SinPoint offset;
				public int originalSize;
				public int size;
				public double force;

				public wa()
				{
					var random = new Random();

					position = new SinPoint(0, 0);
					offset = new SinPoint(0, 0);
					originalSize = size = (int) (6 + random.NextDouble() * 4);
					force = 1 + random.NextDouble() * 0.4;
				}
			}
			#endregion

			#region fa class
			public class fa
			{
				public string type;
				public SinPoint position;
				public int size;
				public double force;

				public fa()
				{
					var random = new Random();

					position = new SinPoint(0, 0);
					size = (int) ( 20 + random.NextDouble() * 4 );
					force = 0.8 + random.NextDouble() * 0.4;

					type = randomizeType();
				}

				static string randomizeType()
				{
					var random = new Random();
					var xa = new string[] { "shield", "shield", "life", "gravitywarp", "gravitywarp", "timewarp", "sizewarp" };
					var element = (int)Math.Round(random.NextDouble() * xa.Length - 1);
					return xa[element];
				}
			}
			#endregion
		}
		#endregion
		#endregion

		// Constructor
		public MainPage()
		{
			InitializeComponent();
		}


	}
}