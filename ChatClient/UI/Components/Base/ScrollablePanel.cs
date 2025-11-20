using ChatClient.Core;
using ChatClient.Core.Input;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Base;

/// <summary>
/// Responsible for: managing scrollable content areas with mouse wheel support and optional scrollbar rendering.
/// Handles DPI-aware scissor clipping, scroll offset calculation, and visibility checking for content optimization.
/// </summary>
public class ScrollablePanel(Rectangle bounds, float scrollSpeed = 20f, bool showScrollbar = true)
{
    private Rectangle Bounds { get; set; } = bounds;
    public float ScrollOffset { get; private set; }
    private float ContentHeight { get; set; }
    private float ScrollSpeed { get; } = scrollSpeed;
    private bool ShowScrollbar { get; } = showScrollbar;
    
    public void SetBounds(Rectangle newBounds) => Bounds = newBounds;

    public void BeginScroll(float totalContentHeight)
    {
        ContentHeight = totalContentHeight;

        if (MouseInput.IsHovered(Bounds))
        {
            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                ScrollOffset -= wheel * ScrollSpeed;
                float maxScroll = Math.Max(0, ContentHeight - Bounds.Height);
                ScrollOffset = Math.Clamp(ScrollOffset, 0, maxScroll);
            }
        }
        
        // Scissor mode to cut content outside of bounds
        Raylib.BeginScissorMode((int)Bounds.X, (int)Bounds.Y, 
            (int)Bounds.Width, (int)Bounds.Height);
    }

    public void EndScroll()
    {
        Raylib.EndScissorMode();
        
        // Draw scrollbar if needed
        if (ShowScrollbar && ContentHeight > Bounds.Height)
        {
            DrawScrollbar();
        }
    }

    private void DrawScrollbar()
    {
        float padding = 10f;
        float scrollbarWidth = 6f;
        float scrollbarX = Bounds.X + Bounds.Width - scrollbarWidth - 4f;
        
        float scrollbarHeight = (Bounds.Height / ContentHeight) * Bounds.Height;
        scrollbarHeight = Math.Max(20f, scrollbarHeight);
        
        float maxScrollOffset = ContentHeight - Bounds.Height;
        float scrollbarY = Bounds.Y + (ScrollOffset / maxScrollOffset) * 
            (Bounds.Height - scrollbarHeight);
        
        scrollbarY += padding;
        scrollbarHeight -= padding * 2;


        var scrollbarRect = new Rectangle(scrollbarX, scrollbarY, 
            scrollbarWidth, scrollbarHeight);

        Raylib.DrawRectangleRounded(scrollbarRect, 0.5f, 4, 
            Colors.BackgroundColor);
    }

    public float GetScrolledY(float originalY) => originalY - ScrollOffset;
    
    public bool IsVisible(float y, float height)
    {
        return y + height >= Bounds.Y && y <= Bounds.Y + Bounds.Height;
    }
    
    public void ScrollToBottom()
    {
        float maxScroll = Math.Max(0, ContentHeight - Bounds.Height);
        ScrollOffset = maxScroll;
    }
}
