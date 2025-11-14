using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class RegisterScreen : ScreenBase<RegisterScreenLayout.LayoutData>
{
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

        userField.Draw();
        passField.Draw();
        passConfirmField.Draw();

        registerButton.Draw();
        backButton.Draw();

        Raylib.DrawTextureEx(ResourceLoader.LogoTexture, 
            new Vector2(layout.LogoX, layout.LogoY), 
            0f, layout.LogoScale, Color.White);

        // Display feedback message if present
        var registerLogic = logic as RegisterScreenLogic;
        if (registerLogic != null && !string.IsNullOrEmpty(registerLogic.FeedbackMessage))
        {
            Color feedbackColor = registerLogic.IsFeedbackSuccess ? new Color(46, 204, 113, 255) : new Color(231, 76, 60, 255);
            float feedbackY = layout.RegisterRect.Y + layout.RegisterRect.Height + 20;
            
            // Measure text to center it
            Vector2 textSize = Raylib.MeasureTextEx(ResourceLoader.MediumFont, registerLogic.FeedbackMessage, 16, 0.5f);
            float feedbackX = (Raylib.GetScreenWidth() - textSize.X) / 2;
            
            Raylib.DrawTextEx(ResourceLoader.MediumFont, registerLogic.FeedbackMessage, 
                new Vector2(feedbackX, feedbackY), 16, 0.5f, feedbackColor);
        }
    }
}
