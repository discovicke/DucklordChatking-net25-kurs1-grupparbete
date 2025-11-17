using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components;

public class ScrollablePanel
{
    private Rectangle bounds;
    private float scrollOffset;
    private float contentHeight;
    private readonly float scrollSpeed;
    private readonly bool showScrollbar;

    public float ScrollOffset => scrollOffset;

    public ScrollablePanel(Rectangle bounds, float scrollSpeed = 20f, bool showScrollbar = true)
    {
        this.bounds = bounds;
        this.scrollSpeed = scrollSpeed;
        this.showScrollbar = showScrollbar;
    }

    public void SetBounds(Rectangle newBounds) => bounds = newBounds;

    public void BeginScroll(float totalContentHeight)
    {
        contentHeight = totalContentHeight;

        if (MouseInput.IsHovered(bounds))
        {
            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                scrollOffset -= wheel * scrollSpeed;
                float maxScroll = Math.Max(0, contentHeight - bounds.Height);
                scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
            }
        }
        
        // Scissor mode to cut content outside of bounds
        Raylib.BeginScissorMode((int)bounds.X, (int)bounds.Y, 
            (int)bounds.Width, (int)bounds.Height);
    }

    public void EndScroll()
    {
        Raylib.EndScissorMode();
        
        // Draw scrollbar if needed
        if (showScrollbar && contentHeight > bounds.Height)
        {
            DrawScrollbar();
        }
    }

    private void DrawScrollbar()
    {
        float padding = 10f;
        float scrollbarWidth = 6f;
        float scrollbarX = bounds.X + bounds.Width - scrollbarWidth - 4f;
        
        float scrollbarHeight = (bounds.Height / contentHeight) * bounds.Height;
        scrollbarHeight = Math.Max(20f, scrollbarHeight);
        
        float maxScrollOffset = contentHeight - bounds.Height;
        float scrollbarY = bounds.Y + (scrollOffset / maxScrollOffset) * 
            (bounds.Height - scrollbarHeight);
        
        scrollbarY += padding;
        scrollbarHeight -= padding * 2;


        var scrollbarRect = new Rectangle(scrollbarX, scrollbarY, 
            scrollbarWidth, scrollbarHeight);

        Raylib.DrawRectangleRounded(scrollbarRect, 0.5f, 4, 
            Colors.BackgroundColor);
    }

    public float GetScrolledY(float originalY) => originalY - scrollOffset;
    
    public bool IsVisible(float y, float height)
    {
        return y + height >= bounds.Y && y <= bounds.Y + bounds.Height;
    }
}
