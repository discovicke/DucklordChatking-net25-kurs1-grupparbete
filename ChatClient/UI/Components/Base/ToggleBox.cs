// File: ChatClient/UI/Components/Base/ToggleBox.cs
using System.Numerics;
using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Base;

/// <summary>
/// Responsible for: rendering a checkbox with label and handling toggle state.
/// Provides visual feedback through hover effects and checked/unchecked states.
/// </summary>
public class ToggleBox : UIComponent
{
    private string label;
    private bool isChecked;

    private const float LabelGap = 5f;
    private const float WrapperPaddingTop = 4f;
    private const float BorderThickness = 2f;

    public bool IsChecked => isChecked;

    public ToggleBox(Rectangle rect, string labelText, bool initialState = false)
    {
        Rect = rect;
        label = labelText;
        isChecked = initialState;
    }

    public override void Draw()
    {
        bool hovered = MouseInput.IsHovered(Rect);

        // Wrapper background
        Raylib.DrawRectangleRounded(Rect, 0.15f, 8, Colors.TextFieldUnselected);
        Color border = hovered ? Colors.BrandGold : Colors.OutlineColor;
        Raylib.DrawRectangleRoundedLinesEx(Rect, 0.15f, 8, BorderThickness, Colors.OutlineColor);

        // Compute checkbox size (square) relative to wrapper
        float boxSize = Rect.Width * 0.5f;
        // Ensure it doesn't exceed a proportion of height
        float maxBoxSize = Rect.Height * 0.45f;
        if (boxSize > maxBoxSize) boxSize = maxBoxSize;

        float boxX = Rect.X + (Rect.Width - boxSize) / 2f;
        float boxY = Rect.Y + WrapperPaddingTop;

        Rectangle boxRect = new(boxX, boxY, boxSize, boxSize);

        // Checkbox fill
        Color boxFill = isChecked ? Colors.BrandGold : Colors.TextFieldUnselected;
        Raylib.DrawRectangleRounded(boxRect, 60f, 4, boxFill);
        Raylib.DrawRectangleRoundedLinesEx(boxRect, 60f, 4, 2, border);

        // Check indicator
        if (isChecked)
        {
            float padding = boxSize * 0.25f;
            Vector2 center = new(boxRect.X + boxRect.Width / 2f, boxRect.Y + boxRect.Height / 2f);
            float radius = (boxSize - (padding * 2)) / 2f;
            Raylib.DrawCircleV(center, radius, Colors.TextColor);
        }

        // Label below checkbox
        int fontSize = 14;
        Vector2 measure = Raylib.MeasureTextEx(ResourceLoader.RegularFont, label, fontSize, 0.5f);
        float textX = Rect.X + (Rect.Width - measure.X) / 2f;
        float textY = boxY + boxSize + LabelGap;
        Raylib.DrawTextEx(ResourceLoader.RegularFont, label, new Vector2(textX, textY), fontSize, 0.5f, Colors.TextColor);
    }

    public override void Update()
    {
        if (MouseInput.IsLeftClick(Rect))
        {
            isChecked = !isChecked;
        }
    }

    public bool IsClicked() => MouseInput.IsLeftClick(Rect);
    public void SetChecked(bool value) => isChecked = value;
}
