using Raylib_cs;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens
{
    // "Where should all fields be placed?""
    public static class StartScreenLayout
    {
        public struct LayoutData
        {
            public Rectangle UserRect, PassRect, LoginRect, RegisterRect, OptionsRect;
            public float LogoX, LogoY, LogoScale, ScreenHeight;
        }

        public static LayoutData Calculate(int logoWidth)
        {
            // Create UI wrapper for layout
            // Covers full window - must be called each time to get current window size
            var wrapper = new UIWrapper();
            wrapper.SetToFullWindow(); // This updates Width/Height based on current window size
            
            // Use wrapper dimensions for all calculations
            float screenWidth = wrapper.Width;
            float screenHeight = wrapper.Height;
            
            // Dynamic sizing based on screen dimensions
            float fieldWidth = screenWidth * 0.3f;
            float fieldHeight = screenHeight * 0.05f;
            float buttonWidth = screenWidth * 0.125f;
            float buttonHeight = screenHeight * 0.05f;
            float gap = screenHeight * 0.02f;
            // Starting top position (Y axis) for first column
            float colTop = screenHeight * 0.45f;

            float logoTargetWidth = screenWidth * 0.15f;
            float logoScale = logoWidth > 0 ? logoTargetWidth / logoWidth : 0.15f;

            return new LayoutData
            {
                // Calculate rectangles for each component
                // Centered horizontally in the window
                UserRect = wrapper.CenterHoriz(fieldWidth, fieldHeight, colTop),
                PassRect = wrapper.CenterHoriz(fieldWidth, fieldHeight, colTop + fieldHeight + gap),
                LoginRect = wrapper.CenterHoriz(buttonWidth, buttonHeight, colTop + 2 * (fieldHeight + gap)),
                RegisterRect = wrapper.CenterHoriz(buttonWidth, buttonHeight, colTop + 3 * (fieldHeight + gap)),
                OptionsRect = wrapper.CenterHoriz(buttonWidth, buttonHeight, colTop + 4 * (fieldHeight + gap)),
                LogoScale = logoScale,
                LogoX = (screenWidth - logoWidth * logoScale) / 2f,
                LogoY = screenHeight * 0.10f,
                ScreenHeight = screenHeight
            };
        }
    }
}
