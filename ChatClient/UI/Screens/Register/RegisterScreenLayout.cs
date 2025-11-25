using Raylib_cs;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Layout;

namespace ChatClient.UI.Screens.Register;

/// <summary>
/// Responsible for: calculating layout positions for all UI elements on the registration screen.
/// Determines field sizes for username, password, confirm password, and register button placement.
/// </summary>
public static class RegisterScreenLayout
{
    public struct LayoutData
    {
        public Rectangle UserRect { get; init; }
        public Rectangle PassRect { get; init; }
        public Rectangle PassConfirmRect { get; init; }
        public Rectangle RegisterRect { get; init; }
        public Rectangle BackRect { get; init; }
        public float LogoX { get; init; }
        public float LogoY { get; init; }
        public float LogoScale { get; init; }
        public float LogoWidth { get; init; }
        public float LogoHeight { get; init; }
    }

    public static LayoutData Calculate(int logoWidth, int logoHeight)
    {
        var wrap = new UIWrapper();
        wrap.SetToFullWindow();

        float w = wrap.Width;
        float h = wrap.Height;

        const float Padding = 30f;
        const float GapBetweenItems = 10f;

        // 1) Logo: top 10px to middle of screen (50% height) minus 10px padding at bottom
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

        // 4) Stack vertically with 10px gaps
        float userY = fieldsStartY;
        float passY = userY + fieldH + GapBetweenItems;
        float passConfirmY = passY + fieldH + GapBetweenItems;
        float registerY = passConfirmY + fieldH + (GapBetweenItems * 3);

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
            RegisterRect = wrap.CenterHoriz(btnW, btnH, registerY)
        };
    }

}
