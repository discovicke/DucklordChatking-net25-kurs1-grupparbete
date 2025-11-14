using Raylib_cs;

namespace ChatClient.UI.Screens
{
    public abstract class ScreenBase<TLayout>
        where TLayout : struct
    {
        protected TLayout layout;
        private bool initialized = false;
        protected IScreenLogic? logic;
        
        // Track last window size to detect resize
        private int lastWidth = 0;
        private int lastHeight = 0;

        // Called once when the screen is first shown to compute positions/sizes
        protected abstract TLayout CalculateLayout();

        // Assign calculated rectangles to components (TextField.SetRect etc.)
        protected abstract void ApplyLayout(TLayout layout);

        // Draw the content of the screen (labels, fields, texture, footer)
        public abstract void RenderContent();

        // Main entry (called every frame)
        public void Run()
        {
            if (!initialized)
            {
                layout = CalculateLayout();
                ApplyLayout(layout);
                initialized = true;
                
                lastWidth = Raylib.GetScreenWidth();
                lastHeight = Raylib.GetScreenHeight();
            }
            
            // Detect resize and recalculate layout
            int w = Raylib.GetScreenWidth();
            int h = Raylib.GetScreenHeight();
            if (w != lastWidth || h != lastHeight)
            {
                lastWidth = w;
                lastHeight = h;
                RecalculateIfNeeded();
            }

            logic?.HandleInput();
            RenderContent();
        }

        // Optional helper to re-calc layout when window size changes
        protected void RecalculateIfNeeded()
        {
            var newLayout = CalculateLayout();
            // caller can decide how to compare / apply; simple replace for brevity
            layout = newLayout;
            ApplyLayout(layout);
        }
    }
}