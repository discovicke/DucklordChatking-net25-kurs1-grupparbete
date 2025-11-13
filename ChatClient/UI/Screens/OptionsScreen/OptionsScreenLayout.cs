using Raylib_cs;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens;

public static class OptionsScreenLayout
{
    public struct LayoutData
    {
        public Rectangle UserRect, PassRect, PassConfirmRect, ConfirmRect, BackRect;
        public float LogoX, LogoY, LogoScale;
    }

    public static LayoutData Calculate(int logoWidth)
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();

        var wrap = new UIWrapper(); wrap.SetToFullWindow();

        float fieldW = w * 0.30f;
        float fieldH = h * 0.05f;
        float btnW = w * 0.16f;
        float btnH = h * 0.05f;
        float gap = h * 0.02f;
        float colTop = h * 0.40f;

        float logoTargetW = w * 0.15f;
        float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.15f;

        return new LayoutData
        {
            UserRect = wrap.CenterHoriz(fieldW, fieldH, colTop),
            PassRect = wrap.CenterHoriz(fieldW, fieldH, colTop + (fieldH + gap)),
            PassConfirmRect = wrap.CenterHoriz(fieldW, fieldH, colTop + 2 * (fieldH + gap)),
            ConfirmRect = wrap.CenterHoriz(btnW, btnH, colTop + 3 * (fieldH + gap)),
            BackRect = new Rectangle(10, 10, 100, 30),
            LogoScale = logoScale,
            LogoX = (w - logoWidth * logoScale) / 2f,
            LogoY = h * 0.10f
        };
    }
}