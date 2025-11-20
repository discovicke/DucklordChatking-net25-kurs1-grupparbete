using System.Numerics;
using ChatClient.Core;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Screens.Options;

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

    private readonly ToggleBox toggleWindowed = new(new Rectangle(), "Windowed");
    private readonly ToggleBox toggleFullscreen = new(new Rectangle(), "Fullscreen");
    private readonly ToggleBox toggleMute = new(new Rectangle(), "Sound OFF");

    public OptionsScreen()
    {
        logic = new OptionsScreenLogic(newUsername, newPassword, confirmPassword,
            confirmButton, backButton, toggleWindowed, toggleFullscreen, toggleMute);
    }

    protected override OptionsScreenLayout.LayoutData CalculateLayout() =>
        OptionsScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(OptionsScreenLayout.LayoutData layout)
    {
        newUsername.SetRect(layout.UserRect);
        newPassword.SetRect(layout.PassRect);
        confirmPassword.SetRect(layout.PassConfirmRect);
        confirmButton.SetRect(layout.ConfirmRect);
        backButton.SetRect(layout.BackRect);
        toggleWindowed.SetRect(layout.ToggleWindowedRect);
        toggleFullscreen.SetRect(layout.ToggleFullscreenRect);
        toggleMute.SetRect(layout.ToggleMuteRect);
    }

    public override void RenderContent()
    {
        newUsername.Draw();
        newPassword.Draw();
        confirmPassword.Draw();

        confirmButton.Draw();
        backButton.Draw();

        toggleWindowed.Draw();
        toggleFullscreen.Draw();
        toggleMute.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        ((OptionsScreenLogic?)logic)?.FeedbackBox.Draw();

    }
}
