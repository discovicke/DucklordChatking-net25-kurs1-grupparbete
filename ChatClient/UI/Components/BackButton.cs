using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class BackButton : Button
    {
        public BackButton(Rectangle rect)
            : base(rect, "← Back", Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor)
        {
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