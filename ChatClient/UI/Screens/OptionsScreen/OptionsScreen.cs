using System.Numerics;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class OptionsScreen : ScreenBase<OptionsScreenLayout.LayoutData>
{
    private readonly Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.2.png");

    private readonly TextField newUsername = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false);
    private readonly TextField newPassword = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, true);
    private readonly TextField confirmPassword = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, true);

    private readonly Button confirmButton = new(new Rectangle(), "Confirm", 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    public OptionsScreen()
    {
        logic = new OptionsScreenLogic(newUsername, newPassword, confirmPassword, confirmButton, backButton);
    }

    protected override OptionsScreenLayout.LayoutData CalculateLayout() => OptionsScreenLayout.Calculate(logo.Width);

    protected override void ApplyLayout(OptionsScreenLayout.LayoutData layout)
    {
        newUsername.SetRect(layout.UserRect);
        newPassword.SetRect(layout.PassRect);
        confirmPassword.SetRect(layout.PassConfirmRect);
        confirmButton.SetRect(layout.ConfirmRect);
        backButton.SetRect(layout.BackRect);
    }

    public override void RenderContent()
    {
        Raylib.DrawText("New username:", 
            (int)(layout.UserRect.X - 145), 
            (int)(layout.UserRect.Y + 5), 
            15, Colors.TextFieldColor);
        Raylib.DrawText("New password:", 
            (int)(layout.PassRect.X - 145), 
            (int)(layout.PassRect.Y + 5), 
            15, Colors.TextFieldColor);
        Raylib.DrawText("Confirm password:", 
            (int)(layout.PassConfirmRect.X - 165), 
            (int)(layout.PassConfirmRect.Y + 5), 
            15, Colors.TextFieldColor);

        newUsername.Draw();
        newPassword.Draw();
        confirmPassword.Draw();

        confirmButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(logo, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);
    }
}
