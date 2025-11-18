using Raylib_cs;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Layout;

namespace ChatClient.UI.Screens.Options;

/// <summary>
/// Responsible for: calculating layout positions for all UI elements on the options/settings screen.
/// Determines field sizes for account updates and window mode toggle buttons (windowed/fullscreen).
/// </summary>
public static class OptionsScreenLayout
{
    public struct LayoutData
    {
        public Rectangle UserRect, PassRect, PassConfirmRect, ConfirmRect, BackRect, OptionsRect;
        public Rectangle BtnWindowedRect, BtnFullscreenRect;
        public float LogoX, LogoY, LogoScale, LogoWidth, LogoHeight;
    }

    public static LayoutData Calculate(int logoWidth)
    {
        var wrap = new UIWrapper(); 
        wrap.SetToFullWindow(); // Must call this to get current window dimensions
        
        float w = wrap.Width;
        float h = wrap.Height;

        float fieldW = w * 0.30f;
        float fieldH = h * 0.05f;
        float btnW = w * 0.25f;
        float btnH = h * 0.05f;
        float gap = h * 0.02f;
        float colTop = h * 0.45f;

        float btnTop = colTop + 4 * (fieldH + gap);
        float windowBtnW = w * 0.15f;
        float windowBtnH = h * 0.04f;
        float windowBtnGap = w * 0.02f;

        float logoTargetW = w * 0.15f;
        float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.15f;

        return new LayoutData
        {
            UserRect = wrap.CenterHoriz(fieldW, fieldH, colTop),
            PassRect = wrap.CenterHoriz(fieldW, fieldH, colTop + (fieldH + gap)),
            PassConfirmRect = wrap.CenterHoriz(fieldW, fieldH, colTop + 2 * (fieldH + gap)),
            ConfirmRect = wrap.CenterHoriz(btnW, btnH, colTop + 3 * (fieldH + gap)),
            BackRect = new Rectangle(10, 10, 100, 30),
            // --- Options ---
            OptionsRect = new Rectangle(10, 100, 100, 30),
            // --- ---
            LogoScale = logoScale,
            BtnWindowedRect = new Rectangle(w / 2 - windowBtnW - windowBtnGap / 2, btnTop, windowBtnW, windowBtnH),
            BtnFullscreenRect = new Rectangle(w / 2 + windowBtnGap / 2, btnTop, windowBtnW, windowBtnH),
            LogoX = (w - logoWidth * logoScale) / 2f,
            LogoY = h * 0.10f,
            LogoWidth = logoWidth * logoScale,
            LogoHeight = logoWidth * logoScale
        };
    }
}