using System.Numerics;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Theme;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Screens.Chat.Components;

/// <summary>
/// Responsible for: rendering individual chat message bubbles with word-wrapping and dynamic sizing.
/// Wraps MessageDTO into a visual bubble with timestamp, sender name, and message content.
/// </summary>
public class ChatMessage
{
    private MessageDTO Message { get; set; }
    private bool IsOwnMessage { get; set; }
    private float MaxWidth { get; set; }
    private List<string> WrappedLines { get; set; }

    public float Height { get; private set; }
    public float Width { get; private set; } 
    private const float Padding = 10f;
    private const float LineSpacing = 18f;
    private const float FontSize = 14f;

    public ChatMessage(MessageDTO message, float maxWidth, bool isOwnMessage, bool playNotificationSound = true)
    {
        Message = message;
        IsOwnMessage = isOwnMessage;
        MaxWidth = maxWidth - (Padding * 2);

        string sender = string.IsNullOrWhiteSpace(message.Sender) ? "Unknown Duck" : message.Sender;
        string timestamp = message.Timestamp.ToLocalTime().ToString("HH:mm");
        string header = $"{timestamp} - {sender}:";

        WrappedLines = new List<string>();

        // Wrap header
        WrappedLines.AddRange(WrapText(header, ResourceLoader.BoldFont));

        // Wrap content
        WrappedLines.AddRange(WrapText(message.Content ?? "", ResourceLoader.RegularFont));

        // Total height
        float maxLineWidth = 0f;
        int headerLines = WrapText(header, ResourceLoader.BoldFont).Count;

        for (int i = 0; i < WrappedLines.Count; i++)
        {
            var font = i < headerLines 
                ? ResourceLoader.BoldFont 
                : ResourceLoader.RegularFont;
            var lineWidth = Raylib.MeasureTextEx(font, WrappedLines[i], FontSize, 0.5f).X;
            maxLineWidth = Math.Max(maxLineWidth, lineWidth);
        }

        Width = maxLineWidth + (Padding * 2);
        Height = WrappedLines.Count * LineSpacing + (Padding * 2);

        // --- Notification sound ---
        /*
        if (playNotificationSound && !isOwnMessage)
        {
            Raylib.PlaySound(ResourceLoader.NotificationSound);
        }
        */

    }

    private List<string> WrapText(string text, Font font)
    {
        var lines = new List<string>();
        var words = text.Split(' ');
        string currentLine = "";

        foreach (var word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
            var size = Raylib.MeasureTextEx(font, testLine, FontSize, 0.5f);

            if (size.X > MaxWidth && !string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        return lines;
    }

    public void Draw(float x, float y, float containerWidth)
    {
        const float RightInset = 5f; // Distance from right edge for own messages
    
        // Right-align own messages with inset from scrollbar
        float bubbleX = IsOwnMessage 
            ? x + containerWidth - Width - RightInset 
            : x - RightInset;

        // Draw bubble background
        var bubbleRect = new Rectangle(bubbleX, y, Width, Height);
        var bubbleColor = IsOwnMessage 
            ? Colors.ChatBubbleSelf 
            : Colors.ChatBubbleOther;
    
        Raylib.DrawRectangleRounded(bubbleRect, 0.15f, 8, bubbleColor);
        Raylib.DrawRectangleRoundedLinesEx(bubbleRect, 0.15f, 8, 1, Colors.OutlineColor);

        // Draw text
        float textY = y + Padding;
    
        string sender = string.IsNullOrWhiteSpace(Message.Sender) 
            ? "Unknown Duck" 
            : Message.Sender;
        string timestamp = Message.Timestamp
            .ToLocalTime()
            .ToString("HH:mm");
        string header = $"{timestamp} - {sender}";
        int headerLineCount = WrapText(header, ResourceLoader.BoldFont).Count;

        for (int i = 0; i < WrappedLines.Count; i++)
        {
            var font = i < headerLineCount 
                ? ResourceLoader.BoldFont 
                : ResourceLoader.RegularFont;
            var color = i < headerLineCount 
                ? Colors.ChatBubbleSelfText 
                : Colors.ChatBubbleOtherText;

            Raylib.DrawTextEx(font, WrappedLines[i],
                new Vector2(bubbleX + Padding, textY), FontSize, 0.5f, color);

            textY += LineSpacing;
        }
    }
}
