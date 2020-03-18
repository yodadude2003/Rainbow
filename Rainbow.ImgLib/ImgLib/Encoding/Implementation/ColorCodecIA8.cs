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

using Rainbow.ImgLib.Common;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecIA8 : ColorCodecEndiannessDependent
    {
        public ColorCodecIA8(ByteOrder order):
            base(order) { }

        public override SKColor[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));
            SKColor[] decoded = new SKColor[length / 2];

            for(int i=0;i<decoded.Length;i++)
            {
                ushort data=reader.ReadUInt16(ByteOrder);
                int alpha=(data>>8)&0xFF;
                int intensity=data&0xFF;
                decoded[i] = new SKColor((byte)intensity, (byte)intensity, (byte)intensity, (byte)alpha);
            }
            reader.Close();
            return decoded;
        }

        public override byte[] EncodeColors(SKColor[] colors, int start, int length)
        {
            byte[] encoded = new byte[length * 2];

            for(int i=0; i<length; i++)
            {
                SKColor gray = ImageUtils.ToGrayScale(colors[start + i]);

                encoded[i * 2] = ByteOrder == ByteOrder.LittleEndian ? gray.Red : gray.Alpha;
                encoded[i * 2 + 1] = ByteOrder == ByteOrder.LittleEndian ? gray.Alpha : gray.Red;
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }
    }
}
