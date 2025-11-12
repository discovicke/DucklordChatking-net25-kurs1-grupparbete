using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChatClient.Configurations;
using ChatClient.Data;
using Raylib_cs;
using Shared;



namespace ChatClient.Windows
{
    public class ChatScreen
    {
        private enum SelectedField
        {
            None,
            TypeWindow
        }

        private static SelectedField selected = SelectedField.None;

        // New message list?
        private static List<MessageDTO> messages = new List<MessageDTO>();


        //Input from user
        private static string inputText = "";
        private static string userMessage = "";

        // Load logo
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        // Text field for user input
        private static TextField textField = new TextField(
            new Rectangle(50, 450, 550, 100),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: true
        );
        
        // Back button
        private static BackButton backButton = new BackButton(new Rectangle(10, 10, 100, 30));


        // Adds a message sender to the text field
        private static MessageHandler? messageSender = new MessageHandler(new HttpClient
            { BaseAddress = new Uri("http://192.168.20.17:5201/scalar/") });

        public static void Run()
        {
            // ChatWindow-test
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);


            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(620, 25), 0, 0.15f, Color.White);
            var tillbakaknapp = new Rectangle(620, 25, 150, 250);
            if (MouseInput.IsLeftClick(tillbakaknapp))
            {
                AppState.CurrentScreen = Screen.Start;
            }

            int rectX = 0;
            int rectY = 0;
            int rectWidth = 0;
            int rectHeight = 0;

            // TODO: Move rectVARIABLES to configuration classes?

            // Rectangles
            Rectangle typeWindow = new Rectangle(rectX + 50, rectY + 450, rectWidth + 550, rectHeight + 100);
            Rectangle sendButtonRect = new Rectangle(rectX + 610, rectY + 450, rectWidth + 100, rectHeight + 100);
            Rectangle chatWindow = new Rectangle(rectX + 50, rectY + 50, rectWidth + 550, rectHeight + 380);

            // ChatWindow
            Raylib.DrawRectangleRounded(chatWindow, 0.1f, 10, Colors.HoverColor);

            //Back button
            backButton.Update();
            backButton.Draw();
            
            // Send button
            Button sendButton = new Button(sendButtonRect, "Send", Colors.TextFieldColor, Colors.HoverColor,
                Colors.TextColor);
            sendButton.Draw();

            // Update and draw text field
            textField.Update();
            textField.Draw();

            // Chat window shit
            int startX = (int)rectX + 90;
            int startY = (int)rectY + 90;
            int lineHeight = 20;

            foreach (var msg in messages)
            {
                string text = $"{msg.Timestamp} - {msg.Sender} : {msg.Content}";
                Raylib.DrawText(text, startX, startY, 15, Colors.TextColor);
                startY += lineHeight;
            }


            // Mouse Logic
            if (MouseInput.IsLeftClick(typeWindow))
            {
                selected = SelectedField.TypeWindow;
            }
            else if (sendButton.IsClicked() || Raylib.IsKeyPressed(KeyboardKey.Enter) && !Raylib.IsKeyDown(KeyboardKey.LeftShift))
            {
                Log.Info("Send button clicked");
                // Click on Send: save message and clear input field
                if (!string.IsNullOrWhiteSpace(textField.Text))
                {
                    if (messageSender != null)
                    {
                        bool success = messageSender.SendMessage(textField.Text);
                        if (!success)
                        {
                            Log.Error("Failed to send message!");
                            Console.WriteLine("Failed to send message!");
                        }
                        else
                        {
                            Log.Success("Message sent successfully");
                            Console.WriteLine("Message sent successfully");

                            //Chat window shit
                            messages.Add(new MessageDTO
                            {
                                Sender = "DuckLord", // Change to real user later
                                Content = textField.Text,
                                Timestamp = DateTime.UtcNow
                            });
                        }

                    }

                }

                    // Empty text field
                    textField.Clear();
                
            }

            Raylib.EndDrawing();
        }

    }

}
  
