using Raylib_cs;

namespace ChatClient.UI.Theme
{
    /// <summary>
    /// Responsible for: defining the global color palette for the entire application UI.
    /// Provides consistent colors for backgrounds, buttons, text fields, and status indicators (success/fail/accent).
    /// </summary>
    public static class Colors
    {
        // Brand
        public static Color BrandBlue = new Color(11, 112, 154, 255); // #0B709A  (logga)
        public static Color BrandGold = new Color(245, 205, 51, 255); // #F5CD33  (knappar / CTA)
        public static Color BrandBeige = new Color(242, 230, 191, 255); // #F2E6BF
        public static Color BrandGreen = new Color(0, 146, 51, 255); // #009233
        public static Color BrandBrown = new Color(175, 86, 14, 255); // #AF560E
        public static Color NewBlue = new Color(33, 109, 148); // #216D94

        // App layout
        public static Color BackgroundColor = NewBlue; // använd loggans blå som bakgrund
        public static Color OutlineColor = new Color(8, 60, 80, 255); // mörkare teal för ramar
        public static Color PanelColor = new Color(20, 90, 120, 255); // befintlig (fungerar med brandblue)

        // TextField
        public static Color TextFieldSelected = new Color(235, 245, 247, 255);
        public static Color TextFieldHovered = new Color(245, 250, 252, 255);
        public static Color TextFieldUnselected = new Color(210, 235, 240, 255);

        // Buttons (CTA)
        public static Color ButtonDefault = BrandGold; // #F5CD33 (primär knappfärg)
        public static Color ButtonHovered = new Color(255, 220, 100, 255); // lite ljusare guld
        public static Color ButtonPressed = new Color(220, 180, 30, 255);

        // Chat bubbles (kontrastvärt, retro-modern)
        public static Color ChatBubbleOther = new Color(100, 113, 120, 255); // #647178 - mörk, kall grå ( inkommande )
        public static Color ChatBubbleSelf = new Color(24, 120, 156, 255); // #18789C - egen (utgående)

        // Textfärger för bubblor
        public static Color ChatBubbleOtherText = new Color(245, 249, 250, 255); // nästintill vit, mjuk
        public static Color ChatBubbleSelfText = new Color(20, 40, 50, 255); // mörk text på egen bubbla
        public static Color Success = new Color(140, 255, 210, 255); // Mint Green
        public static Color Fail = new Color(255, 110, 110, 255);    // Soft Coral Red


        // Online/offline indicators
        public static Color Online = BrandGreen; // #009233
        public static Color Offline = new Color(125, 138, 146, 255); // neutral mellangrå

        // Text
        public static Color TextColor = new Color(20, 40, 50, 255);
        public static Color PlaceholderText = new Color(140, 170, 180, 255);
        public static Color UiText = new Color(230, 240, 245, 255); // ljus text för mörka paneler
        public static Color SubtleText = new Color(10, 70, 95);
    }
}