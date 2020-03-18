//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using Rainbow.ImgLib.Filters;
using SkiaSharp;
using System;
using System.Drawing;

namespace Rainbow.ImgLib.Encoding
{
    public class ImageDecoderIndexed : ImageDecoder
    {

        protected byte[] pixelData;

        protected int width, height;

        protected IndexCodec indexCodec;

        protected SKColor[] grayScale;

        public ImageDecoderIndexed(byte[] pixelData, int width, int height, IndexCodec codec, SKColor[] palette = null, ImageFilter imageFilter=null, PaletteFilter paletteFilter=null)
        {
            this.pixelData = pixelData;
            if (imageFilter != null)
                this.pixelData = imageFilter.Defilter(pixelData);

            this.width = width;
            this.height = height;
            this.indexCodec = codec;

            grayScale = new SKColor[1 << codec.BitDepth];

            for (int i = 0; i < grayScale.Length; i++)
            {
                grayScale[i] = new SKColor((byte)(i * (256 / grayScale.Length)), (byte)(i * (256 / grayScale.Length)), (byte)(i * (256 / grayScale.Length)));
            }

            if (paletteFilter != null && palette != null)
            {
                Palette = paletteFilter.Defilter(palette);
            }
            else if (palette == null)
            {
                palette = (SKColor[])grayScale.Clone();
                Palette = palette;
            }
            else
            {
                Palette = (SKColor[])palette.Clone();
            }
        }

        public SKColor[] Palette { get; set; }

        public int BitDepth { get { return indexCodec.BitDepth; } }

        public SKBitmap ReferenceImage
        {
            get { return DecodeImage(grayScale); }
        }

        public SKBitmap DecodeImage()
        {
            return DecodeImage(Palette);
        }

        private SKBitmap DecodeImage(SKColor[] pal)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            SKBitmap bmp = new SKBitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, y, pal[indexCodec.GetPixelIndex(pixelData, width, height, x, y)]);
                }
            }
            return bmp;
        }

    }
}