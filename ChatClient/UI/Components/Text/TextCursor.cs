namespace ChatClient.UI.Components.Text
{
    /// <summary>
    /// Responsible for: managing text cursor position and blinking animation within text fields.
    /// Handles cursor movement (left, right, home, end) and provides visible/invisible blinking feedback.
    /// </summary>
    public class TextCursor
    {
        private float BlinkTimer { get; set; }
        private const float BlinkInterval = 0.5f;
        public int Position { get; set; }
        public bool IsVisible { get; private set; } = false;
        
        
        public void ResetInvisible()
        {
            IsVisible = false;
            BlinkTimer = 0f;
        }

        public void Update(float deltaTime)
        {
            BlinkTimer += deltaTime;
            if (BlinkTimer >= BlinkInterval)
            {
                BlinkTimer = 0f;
                IsVisible = !IsVisible;
            }
        }

        public void ResetBlink()
        {
            BlinkTimer = 0f;
            IsVisible = true;
        }

        public void MoveLeft(int textLength)
        {
            if (Position > 0)
            {
                Position--;
                ResetBlink();
            }
        }

        public void MoveRight(int textLength)
        {
            if (Position < textLength)
            {
                Position++;
                ResetBlink();
            }
        }

        public void MoveToStart()
        {
            Position = 0;
            ResetBlink();
        }

        public void MoveToEnd(int textLength)
        {
            Position = textLength;
            ResetBlink();
        }

        public void Reset()
        {
            Position = 0;
            ResetBlink();
        }

        // New: central clamp tied to current text length
        public void ClampToTextLength(int textLength)
        {
            Position = Math.Clamp(Position, 0, textLength);
        }
    }
}