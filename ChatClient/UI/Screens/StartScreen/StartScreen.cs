using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class StartScreen : ScreenBase<StartScreenLayout.LayoutData>
{

    private readonly TextField userField = new(new Rectangle(0, 0, 0, 0),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        false, false, "StartScreen_Username", "Enter username...");

    private readonly TextField passwordField = new(new Rectangle(0, 0, 0, 0),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        false, true, "StartScreen_Password", "Enter password...");

    private readonly Button registerButton = new(new Rectangle(0, 0, 0, 0), "Register",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);

    private readonly Button loginButton = new(new Rectangle(0, 0, 0, 0), "Login",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);

    private readonly OptionsButton optionsButton = new(new Rectangle(0, 0, 0, 0));

    public StartScreen()
    {
        logic = new StartScreenLogic(userField, passwordField, loginButton, registerButton, optionsButton);
    }

    protected override StartScreenLayout.LayoutData CalculateLayout() =>
        StartScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

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
        float labelFont = 15;
        float labelYUser = layout.UserRect.Y + (layout.UserRect.Height - labelFont) / 2f;
        float labelYPass = layout.PassRect.Y + (layout.PassRect.Height - labelFont) / 2f;

        Raylib.DrawTextEx(ResourceLoader.BoldFont, "QUACKERNAME",
            new Vector2(layout.UserRect.X - 110, labelYUser),
            labelFont, 1, Colors.TextColor);
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "DUCKWORD",
            new Vector2(layout.PassRect.X - 110, labelYPass),
            labelFont, 1, Colors.TextColor);

        userField.Update();
        userField.Draw();
        passwordField.Update();
        passwordField.Draw();

        registerButton.Draw();
        loginButton.Draw();
        optionsButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // DEV MODE indicator (remove before production)
#if DEBUG
        float screenHeight = layout.ScreenHeight;
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "DEV: Ctrl+Shift+D = Quack Login",
            new Vector2(10, screenHeight - 40), 10, 1, Colors.SubtleText);
#endif
    }
}

