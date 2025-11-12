using System.Numerics;
using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI.Components;
using Raylib_cs;
using Shared;


namespace ChatClient.UI.Screens
{
    public class ChatScreen
    {
        private enum SelectedField
        {
            None,
            TypeWindow
        }

        private static SelectedField selected = SelectedField.None;


        // Chat window shit: List of messages that are drawn every frame
        private static List<MessageDTO> messages = new List<MessageDTO>();
        private static double lastUpdateTime = 0;



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
            { BaseAddress = new Uri("http://192.168.20.17:5201/") });

        public static void Run()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(620, 25), 0, 0.15f, Color.White);
            var tillbakaknapp = new Rectangle(620, 25, 150, 250);
            if (MouseInput.IsLeftClick(tillbakaknapp))
            {
                AppState.CurrentScreen = Screen.Start;
            }

            // Chat window shit
            double currentTime = Raylib.GetTime(); // Raylib.GetTime() seconds since start
            if (currentTime - lastUpdateTime >= 1.0) // 1 second has passed
            {
                lastUpdateTime = currentTime;

                if (messageSender != null)
                {
                    var recieve = messageSender.ReceiveHistory();
                    if (recieve != null && recieve.Any())
                    {
                        messages = recieve.ToList();
                    }
                }
            }


            int rectX = 0;
            int rectY = 0;
            int rectWidth = 0;
            int rectHeight = 0;

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


            // Chat window shit: Draw all messages from messages every frame
            int startX = (int)chatWindow.X + 10;
            int startY = (int)chatWindow.Y + 10;
            int lineHeight = 20;

            foreach (var msg in messages)
            {
                string sender = string.IsNullOrWhiteSpace(msg.Sender) ? "Unknown" : msg.Sender;
                string text = $"{msg.Timestamp}  -  {sender} :  {msg.Content}";
                Raylib.DrawText(text, startX, startY, 15, Colors.TextColor);
                startY += lineHeight;
            }

            // Mouse Logic
            if (MouseInput.IsLeftClick(typeWindow))
            {
                selected = SelectedField.TypeWindow;
            }
            else if (sendButton.IsClicked() ||
                     Raylib.IsKeyPressed(KeyboardKey.Enter) && !Raylib.IsKeyDown(KeyboardKey.LeftShift))
            {
                Log.Info("Send button clicked");
                if (!string.IsNullOrWhiteSpace(textField.Text))
                {
                    if (messageSender != null)
                    {
                        bool success = messageSender.SendMessage(textField.Text);

                        // Chat window shit: Get history and update messages
                        var recieve = messageSender.ReceiveHistory();

                        if (!success)
                        {
                            Log.Error("Failed to send message!");
                            Console.WriteLine("Failed to send message!");
                        }
                        else
                        {
                            Log.Success("Message sent successfully");
                            Console.WriteLine("Message sent successfully");

                            if (recieve != null && recieve.Any())
                            {
                                // Chat window shit: Update message-list and draw it automaticly next fram
                                messages = recieve.ToList();
                            }
                        }
                    }

                    // Empty text field
                    textField.Clear();
                }

                textField.Clear();
            }

            Raylib.EndDrawing();
        }
    }
}