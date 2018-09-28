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

namespace GlowEffect
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            GlowingEllipse.Spread = 20;
            GlowingEllipse.ShapeHeight = 100;
            GlowingEllipse.ShapeWidth = 100;
            GlowingRectangle.Spread = 20;
            GlowingRectangle.ShapeHeight = 100;
            GlowingRectangle.ShapeWidth = 100;
            GlowingRectangle.GlowColor = Colors.Red;
            GlowingEllipse.GlowColor = Colors.Red;
            GlowingEllipse.BackgroundColor = Colors.White;
            GlowingRectangle.BackgroundColor = Colors.White;
        }

        private void Apply_Click( object sender, RoutedEventArgs e )
        {
            int height = 0;
            int width = 0;
            int spread = 0;

            if( int.TryParse( txtHeight.Text, out height ) && int.TryParse( txtSpread.Text, out spread ) && int.TryParse( txtWidth.Text, out width ) )
            {
                if( height > 200 || width > 200 || spread > 100 )
                {
                    return;
                }

                GlowingEllipse.Spread = spread;
                GlowingEllipse.ShapeHeight = height;
                GlowingEllipse.ShapeWidth = width;
                GlowingRectangle.Spread = spread;
                GlowingRectangle.ShapeHeight = height;
                GlowingRectangle.ShapeWidth = width;
            }
        }

        private void btnBlack_Click( object sender, RoutedEventArgs e )
        {
            GlowingRectangle.GlowColor = Colors.Black;
            GlowingEllipse.GlowColor = Colors.Black;
        }

        private void btnRed_Click( object sender, RoutedEventArgs e )
        {
            GlowingRectangle.GlowColor = Colors.Red;
            GlowingEllipse.GlowColor = Colors.Red;
        }

        private void btnGreen_Click( object sender, RoutedEventArgs e )
        {
            GlowingRectangle.GlowColor = Colors.Green;
            GlowingEllipse.GlowColor = Colors.Green;
        }

        private void btnWhite_Click( object sender, RoutedEventArgs e )
        {
            GlowingEllipse.BackgroundColor = Colors.White;
            GlowingRectangle.BackgroundColor = Colors.White;
            LayoutRoot.Background = new SolidColorBrush( Colors.White );
        }

        private void btnYellow_Click( object sender, RoutedEventArgs e )
        {
            GlowingEllipse.BackgroundColor = Colors.Yellow;
            GlowingRectangle.BackgroundColor = Colors.Yellow;
            LayoutRoot.Background = new SolidColorBrush( Colors.Yellow );
        }

        private void btnOrange_Click( object sender, RoutedEventArgs e )
        {
            GlowingEllipse.BackgroundColor = Colors.Orange;
            GlowingRectangle.BackgroundColor = Colors.Orange;
            LayoutRoot.Background = new SolidColorBrush( Colors.Orange );
        }
    }
}
