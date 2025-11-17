using System.Collections.Generic;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Components;

/// <summary>
/// Responsible for: wrapping MessageDTO into ChatMessage bubbles, measuring, scrolling and rendering.
/// </summary>
public class ChatMessagesView
{
    private readonly ScrollablePanel _panel;
    private Rectangle _bounds;

    private readonly List<ChatMessage> _bubbles = new();
    private int _lastMessageCount;
    private float _lastContentHeight;
    private bool _firstLoad = true;

    // layout constants
    private const float PaddingTop = 10f;
    private const float Spacing = 8f;
    private const float PaddingBottom = 10f;
    private const float LeftInset = 10f;
    private const float BottomTolerancePx = 24f;

    public ChatMessagesView(ScrollablePanel panel)
    {
        _panel = panel;
    }

    public void SetBounds(Rectangle bounds)
    {
        _bounds = bounds;
        _panel.SetBounds(bounds);
    }

    public void UpdateMessages(IReadOnlyList<MessageDTO> messages, float contentWidth)
    {
        if (messages.Count == 0)
        {
            _bubbles.Clear();
            _lastMessageCount = 0;
            return;
        }

        // Only rebuild bubbles if count changed or width changed significantly
        if (_bubbles.Count != messages.Count || Math.Abs(contentWidth - GetCurrentContentWidth()) > 0.5f)
        {
            _bubbles.Clear();
            foreach (var m in messages)
            {
                _bubbles.Add(new ChatMessage(m, contentWidth));
            }
        }
    }

    // Render and handle scroll/autoscroll. Should be called each frame.
    public void Render()
    {
        // Compute total content height using same spacing as draw
        float totalHeight = PaddingTop + PaddingBottom;
        foreach (var b in _bubbles)
            totalHeight += b.Height + Spacing;

        // Determine if user was at bottom based on previous content height
        float prevMaxScroll = MathF.Max(0, _lastContentHeight - _bounds.Height);
        bool wasAtBottom = _panel.ScrollOffset >= (prevMaxScroll - BottomTolerancePx);

        // Begin scroll region with current height (also sets scissor)
        _panel.BeginScroll(totalHeight);

        // Auto-scroll on first load or if new messages arrived while user was at bottom
        bool hasMessages = _bubbles.Count > 0;
        bool hasNewMessages = _bubbles.Count > _lastMessageCount;
        if ((_firstLoad && hasMessages) || (hasNewMessages && wasAtBottom))
        {
            _panel.ScrollToBottom();
            _firstLoad = false;
        }

        // Draw bubbles
        float currentY = _bounds.Y + PaddingTop;
        foreach (var b in _bubbles)
        {
            float scrolledY = _panel.GetScrolledY(currentY);
            if (_panel.IsVisible(scrolledY, b.Height))
            {
                b.Draw(_bounds.X + LeftInset, scrolledY);
            }
            currentY += b.Height + Spacing;
        }

        _panel.EndScroll();

        // Track for next frame
        _lastMessageCount = _bubbles.Count;
        _lastContentHeight = totalHeight;
    }

    public void ScrollToBottom() => _panel.ScrollToBottom();

    private float GetCurrentContentWidth()
    {
        return MathF.Max(0, _bounds.Width - LeftInset * 2);
    }
}

