using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class RegisterScreen : ScreenBase<RegisterScreenLayout.LayoutData>
{
    private readonly TextField userField = new(new Rectangle(),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        false, false, "RegisterScreen_Username", "Choose quackername...");
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
        logic = new RegisterScreenLogic(userField, passField, passConfirmField, registerButton, backButton);
    }

    protected override RegisterScreenLayout.LayoutData CalculateLayout() => RegisterScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

    protected override void ApplyLayout(RegisterScreenLayout.LayoutData layout)
    {
        userField.SetRect(layout.UserRect);
        passField.SetRect(layout.PassRect);
        passConfirmField.SetRect(layout.PassConfirmRect);
        registerButton.SetRect(layout.RegisterRect);
        backButton.SetRect(layout.BackRect);
    }

    public override void RenderContent()
    {
        userField.Draw();
        passField.Draw();
        passConfirmField.Draw();

        registerButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // Draw feedback box
        var registerLogic = logic as RegisterScreenLogic;
        registerLogic?.FeedbackBox.Draw();
    }
}
