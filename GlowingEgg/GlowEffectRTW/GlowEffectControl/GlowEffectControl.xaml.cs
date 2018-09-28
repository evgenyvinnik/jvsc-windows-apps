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

namespace GlowEffectControl
{
    public partial class GlowEffectControl : UserControl
    {
        private GlowShapes shape = GlowShapes.Rectangular;
        private double shapeWidth = 0;
        private double shapeHeight = 0;
        private Color glowColor = Color.FromArgb( 255, 255, 255, 255 );
        private Color backgroundColor = Color.FromArgb( 255, 255, 255, 255 );
        private double spread = 0;

        public GlowShapes Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
                this.AdjustSize();
            }
        }

        public double ShapeHeight
        {
            get
            {
                return shapeHeight;
            }
            set
            {
                shapeHeight = value;
                this.AdjustSize();
            }
        }

        public double ShapeWidth
        {
            get
            {
                return shapeWidth;
            }
            set
            {
                shapeWidth = value;
                this.AdjustSize();
            }
        }

        public Color GlowColor
        {
            get
            {
                return glowColor;
            }
            set
            {
                glowColor = value;
                this.TopLeftPrimary.Color = value;
                this.TopPrimary.Color = value;
                this.TopRightPrimary.Color = value;
                this.MiddleRightPrimary.Color = value;
                this.MiddleLeftPrimary.Color = value;
                this.BottomLeftPrimary.Color = value;
                this.BottomRightPrimary.Color = value;
                this.BottomPrimary.Color = value;
                this.Middle.Fill = new SolidColorBrush( value );
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                glowColor = value;
                this.TopLeftSecondary.Color = value;
                this.TopSecondary.Color = value;
                this.TopRightSecondary.Color = value;
                this.MiddleRightSecondary.Color = value;
                this.MiddleLeftSecondary.Color = value;
                this.BottomLeftSecondary.Color = value;
                this.BottomRightSecondary.Color = value;
                this.BottomSecondary.Color = value;
            }
        }

        public double Spread
        {
            get
            {
                return spread;
            }
            set
            {
                spread = value;
                this.AdjustSize();
            }
        }

        public GlowEffectControl()
        {
            InitializeComponent();
        }

        private void AdjustSize()
        {
            switch( this.Shape )
            {
                case GlowShapes.Rectangular:
                    this.TopLeft.Height = this.Spread;
                    this.TopLeft.Width = this.Spread;
                    this.TopRight.Height = this.Spread;
                    this.TopRight.Width = this.Spread;
                    this.Top.Height = this.Spread;
                    this.Top.Width = this.ShapeWidth;
                    this.MiddleLeft.Width = this.Spread;
                    this.MiddleLeft.Height = this.ShapeHeight;
                    this.MiddleRight.Width = this.Spread;
                    this.MiddleRight.Height = this.ShapeHeight;
                    this.Middle.Height = this.ShapeHeight;
                    this.Middle.Width = this.ShapeWidth;
                    this.BottomLeft.Height = this.Spread;
                    this.BottomLeft.Width = this.Spread;
                    this.BottomRight.Height = this.Spread;
                    this.BottomRight.Width = this.Spread;
                    this.Bottom.Width = this.ShapeWidth;
                    this.Bottom.Height = this.Spread;
                    break;
                case GlowShapes.Oval:
                    this.TopLeft.Height = this.Spread + ( this.ShapeHeight / 2 );
                    this.TopLeft.Width = this.Spread + ( this.ShapeWidth / 2 );
                    this.TopRight.Height = this.Spread + ( this.ShapeHeight / 2 );
                    this.TopRight.Width = this.Spread + ( this.ShapeWidth / 2 );
                    this.Top.Height = 0;
                    this.Top.Width = 0;
                    this.MiddleLeft.Width = 0;
                    this.MiddleLeft.Height = 0;
                    this.MiddleRight.Width = 0;
                    this.MiddleRight.Height = 0;
                    this.Middle.Height = 0;
                    this.Middle.Width = 0;
                    this.BottomLeft.Height = this.Spread + ( this.ShapeHeight / 2 );
                    this.BottomLeft.Width = this.Spread + ( this.ShapeWidth / 2 );
                    this.BottomRight.Height = this.Spread + ( this.ShapeHeight / 2 );
                    this.BottomRight.Width = this.Spread + ( this.ShapeWidth / 2 );
                    this.Bottom.Width = 0;
                    this.Bottom.Height = 0;

                    double offset = 0;

                    if( this.ShapeWidth != 0 && this.Spread != 0 )
                        offset = this.ShapeWidth / ( 2 * this.Spread + this.ShapeWidth );

                    this.TopLeftPrimary.Offset = offset;
                    this.TopRightPrimary.Offset = offset;
                    this.BottomLeftPrimary.Offset = offset;
                    this.BottomRightPrimary.Offset = offset;
                    break;

            }

            this.LayoutRoot.UpdateLayout();
        }
    }

    public enum GlowShapes
    {
        Rectangular,
        Oval
    }
}
