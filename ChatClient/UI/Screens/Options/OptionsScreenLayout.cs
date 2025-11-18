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
        public Rectangle UserRect, PassRect, PassConfirmRect, ConfirmRect, BackRect;
        public Rectangle BtnWindowedRect, BtnFullscreenRect;
        public float LogoX, LogoY, LogoScale, LogoWidth, LogoHeight;
    }

    public static LayoutData Calculate(int logoWidth, int logoHeight)
    {
        var wrap = new UIWrapper();
        wrap.SetToFullWindow();

        float w = wrap.Width;
        float h = wrap.Height;

        const float Padding = 30f;
        const float GapBetweenItems = 10f;

        // 1) Logo: top padding to middle of screen (50% height) minus padding at bottom
        float logoY = Padding;
        float logoAvailableH = (h * 0.5f) - (Padding * 2);

        // Scale by height, then clamp to width if needed
        float logoScale = (logoHeight > 0)
            ? (logoAvailableH / logoHeight)
            : 1f;
        float scaledLogoW = logoWidth * logoScale;
        float scaledLogoH = logoHeight * logoScale;

        // Clamp to 90% of width if too wide
        if (scaledLogoW > w * 0.90f && logoWidth > 0)
        {
            logoScale = (w * 0.90f) / logoWidth;
            scaledLogoW = logoWidth * logoScale;
            scaledLogoH = logoHeight * logoScale;
        }

        float logoX = (w - scaledLogoW) / 2f;

        // 2) Fields and button start at middle of screen (50% y)
        float fieldsStartY = h * 0.5f;

        // 3) Element sizes
        float fieldW = w * 0.45f;
        float fieldH = h * 0.035f;
        float btnW = w * 0.25f;
        float btnH = h * 0.045f;
        float windowBtnW = w * 0.15f;
        float windowBtnH = h * 0.04f;

        // 4) Stack vertically with 10px gaps
        float userY = fieldsStartY;
        float passY = userY + fieldH + GapBetweenItems;
        float passConfirmY = passY + fieldH + GapBetweenItems;
        float confirmY = passConfirmY + fieldH + (GapBetweenItems * 3);
        
        // Window mode buttons below confirm button
        float windowBtnY = confirmY + btnH + (GapBetweenItems * 3);

        return new LayoutData
        {
            BackRect = new Rectangle(10, 10, 100, 30),

            LogoScale = logoScale,
            LogoX = logoX,
            LogoY = logoY,
            LogoWidth = scaledLogoW,
            LogoHeight = scaledLogoH,

            UserRect = wrap.CenterHoriz(fieldW, fieldH, userY),
            PassRect = wrap.CenterHoriz(fieldW, fieldH, passY),
            PassConfirmRect = wrap.CenterHoriz(fieldW, fieldH, passConfirmY),
            ConfirmRect = wrap.CenterHoriz(btnW, btnH, confirmY),

            // Window mode buttons side-by-side, centered
            BtnWindowedRect = new Rectangle(
                (w / 2) - windowBtnW - (GapBetweenItems / 2),
                windowBtnY,
                windowBtnW,
                windowBtnH),
            BtnFullscreenRect = new Rectangle(
                (w / 2) + (GapBetweenItems / 2),
                windowBtnY,
                windowBtnW,
                windowBtnH)
        };
    }
}
