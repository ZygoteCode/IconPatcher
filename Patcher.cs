using System;
using System.Drawing;
using System.IO;

namespace IconPatcher
{
    public class Patcher
    {
        public static void PatchIcon(string executablePath, string iconPath)
        {
            if (!System.IO.File.Exists(executablePath))
            {
                throw new Exception("The specified executable file does not exist.");
            }

            if (!System.IO.Path.GetExtension(executablePath).ToLower().Equals(".exe"))
            {
                throw new Exception("The specified executable file has no a valid extension.");
            }

            if (!System.IO.File.Exists(iconPath))
            {
                throw new Exception("The specified icon file does not exist.");
            }

            if (!System.IO.Path.GetExtension(iconPath).ToLower().Equals(".ico"))
            {
                throw new Exception("The specified icon file has no a valid extension.");
            }

            try
            {
                IconEditor editor = new IconEditor();
                editor.InjectIcon(executablePath, iconPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to patch the icon.\r\n{ex.Message}\r\n{ex.StackTrace}\r\n{ex.Source}");
            }
        }
    }
}