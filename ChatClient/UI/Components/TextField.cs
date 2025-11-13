using ChatClient.Core;
using ChatClient.UI.Components;
using ChatClient.UI.Rendering;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class TextField : UIComponent
    {
        // TODO:
        // - Add scroll logic
        // - Add copy/paste support (Ctrl + C / Ctrl + V)
        // - Add cut support (Ctrl + X)
        // - Add undo/redo support (Ctrl + Z / Ctrl + Y)
        // - Add text selection support (Mouse drag || shift key)
        // - Add font support
        public string Text { get; private set; } = string.Empty;
        private string FieldName { get; set; } = "TextField";

        private bool IsSelected { get; set; }
        private readonly bool AllowMultiline;
        private readonly TextCursor cursor;
        private readonly TextRenderer renderer;
        private bool backspaceHandledThisFrame;


        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor,
            bool allowMultiline = false, bool isPassword = false, string fieldName = "TextField")
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            AllowMultiline = allowMultiline;
            FieldName = fieldName;

            cursor = new TextCursor();
            renderer = new TextRenderer(rect, textColor, isPassword, allowMultiline);
        }

        public override void Draw()
        {
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : BackgroundColor;
            if (IsSelected)
                fill = HoverColor;
            Raylib.DrawRectangleRounded(Rect, 0.3f, 10, fill);

            if (MouseInput.IsHovered(Rect) || IsSelected)
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.3f, 10, 2, Color.Black);
            }

            renderer.Draw(Text, cursor, IsSelected);
        }

        public override void Update()
        {
            if (MouseInput.IsLeftClick(Rect))
            {
                if (!IsSelected)
                {
                    Log.Info($"[{FieldName}] Field selected");
                }
                IsSelected = true;
                cursor.ResetBlink();
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !MouseInput.IsHovered(Rect))
            {
                if (IsSelected)
                {
                    Log.Info($"[{FieldName}] Field deselected - Final text: '{Text}'");
                }
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
                backspaceHandledThisFrame = false;
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
            
            string displayChar = s == "\n" ? "\\n" : s;
            Log.Info($"[{FieldName}] Text inserted: '{displayChar}' - Current text: '{Text.Replace("\n", "\\n")}'");
        }

        private void DeleteCharacter()
        {
            if (cursor.Position > 0 && Text.Length > 0)
            {
                int removeIndex = Math.Clamp(cursor.Position - 1, 0, Text.Length - 1);
                char deletedChar = Text[removeIndex];
                Text = Text.Remove(removeIndex, 1);
                cursor.Position = removeIndex;
                cursor.ResetBlink();
                
                string displayChar = deletedChar == '\n' ? "\\n" : deletedChar.ToString();
                Log.Info($"[{FieldName}] Character deleted: '{displayChar}' - Current text: '{Text.Replace("\n", "\\n")}'");
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
            Log.Info($"[{FieldName}] Field cleared - Previous text: '{Text.Replace("\n", "\\n")}'");
            Text = string.Empty;
            cursor.Reset();
        }

        // TODO: Scroll logicZ

        // TODO: Font?
    }
}