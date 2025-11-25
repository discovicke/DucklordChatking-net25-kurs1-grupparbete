namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Provides validation logic for user input
/// </summary>
public static class InputValidator
{
    public static ValidationResult ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return ValidationResult.Failure("Quackername cannot be empty!");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Failure("Password cannot be empty!");
        }

        if (password.Length < 8)
        {
            return ValidationResult.Failure("Password must be at least 8 characters!");
        }

        if (password.Any(char.IsWhiteSpace))
        {
            return ValidationResult.Failure("Password can not contain blank spaces!");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidatePasswordMatch(string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            return ValidationResult.Failure("Passwords do not match!");
        }

        return ValidationResult.Success();
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(string message) => new ValidationResult(false, message);
}

