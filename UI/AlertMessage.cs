namespace UI;

public class AlertMessage(string message)
{
    public string Message { get; set; } = message;
    public bool? OverrideSuccess { get; set; }
}