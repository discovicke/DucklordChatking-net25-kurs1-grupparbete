using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class RegisterScreen : ScreenBase<RegisterScreenLayout.LayoutData>
{
    private readonly TextField idField = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, false, "RegisterScreen_ID", "Enter registration ID...");
    private readonly TextField userField = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, false, "RegisterScreen_Username", "Choose username...");
    private readonly TextField passField = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "RegisterScreen_Password", "Choose password...");
    private readonly TextField passConfirmField = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "RegisterScreen_PasswordConfirm", "Confirm password...");

    private readonly Button registerButton = new(new Rectangle(), "Register", 
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    public RegisterScreen()
    {
        logic = new RegisterScreenLogic(idField, userField, passField, passConfirmField, registerButton, backButton);
    }

    protected override RegisterScreenLayout.LayoutData CalculateLayout() => RegisterScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

    protected override void ApplyLayout(RegisterScreenLayout.LayoutData layout)
    {
        idField.SetRect(layout.IdRect);
        userField.SetRect(layout.UserRect);
        passField.SetRect(layout.PassRect);
        passConfirmField.SetRect(layout.PassConfirmRect);
        registerButton.SetRect(layout.RegisterRect);
        backButton.SetRect(layout.BackRect);
    }

    public override void RenderContent()
    {
        const float labelFont = 15;
        
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "Register ID:", 
            new Vector2(layout.IdRect.X - 145, layout.IdRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "New username:", 
            new Vector2(layout.UserRect.X - 145, layout.UserRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "New password:", 
            new Vector2(layout.PassRect.X - 145, layout.PassRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "Confirm password:", 
            new Vector2(layout.PassConfirmRect.X - 165, layout.PassConfirmRect.Y + 5), 
            labelFont, 0.5f, Colors.TextColor);

        idField.Draw();
        userField.Draw();
        passField.Draw();
        passConfirmField.Draw();

        registerButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);
    }
}
