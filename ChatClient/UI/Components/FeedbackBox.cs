using System.Numerics;
using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components;

public class FeedbackBox
{
    private string message = "";
    private bool isSuccess = false;
    private double startTime = 0;
    private const double DisplayDuration = 3.0;
    private const float PaddingFromBottom = 50f;
    private const float BoxPadding = 15f;

    public void Show(string message, bool isSuccess)
    {
        this.message = message;
        this.isSuccess = isSuccess;
        this.startTime = Raylib.GetTime();
    }

    public void Update()
    {
        // Clear message after duration
        if (!string.IsNullOrEmpty(message) && Raylib.GetTime() - startTime > DisplayDuration)
        {
            message = "";
        }
    }

    public void Draw()
    {
        if (string.IsNullOrEmpty(message)) return;

        Color feedbackColor = isSuccess ? Colors.Success : Colors.Fail;

        // Measure text size
        Vector2 textSize = Raylib.MeasureTextEx(ResourceLoader.MediumFont, message, 16, 0.5f);

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

        Raylib.DrawTextEx(ResourceLoader.MediumFont, message,
            new Vector2(textX, textY), 16, 0.5f, feedbackColor);
    }
}