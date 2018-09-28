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
using System.Windows.Forms;
using System.Drawing.Imaging;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using System.Runtime.InteropServices;


namespace MyNes.Nes.Output.Video
{

    class bitmapF
    {
        public int bitmapID;
        public int color1;
        public int color2;
        public int color3;
        public int x;
        public int y;
    }

    class TileData
    {
        public List<bitmapF> bitmapP;
        public int defaultID;
    }

    public unsafe class Video_HiRes : IGraphicDevice, IDisposable
    {
        bool _disposed;
        bool _IsRendering = false;
        bool _CanRender = true;
        int _ScanLines = 240;
        int _FirstLinesTCut = 0;
        Control _Surface;
        Texture _backBuffer;
        bool _deviceLost;
        byte[] _displayData = new byte[983040];
        int BuffSize = 917504;
        Device _displayDevice;
        Format _displayFormat;
        Rectangle _displayRect;
        Sprite _displaySprite;
        Texture _nesDisplay;
        public Direct3D d3d = new Direct3D();
        PresentParameters _presentParams;
        bool Initialized = false;
        PPU oppu;

        //graphics pack data
        bool hasHiResPack = false;
        List<Bitmap> bitmaps;
        List<byte[]> rawBits;
        List<TileData> tdata;
        int[][] packData;

        //snapshot and log 
        StreamWriter log;
        bool highResSnap = false;
        bool takingSnap = false;
        string snapfile;
        string snapformat;

        int lastTileNo;
        RenderLineState lastLineState;
        int[] tilePalette;
        bool usePack;
        int packID;
        int[] tileRow;
        TileData t;
        int lastTileOffSet;
        int lastVirtualScanline;
        int lastPackOffSet;
        int lastPackID;

