using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IconPatcher
{
    public class IconEditor
    {
        private Structures.ICONDIR iconDir = new Structures.ICONDIR();
        private Structures.ICONDIRENTRY[] iconEntry;
        private byte[][] iconImage;
        public int ImageCount => iconDir.Count;
        private const uint RT_ICON = 3, RT_GROUP_ICON = 14;

        public byte[] GetImageData(int index)
        {
            return iconImage[index];
        }

        public void InjectIcon(string exeFileName, string iconFileName)
        {
            InjectIcon(exeFileName, iconFileName, 1, 1);
        }

        public void InjectIcon(string exeFileName, string iconFileName, uint iconGroupID, uint iconBaseID)
        {
            IconEditor iconFile = FromFile(iconFileName);
            IntPtr hUpdate = NativeMethods.BeginUpdateResource(exeFileName, false);
            byte[] data = iconFile.CreateIconGroupData(iconBaseID);
            NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_GROUP_ICON), new IntPtr(iconGroupID), 0, data, data.Length);

            for (int i = 0, loopTo = iconFile.ImageCount - 1; i <= loopTo; i++)
            {
                byte[] image = iconFile.GetImageData(i);
                NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_ICON), new IntPtr(iconBaseID + i), 0, image, image.Length);
            }

            NativeMethods.EndUpdateResource(hUpdate, false);
        }

        public IconEditor FromFile(string filename)
        {
            IconEditor instance = new IconEditor();

            try
            {
                byte[] fileBytes = File.ReadAllBytes(filename);
                GCHandle pinnedBytes = GCHandle.Alloc(fileBytes, GCHandleType.Pinned);
                instance.iconDir = (Structures.ICONDIR)Marshal.PtrToStructure(pinnedBytes.AddrOfPinnedObject(), typeof(Structures.ICONDIR));
                instance.iconEntry = new Structures.ICONDIRENTRY[instance.iconDir.Count];
                instance.iconImage = new byte[instance.iconDir.Count][];
                int offset = Marshal.SizeOf(instance.iconDir);
                Type iconDirEntryType = typeof(Structures.ICONDIRENTRY);
                int size = Marshal.SizeOf(iconDirEntryType);

                for (int i = 0, loopTo = instance.iconDir.Count - 1; i <= loopTo; i++)
                {
                    var entry = (Structures.ICONDIRENTRY)Marshal.PtrToStructure(new IntPtr(pinnedBytes.AddrOfPinnedObject().ToInt64() + offset), iconDirEntryType);
                    instance.iconEntry[i] = entry;
                    instance.iconImage[i] = new byte[entry.BytesInRes];
                    Buffer.BlockCopy(fileBytes, entry.ImageOffset, instance.iconImage[i], 0, entry.BytesInRes);
                    offset += size;
                }

                pinnedBytes.Free();
            }
            catch
            {

            }

            return instance;
        }

        public byte[] CreateIconGroupData(uint iconBaseID)
        {
            try
            {
                int sizeOfIconGroupData = Marshal.SizeOf(typeof(Structures.ICONDIR)) + (Marshal.SizeOf(typeof(Structures.GRPICONDIRENTRY)) * ImageCount);
                byte[] data = new byte[sizeOfIconGroupData];
                GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                Marshal.StructureToPtr(iconDir, pinnedData.AddrOfPinnedObject(), false);
                int offset = Marshal.SizeOf(iconDir);

                for (int i = 0, loopTo = ImageCount - 1; i <= loopTo; i++)
                {
                    Structures.GRPICONDIRENTRY grpEntry = new Structures.GRPICONDIRENTRY();
                    Structures.BITMAPINFOHEADER bitmapheader = new Structures.BITMAPINFOHEADER();
                    GCHandle pinnedBitmapInfoHeader = GCHandle.Alloc(bitmapheader, GCHandleType.Pinned);
                    Marshal.Copy(GetImageData(i), 0, pinnedBitmapInfoHeader.AddrOfPinnedObject(), Marshal.SizeOf(typeof(Structures.BITMAPINFOHEADER)));
                    pinnedBitmapInfoHeader.Free();
                    grpEntry.Width = iconEntry[i].Width;
                    grpEntry.Height = iconEntry[i].Height;
                    grpEntry.ColorCount = iconEntry[i].ColorCount;
                    grpEntry.Reserved = iconEntry[i].Reserved;
                    grpEntry.Planes = bitmapheader.Planes;
                    grpEntry.BitCount = bitmapheader.BitCount;
                    grpEntry.BytesInRes = iconEntry[i].BytesInRes;
                    ushort result = unchecked((ushort)(iconBaseID + i));
                    grpEntry.ID = result;
                    Marshal.StructureToPtr(grpEntry, new IntPtr(pinnedData.AddrOfPinnedObject().ToInt64() + offset), false);
                    offset += Marshal.SizeOf(typeof(Structures.GRPICONDIRENTRY));
                }

                pinnedData.Free();
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}