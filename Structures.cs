using System.Runtime.InteropServices;

namespace IconPatcher
{
    public static class Structures
    {
        [StructLayout(LayoutKind.Sequential)]
        public partial struct ICONDIR
        {
            public ushort Reserved;
            public ushort Type;
            public ushort Count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public partial struct ICONDIRENTRY
        {
            public byte Width;
            public byte Height;
            public byte ColorCount;
            public byte Reserved;
            public ushort Planes;
            public ushort BitCount;
            public int BytesInRes;
            public int ImageOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public partial struct BITMAPINFOHEADER
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public uint ClrUsed;
            public uint ClrImportant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GRPICONDIR
        {
            public ushort wReserved;
            public ushort wType;
            public ushort wImageCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public partial struct GRPICONDIRENTRY
        {
            public byte Width;
            public byte Height;
            public byte ColorCount;
            public byte Reserved;
            public ushort Planes;
            public ushort BitCount;
            public int BytesInRes;
            public ushort ID;
        }
    }
}