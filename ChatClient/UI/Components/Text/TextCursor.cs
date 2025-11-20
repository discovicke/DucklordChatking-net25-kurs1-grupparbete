namespace ChatClient.UI.Components.Text
{
    /// <summary>
    /// Responsible for: managing text cursor position and blinking animation within text fields.
    /// Handles cursor movement (left, right, home, end) and provides visible/invisible blinking feedback.
    /// </summary>
    public class TextCursor
    {
        private int position;
        private float blinkTimer;
        private bool visible = false;
        private const float BlinkInterval = 0.5f;

        public int Position
        {
            get => position;
            set => position = Math.Clamp(value, 0, int.MaxValue);
        }

        public bool IsVisible => visible;

        public void ResetInvisible()
        {
            visible = false;
            blinkTimer = 0f;
        }

        public void Update(float deltaTime)
        {
            blinkTimer += deltaTime;
            if (blinkTimer >= BlinkInterval)
            {
                blinkTimer = 0f;
                visible = !visible;
            }
        }

        public void ResetBlink()
        {
            blinkTimer = 0f;
            visible = true;
        }

        public void MoveLeft(int textLength)
        {
            if (position > 0)
            {
                position--;
                ResetBlink();
            }
        }

        public void MoveRight(int textLength)
        {
            if (position < textLength)
            {
                position++;
                ResetBlink();
            }
        }

        public void MoveToStart()
        {
            position = 0;
            ResetBlink();
        }

        public void MoveToEnd(int textLength)
        {
            position = textLength;
            ResetBlink();
        }

        public void Reset()
        {
            position = 0;
            ResetBlink();
        }

        // New: central clamp tied to current text length
        public void ClampToTextLength(int textLength)
        {
            position = Math.Clamp(position, 0, textLength);
        }
    }
}