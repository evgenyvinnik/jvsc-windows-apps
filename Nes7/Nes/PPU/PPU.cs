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
using MyNes.Nes.Output.Video;
using MyNes.Nes.Input;
namespace MyNes.Nes
{
    /// <summary>
    /// Picture Processing Unit class
    /// </summary>
    [Serializable()]
    public class PPU
    {
        /// <summary>
        /// Picture Processing Unit class
        /// </summary>
        /// <param name="nes">The nes</param>
        public PPU(NES nes)
        {
            MEM_PPU = new PPUMemory(nes);
            nes.Memory.ppu = this;
            MyNesDEBUGGER.WriteLine(this, "PPU Initialized ok.", DebugStatus.Cool);
        }

        public PPUMemory MEM_PPU;
        public CPUMemory MEM_CPU;
        public CPU CPU;
        [NonSerialized()]
        public IGraphicDevice VIDEO;
        public TVFORMAT TV = TVFORMAT.NTSC;
        public bool NoLimiter = false;
        bool ExecuteNMIOnVBlank = false;
        bool SpriteSize = false;//(false=8x8, true=8x16)
        int PatternTableAddressBackground = 0;
        int PatternTableAddress8x8Sprites = 0;
        int VRAMAddressIncrement = 1;
        bool MonochromeMode = false;
        bool BackgroundClipping = false;
        bool SpriteClipping = false;
        bool BackgroundVisibility = false;
        bool BBackgroundVisibility = false;
        bool SpriteVisibility = false;
        byte ColorEmphasis = 0;
        byte SpriteRamAddress = 0;
        ushort VRAM_ADDRESS = 0;
        ushort VRAM_TEMP = 0;
        byte VRAM_READBUFFER = 0;
        bool PPU_TOGGLE = true;
        bool VBlank = false;
        bool Sprite0Hit = false;
        bool OddFrame = false;
        public byte[] piorityBuff = new byte[256];
        int ScanLine = -1;
        int ScanCycle = 0;
        int TileCycle = 8;//?
        int TileX = 0;
        int SpriteCrossed = 0;
        bool SpritesRendered = false;
        public int FPS = 0;
        //Timing for pal and ntsc
        [NonSerialized()]
        public TIMER Timer = new TIMER();
        double _currentFrameTime = 0;
        double _lastFrameTime = 0;
        double FramePeriod = 0.01667;
        int ScanlinesPerFrame = 261;
        int ScanlineOfEndOfVblank = 21;
        //Draw
        int nameTableAddress = 0x2000;
        int HScroll = 0;
        int VScroll = 0;
        public int[] PALETTE =//PAL palette
        { 
        0x808080, 0xbb, 0x3700bf, 0x8400a6, 0xbb006a, 0xb7001e, 0xb30000, 0x912600, 
        0x7b2b00, 0x3e00, 0x480d, 0x3c22, 0x2f66, 0, 0x50505, 0x50505, 
        0xc8c8c8, 0x59ff, 0x443cff, 0xb733cc, 0xff33aa, 0xff375e, 0xff371a, 0xd54b00,
        0xc46200, 0x3c7b00, 0x1e8415, 0x9566, 0x84c4, 0x111111, 0x90909, 0x90909, 
        0xffffff, 0x95ff, 0x6f84ff, 0xd56fff, 0xff77cc, 0xff6f99, 0xff7b59, 0xff915f,
        0xffa233, 0xa6bf00, 0x51d96a, 0x4dd5ae, 0xd9ff, 0x666666, 0xd0d0d, 0xd0d0d, 
        0xffffff, 0x84bfff, 0xbbbbff, 0xd0bbff, 0xffbfea, 0xffbfcc, 0xffc4b7, 0xffccae,
        0xffd9a2, 0xcce199, 0xaeeeb7, 0xaaf7ee, 0xb3eeff, 0xdddddd, 0x111111, 0x111111
        };

        public byte tiledata1;
        public byte tiledata2;
        public uint tilepage;
        public int tilenumber;
        public int tileDataOffset;
        public int virtualScanline;
        public int virtualColumn;
        public byte PaletteUpperBits;
        public RenderLineState rstate;
        public bool vflip;
        public bool hflip;

