using Raylib_cs;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens;

public static class ChatScreenLayout
{
    public struct LayoutData
    {
        public Rectangle ChatRect, InputRect, SendRect, BackRect, UserListRect;
        public float LogoX, LogoY, LogoScale, LogoWidth, LogoHeight;
    }

    public static LayoutData Calculate(int logoWidth, int logoHeight)
    {
        var wrap = new UIWrapper(); 
        wrap.SetToFullWindow(); // Must call this to get current window dimensions
        
        float w = wrap.Width;
        float h = wrap.Height;

        // Logo
        float logoTargetW = w * 0.15f;
        float logoScale = logoWidth > 0 ? logoTargetW / logoWidth : 0.14f;
        float scaledLogoHeight = logoHeight * logoScale;
        float scaledLogoWidth = logoWidth * logoScale;
        float logoX = w - scaledLogoWidth - 20f;
        float logoY = 20f;
        
        // Chat window: 10px spacing to logo border and top left of screen
        float chatX = 10f;
        float chatW = logoX - chatX - 10f; // To logos left border minus 10px spacing
        float chatH = h * 0.65f;
        float chatY = h * 0.10f;
        
        // Input field: samma bredd som chat, men lämna plats för send-knappen
        float inputY = chatY + chatH + h * 0.03f;
        float inputH = h * 0.14f;
        float btnW = 100f; // Fast bredd för send-knapp
        float inputW = chatW - btnW - 10f; // Minus knappbredd och spacing
        
        // User list (höger om chat, under loggan)
        float userListX = logoX;
        float userListY = logoY + scaledLogoHeight + 10f; // 10px under loggan
        float userListW = scaledLogoWidth;
        float userListH = h - userListY - 10f; // Till fönstrets nederkant minus 10px
        
        var chatRect = new Rectangle(chatX, chatY, chatW, chatH);
        var inputRect = new Rectangle(chatX, inputY, inputW, inputH);
        var sendRect = new Rectangle(chatX + inputW + 10f, inputY, btnW, inputH);
        var backRect = new Rectangle(10, 10, 100, 30);
        var userListRect = new Rectangle(userListX, userListY, userListW, userListH);

        return new LayoutData
        {
            ChatRect = chatRect,
            InputRect = inputRect,
            SendRect = sendRect,
            BackRect = backRect,
            UserListRect = userListRect,
            LogoScale = logoScale,
            LogoX = logoX,
            LogoY = logoY,
            LogoWidth = scaledLogoWidth,
            LogoHeight = scaledLogoHeight
        };
    }
}