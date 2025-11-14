using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class OptionsScreen : ScreenBase<OptionsScreenLayout.LayoutData>
{
    private readonly TextField newUsername = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, false, "OptionsScreen_NewUsername", "New username...");
    private readonly TextField newPassword = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "OptionsScreen_NewPassword", "New password...");
    private readonly TextField confirmPassword = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "OptionsScreen_ConfirmPassword", "Confirm new password...");
    
    private readonly Button confirmButton = new(new Rectangle(), "Confirm", 
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    public OptionsScreen()
    {
        logic = new OptionsScreenLogic(newUsername, newPassword, confirmPassword, confirmButton, backButton);
    }

    protected override OptionsScreenLayout.LayoutData CalculateLayout() => OptionsScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

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
        const float labelFont = 15;
        
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "New username:", 
            new Vector2(layout.UserRect.X - 145, layout.UserRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "New password:", 
            new Vector2(layout.PassRect.X - 145, layout.PassRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "Confirm password:", 
            new Vector2(layout.PassConfirmRect.X - 165, layout.PassConfirmRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);

        newUsername.Draw();
        newPassword.Draw();
        confirmPassword.Draw();

        confirmButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);
    }
}
