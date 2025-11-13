using System.Numerics;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class RegisterScreen : ScreenBase<RegisterScreenLayout.LayoutData>
{
    private readonly Texture2D logo = Raylib.LoadTexture(@"Resources/DuckLord1.2slim.png");

    private readonly TextField idField = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, false, "RegisterScreen_ID");
    private readonly TextField userField = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, false, "RegisterScreen_Username");
    private readonly TextField passField = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, true, "RegisterScreen_Password");
    private readonly TextField passConfirmField = new(new Rectangle(), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, true, "RegisterScreen_PasswordConfirm");

    private readonly Button registerButton = new(new Rectangle(), "Register", 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    public RegisterScreen()
    {
        logic = new RegisterScreenLogic(idField, userField, passField, passConfirmField, registerButton, backButton);
    }

    protected override RegisterScreenLayout.LayoutData CalculateLayout() => RegisterScreenLayout.Calculate(logo.Width);

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
        Raylib.DrawText("Register ID:", 
            (int)(layout.IdRect.X - 145), 
            (int)(layout.IdRect.Y + 5), 
            15, Colors.TextFieldColor);
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

        idField.Draw();
        userField.Draw();
        passField.Draw();
        passConfirmField.Draw();

        registerButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(logo, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);
    }
}
