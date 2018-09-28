using System.Windows.Media;

namespace Love
{
	public partial class Letter
	{
		
		public Letter(char letter)
		{
			InitializeComponent();
			LetterTextBlock.Text = letter.ToString();
		}

		public bool Placed { get; set; }
		public int Placing { get; set; }

		public Brush Fill
		{
			get { return HeartArea.Fill; }
			set { HeartArea.Fill = value; }
		}

	}
}
