using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ChatClient.Configurations;
using Raylib_cs;


namespace ChatClient
{
    public class ChatScreen
    {

        private enum SelectedField { None, Username, Password }
        private static SelectedField selected = SelectedField.None;

        //Input from user
        private static string inputText = "";
        private static string userMessage = "";

        public static void Run()
        {
            // ChatWindow-test
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            // Logo

            Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");
            Raylib.DrawTextureEx(logo, new Vector2(620, 25), 0, 0.15f, Color.White);


            int rectX = 0;
            int rectY = 0;
            int rectWidth = 0;
            int rectHeight = 0;


            // ChatWindow
            Rectangle chatWindow = new Rectangle(rectX + 50, rectY + 50, rectWidth + 550, rectHeight + 380);
            Raylib.DrawRectangleRounded(chatWindow, 0.1f, 10, Colors.HoverColor);

            // TypeWindow
            Rectangle typeWindow = new Rectangle(rectX + 50, rectY + 450, rectWidth + 550, rectHeight + 100);
            Raylib.DrawRectangleRounded(typeWindow, 0.3f, 10, Colors.TextFieldColor);

            // SendButton
            Rectangle sendButton = new Rectangle(rectX + 610, rectY + 450, rectWidth + 100, rectHeight + 100);
            Raylib.DrawRectangleRounded(sendButton, 0.3f, 10, Colors.TextFieldColor);

            // --- Inputhantering ---
            if (selected == SelectedField.Username)
            {
                // Läs tecken
                int key = Raylib.GetCharPressed();
                while (key > 0)
                {
                    if (key >= 32 && key <= 126) // synliga ASCII-tecken
                    {
                        inputText += (char)key;
                    }
                    key = Raylib.GetCharPressed();
                }

                // Backspace
                if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
                {
                    inputText = inputText.Substring(0, inputText.Length - 1);
                }
            }

            // Mouse LogicZ
            bool hoverUser = MouseInput.IsHovered(typeWindow);
            bool hoverPassword = MouseInput.IsHovered(sendButton);
            bool leftPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);


            if (MouseInput.IsLeftClick(typeWindow))
            {
                selected = SelectedField.Username;
            }
            else if (MouseInput.IsLeftClick(sendButton))
            {
                selected = SelectedField.Password;
                if (MouseInput.IsLeftClick(sendButton))
                {
                    if (!string.IsNullOrWhiteSpace(inputText))
                    {
                        userMessage = inputText;       // lagra bufferten

                        inputText = "";                // töm inputfältet
                    }
                }

            }
            else if (leftPressed && !hoverUser && !hoverPassword)
            {
                selected = SelectedField.None;
            }

            // Visual hover feedback (outline)
            if (hoverUser)
            {
                Raylib.DrawRectangleRounded(typeWindow, 0.3f, 10, Colors.HoverColor);
            }
            if (hoverPassword)
            {
                // Rita en ram runt knappen istället för att fylla den igen
                Raylib.DrawRectangleRoundedLinesEx(sendButton, 0.3f, 10, 3, Colors.TextColor);
            }

            Raylib.DrawText("Send", rectX + 635, rectY + 490, 20, Colors.TextColor);

            // Rita texten i typeWindow
            Raylib.DrawText(inputText, (int)typeWindow.X + 10, (int)typeWindow.Y + 40, 20, Colors.TextColor);


            // Client Version
            Raylib.DrawText("DuckLord v.1.0.0", 10, 580, 10, Colors.TextColor);

            Raylib.EndDrawing();

        }
    }
}