        public unsafe void RunPPU()
        {
            ScanCycle++;
            TileCycle++;
            if (ScanCycle == 341)
            {
                ScanCycle = 0;
                TileCycle = 8;
                SpritesRendered = false;
                ScanLine++;
                //Render the actual data to be displayed on the screen
                if (ScanLine >= ScanlineOfEndOfVblank & ScanLine <= 239 + ScanlineOfEndOfVblank)
                {
                    rstate = RenderLineState.ClearScanline;
                    //Clean up the line from before
                    fixed (byte* pSrc = piorityBuff)
                    {
                        byte* ps = pSrc;
                        for (int i = 0; i < 256; i += 8)
                        {
                            *((UInt64*)(ps + i)) = 0;
                        }
                    }
                    //Draw background color
                    byte B = MEM_PPU[0x3F00];
                    if (B >= 63)
                        B = 63;
                    for (int i = 0; i < 256; i++)
                    {
                        VIDEO.DrawPixel(i, ScanLine - ScanlineOfEndOfVblank, PALETTE[B]);
                        piorityBuff[i] = 0;
                    }
                    //Tick mapper timer (scanline timer)
                    if (BackgroundVisibility | SpriteVisibility)
                        MEM_CPU.MAPPER.TickScanlineTimer();
                }
                //do nothing for 1 scanline
                if (ScanLine == ScanlinesPerFrame)
                {
                    //Handle the speed
                    if (!NoLimiter)
                    {
                        double currentTime = Timer.GetCurrentTime();
                        _currentFrameTime = currentTime - _lastFrameTime;
                        if ((_currentFrameTime < FramePeriod))
                        {
                            while (true)
                            {
                                if ((Timer.GetCurrentTime() - _lastFrameTime) >= FramePeriod)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    _lastFrameTime = Timer.GetCurrentTime();
                    ScanLine = -1;
                    FPS++;
                    //Odd frame ? only in ntsc
                    if (TV == TVFORMAT.NTSC)
                    {
                        OddFrame = !OddFrame;
                        if (!OddFrame & BackgroundVisibility)
                        {
                            if (BBackgroundVisibility)
                                ScanCycle += 1;
                        }
                        BBackgroundVisibility = BackgroundVisibility;
                    }
                    //render into screen
                    VIDEO.RenderFrame();
                }
            }
            else if (ScanLine >= ScanlineOfEndOfVblank & ScanLine <= 239 + ScanlineOfEndOfVblank)
            {
                if (ScanCycle < 256)//Render on the screen
                {
                    if (TileCycle >= 8)//Render BG tile each 8 cycles
                    {
                        if (BackgroundVisibility)
                        {
                            if (rstate != RenderLineState.RenderBackground)
                            {
                                rstate = RenderLineState.RenderBackground;
                                hflip = true;
                                vflip = false;
                            }
                            RenderNextBackgroundTile();
                        }
                        TileCycle -= 8;
                    }
                }
                else if (!SpritesRendered)
                {
                    if (SpriteVisibility)
                    {
                        rstate = RenderLineState.RenderSprite;
                        RenderSprites();
                    }
                    if (BackgroundClipping)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            VIDEO.DrawPixel(i, ScanLine - ScanlineOfEndOfVblank, 0);
                        }
                    }
                    //Update Hscroll
                    if (SpriteVisibility | BackgroundVisibility)
                    {
                        VRAM_ADDRESS = (ushort)((VRAM_ADDRESS & 0x7BE0) | (VRAM_TEMP & 0x041F));
                        nameTableAddress = (VRAM_ADDRESS & 0x0800) +
                            (VRAM_TEMP & 0x0400) + 0x2000;
                        HScroll = (((VRAM_TEMP & 0x001F) << 3) | TileX);
                    }
                    SpritesRendered = true;
                }
            }
            //Start of vblank
            if (ScanLine == 0 & ScanCycle == 339)
            {
                if (ExecuteNMIOnVBlank)
                {
                    CPU.NMIRequest = true;
                }
                VBlank = true;
            }
            //End of vblank
            else if (ScanLine == ScanlineOfEndOfVblank)
            {
                if (ScanCycle == 305)
                {
                    //Update Vscroll
                    if (SpriteVisibility | BackgroundVisibility)
                    {
                        VRAM_ADDRESS = VRAM_TEMP;
                        VScroll = (((VRAM_TEMP & 0x03E0) >> 2) | ((VRAM_TEMP & 0x7000) >> 12));
                        if (VScroll >= 240)
                            VScroll -= 256;
                    }
                }
                if (ScanCycle == 339)
                {
                    VIDEO.Begin();
                    //Clear flags
                    Sprite0Hit = false;
                    VBlank = false;
                    SpriteCrossed = 0;
                }
            }
        }
        void RenderNextBackgroundTile()
        {
            bool vScrollSide = ((HScroll / 8) + (ScanCycle / 8)) >= 32;
            int virtualRow = (ScanLine - ScanlineOfEndOfVblank) + VScroll;
            if (virtualRow < 0)
                virtualRow = 0;
            int nameTableBase = nameTableAddress;
            int startColumn = 0;
            int endColumn = 0;
            if (!vScrollSide)
            {
                if (virtualRow >= 240)
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2800;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2000;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2400;
                    virtualRow -= 240;
                }
                startColumn = (HScroll / 8) + (ScanCycle / 8);
            }
            else
            {
                if (virtualRow >= 240)
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2800;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2400;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2000;
                    virtualRow -= 240;
                }
                else
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2400;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2000;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2800;
                }
                startColumn = ((HScroll / 8) + (ScanCycle / 8) - 32);
            }

