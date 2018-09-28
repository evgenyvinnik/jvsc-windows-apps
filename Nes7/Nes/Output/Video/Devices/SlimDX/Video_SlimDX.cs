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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
namespace MyNes.Nes.Output.Video
{
    public class Video_SlimDX : IGraphicDevice, IDisposable
    {
        bool _disposed;
        bool _IsRendering = false;
        bool _CanRender = true;
        int _ScanLines = 240;
        int _FirstLinesTCut = 0;
        Control _Surface;
        Texture _backBuffer;
        bool _deviceLost;
        byte[] _displayData = new byte[245760];
        int BuffSize = 229376;
        Device _displayDevice;
        Format _displayFormat;
        Rectangle _displayRect;
        Sprite _displaySprite;
        Texture _nesDisplay;
        public Direct3D d3d = new Direct3D();
        PresentParameters _presentParams;
        bool Initialized = false;
        bool _FullScreen = false;
        Vector3 _Position;
        int ModeIndex = 0;
        DisplayMode mode;

        public Video_SlimDX(TVFORMAT TvFormat, Control Surface)
        {
            if (Surface == null)
                return;
            _Surface = Surface;
            switch (TvFormat)
            {
                case TVFORMAT.NTSC:
                    _ScanLines = 224;
                    _FirstLinesTCut = 8;
                    break;
                case TVFORMAT.PAL:
                    _ScanLines = 240;
                    _FirstLinesTCut = 0;
                    break;
            }
            VideoModeSettings sett = new VideoModeSettings();
            sett.Reload();
            ApplaySettings(sett);
            //InitializeDirect3D();
        }
        void InitializeDirect3D()
        {
            try
            {
                if (!_FullScreen)
                {
                    d3d = new Direct3D();
                    _presentParams = new PresentParameters();
                    _presentParams.BackBufferWidth = 256;
                    _presentParams.BackBufferHeight = _ScanLines;
                    _presentParams.Windowed = true;
                    _presentParams.SwapEffect = SwapEffect.Discard;
                    _presentParams.Multisample = MultisampleType.None;
                    _presentParams.PresentationInterval = PresentInterval.Immediate;
                    _displayFormat = d3d.Adapters[0].CurrentDisplayMode.Format;
                    _displayDevice = new Device(d3d, 0, DeviceType.Hardware, _Surface.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { _presentParams });
                    _displayRect = new Rectangle(0, 0, 256, _ScanLines);
                    BuffSize = (256 * _ScanLines) * 4;
                    _displayData = new byte[BuffSize];
                    CreateDisplayObjects(_displayDevice);
                    Initialized = true;
                    _disposed = false;
                    MyNesDEBUGGER.WriteLine(this, "SlimDX Initialized ok.", DebugStatus.Cool);
                }
                else
                {
                    d3d = new Direct3D();
                    mode = this.FindSupportedMode();
                    _presentParams = new PresentParameters();
                    _presentParams.BackBufferFormat = mode.Format;
                    _presentParams.BackBufferCount = 1;
                    _presentParams.BackBufferWidth = mode.Width;
                    _presentParams.BackBufferHeight = mode.Height;
                    _presentParams.Windowed = false;
                    _presentParams.FullScreenRefreshRateInHertz = mode.RefreshRate;
                    _presentParams.SwapEffect = SwapEffect.Discard;
                    _presentParams.Multisample = MultisampleType.None;
                    _presentParams.PresentationInterval = PresentInterval.Immediate;
                    _displayFormat = mode.Format;
                    _displayDevice = new Device(d3d, 0, DeviceType.Hardware, _Surface.Parent.Parent.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { _presentParams });
                    _displayDevice.ShowCursor = false;
                    _displayDevice.SetRenderState(RenderState.PointScaleEnable, true);
                    _displayDevice.SetRenderState(RenderState.PointSpriteEnable, true);
                    _displayRect = new Rectangle(0, 0, 256, _ScanLines);
                    _Position = new Vector3((mode.Width - 256) / 2, (mode.Height - _ScanLines) / 2, 0);
                    BuffSize = (256 * _ScanLines) * 4;
                    _displayData = new byte[BuffSize];
                    CreateDisplayObjects(_displayDevice);
                    Initialized = true;
                    _disposed = false;
                    MyNesDEBUGGER.WriteLine(this, "SlimDX video mode Initialized ok.", DebugStatus.Cool);
                  }
            }
            catch (Exception EX)
            {
                Initialized = false; MyNesDEBUGGER.WriteLine(this, "Can't initialize SlimDX video mode, system message:\n"+EX.Message+"\nIt's recommanded that you change this video mode via video options.", DebugStatus.Error);
            }
        }
        DisplayMode FindSupportedMode()
        {
            DisplayModeCollection supportedDisplayModes = d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8);
            DisplayMode mode = supportedDisplayModes[ModeIndex];
            return mode;
        }
        private void CreateDisplayObjects(Device device)
        {
            _displaySprite = new Sprite(device);
            _backBuffer = new Texture(device, 256, _ScanLines, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.SystemMemory);
            _nesDisplay = new Texture(device, 256, _ScanLines, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
        }
        public string Name
        {
            get { return "SlimDX"; }
        }
        public string Description
        {
            get { return "Render the video using SlimDX library.\nYou can switch into fullscreen using this mode with the resolution you set in the settings of this mode. This mode is accurate and fast."; }
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
                if (Line >= _FirstLinesTCut & Line < (_ScanLines + _FirstLinesTCut))
                {
                    for (int i = 0; i < 256; i++)
                    {
                        //Extract the colors of each pixel
                        //To arrange the B,G,R,A
                        int liner = ((Line - _FirstLinesTCut) * 256) + i;
                        int BuffPos = liner * 4;
                        _displayData[BuffPos] = (byte)(ScanlineBuffer[i] & 0xFF); BuffPos++;//Blue
                        _displayData[BuffPos] = (byte)(((ScanlineBuffer[i] & 0xFF00) >> 8)); BuffPos++;//Green
                        _displayData[BuffPos] = (byte)(((ScanlineBuffer[i] & 0xFF0000) >> 0x10)); BuffPos++;//Red
                        _displayData[BuffPos] = 0xFF; BuffPos++;//Alpha
                    }
                }
            }
        }
        public unsafe void DrawPixel(int X, int Y, int Color)
        {
            if (_CanRender & _IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= _FirstLinesTCut & Y < (_ScanLines + _FirstLinesTCut))
                {
                    int liner = ((Y - _FirstLinesTCut) * 256) + X;
                    int BuffPos = liner * 4;
                    fixed (byte* pSrc = _displayData)
                    {
                        byte* ps = pSrc;
                        *((int*)(ps + BuffPos)) = Color;
                    }
                }
            }
        }
        public void BlankPixel(int X, int Y) { 
        }
        public void RenderFrame()
        {
            if (_Surface != null & _CanRender & !_disposed & Initialized)
            {
                Result result = _displayDevice.TestCooperativeLevel();
                switch (result.Code)
                {
                    case -2005530520:
                        _deviceLost = true;
                        break;

                    case -2005530519:
                        ResetDirect3D();
                        _deviceLost = false;
                        break;
                }
                if (!_deviceLost)
                {
                    this.UpdateDisplayTexture();
                    _displayDevice.BeginScene();
                    _displayDevice.Clear(ClearFlags.Target, Color.Black, 0f, 0);
                    _displaySprite.Begin(SpriteFlags.None);
                    if (_FullScreen)
                    {
                        _displaySprite.Draw(_nesDisplay, _displayRect, new Vector3(), _Position, Color.White);
                    }
                    else
                    {
                        _displaySprite.Draw(_nesDisplay, _displayRect, Color.White);
                    }
                    _displaySprite.End();
                    _displayDevice.EndScene();
                    _displayDevice.Present();
                }
                _IsRendering = false;
            }
        }
        void UpdateDisplayTexture()
        {
            if ((_displayData != null) && !_deviceLost)
            {
                DataRectangle rect = _backBuffer.LockRectangle(0, LockFlags.DoNotWait);
                rect.Data.Write(_displayData, 0, BuffSize);
                rect.Data.Close();
                _backBuffer.UnlockRectangle(0);
                if (_CanRender & !_disposed)
                    _displayDevice.UpdateTexture(_backBuffer, _nesDisplay);
            }
        }
        void ResetDirect3D()
        {
            if (!Initialized)
                return;
            this.DisposeDisplayObjects();
            try
            {
                _displayDevice.Reset(_presentParams);
                this.CreateDisplayObjects(_displayDevice);
            }
            catch
            {
                _displayDevice.Dispose();
                this.InitializeDirect3D();
            }
        }
        void DisposeDisplayObjects()
        {
            if (_displaySprite != null)
            {
                _displaySprite.Dispose();
            }
            if (_nesDisplay != null)
            {
                _nesDisplay.Dispose();
            }
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
            }
        }
        public void Dispose()
        {
            _disposed = true;
            if (_displayDevice != null)
            {
                _displayDevice.Dispose();
            }
            this.DisposeDisplayObjects();
        }
        public unsafe void TakeSnapshot(string SnapPath, string Format)
        {
            Bitmap bmp = new Bitmap(256, _ScanLines);
            BitmapData bmpData32 = bmp.LockBits(new Rectangle(0, 0, 256, _ScanLines), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            int* numPtr32 = (int*)bmpData32.Scan0;
            int j = 0;
            for (int i = (_FirstLinesTCut * 256); i < 61440 - (_FirstLinesTCut * 256); i++)
            {
                byte VALBLUE = (byte)(_displayData[j]);
                j++;
                byte VALGREEN = (byte)(_displayData[j]);
                j++;
                byte VALRED = (byte)(_displayData[j]);
                j++;
                byte VALALHA = (byte)(_displayData[j]);
                j++;
                numPtr32[i - (_FirstLinesTCut * 256)] =
                   (VALALHA << 24 |//Alpha
                   VALRED << 16 |//Red
                  VALGREEN << 8 |//Green
                  VALBLUE)//Blue
                             ;
            }
            bmp.UnlockBits(bmpData32);
            switch (Format)
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
            }
        }
        public void DrawText(string Text, int Frames)
        {

        }
        public void ChangeSettings()
        {
            Frm_SlimDXOptions Op = new Frm_SlimDXOptions(this);
            Op.ShowDialog(_Surface.Parent);
        }
        public void Clear()
        {
            //??!!
        }
        public void UpdateSize(int X, int Y, int W, int H)
        {

        }
        public bool IsSizable
        {
            get { return false; }
        }
        public bool IsRendering
        {
            get { return _IsRendering; }
        }
        public bool CanRender
        {
            get
            {
                return _CanRender;
            }
            set
            {
                _CanRender = value;
            }
        }
        public bool FullScreen
        {
            get
            {
                return _FullScreen;
            }
            set
            {
                _FullScreen = value;
                Dispose();
                InitializeDirect3D();
            }
        }
        public bool SupportFullScreen
        {
            get { return true; }
        }
        public void ApplaySettings(VideoModeSettings NewSettings)
        {
            ModeIndex = NewSettings.SlimDX_ResMode;
        }
    }
}
