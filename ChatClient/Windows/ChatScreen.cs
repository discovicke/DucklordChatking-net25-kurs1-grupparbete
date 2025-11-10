using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChatClient.Configurations;
using Raylib_cs;


namespace ChatClient.Windows
{
    public class ChatScreen
    {

        private enum SelectedField { None, Username, Password }
        private static SelectedField selected = SelectedField.None;

        //Input from user
        private static string inputText = "";
        private static string userMessage = "";

        // Load logo
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        // Text field for user input
        private static TextField textField = new TextField(new Rectangle(50, 450, 550, 100), Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);

         public static void Run()
         {
             // ChatWindow-test
             Raylib.BeginDrawing();
             Raylib.ClearBackground(Colors.BackgroundColor);

             // Update mouse state early each frame
             MouseInput.Update();

             // Logo
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

            // SendButton rectangle
            Rectangle sendButtonRect = new Rectangle(rectX + 610, rectY + 450, rectWidth + 100, rectHeight + 100);

            // Create reusable Button and draw it
            var sendButton = new Button(sendButtonRect, "Send", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            sendButton.Draw();

            // Update and draw text field
            textField.Update();
            textField.Draw();

            // Mouse Logic
            bool hoverUser = MouseInput.IsHovered(typeWindow);
            bool hoverSend = sendButton.IsHovered();
            bool leftPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);

            if (MouseInput.IsLeftClick(typeWindow))
            {
                selected = SelectedField.Username;
            }
            else if (sendButton.IsClicked())
             {
                // Click on Send: save message and clear input field
                if (!string.IsNullOrWhiteSpace(textField.Text))
                {
                    userMessage = textField.Text;       // store buffert field
                    textField.Clear();                  // empty text field
                }
             }
            else if (leftPressed && !hoverUser && !hoverSend)
            {
                selected = SelectedField.None;
            }

            // Visual hover feedback (outline)
            if (hoverUser)
            {
                Raylib.DrawRectangleRounded(typeWindow, 0.3f, 10, Colors.HoverColor);
            }


            // Client Version
            Raylib.DrawText("DuckLord v.0.0.1", 10, 580, 10, Colors.TextColor);

             Raylib.EndDrawing();

         }
     }
 }
