using System.Numerics;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Specialized;

/// <summary>
/// Responsible for: displaying temporary feedback messages with success/error styling.
/// Shows colored message boxes that auto-hide after a timeout period.
/// </summary>
public class FeedbackBox
{
    public string Message { get; private set; } = "";
    public bool IsSuccess { get; private set; } = false;
    public double StartTime { get; private set; } = 0;
    private const double DisplayDuration = 3.0;
    private const float PaddingFromBottom = 50f;
    private const float BoxPadding = 15f;

    public void Show(string message, bool isSuccess)
    {
        Message = message;
        IsSuccess = isSuccess;
        StartTime = Raylib.GetTime();
    }

    public void Update()
    {
        // Clear message after duration
        if (!string.IsNullOrEmpty(Message) && Raylib.GetTime() - StartTime > DisplayDuration)
        {
            Message = "";
        }
    }

    public void Draw()
    {
        if (string.IsNullOrEmpty(Message)) return;

        Color feedbackColor = IsSuccess ? Colors.Success : Colors.Fail;

        // Measure text size
        Vector2 textSize = Raylib.MeasureTextEx(ResourceLoader.MediumFont, Message, 16, 0.5f);

        // Calculate box dimensions
        float boxWidth = textSize.X + (BoxPadding * 2);
        float boxHeight = textSize.Y + (BoxPadding * 2);
        float boxX = (Raylib.GetScreenWidth() - boxWidth) / 2;
        float boxY = Raylib.GetScreenHeight() - PaddingFromBottom - boxHeight;

        Rectangle boxRect = new Rectangle(boxX, boxY, boxWidth, boxHeight);

        // Draw box background
        Raylib.DrawRectangleRec(boxRect, Colors.PanelColor);

        // Draw box outline
        Raylib.DrawRectangleLinesEx(boxRect, 2, Colors.OutlineColor);

        // Draw text centered in box
        float textX = boxX + BoxPadding;
        float textY = boxY + BoxPadding;

        Raylib.DrawTextEx(ResourceLoader.MediumFont, Message,
            new Vector2(textX, textY), 16, 0.5f, feedbackColor);
    }
}