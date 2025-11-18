﻿using Raylib_cs;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Layout;

namespace ChatClient.UI.Screens.Start
{
    /// <summary>
    /// Responsible for: calculating layout positions for all UI elements on the start/login screen.
    /// Determines field sizes, button positions, and logo placement based on window dimensions.
    /// </summary>
    public static class StartScreenLayout
    {
        public struct LayoutData
        {
            public Rectangle UserRect, PassRect, LoginRect, RegisterRect;
            public float LogoX, LogoY, LogoWidth, LogoHeight, LogoScale, ScreenHeight;
        }

        public static LayoutData Calculate(int logoWidth, int logoHeight)
        {
            // Create UI wrapper for layout
            // Covers full window - must be called each time to get current window size
            var wrapper = new UIWrapper();
            wrapper.SetToFullWindow(); // This updates Width/Height based on current window size
            
            // Use wrapper dimensions for all calculations
            float w = wrapper.Width;
            float h = wrapper.Height;
            
            float colTop = h * 0.45f;
            
            // Logo
            float logoTargetW = w * 0.15f;
            float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.15f;
            float scaledLogoHeight = logoHeight * logoScale;
            float scaledLogoWidth = logoWidth * logoScale;
            float logoX = (w - scaledLogoWidth) / 2f;
            float logoY = h * 0.10f;
            
            // Dynamic sizing based on screen dimensions
            float fieldWidth = w * 0.3f;
            float fieldHeight = h * 0.05f;
            float buttonWidth = w * 0.25f;
            float buttonHeight = h * 0.05f;
            float gap = h * 0.02f;
            // Starting top position (Y axis) for first column


            return new LayoutData
            {
                // Calculate rectangles for each component
                // Centered horizontally in the window
                UserRect = wrapper.CenterHoriz(fieldWidth, fieldHeight, colTop),
                PassRect = wrapper.CenterHoriz(fieldWidth, fieldHeight, colTop + fieldHeight + gap),
                LoginRect = wrapper.CenterHoriz(buttonWidth, buttonHeight, colTop + 2 * (fieldHeight + gap)),
                RegisterRect = wrapper.CenterHoriz(buttonWidth, buttonHeight, colTop + 3 * (fieldHeight + gap)),
                LogoScale = logoScale,
                LogoX = logoX,
                LogoY = logoY,
                LogoWidth = scaledLogoWidth,
                LogoHeight = scaledLogoHeight,
                ScreenHeight = h
            };
        }
    }
}
