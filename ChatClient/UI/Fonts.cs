using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace ChatClient.UI
{
    public static class Fonts
    {
        public static Font lightFont;
        public static Font mediumFont;
        public static Font boldFont;

        static Fonts()
        {
            List<int> chars = new List<int>();

            // Basic ASCII
            for (int i = 32; i <= 126; i++) chars.Add(i);

            // Support for ÅÄÖ etc
            for (int i = 192; i <= 255; i++) chars.Add(i);

            lightFont = Raylib.LoadFontEx("Resources/JetBrainsMono-Light.ttf", 32, chars.ToArray(), chars.Count);
            mediumFont = Raylib.LoadFontEx("Resources/JetBrainsMono-Medium.ttf", 32, chars.ToArray(), chars.Count);
            boldFont = Raylib.LoadFontEx("Resources/JetBrainsMono-Bold.ttf", 32, chars.ToArray(), chars.Count);

        }

    }
}