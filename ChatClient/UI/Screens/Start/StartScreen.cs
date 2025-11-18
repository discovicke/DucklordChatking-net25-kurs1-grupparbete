﻿using System.Numerics;
using ChatClient.Core;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Screens.Start;

/// <summary>
/// Responsible for: composition and rendering of the start/login screen.
/// Coordinates username/password fields, login/register/options buttons, and delegates input to StartScreenLogic.
/// </summary>
public class StartScreen : ScreenBase<StartScreenLayout.LayoutData>
{
    private readonly TextField userField = new(new Rectangle(0, 0, 0, 0),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        false, false, "StartScreen_Username", "Enter quackername...");

    private readonly TextField passwordField = new(new Rectangle(0, 0, 0, 0),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        false, true, "StartScreen_Password", "Enter password...");

    private readonly Button registerButton = new(new Rectangle(0, 0, 0, 0), "Register",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);

    private readonly Button loginButton = new(new Rectangle(0, 0, 0, 0), "Login",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);

    public StartScreen()
    {
        logic = new StartScreenLogic(userField, passwordField, loginButton, registerButton);
    }

    protected override StartScreenLayout.LayoutData CalculateLayout() =>
        StartScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(StartScreenLayout.LayoutData layout)
    {
        userField.SetRect(layout.UserRect);
        passwordField.SetRect(layout.PassRect);
        loginButton.SetRect(layout.LoginRect);
        registerButton.SetRect(layout.RegisterRect);
    }

    public override void RenderContent()
    {
        userField.Update();
        userField.Draw();
        passwordField.Update();
        passwordField.Draw();

        registerButton.Draw();
        loginButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // Draw feedback box
        var startLogic = logic as StartScreenLogic;
        startLogic?.FeedbackBox.Draw();

#if DEBUG
        float screenHeight = layout.ScreenHeight;
        Raylib.DrawTextEx(ResourceLoader.RegularFont, "DEV: Ctrl+Shift+D = Quack Login",
            new Vector2(10, screenHeight - 40), 10, 0.5f, Colors.SubtleText);
#endif
    }
}
