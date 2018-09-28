/*********************************************************************\
*This file is part of My Nes                                          *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2009 - 2010 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My Nes is free software: you can redistribute it and/or modify       *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My Nes is distributed in the hope that it will be useful,            *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with this program.  If not, see <http://www.gnu.org/licenses/>.*
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace MyNes.Nes.Output.Video
{
	public class Video_GDI : IGraphicDevice
	{
		bool _IsRendering = false;
		bool _CanRender = true;
		/*Bitmap bmp;
		Control _Surface;
		Graphics GR;
		BitmapData bmpData;*/
		int Screen_W = 0;
		int Screen_H = 0;
		int Screen_X = 0;
		int Screen_Y = 0;
		string TextToRender = "";
		int TextApperance = 200;
		int ScanlinesToCut = 0;
		int _Scanlines = 0;
		//int* numPtr;
		int[] Buffer;

		public Video_GDI(TVFORMAT TvFormat/*, Control Surface*/)
		{
			/*if (Surface == null)
				return;
			switch (TvFormat)
			{
				case TVFORMAT.NTSC:
					ScanlinesToCut = 8;
					_Scanlines = 224;
					bmp = new Bitmap(256, 224);
					break;
				case TVFORMAT.PAL:
					ScanlinesToCut = 0;
					_Scanlines = 240;
					bmp = new Bitmap(256, 240);
					break;
			}
			Buffer = new int[256 * _Scanlines];
			_Surface = Surface;
			UpdateSize(0, 0, _Surface.Width + 1, _Surface.Height + 1);*/
		}
		public string Name
		{
			get { return "Windows GDI"; }
		}
		public string Description
		{
			get { return "Render the video using System.Drawing\nYou can not switch to fullscreen with this mode. This mode is perfect (best quality) and accurate but needs pc power and not recommanded for old pcs."; }
		}
		public void Begin()
		{
			if (_CanRender)
			{
				_IsRendering = true;
			}
		}
		public void AddScanline(int Line, int[] ScanlineBuffer)
		{
			if (_CanRender & _IsRendering)
			{
				//Check if we should cut this line 
				if (Line > ScanlinesToCut & Line < (_Scanlines + ScanlinesToCut))
				{
					int liner = ((Line - ScanlinesToCut) * 256);
					//Set the scanline into the buffer
					for (int i = 0; i < 256; i++)
						Buffer[liner + i] = ScanlineBuffer[i];
				}
			}
		}
		public void DrawPixel(int X, int Y, int Color)
		{
			if (_CanRender & _IsRendering)
			{
				//Check if we should cut this line 
				if (Y >= ScanlinesToCut & Y < (_Scanlines + ScanlinesToCut))
				{
					int liner = ((Y - ScanlinesToCut) * 256) + X;
					Buffer[liner] = Color;
					liner++;
				}
			}
		}
		public void BlankPixel(int X, int Y)
		{
		}
		public void RenderFrame()
		{
			if (_CanRender & _IsRendering)
			{
				/*bmpData = bmp.LockBits(new Rectangle(0, 0, 256, _Scanlines),
					ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
				numPtr = (int*)bmpData.Scan0;
				for (int i = 0; i < Buffer.Length; i++)
				{
					numPtr[i] = Buffer[i];
				}
				bmp.UnlockBits(bmpData);
				//Draw it !!
				GR.DrawImage(bmp, 0, 0, _Surface.Size.Width, _Surface.Size.Height);
				//Draw the text
				if (TextApperance > 0)
				{
					GR.DrawString(TextToRender, new System.Drawing.Font("Tohama", 16, FontStyle.Bold),
					   new SolidBrush(Color.White), new PointF(30, _Surface.Height - 50));
					TextApperance--;
				}*/
			}
			_IsRendering = false;
		}
		public void TakeSnapshot(string SnapPath, string Format)
		{
			/*switch (Format)
			{
				case ".bmp":
					bmp.Save(SnapPath, ImageFormat.Bmp);
					_CanRender = true;
					break;
				case ".gif":
					bmp.Save(SnapPath, ImageFormat.Gif);
					_CanRender = true;
					break;
				case ".jpg":
					bmp.Save(SnapPath, ImageFormat.Jpeg);
					_CanRender = true;
					break;
				case ".png":
					bmp.Save(SnapPath, ImageFormat.Png);
					_CanRender = true;
					break;
				case ".tiff":
					bmp.Save(SnapPath, ImageFormat.Tiff);
					_CanRender = true;
					break;
			}*/
		}
		public void DrawText(string Text, int Frames)
		{
			TextToRender = Text;
			TextApperance = Frames;
			//This draw is useful when the nes is paused.
			/*if (_Surface != null & !_IsRendering)
			{
				if (TextApperance > 0)
				{
					Graphics GR = _Surface.CreateGraphics();
					GR.DrawString(TextToRender, new System.Drawing.Font("Tohama", 16, FontStyle.Bold),
						new SolidBrush(Color.White), new PointF(30, _Surface.Height - 50));
					TextApperance--;
				}
			}*/
		}
		public void ChangeSettings()
		{
			MessageBox.Show("Currently there's no settings for this mode.");
		}
		public void Clear()
		{
			//GR.Clear(Color.Black);
		}
		public void UpdateSize(int X, int Y, int W, int H)
		{
			Screen_W = W;
			Screen_H = H;
			Screen_X = X;
			Screen_Y = Y;
			//GR = _Surface.CreateGraphics();
			//GR.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			//GR.Clear(Color.Black);
		}
		public bool IsSizable
		{ get { return true; } }
		public bool IsRendering
		{
			get { return _IsRendering; }
		}
		public bool CanRender
		{
			get { return _CanRender; }
			set { _CanRender = value; }
		}
		public bool FullScreen
		{
			get
			{
				return false;
			}
			set
			{

			}
		}
		public bool SupportFullScreen
		{
			get { return false; }
		}
	}
}
