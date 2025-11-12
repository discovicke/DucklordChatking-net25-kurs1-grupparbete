using ChatClient.Core;
using ChatClient.UI.Components;
using ChatClient.UI.Rendering;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class TextField : UIComponent
    {
        public string Text { get; private set; } = string.Empty;

        private bool IsSelected { get; set; }
        private readonly bool AllowMultiline;
        private readonly TextCursor cursor;
        private readonly TextRenderer renderer;
        private bool backspaceHandledThisFrame;


        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor,
            bool allowMultiline = false, bool isPassword = false)
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            AllowMultiline = allowMultiline;

            cursor = new TextCursor();
            renderer = new TextRenderer(rect, textColor, isPassword, allowMultiline);
        }

        public override void Draw()
        {
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : BackgroundColor;
            Raylib.DrawRectangleRounded(Rect, 0.3f, 10, fill);

            if (MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.3f, 10, 2, Color.Black);
            }

            renderer.Draw(Text, cursor, IsSelected);
        }

        public override void Update()
        {
            if (MouseInput.IsLeftClick(Rect))
            {
                IsSelected = true;
                cursor.ResetBlink();
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !MouseInput.IsHovered(Rect))
            {
                IsSelected = false;
                cursor.ResetInvisible();
            }

            if (!IsSelected) return;
            
            cursor.Update(Raylib.GetFrameTime());

            HandleTextInput();
                HandleNavigation();
                
        }

        private void HandleTextInput()
        {
            if (AllowMultiline && IsShiftEnterPressed())
            {
                InsertText("\n");
                return;
            }

            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if (key >= 32)
                    InsertText(char.ConvertFromUtf32(key));

                key = Raylib.GetCharPressed();
            }

            bool backspacePressed = Raylib.IsKeyPressed(KeyboardKey.Backspace) 
                                    || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace);

            if (backspacePressed && !backspaceHandledThisFrame)
            {
                DeleteCharacter();
                backspaceHandledThisFrame = true;
            }
            else if (!backspacePressed)
            {
                backspaceHandledThisFrame = false;
            }
        }

        private void HandleNavigation()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressedRepeat(KeyboardKey.Left))
            {
                cursor.MoveLeft(Text.Length);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressedRepeat(KeyboardKey.Right))
            {
                cursor.MoveRight(Text.Length);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Home))
            {
                cursor.MoveToStart();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.End))
            {
                cursor.MoveToEnd(Text.Length);
            }
        }

        private void InsertText(string s)
        {
            cursor.Position = Math.Clamp(cursor.Position, 0, Text.Length);
            Text = Text.Insert(cursor.Position, s);
            cursor.Position += s.Length;
            cursor.ResetBlink();
        }

        private void DeleteCharacter()
        {
            if (cursor.Position > 0 && Text.Length > 0)
            {
                int removeIndex = Math.Clamp(cursor.Position - 1, 0, Text.Length - 1);
                Text = Text.Remove(removeIndex, 1);
                cursor.Position = removeIndex;
                cursor.ResetBlink();
            }
        }

        private bool IsShiftEnterPressed()
        {
            return (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                   && Raylib.IsKeyPressed(KeyboardKey.Enter);
        }
        
        public void SetRect(Rectangle rect)
        {
            Rect = rect;
            renderer.UpdateBounds(rect);
        }

        public void Clear()
        {
            Text = string.Empty;
            cursor.Reset();
        }
        // TODO: Text row break when hitting border

        // TODO: Scroll logicZ

        // TODO: Font?
    }
}