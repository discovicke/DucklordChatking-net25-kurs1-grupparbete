using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace ChatClient
{
    internal class UIkladd
    {
        public static void Run()
        {
            Raylib.BeginDrawing();
            // LoginScreen-test
            Raylib.ClearBackground(new Color(15, 112, 152));
            int fontSize = 15;
            string userName = "Username:";
            string passWord = "Password";

            // Rita label
            Raylib.DrawText(userName, 220, 300, fontSize, Color.Gold);
            Raylib.DrawText(passWord, 220, 350, fontSize, Color.Gold);

            // Beräkna textens bredd
            int textWidth = Raylib.MeasureText(userName, fontSize);

            // Placera rektangeln direkt efter texten
            int rectX = 220 + textWidth + 10; // +10 för lite mellanrum
            int rectY = 300;
            int rectWidth = 150;
            int rectHeight = fontSize + 10; // lite högre än texten

            // Textfält
            Rectangle rectUser = new Rectangle(rectX, rectY, rectWidth, rectHeight);
            Rectangle rectPassword = new Rectangle(rectX, rectY + 50, rectWidth, rectHeight);
            Rectangle rectLogIn = new Rectangle(rectX, rectY, rectWidth, rectHeight);



            Raylib.DrawRectangleRounded(rectUser, 0.3f, 10, Color.Gold);

            Raylib.DrawRectangleRounded(rectPassword, 0.3f, 10, Color.Gold);

            // Logo
            Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");
            Raylib.DrawTextureEx(logo, new Vector2(400, 50), 0, 0.15f, Color.White);


            Raylib.DrawText("DuckLord v.1.0.0", 10, 460, 10, Color.Black);

            Raylib.EndDrawing();
        }
    }
}
