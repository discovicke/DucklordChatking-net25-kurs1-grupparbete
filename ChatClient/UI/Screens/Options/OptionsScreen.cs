﻿using System.Numerics;
using ChatClient.Core;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Screens.Options;

/// <summary>
/// Responsible for: composition and rendering of the options/settings screen.
/// Coordinates user account fields, window mode buttons, and delegates input to OptionsScreenLogic.
/// </summary>
public class OptionsScreen : ScreenBase<OptionsScreenLayout.LayoutData>
{
    private readonly TextField newUsername = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, false, "OptionsScreen_NewUsername", "New quackername...");
    private readonly TextField newPassword = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "OptionsScreen_NewPassword", "New password...");
    private readonly TextField confirmPassword = new(new Rectangle(), 
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor, 
        false, true, "OptionsScreen_ConfirmPassword", "Confirm new password...");
    
    private readonly Button confirmButton = new(new Rectangle(), "Confirm", 
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    private readonly Button btnWindowed = new(new Rectangle(), "Windowed", 
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly Button btnFullscreen = new(new Rectangle(), "Fullscreen", 
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    
    
    public OptionsScreen()
    {
        logic = new OptionsScreenLogic(newUsername, newPassword, confirmPassword, confirmButton, backButton, btnWindowed, btnFullscreen);
    }

    protected override OptionsScreenLayout.LayoutData CalculateLayout() => OptionsScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(OptionsScreenLayout.LayoutData layout)
    {
        newUsername.SetRect(layout.UserRect);
        newPassword.SetRect(layout.PassRect);
        confirmPassword.SetRect(layout.PassConfirmRect);
        confirmButton.SetRect(layout.ConfirmRect);
        backButton.SetRect(layout.BackRect);
        btnWindowed.SetRect(layout.BtnWindowedRect);
        btnFullscreen.SetRect(layout.BtnFullscreenRect);
    }

    public override void RenderContent()
    {
        newUsername.Draw();
        newPassword.Draw();
        confirmPassword.Draw();

        confirmButton.Draw();
        backButton.Draw();
        
        btnWindowed.Draw();
        btnFullscreen.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);
    }
}
