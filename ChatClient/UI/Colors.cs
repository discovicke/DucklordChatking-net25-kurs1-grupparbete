using Raylib_cs;

namespace ChatClient.UI
{
    public static class Colors
    {
        // App background color
        public static Color BackgroundColor = new Color(15, 112, 152);
        
        // Outline color (Borders, focus, etc)
        public static Color OutlineColor = new Color(8, 60, 80, 255); // Dark teal
        
        // Panel color (boxes, menus, etc)
        public static Color PanelColor = new Color(20, 90, 120, 255); // Darkened teal

        // TextField colors
        public static Color TextFieldSelected = new Color(235, 245, 247, 255);
        public static Color TextFieldHovered = new Color(245, 250, 252, 255);
        public static Color TextFieldUnselected = new Color(210, 235, 240, 255);
        
        // Button colors
        public static Color ButtonDefault = new Color(210, 150, 40, 255);
        public static Color ButtonHovered = new Color(230, 170, 70, 255);
        public static Color ButtonPressed = new Color(180, 120, 30, 255);
        
        // Success / Accent / Highlights color
        public static Color AccentColor = Color.Gold;

        // Text colors
        public static Color TextColor = new Color(20, 40, 50, 255);
        public static Color PlaceholderText = new Color(140, 170, 180, 255);
        public static Color SubtleText = new Color(10, 70, 95);
    }
}