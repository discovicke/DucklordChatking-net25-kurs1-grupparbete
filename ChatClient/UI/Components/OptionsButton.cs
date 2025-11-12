using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class OptionsButton : Button
    {
        public OptionsButton(Rectangle rect)
            : base(rect, "Ducktions", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor)
        {
        }
        

        public override void Draw()
        {
               base.Draw();
            
        }

      
    }
}
