using Raylib_cs;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens;

public static class RegisterScreenLayout
{
    public struct LayoutData
    {
        public Rectangle UserRect, PassRect, PassConfirmRect, RegisterRect, BackRect;
        public float LogoX, LogoY, LogoScale, LogoWidth, LogoHeight;
    }

    public static LayoutData Calculate(int logoWidth, int logoHeight)
    {
        var wrap = new UIWrapper();
        wrap.SetToFullWindow();
        
        float w = wrap.Width;
        float h = wrap.Height;

        // Logo
        float logoTargetW = w * 0.15f;
        float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.15f;
        float scaledLogoHeight = logoHeight * logoScale;
        float scaledLogoWidth = logoWidth * logoScale;
        float logoX = (w - scaledLogoWidth) / 2f;
        float logoY = h * 0.10f;

        // Fields & button - börja UNDER loggan
        float colTop = logoY + scaledLogoHeight + (h * 0.05f); // 5% gap
        float fieldW = w * 0.30f;
        float fieldH = h * 0.05f;
        float btnW = w * 0.25f;
        float btnH = h * 0.05f;
        float gap = h * 0.02f;

        return new LayoutData
        {
            UserRect = wrap.CenterHoriz(fieldW, fieldH, colTop),
            PassRect = wrap.CenterHoriz(fieldW, fieldH, colTop + (fieldH + gap)),
            PassConfirmRect = wrap.CenterHoriz(fieldW, fieldH, colTop + 2 * (fieldH + gap)),
            RegisterRect = wrap.CenterHoriz(btnW, btnH, colTop + 3 * (fieldH + gap)),
            BackRect = new Rectangle(10, 10, 100, 30),
            LogoScale = logoScale,
            LogoX = logoX,
            LogoY = logoY,
            LogoWidth = scaledLogoWidth,
            LogoHeight = scaledLogoHeight
        };
    }
}