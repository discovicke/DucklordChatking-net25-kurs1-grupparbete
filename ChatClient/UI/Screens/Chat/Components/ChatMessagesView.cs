using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Screens.Chat.Components;

/// <summary>
/// Responsible for: wrapping MessageDTO into ChatMessage bubbles, measuring, scrolling and rendering.
/// </summary>
public class ChatMessagesView
{
    private readonly ScrollablePanel panel;
    private Rectangle bounds;

    private readonly List<ChatMessage> bubbles = new();
    private int lastMessageCount;
    private float lastContentHeight;
    private bool firstLoad = true;

    // layout constants
    private const float PaddingTop = 10f;
    private const float Spacing = 8f;
    private const float PaddingBottom = 10f;
    private const float LeftInset = 10f;
    private const float BottomTolerancePx = 24f;

    public ChatMessagesView(ScrollablePanel panel)
    {
        this.panel = panel;
    }

    public void SetBounds(Rectangle bounds)
    {
        this.bounds = bounds;
        panel.SetBounds(bounds);
    }

    public void UpdateMessages(IReadOnlyList<MessageDTO> messages, float contentWidth)
    {
        if (messages.Count == 0)
        {
            bubbles.Clear();
            lastMessageCount = 0;
            return;
        }

        // Only rebuild bubbles if count changed or width changed significantly
        if (bubbles.Count != messages.Count || Math.Abs(contentWidth - GetCurrentContentWidth()) > 0.5f)
        {
            bubbles.Clear();
            foreach (var m in messages)
            {
                bubbles.Add(new ChatMessage(m, contentWidth));
            }
        }
    }

    // Render and handle scroll/autoscroll. Should be called each frame.
    public void Render()
    {
        // Compute total content height using same spacing as draw
        float totalHeight = PaddingTop + PaddingBottom;
        foreach (var b in bubbles)
        {
            totalHeight += b.Height + Spacing;
        }

        // Determine if user was at bottom based on previous content height
        float prevMaxScroll = MathF.Max(0, lastContentHeight - bounds.Height);
        bool wasAtBottom = panel.ScrollOffset >= (prevMaxScroll - BottomTolerancePx);

        // Begin scroll region with current height (also sets scissor)
        panel.BeginScroll(totalHeight);

        // Auto-scroll on first load or if new messages arrived while user was at bottom
        bool hasMessages = bubbles.Count > 0;
        bool hasNewMessages = bubbles.Count > lastMessageCount;
        if ((firstLoad && hasMessages) || (hasNewMessages && wasAtBottom))
        {
            panel.ScrollToBottom();
            firstLoad = false;
        }

        // Draw bubbles
        float currentY = bounds.Y + PaddingTop;
        foreach (var b in bubbles)
        {
            float scrolledY = panel.GetScrolledY(currentY);
            if (panel.IsVisible(scrolledY, b.Height))
            {
                b.Draw(bounds.X + LeftInset, scrolledY);
            }
            currentY += b.Height + Spacing;
        }

        panel.EndScroll();

        // Track for next frame
        lastMessageCount = bubbles.Count;
        lastContentHeight = totalHeight;
    }

    public void ScrollToBottom() => panel.ScrollToBottom();

    private float GetCurrentContentWidth()
    {
        return MathF.Max(0, bounds.Width - LeftInset * 2);
    }
}

