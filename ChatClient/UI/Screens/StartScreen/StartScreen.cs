using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class StartScreen : ScreenBase<StartScreenLayout.LayoutData>
{

    private readonly TextField userField = new(new Rectangle(0, 0, 0, 0), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, false, "StartScreen_Username");
    private readonly TextField passwordField = new(new Rectangle(0, 0, 0, 0), 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, false, true, "StartScreen_Password");

    private readonly Button registerButton = new(new Rectangle(0, 0, 0, 0), "Register", 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    private readonly Button loginButton = new(new Rectangle(0, 0, 0, 0), "Login", 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    private readonly OptionsButton optionsButton = new(new Rectangle(0, 0, 0, 0));

    public StartScreen()
    {
        logic = new StartScreenLogic(userField, passwordField, loginButton, registerButton, optionsButton);
    }

    protected override StartScreenLayout.LayoutData CalculateLayout() => StartScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

    protected override void ApplyLayout(StartScreenLayout.LayoutData layout)
    {
        userField.SetRect(layout.UserRect);
        passwordField.SetRect(layout.PassRect);
        loginButton.SetRect(layout.LoginRect);
        registerButton.SetRect(layout.RegisterRect);
        optionsButton.SetRect(layout.OptionsRect);
    }

    public override void RenderContent()
    {
        int labelFont = 15;
        int labelYUser = (int)(layout.UserRect.Y + (layout.UserRect.Height - labelFont) / 2f);
        int labelYPass = (int)(layout.PassRect.Y + (layout.PassRect.Height - labelFont) / 2f);
        Raylib.DrawText("Username:", 
            (int)(layout.UserRect.X - 110), 
            labelYUser, labelFont, Colors.TextFieldColor);
        Raylib.DrawText("Password:", 
            (int)(layout.PassRect.X - 110), 
            labelYPass, labelFont, Colors.TextFieldColor);

        userField.Update(); userField.Draw();
        passwordField.Update(); passwordField.Draw();

        registerButton.Draw();
        loginButton.Draw();
        optionsButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);

    }
}
