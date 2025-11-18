using Raylib_cs;

namespace ChatClient.UI.Theme
{
    /// <summary>
    /// Responsible for: defining the global color palette for the entire application UI.
    /// Provides consistent colors for backgrounds, buttons, text fields, and status indicators (success/fail/accent).
    /// </summary>
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
        public static Color ChatBubbleOtherText = new Color(240, 245, 247, 255); // Soft white-blue
        
        // Button colors
        public static Color ButtonDefault = new Color(210, 150, 40, 255);
        public static Color ButtonHovered = new Color(230, 170, 70, 255);
        public static Color ButtonPressed = new Color(180, 120, 30, 255);
        
        // Success / Accent / Highlights color
        public static Color AccentColor = Color.Gold;
        public static Color Success = new Color(120, 255, 200, 255); 
        public static Color Fail = new Color(255, 130, 120, 255);
        
        // Chat bubbles
        public static Color ChatBubbleOther = new Color(110, 125, 135, 255); // Ljusgrå
        public static Color ChatBubbleSelf = new Color(30, 135, 170, 255);   // Teal-blå (egen)

        // Online/offline indicators
        public static Color Online = Color.Gold;
        public static Color Offline = new Color(125, 138, 146, 255);         // Neutral mellangrå
        
        // Text colors
        public static Color TextColor = new Color(20, 40, 50, 255);
        public static Color PlaceholderText = new Color(140, 170, 180, 255);
        public static Color SubtleText = new Color(10, 70, 95);
        public static Color InputText = new Color(20, 40, 50, 255); // mörk text
        public static Color UiText = new Color(230, 240, 245, 255); // ljus text (på paneler)
    }
}