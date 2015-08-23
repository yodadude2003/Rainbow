﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;
using System.Drawing;
using Rainbow.ImgLib.Filters;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecDXT1 : ColorCodec, EndiannessDependent
    {
        private int width, height;
        private static Color[] clut = new Color[4];

        public ColorCodecDXT1(ByteOrder order, int width, int height)
        {
            ByteOrder = order;
            this.width = width;
            this.height = height;
        }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            ImageFilter filter = GetImageFilter();

            BinaryReader reader=null;
            if (filter != null)
            {
                byte[] data = filter.Defilter(colors, start, length);
                reader = new BinaryReader(new MemoryStream(data));
            }
            else
                reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] decoded = new Color[length * 2];
            Color[] tile = new Color[4 * 4];

            int block = 0;
            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    DecodeDXT1Block(reader, tile);
                    for (int line = 0; line < 4; line++)
                    {
                        Array.Copy(tile, line * 4, decoded, width * (y + line) + x, 4);

                    }
                    block++;
                }
            }

            return decoded;
        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }

        public override int BitDepth
        {
            get { return 4; }
        }

        public ByteOrder ByteOrder { get; set; }

        protected void DecodeDXT1Block(BinaryReader reader, Color[] tile)
        {

            ushort color1, color2;
            byte[] table;
            color1 = reader.ReadUInt16(ByteOrder);
            color2 = reader.ReadUInt16(ByteOrder);

            table = reader.ReadBytes(4);

            int blue1 = (color1 & 0x1F)*8;
	        int blue2 = (color2 & 0x1F)*8;
	        int green1 = ((color1 >> 5) & 0x3F)*4;
	        int green2 = ((color2 >> 5) & 0x3F)*4;
	        int red1 = ((color1 >> 11) & 0x1F)*8;
	        int red2 = ((color2 >> 11) & 0x1F)*8;

	        clut[0] = Color.FromArgb(255,red1, green1, blue1);
            clut[1] = Color.FromArgb(255, red2, green2, blue2);

            if (color1 > color2)
            {
                int blue3 = (2 * blue1 + blue2) / 3;
                int green3 = (2 * green1 + green2) / 3;
                int red3 = (2 * red1 + red2) / 3;

                int blue4 = (2 * blue2 + blue1) / 3;
                int green4 = (2 * green2 + green1) / 3;
                int red4 = (2 * red2 + red1) / 3;

                clut[2] = Color.FromArgb(255, red3,green3,blue3);
                clut[3] = Color.FromArgb(255,red4,green4,blue4);
            }
            else
            {
                clut[2] = Color.FromArgb(255, (red1 + red2)  / 2, // Average
                                              (green1 + green2) / 2,
                                              (blue1 + blue2) / 2);
                clut[3] = Color.FromArgb(0, 0, 0, 0);  // Color2 but transparent
            }

            int k=0;
            for (int y = 0; y < 4; y++)
            {
                int val = table[y];
                for (int x = 0; x < 4; x++)
                {
                    tile[k++] = clut[(val >> 6) & 3];
                    val <<= 2;
                }
            }
        }

        protected virtual ImageFilter GetImageFilter()
        {
            return null;
        }
    }
}