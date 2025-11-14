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
        private string PlaceholderText { get; set; } = "";

        private bool IsSelected { get; set; }
        private readonly bool AllowMultiline;
        private readonly TextCursor cursor;
        private readonly TextRenderer renderer;
        private bool backspaceHandledThisFrame;


        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor,
            bool allowMultiline = false, bool isPassword = false, string fieldName = "TextField", string placeholderText = "")
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            AllowMultiline = allowMultiline;
            FieldName = fieldName;
            PlaceholderText = placeholderText;

            cursor = new TextCursor();
            renderer = new TextRenderer(rect, textColor, isPassword, allowMultiline);
        }

        // TODO: Manage corner roundness
        public override void Draw()
        {
            // Determine fill color based on state
            Color fill;
            if (IsSelected)
                fill = Colors.TextFieldSelected;
            else if (MouseInput.IsHovered(Rect))
                fill = Colors.TextFieldHovered;
            else
                fill = Colors.TextFieldUnselected;
            
            // Draw background
            Raylib.DrawRectangleRounded(Rect, 0.1f, 10, fill);

            // Draw border/outline
            if (IsSelected || MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.1f, 10, 2, Colors.OutlineColor);
            }
            else
            {
                // Subtle outline when not selected/hovered
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.1f, 10, 1, Colors.OutlineColor);
            }

            // Draw text or placeholder
            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(PlaceholderText))
            {
                // Draw placeholder text (always shown when text is empty)
                int textX = (int)(Rect.X + 4);
                int textY = (int)(Rect.Y + 4);
                Raylib.DrawText(PlaceholderText, textX, textY, 20, Colors.PlaceholderText);
            }
            
            // Always draw actual text and cursor if there is text or field is selected
            if (!string.IsNullOrEmpty(Text) || IsSelected)
            {
                renderer.Draw(Text, cursor, IsSelected);
            }
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

            // Normal character input
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if (key >= 32)
                    InsertText(char.ConvertFromUtf32(key));

                key = Raylib.GetCharPressed();
            }

            // Backspace handling (single press / repeat)
            bool backspacePressed = (Raylib.IsKeyPressed(KeyboardKey.Backspace) && !backspaceHandledThisFrame)
                                    || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace);

            if (backspacePressed)
            {
                DeleteCharacter();
                backspaceHandledThisFrame = true;
            }
            else if (!backspacePressed)
            {
                backspaceHandledThisFrame = false;
            }

            // Clipboard / copy-paste / cut handling
            bool ctrlDown = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);

       
            // Copy: Ctrl + C
            if (ctrlDown && Raylib.IsKeyPressed(KeyboardKey.C))
            {
                try
                {
                    Raylib.SetClipboardText(Text ?? string.Empty);
                    Log.Info($"[{FieldName}] Copied to clipboard - Length: {(Text?.Length ?? 0)}");
                }
                catch (Exception ex)
                {
                    Log.Info($"[{FieldName}] Copy failed: {ex.Message}");
                }
            }

            // Paste: Ctrl + V
           

            if (ctrlDown && Raylib.IsKeyPressed(KeyboardKey.V))
            {
                try
                {
                    string clipboard = Raylib.GetClipboardText_();
                    if (!string.IsNullOrEmpty(clipboard))
                    {
                        InsertText(clipboard);
                        Log.Info($"[{FieldName}] Pasted from clipboard - Text length: {clipboard.Length}");
                    }
                    else
                    {
                        Log.Info($"[{FieldName}] Clipboard empty on paste");
                    }
                }
                catch (Exception ex)
                {
                    Log.Info($"[{FieldName}] Paste failed: {ex.Message}");
                }
            }


            // Cut: Ctrl + X

            if (ctrlDown && Raylib.IsKeyPressed(KeyboardKey.X))
            {
                try
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        Raylib.SetClipboardText(Text);
                        Log.Info($"[{FieldName}] Cut to clipboard - Previous text: '{Text.Replace("\n", "\\n")}'");
                        Text = string.Empty;
                        cursor.Position = 0;
                        cursor.ResetBlink();
                    }
                    else
                    {
                        Log.Info($"[{FieldName}] Cut requested but field is empty");
                    }
                }
                catch (Exception ex)
                {
                    Log.Info($"[{FieldName}] Cut failed: {ex.Message}");
                }
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
        // TODO: Mouse click to get position in text
        // TODO: Crtl backspace to  delete one word
        // TODO: Scroll logicZ
        // TODO: Font?
    }
}