            if ((HScroll % 8) != 0)
            {
                endColumn = startColumn + 2;
                if (endColumn > 32)
                    endColumn = 32;
            }
            else
            {
                endColumn = startColumn + 1;
            }

            for (int currentTileColumn = startColumn; currentTileColumn < endColumn;
                currentTileColumn++)
            {
                //Starting tile row is currentScanline / 8
                //The offset in the tile is currentScanline % 8

                //Step #1, get the tile number
                tilenumber = MEM_PPU[(ushort)(nameTableBase + ((virtualRow / 8) * 32) + currentTileColumn)];

                //Step #2, get the offset for the tile in the tile data
                tileDataOffset = PatternTableAddressBackground + (tilenumber * 16);

                //Step #3, get the tile data from chr rom
                tiledata1 = MEM_PPU[(ushort)(tileDataOffset + (virtualRow % 8))];
                tiledata2 = MEM_PPU[(ushort)(tileDataOffset + (virtualRow % 8) + 8)];
                tilepage = MEM_CPU.CHR_PAGE[((tileDataOffset + (virtualRow % 8)) & 0x1C00) >> 10];
                       
                //Step #4, get the attribute byte for the block of tiles we're in
                //this will put us in the correct section in the palette table
                PaletteUpperBits = MEM_PPU[(ushort)((nameTableBase +
                    0x3c0 + (((virtualRow / 8) / 4) * 8) + (currentTileColumn / 4)))];
                PaletteUpperBits = (byte)(PaletteUpperBits >> ((4 * (((virtualRow / 8) % 4) / 2)) +
                    (2 * ((currentTileColumn % 4) / 2))));
                PaletteUpperBits = (byte)((PaletteUpperBits & 0x3) << 2);

                //Step #5, render the line inside the tile to the offscreen buffer
                int startTilePixel = 0;
                int endTilePixel = 0;
                if (!vScrollSide)
                {
                    if (currentTileColumn == startColumn)
                    {
                        startTilePixel = HScroll % 8;
                        endTilePixel = 8;
                    }
                    else
                    {
                        startTilePixel = 0;
                        endTilePixel = 8;
                    }
                }
                else
                {
                    if (currentTileColumn == 32 + (HScroll / 8))
                    {
                        startTilePixel = 0;
                        endTilePixel = HScroll % 8;
                    }
                    else
                    {
                        startTilePixel = 0;
                        endTilePixel = 8;
                    }
                }
                for (int i = startTilePixel; i < endTilePixel; i++)
                {
                    virtualColumn = 7 - i;
                    virtualScanline = virtualRow % 8;
                    int pixelColor = PaletteUpperBits + (((tiledata2 & (1 << (7 - i))) >> (7 - i)) << 1) +
                        ((tiledata1 & (1 << (7 - i))) >> (7 - i));

                    if ((pixelColor % 4) != 0)
                    {
                        if (!vScrollSide)
                        {
                            int tmpX = (8 * currentTileColumn) - HScroll + i;
                            piorityBuff[tmpX] = 1;
                            VIDEO.DrawPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank), PALETTE[MEM_PPU[(ushort)(0x3F00 + pixelColor)]]);
                        }
                        else
                        {
                            int tmpX = (8 * currentTileColumn) + (256 - HScroll) + i;
                            if (tmpX < 256)
                            {
                                piorityBuff[tmpX] = 1;
                                VIDEO.DrawPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank), PALETTE[MEM_PPU[(ushort)(0x3F00 + pixelColor)]]);
                            }
                        }
                    }
                    else
                    {
                        if (!vScrollSide)
                        {
                            VIDEO.BlankPixel((8 * currentTileColumn) - HScroll + i, ScanLine);
                        }
                        else
                        {
                            if (((8 * currentTileColumn) + (256 - HScroll) + i) < 256)
                            {
                                VIDEO.BlankPixel((8 * currentTileColumn) + (256 - HScroll) + i, ScanLine);
                            }
                        }
                    }
                }
            }
        }
        void RenderSprites()
        {
            int tmpX;
            bool drawPixel;
            int _SpriteSize = SpriteSize ? 16 : 8;
            virtualScanline = 0;
            //1: loop through SPR-RAM
            for (tilenumber = 0; tilenumber < 256; tilenumber += 4)
            {
                int PixelColor = 0;
                byte YCoordinate = (byte)(MEM_PPU.SPR_RAM[tilenumber] + 1);
                //2: if the sprite falls on the current scanline, draw it
                if ((YCoordinate <= (ScanLine - ScanlineOfEndOfVblank)) &&
                    ((YCoordinate + _SpriteSize) > (ScanLine - ScanlineOfEndOfVblank)))
                {
                    SpriteCrossed++;
                    //3: Draw the sprites differently if they are 8x8 or 8x16
                    if (!SpriteSize)//8x8
                    {
                        //4: calculate which line of the sprite is currently being drawn
                        //Line to draw is: currentScanline - Y coord + 1
                        if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x80) != 0x80)
                        {
                            vflip = false;
                            virtualScanline = (ScanLine - ScanlineOfEndOfVblank) - YCoordinate;
                        }
                        else
                        {
                            vflip = true;
                            virtualScanline = YCoordinate + 7 - (ScanLine - ScanlineOfEndOfVblank);
                        }
                        //5: calculate the offset to the sprite's data in
                        //our chr rom data 
                        tileDataOffset = PatternTableAddress8x8Sprites + MEM_PPU.SPR_RAM[tilenumber + 1] * 16;
                        //6: extract our tile data
                        tiledata1 = MEM_PPU[(ushort)(tileDataOffset + virtualScanline)];
                        tiledata2 = MEM_PPU[(ushort)((tileDataOffset + virtualScanline) + 8)];
                        tilepage = MEM_CPU.CHR_PAGE[((tileDataOffset + virtualScanline) & 0x1C00) >> 10];
                        //7: get the palette attribute data
                        byte PaletteUpperBits = (byte)((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x3) << 2);
                        int virtualColumn = 0;
                        //8: render the line inside the tile into the screen direcly
                        for (int j = 0; j < 8; j++)
                        {
                            if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x40) == 0x40)
                            {
                                //hflip = false;
                                virtualColumn = j;
                            }
                            else
                            {
                                //hflip = true;
                                virtualColumn = 7 - j;
                            }
                            PixelColor = PaletteUpperBits + (((tiledata2 & (1 << (virtualColumn))) >> (virtualColumn)) << 1) +
                            ((tiledata1 & (1 << (virtualColumn))) >> (virtualColumn));
                            tmpX = MEM_PPU.SPR_RAM[tilenumber + 3] + j;
                            if (tmpX < 256)
                            {
                                drawPixel = false;
                                //Sprite 0 hit
                                if ((PixelColor % 4) != 0)
                                {
                                    if (tilenumber == 0 & piorityBuff[tmpX] == 1)
                                    {
                                        Sprite0Hit = true;
                                    }
                                }
                                if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x20) == 0x20)
                                {
                                    if (piorityBuff[tmpX] == 0)
                                        drawPixel = true;
                                }
                                else
                                {
                                    if (piorityBuff[tmpX] < 2)
                                        drawPixel = true;
                                }
                                if ((PixelColor % 4) != 0)
                                    piorityBuff[tmpX] = 2;
                                if (drawPixel)
                                {
                                    if ((PixelColor % 4) != 0)
                                    {
                                        VIDEO.DrawPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank),
                                            PALETTE[MEM_PPU[(ushort)(0x3F10 + PixelColor)]]);
                                    }
                                    else
                                    {
                                        VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                    }
                                }
                                else if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x20) == 0)
                                {
                                    VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                }
                            }
                        }
                    }
                    else//8x16
                    {
                        //4: get the sprite id
                        byte SpriteId = MEM_PPU.SPR_RAM[tilenumber + 1];
                        if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x80) != 0x80)
                        {
                            vflip = false;
                            virtualScanline = (ScanLine - ScanlineOfEndOfVblank) - YCoordinate;
                        }
                        else
                        {
                            vflip = true;
                            virtualScanline = YCoordinate + 15 - (ScanLine - ScanlineOfEndOfVblank);
                        }
                        //5: We draw the sprite like two halves, so getting past the 
                        //first 8 puts us into the next tile
                        //If the ID is even, the tile is in 0x0000, odd 0x1000
                        tileDataOffset = 0;
                        if (virtualScanline < 8)
                        {
                            //Draw the top tile
                            if ((SpriteId % 2) == 0)
                                tileDataOffset = 0x0000 + (SpriteId) * 16;
                            else
                                tileDataOffset = 0x1000 + (SpriteId - 1) * 16;
                        }
                        else
                        {
                            //Draw the bottom tile
                            virtualScanline -= 8;
                            if ((SpriteId % 2) == 0)
                                tileDataOffset = 0x0000 + (SpriteId + 1) * 16;
                            else
                                tileDataOffset = 0x1000 + (SpriteId) * 16;
                        }
                        //6: extract our tile data
                        tiledata1 = MEM_PPU[(ushort)(tileDataOffset + virtualScanline)];
                        tiledata2 = MEM_PPU[(ushort)((tileDataOffset + virtualScanline) + 8)];
                        tilepage = MEM_CPU.CHR_PAGE[(((tileDataOffset + virtualScanline) + 8) & 0x1C00) >> 10];
                        //7: get the palette attribute data
                        PaletteUpperBits = (byte)((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x3) << 2);
                        //8, render the line inside the tile to the screen
                        for (int j = 0; j < 8; j++)
                        {
                            if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x40) == 0x40)
                            {
                                hflip = false;
                                virtualColumn = j;
                            }
                            else
                            {
                                hflip = true;
                                virtualColumn = 7 - j;
                            }
                            PixelColor = PaletteUpperBits + (((tiledata2 & (1 << (virtualColumn))) >> (virtualColumn)) << 1) +
                                    ((tiledata1 & (1 << (virtualColumn))) >> (virtualColumn));
                            tmpX = MEM_PPU.SPR_RAM[tilenumber + 3] + j;
                            if (tmpX < 256)
                            {
                                drawPixel = false;
                                //Sprite 0 hit
                                if ((PixelColor % 4) != 0)
                                {
                                    if (tilenumber == 0 & piorityBuff[tmpX] == 1)
                                    {
                                        Sprite0Hit = true;
                                    }
                                }
                                if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x20) == 0x20)
                                {
                                    if (piorityBuff[tmpX] == 0)
                                        drawPixel = true;
                                }
                                else
                                {
                                    if (piorityBuff[tmpX] < 2)
                                        drawPixel = true;
                                }
                                if ((PixelColor % 4) != 0)
                                    piorityBuff[tmpX] = 2;
                                if (drawPixel)
                                {
                                    if ((PixelColor % 4) != 0)
                                    {
                                        VIDEO.DrawPixel(tmpX,
                                            (ScanLine - ScanlineOfEndOfVblank),
                                            PALETTE[MEM_PPU[(ushort)(0x3F10 + PixelColor)]]);
                                    }
                                    else
                                    {
                                        VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                    }
                                }
                                else if ((MEM_PPU.SPR_RAM[tilenumber + 2] & 0x20) == 0)
                                {
                                    VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                }
                            }
                        }
                    }
                }
                else
                {
                    SpriteRamAddress = 0;
                }
            }
        }
        public void SetPallete(TVFORMAT FORMAT, PaletteFormat PlFormat)
        {
            TV = FORMAT;
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    //Setup timing
                    FramePeriod = 0.01667;
                    ScanlinesPerFrame = 261;
                    ScanlineOfEndOfVblank = 21;
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        if (Paletter.LoadPalette(PlFormat.ExternalPalettePath) != null)
                        {
                            PALETTE = Paletter.LoadPalette(PlFormat.ExternalPalettePath);
                        }
                        else
                        {
                            PALETTE = Paletter.NTSCPalette;
                        }
                    }
                    break;
                case TVFORMAT.PAL:
                    //Setup timing
                    FramePeriod = 0.020;
                    ScanlinesPerFrame = 311;
                    ScanlineOfEndOfVblank = 71;
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                PALETTE = Paletter.PALPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        if (Paletter.LoadPalette(PlFormat.ExternalPalettePath) != null)
                        {
                            PALETTE = Paletter.LoadPalette(PlFormat.ExternalPalettePath);
                        }
                        else
                        {
                            PALETTE = Paletter.PALPalette;
                        }
                    }
                    break;
            }
        }
        #region Registers
        public void Write2000(byte value)
        {
            VRAM_TEMP = (ushort)((VRAM_TEMP & 0xF3FF) | ((value & 0x3) << 10));
            VRAMAddressIncrement = ((value & 0x04) != 0) ? 32 : 1;//Bit2
            PatternTableAddress8x8Sprites = ((value & 0x8) == 0x8) ? 0x1000 : 0;//Bit3
            PatternTableAddressBackground = ((value & 0x10) == 0x10) ? 0x1000 : 0;//Bit4
            SpriteSize = (value & 0x20) == 0x20;//Bit5
            ExecuteNMIOnVBlank = (value & 0x80) == 0x80;//Bit 7
        }
        public void Write2001(byte value)
        {
            MonochromeMode = (value & 0x01) != 0;//Bit 0
            BackgroundClipping = (value & 0x02) == 0;//Bit 1
            SpriteClipping = (value & 0x04) == 0;//Bit 2
            BackgroundVisibility = (value & 0x8) != 0;//Bit 3
            SpriteVisibility = (value & 0x10) != 0;//Bit 4
            ColorEmphasis = (byte)((value & 0xE0) << 1);//Bit 5 - 7
        }
        public byte Read2002()
        {
            byte status = 0;
            // VBlank
            if (VBlank)
                status |= 0x80;

            //Sprite 0 Hit
            if (Sprite0Hit & (BackgroundVisibility & SpriteVisibility))
                status |= 0x40;

            //More than 8 sprites in 1 scanline
            if ((SpriteCrossed > 8) & (SpriteVisibility | BackgroundVisibility))
                status |= 0x20;

            //If it should ignore any write into 2007
            if ((ScanLine < ScanlineOfEndOfVblank) | !(BackgroundVisibility & SpriteVisibility))
                status |= 0x10;

            VBlank = false;
            PPU_TOGGLE = true;
            return status;
        }
        public void Write2003(byte value)
        {
            SpriteRamAddress = value;
        }
        public void Write2004(byte value)
        {
            MEM_PPU.SPR_RAM[SpriteRamAddress] = value;
            SpriteRamAddress++;
        }
        public byte Read2004()
        {
            return MEM_PPU.SPR_RAM[SpriteRamAddress];
        }
        public void Write2005(byte value)
        {
            if (PPU_TOGGLE)
            {
                TileX = (byte)(value & 0x07);
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x7FE0) | (value >> 3));
            }
            else
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x0C1F) | ((value & 7) << 12) | ((value & 0xF8) << 2));
            }
            PPU_TOGGLE = !PPU_TOGGLE;
        }
        public void Write2006(byte value)
        {
            if (PPU_TOGGLE)
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x00FF) | ((value & 0x3F) << 8));
            }
            else
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x7F00) | value);
                VScroll = (((VRAM_TEMP & 0x03E0) >> 2) | ((VRAM_TEMP & 0x7000) >> 12));
                HScroll = (((VRAM_TEMP & 0x001F) << 3) | TileX);
                if (ScanLine < ScanlinesPerFrame)
                    VScroll -= (ScanLine - ScanlineOfEndOfVblank);
                VRAM_ADDRESS = VRAM_TEMP;
            }
            PPU_TOGGLE = !PPU_TOGGLE;
        }
        public void Write2007(byte value)
        {
            MEM_PPU[(ushort)(VRAM_ADDRESS & 0x3FFF)] = value;
            VRAM_ADDRESS += (ushort)VRAMAddressIncrement;
        }
        public byte Read2007()
        {
            byte returnedValue = VRAM_READBUFFER;
            if ((ushort)(VRAM_ADDRESS & 0x3FFF) < 0x3F00)
                VRAM_READBUFFER = MEM_PPU[(ushort)(VRAM_ADDRESS & 0x3FFF)];
            else
                returnedValue = MEM_PPU[(ushort)(VRAM_ADDRESS & 0x3FFF)];
            VRAM_ADDRESS += (ushort)VRAMAddressIncrement;
            return returnedValue;
        }
        public void Write4014(byte value)
        {
            ushort a = (ushort)(value << 8);
            for (int i = 0; i < 0x100; i++)
            {
                MEM_PPU.SPR_RAM[(byte)(0xFF & (SpriteRamAddress + i))] =
                  MEM_CPU[(ushort)(a + i)];
            }
            CPU.DMA = true;
        }
        #endregion
    }
    public enum RenderLineState
    {
        ClearScanline,
        RenderSprite,
        RenderBackground,
        RenderClipping,
    }
}
