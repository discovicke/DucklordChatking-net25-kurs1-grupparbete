using System.Numerics;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Screens.Chat.Components;

/// <summary>
/// Responsible for: rendering online/offline user lists in a scrollable panel.
/// </summary>
public class UserListView(ScrollablePanel panel)
{
    private Rectangle rectangleBounds;

    public void SetBounds(Rectangle bounds)
    { 
        rectangleBounds = bounds;
        panel.SetBounds(bounds);
    }

    public void Render(IReadOnlyList<string> onlineUsers, IReadOnlyList<string> offlineUsers)
    {
        const float lineH = 22f;
        const float fontSize = 14f;
        const float inset = 10f;

        float totalHeight = lineH + (onlineUsers.Count * lineH) + 10 + lineH + (offlineUsers.Count * lineH);
        panel.BeginScroll(totalHeight);

        float x = rectangleBounds.X + inset;
        float y = rectangleBounds.Y + inset;

        // Online header
        float scrolledY = panel.GetScrolledY(y);
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "ONLINE", new Vector2(x, scrolledY), fontSize, 0.5f, Colors.Online);
        y += lineH;

        // Online users
        foreach (var user in onlineUsers)
        {
            scrolledY = panel.GetScrolledY(y);
            if (panel.IsVisible(scrolledY, lineH))
            {
                Raylib.DrawCircle((int)x + 5, (int)scrolledY + 7, 4f, Colors.Online);
                Raylib.DrawTextEx(ResourceLoader.RegularFont, user, new Vector2(x + 15, scrolledY), fontSize, 0.5f, Colors.UiText);
            }
            y += lineH;
        }

        y += 10;

        // Offline header
        scrolledY = panel.GetScrolledY(y);
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "OFFLINE", new Vector2(x, scrolledY), fontSize, 0.5f, Colors.Offline);
        y += lineH;

        // Offline users
        foreach (var user in offlineUsers)
        {
            scrolledY = panel.GetScrolledY(y);
            if (panel.IsVisible(scrolledY, lineH))
            {
                Raylib.DrawCircle((int)x + 5, (int)scrolledY + 7, 4f, Colors.Offline);
                Raylib.DrawTextEx(ResourceLoader.RegularFont, user, new Vector2(x + 15, scrolledY), fontSize, 0.5f, Colors.Offline);
            }
            y += lineH;
        }

        panel.EndScroll();
    }
}

