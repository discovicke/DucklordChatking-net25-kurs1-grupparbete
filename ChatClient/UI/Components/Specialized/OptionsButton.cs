 using ChatClient.Core.Application;
 using ChatClient.Core.Infrastructure;
 using ChatClient.UI.Components.Base;
 using ChatClient.UI.Theme;
 using Raylib_cs;

namespace ChatClient.UI.Components.Specialized
{
    /// <summary>
    /// Responsible for: providing a pre-configured "Ducktions" (Options) button.
    /// Extends Button with default styling and text for accessing settings/options screens.
    /// </summary>
    public class OptionsButton(Rectangle rect)
        : Button(rect, "Ducktions", Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor)
    {
        public override void Draw()
        {
            base.Draw();
        }
        
        public override void Update()
        {
            if (AppState.CanGoBack && IsClicked())
            {
                Log.Info($"[BackButton] Navigating back from {AppState.CurrentScreen}");
                AppState.GoBack();
                Log.Info($"[BackButton] Navigated to {AppState.CurrentScreen}");
            }
        }
    }
}