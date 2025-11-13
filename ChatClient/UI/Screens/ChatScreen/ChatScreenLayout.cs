using Raylib_cs;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens;

public static class ChatScreenLayout
{
    public struct LayoutData
    {
        public Rectangle ChatRect, InputRect, SendRect, BackRect;
        public float LogoX, LogoY, LogoScale;
    }

    public static LayoutData Calculate(int logoWidth)
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();

        var wrap = new UIWrapper(); wrap.SetToFullWindow();

        float chatW = w * 0.70f;
        float chatH = h * 0.65f;
        float inputH = h * 0.14f;
        float btnW = w * 0.12f;

        float chatX = (w - chatW) * 0.5f;
        float chatY = h * 0.10f;
        float inputY = chatY + chatH + h * 0.03f;

        float logoTargetW = w * 0.15f;
        float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.14f;

        var chatRect = new Rectangle(chatX, chatY, chatW, chatH);
        var inputRect = new Rectangle(chatX, inputY, chatW - (btnW + 10), inputH);
        var sendRect = new Rectangle(chatX + chatW - btnW, inputY, btnW, inputH);
        var backRect = new Rectangle(10, 10, 100, 30);

        return new LayoutData
        {
            ChatRect = chatRect,
            InputRect = inputRect,
            SendRect = sendRect,
            BackRect = backRect,
            LogoScale = logoScale,
            LogoX = w - (logoWidth * logoScale) - 20f,
            LogoY = 20f
        };
    }
}