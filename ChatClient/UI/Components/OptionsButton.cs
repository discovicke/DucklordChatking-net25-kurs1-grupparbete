using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class OptionsButton : Button
    {
        public OptionsButton(Rectangle rect)
            : base(rect, "← Options", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor)
        {
        }

        public override void Draw()
        {
            if (AppState.CanGoBack)
            {
                base.Draw();
            }
        }

        public override void Update()
        {
            if (AppState.CanGoBack && IsClicked())
            {
                AppState.GoBack();
                Log.Info($"Navigated back to {AppState.CurrentScreen}");
            }
        }
    }
}
