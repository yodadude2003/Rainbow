﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Rainbow.ImgLib.Encoding
{
    /// <summary>
    /// This  ColorDecoder decodes sequences of pixels in 32 bit RGBA format.
    /// </summary>
    public class ColorDecoder32BitRGBA : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
                pal.Add(Color.FromArgb(palette[start + i * 4 + 3], palette[start + i * 4], palette[start + i * 4 + 1], palette[start + i * 4 + 2]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 32; } }
    }

    public class ColorDecoder24BitRGB : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 3; i++)
                pal.Add(Color.FromArgb(255, palette[start + i * 3], palette[start + i * 3 + 1], palette[start + i * 3 + 2]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 24; } }
    }

    public class ColorDecoder16BitLEABGR : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            BinaryReader reader = new BinaryReader(new MemoryStream(palette, start, size));
            for (int i = 0; i < size / 2; i++)
            {
                ushort data = reader.ReadUInt16();

                int red = data & 0x1F;
                data >>= 5;
                int green = data & 0x1F;
                data >>= 5;
                int blue = data & 0x1F;
                int alpha = data == 0 ? 0 : 255;

                pal.Add(Color.FromArgb(alpha, red * 8, green * 8, blue * 8));
            }

            return pal.ToArray();
        }

        public override int BitDepth { get { return 16; } }
    }

}