        public Video_HiRes(TVFORMAT TvFormat, Control Surface, string pPath, int chrPages, PPU pp)
        {

            if (Surface == null)
                return;
            _Surface = Surface;
            oppu = pp;
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

            if (pPath != "") ReadHiResPack(Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pPath), chrPages);
            tilePalette = new int[3];
            tileRow = new int[8];
            MyNesDEBUGGER.WriteLine(this, "Video device " + @"""" + "Hi Res" + @"""" + " Ok !!", DebugStatus.Cool);
        }
        void InitializeDirect3D()
        {
            try
            {
                d3d = new Direct3D();
                _presentParams = new PresentParameters();
                _presentParams.BackBufferWidth = 512;
                _presentParams.BackBufferHeight = _ScanLines * 2;
                _presentParams.Windowed = true;
                _presentParams.SwapEffect = SwapEffect.Discard;
                _presentParams.Multisample = MultisampleType.None;
                _presentParams.PresentationInterval = PresentInterval.Immediate;
                _displayFormat = d3d.Adapters[0].CurrentDisplayMode.Format;
                _displayDevice = new Device(d3d, 0, DeviceType.Hardware, _Surface.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { _presentParams });
                _displayRect = new Rectangle(0, 0, 512, _ScanLines * 2);
                BuffSize = (512 * _ScanLines * 2) * 4;
                _displayData = new byte[BuffSize];
                CreateDisplayObjects(_displayDevice);
                Initialized = true;
                _disposed = false;
                MyNesDEBUGGER.WriteLine(this, "SlimDX video mode (Windowed) Initialized ok.", DebugStatus.Cool);

            }
            catch (Exception EX)
            { MyNesDEBUGGER.WriteLine(this, "Could not Initialize SlimDX mode because of : \n" + EX.Message, DebugStatus.Error); Initialized = false; }
        }
        private void CreateDisplayObjects(Device device)
        {
            _displaySprite = new Sprite(device);
            _backBuffer = new Texture(device, 512, _ScanLines * 2, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.SystemMemory);
            _nesDisplay = new Texture(device, 512, _ScanLines * 2, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
        }
        void ReadHiResPack(string pPath, int chrPages)
        {
            byte[] bits;
            Bitmap tBmp;
            Bitmap tBmp2;
            Graphics g;
            GCHandle handle;


            if (Directory.Exists(pPath))
            {
                if (File.Exists(pPath + Path.DirectorySeparatorChar.ToString() + "hires.txt"))
                {

                    packData = new int[chrPages * 8][];
                    for (int i = 0; i < chrPages * 8; i++)
                    {
                        packData[i] = new int[1024];
                        for (int j = 0; j < 1024; j++)
                        {
                            packData[i][j] = -1;
                        }
                    }
                    bitmaps = new List<Bitmap>();
                    tdata = new List<TileData>();
                    rawBits = new List<byte[]>();

                    StreamReader r = new StreamReader(pPath + Path.DirectorySeparatorChar.ToString() + "hires.txt");
                    while (!r.EndOfStream)
                    {
                        String s = r.ReadLine();
                        if (s.StartsWith("<img>"))
                        {
                            //read file
                            tBmp = new Bitmap(pPath + Path.DirectorySeparatorChar.ToString() + s.Substring(5));

                            //create direct access
                            bits = new byte[tBmp.Width * tBmp.Height * 4];
                            handle = GCHandle.Alloc(bits, GCHandleType.Pinned);
                            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
                            tBmp2 = new Bitmap(tBmp.Width, tBmp.Height, tBmp.Width * 4, tBmp.PixelFormat, pointer);

                            g = Graphics.FromImage(tBmp2);
                            //'source' is the source bitmap
                            g.DrawImageUnscaledAndClipped(tBmp, new Rectangle(0, 0, tBmp.Width, tBmp.Height));
                            g.Dispose();

                            tBmp.Dispose();

                            bitmaps.Add(tBmp2);
                            rawBits.Add(bits);

                        }
                        else if (s.StartsWith("<tile>"))
                        {
                            String[] d = s.Substring(6).Split(',');
                            if (d.Count() == 9)
                            {
                                if (packData[int.Parse(d[0])][int.Parse(d[1])] == -1)
                                {
                                    packData[int.Parse(d[0])][int.Parse(d[1])] = tdata.Count;
                                    tdata.Add(new TileData());
                                    tdata.Last().bitmapP = new List<bitmapF>();
                                    tdata.Last().defaultID = -1;
                                }
                                bitmapF b = new bitmapF();
                                b.bitmapID = int.Parse(d[2]);
                                b.color1 = int.Parse(d[3]);
                                b.color2 = int.Parse(d[4]);
                                b.color3 = int.Parse(d[5]);
                                b.x = int.Parse(d[6]);
                                b.y = int.Parse(d[7]);
                                if (d[8] == "Y") tdata[packData[int.Parse(d[0])][int.Parse(d[1])]].defaultID = tdata[packData[int.Parse(d[0])][int.Parse(d[1])]].bitmapP.Count();
                                tdata[packData[int.Parse(d[0])][int.Parse(d[1])]].bitmapP.Add(b);
                            }
                        }
                    }
                    r.Close();
                    if (chrPages > 0) hasHiResPack = true;
                }
            }
        }
        public string Name
        {
            get { return "Hi Res"; }
        }
        public string Description
        {
            get { return "Render with double resolution and uses hi res graphics packs."; }
        }
        public unsafe void Begin()
        {
            if (_CanRender)
            {
                _IsRendering = true;
                lastLineState = RenderLineState.RenderClipping;
                if (highResSnap)
                {
                    log = new StreamWriter(snapfile + "log.txt");
                    takingSnap = true;
                }
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
                        int BuffPos = ((Line - _FirstLinesTCut) * 4096) + i * 8;
                        fixed (byte* pSrc = _displayData)
                        {
                            byte* ps = pSrc;
                            ps += BuffPos;
                            *((int*)ps) = ScanlineBuffer[i];
                            ps += 4;
                            *((int*)ps) = ScanlineBuffer[i];
                            ps += 2048;
                            *((int*)ps) = ScanlineBuffer[i];
                            ps -= 4;
                            *((int*)ps) = ScanlineBuffer[i];
                        }
                    }
                }
            }
        }

