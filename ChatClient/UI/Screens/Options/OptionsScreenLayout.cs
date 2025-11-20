// File: ChatClient/UI/Screens/Options/OptionsScreenLayout.cs
using Raylib_cs;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Layout;

namespace ChatClient.UI.Screens.Options;

public static class OptionsScreenLayout
{
    public struct LayoutData
    {
        public Rectangle UserRect { get; init; }
        public Rectangle PassRect { get; init; }
        public Rectangle PassConfirmRect { get; init; }
        public Rectangle ConfirmRect { get; init; }
        public Rectangle BackRect { get; init; }
        public Rectangle ToggleWindowedRect { get; init; }
        public Rectangle ToggleFullscreenRect { get; init; }
        public Rectangle ToggleMuteRect { get; init; }
        public float LogoX { get; init; }
        public float LogoY { get; init; }
        public float LogoScale { get; init; }
        public float LogoWidth { get; set; }
        public float LogoHeight { get; set; }
    }

    public static LayoutData Calculate(int logoWidth, int logoHeight)
    {
        var wrap = new UIWrapper();
        wrap.SetToFullWindow();

        float w = wrap.Width;
        float h = wrap.Height;

        const float Padding = 30f;
        const float GapBetweenItems = 10f;

        // Logo
        float logoY = Padding;
        float logoAvailableH = (h * 0.5f) - (Padding * 2);
        float logoScale = (logoHeight > 0) ? (logoAvailableH / logoHeight) : 1f;
        float scaledLogoW = logoWidth * logoScale;
        float scaledLogoH = logoHeight * logoScale;
        if (scaledLogoW > w * 0.90f && logoWidth > 0)
        {
            logoScale = (w * 0.90f) / logoWidth;
            scaledLogoW = logoWidth * logoScale;
            scaledLogoH = logoHeight * logoScale;
        }
        float logoX = (w - scaledLogoW) / 2f;

        // Fields start
        float fieldsStartY = h * 0.5f;

        // Element sizes
        float fieldW = w * 0.45f;
        float fieldH = h * 0.035f;
        float btnW = w * 0.25f;
        float btnH = h * 0.045f;

        // Vertical positions
        float userY = fieldsStartY;
        float passY = userY + fieldH + GapBetweenItems;
        float passConfirmY = passY + fieldH + GapBetweenItems;
        float confirmY = passConfirmY + fieldH + (GapBetweenItems * 3);

        // Toggle wrappers below confirm
        float toggleBoxSize = h * 0.025f; // intended checkbox size
        float labelHeightApprox = toggleBoxSize * 0.8f;
        float wrapperW = toggleBoxSize * 5f;
        float wrapperH = toggleBoxSize + 5f + labelHeightApprox + 8f; // 8px internal padding
        float toggleY = confirmY + btnH + (GapBetweenItems * 3);

        // --- Calc position for buttons ---
        float centerX = w / 2f;
        float totalWidth = (wrapperW * 3) + (GapBetweenItems * 2);
        float startX = centerX - (totalWidth / 2f);

        float windowedX = startX;
        float fullscreenX = startX + wrapperW + GapBetweenItems;
        float muteX = fullscreenX + wrapperW + GapBetweenItems;

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

            ToggleWindowedRect = new Rectangle(windowedX, toggleY, wrapperW, wrapperH),
            ToggleFullscreenRect = new Rectangle(fullscreenX, toggleY, wrapperW, wrapperH),
            ToggleMuteRect = new Rectangle(muteX, toggleY, wrapperW, wrapperH)
        };
    }
}
