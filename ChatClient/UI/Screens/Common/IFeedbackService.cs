using ChatClient.UI.Components.Specialized;

namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Interface for displaying feedback messages
/// </summary>
public interface IFeedbackService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void Update();
    void Draw();
}

public class FeedbackService(FeedbackBox feedbackBox) : IFeedbackService
{
    public void ShowSuccess(string message) => feedbackBox.Show(message, true);
    
    public void ShowError(string message) => feedbackBox.Show(message, false);
    
    public void Update() => feedbackBox.Update();
    
    public void Draw() => feedbackBox.Draw();
}