        private void RenderTile(int X, int Y) {

            if (lastTileNo != oppu.tilenumber) {
                lastTileOffSet = oppu.tileDataOffset & 0x0003ff;
                lastVirtualScanline = -1;
                lastPackID = -1;
                //decode tile color data
                for(int i = 0; i < 8; i++){
                    tileRow[i] = (((oppu.tiledata2 & (1 << (i))) >> (i)) << 1) + ((oppu.tiledata1 & (1 << (i))) >> (i));
                }
                //read tile palette
                if (lastLineState == RenderLineState.RenderSprite)
                {
                    tilePalette[0] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 1)];
                    tilePalette[1] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 2)];
                    tilePalette[2] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 3)];
                }
                else {
                    tilePalette[0] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 1)];
                    tilePalette[1] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 2)];
                    tilePalette[2] = oppu.MEM_PPU[(ushort)(0x3F10 + oppu.PaletteUpperBits + 3)];                
                }
                //look for valid tile pack
                usePack = false;
                packID = -1;
                if (packData[oppu.tilepage][lastTileOffSet] != -1)
                {
                    usePack = true;
                    t = tdata[packData[oppu.tilepage][lastTileOffSet]];
                    for (int idx = 0; idx < t.bitmapP.Count; idx++)
                    {
                        if (t.bitmapP[idx].color1 == tilePalette[0] && t.bitmapP[idx].color2 == tilePalette[1] && t.bitmapP[idx].color3 == tilePalette[2]) packID = idx;
                    }
                    if (packID == -1) packID = t.defaultID;
                }
                lastTileNo = oppu.tilenumber;
            }
            //do snap 
            if (takingSnap)
            {
                if (oppu.virtualScanline == 0 && oppu.virtualColumn == 0)
                {
                    log.WriteLine("Page = " + oppu.tilepage.ToString() + ", Offset = " + lastTileOffSet.ToString() + ", X = " + X.ToString() + ", Y = " + Y.ToString() + ", Color1 = " + tilePalette[0].ToString() + ", Color2 = " + tilePalette[1].ToString() + ", Color3 = " + tilePalette[2].ToString());
                }
            }
            if (usePack && packID > -1)
            {
                int BuffPos = ((Y - _FirstLinesTCut) * 4096) + (X * 8);
                int BuffPos2 = BuffPos + 2048;
                

                Bitmap b = bitmaps[t.bitmapP[packID].bitmapID];
                if(lastVirtualScanline != oppu.virtualScanline || lastPackID != packID){
                    lastPackOffSet = (t.bitmapP[packID].y + (oppu.virtualScanline + oppu.virtualScanline)) * b.Width + t.bitmapP[packID].x + 14;
                    lastVirtualScanline = oppu.virtualScanline;
                    lastPackID = packID;
                }
                fixed (byte* pSrc = _displayData, pBmp = rawBits[t.bitmapP[packID].bitmapID])
                {
                    byte* ps = pSrc;

                    //int* tpt = (int*)(pBmp + 4 * ((t.bitmapP[packID].y + (oppu.virtualScanline * 2)) * b.Width + t.bitmapP[packID].x + 14 - (oppu.virtualColumn * 2)));
                    int* tpt = (int*)(pBmp + 4 * (lastPackOffSet - (oppu.virtualColumn + oppu.virtualColumn)));
                    if ((*tpt & 0xFF000000) != 0)
                    {
                        if (oppu.hflip)
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos2)) = *tpt;
                            else *((int*)(ps + BuffPos)) = *tpt;
                        }
                        else
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos2 + 4)) = *tpt;
                            else *((int*)(ps + BuffPos + 4)) = *tpt;
                        }
                    }
                    tpt++;
                    if ((*tpt & 0xFF000000) != 0)
                    {
                        if (oppu.hflip)
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos2 + 4)) = *tpt;
                            else *((int*)(ps + BuffPos + 4)) = *tpt;
                        }
                        else
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos2)) = *tpt;
                            else *((int*)(ps + BuffPos)) = *tpt;
                        }
                    }
                    tpt += b.Width;
                    if ((*tpt & 0xFF000000) != 0)
                    {
                        if (oppu.hflip)
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos + 4)) = *tpt;
                            else *((int*)(ps + BuffPos2 + 4)) = *tpt;
                        }
                        else
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos)) = *tpt;
                            else *((int*)(ps + BuffPos2)) = *tpt;
                        }
                    }
                    tpt--;
                    if ((*tpt & 0xFF000000) != 0)
                    {
                        if (oppu.hflip)
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos)) = *tpt;
                            else *((int*)(ps + BuffPos2)) = *tpt;
                        }
                        else
                        {
                            if (oppu.vflip) *((int*)(ps + BuffPos + 4)) = *tpt;
                            else *((int*)(ps + BuffPos2 + 4)) = *tpt;
                        }
                    }
                }

            }
            else{
                if (tileRow[oppu.virtualColumn] > 0)
                    DrawBasicPixel(X, Y, oppu.PALETTE[0x3f & tilePalette[tileRow[oppu.virtualColumn] - 1]]);
            }
        }

        public void DrawBasicPixel(int X, int Y, int Color) {
            int BuffPos = ((Y - _FirstLinesTCut) * 4096) + X * 8;
            fixed (byte* pSrc = _displayData)
            {
                byte* ps = pSrc;
                ps += BuffPos;
                *((int*)ps) = Color;
                ps += 4;
                *((int*)ps) = Color;
                ps += 2048;
                *((int*)ps) = Color;
                ps -= 4;
                *((int*)ps) = Color;
            }
        
        }

        public void BlankPixel(int X, int Y)
        {
            if (_CanRender & _IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= _FirstLinesTCut & Y < (_ScanLines + _FirstLinesTCut))
                {
                    if (lastLineState != oppu.rstate)
                    {
                        lastLineState = oppu.rstate;
                        lastTileNo = -1;
                    }
                    if (hasHiResPack) RenderTile(X, Y);
                }
            }
        }

        public void DrawPixel(int X, int Y, int Color)
        {

            if (_CanRender & _IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= _FirstLinesTCut & Y < (_ScanLines + _FirstLinesTCut))
                {
                    if (lastLineState != oppu.rstate)
                    {
                        lastLineState = oppu.rstate;
                        lastTileNo = -1;
                    }

                    if (oppu.rstate == RenderLineState.ClearScanline || oppu.rstate == RenderLineState.RenderClipping || !hasHiResPack)
                    {
                        DrawBasicPixel(X, Y, Color);
                    }
                    else {
                        RenderTile(X, Y);
                    }
                }
            }
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
                    _displaySprite.Draw(_nesDisplay, _displayRect, Color.White);
                    _displaySprite.End();
                    _displayDevice.EndScene();
                    _displayDevice.Present();
                }
                _IsRendering = false;
            }
            if (highResSnap && takingSnap)
            {
                log.Flush();
                log.Close();
                saveSnap();
                highResSnap = false;
                takingSnap = false;
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
            if (_displayDevice != null)
            {
                _displayDevice.Dispose();
            }
            this.DisposeDisplayObjects();
            _disposed = true;
        }

        public void TakeSnapshot(string SnapPath, string Format)
        {
            highResSnap = true;
            takingSnap = false;
            snapfile = SnapPath;
            snapformat = Format;
        }

        void saveSnap()
        {
            Bitmap bmp = new Bitmap(512, _ScanLines * 2);
            BitmapData bmpData32 = bmp.LockBits(new Rectangle(0, 0, 512, _ScanLines * 2), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            int* numPtr32 = (int*)bmpData32.Scan0;
            int j = 0;
            for (int i = (_FirstLinesTCut * 2 * 512); i < 245760 - (_FirstLinesTCut * 2 * 512); i++)
            {
                byte VALBLUE = (byte)(_displayData[j]);
                j++;
                byte VALGREEN = (byte)(_displayData[j]);
                j++;
                byte VALRED = (byte)(_displayData[j]);
                j++;
                byte VALALHA = (byte)(_displayData[j]);
                j++;
                numPtr32[i - (_FirstLinesTCut * 2 * 512)] =
                   (VALALHA << 24 |//Alpha
                   VALRED << 16 |//Red
                  VALGREEN << 8 |//Green
                  VALBLUE)//Blue
                             ;
            }
            bmp.UnlockBits(bmpData32);
            switch (snapformat)
            {
                case ".bmp":
                    bmp.Save(snapfile, ImageFormat.Bmp);
                    _CanRender = true;
                    break;
                case ".gif":
                    bmp.Save(snapfile, ImageFormat.Gif);
                    _CanRender = true;
                    break;
                case ".jpg":
                    bmp.Save(snapfile, ImageFormat.Jpeg);
                    _CanRender = true;
                    break;
                case ".png":
                    bmp.Save(snapfile, ImageFormat.Png);
                    _CanRender = true;
                    break;
                case ".tiff":
                    bmp.Save(snapfile, ImageFormat.Tiff);
                    _CanRender = true;
                    break;
            }
        }

        public void DrawText(string Text, int Frames){}
        public void ChangeSettings(){}
        public void Clear(){}
        public void UpdateSize(int X, int Y, int W, int H){}
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
            { return _CanRender; }
            set
            { _CanRender = value; }
        }
        public bool FullScreen
        {
            get
            { return false; }
            set
            {
                Dispose();
                InitializeDirect3D();
            }
        }
        public bool SupportFullScreen
        {
            get { return true; }
        }
    }
}
