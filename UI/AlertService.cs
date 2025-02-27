using Player;

namespace UI.Services;

public class AlertService
{
    public event Action<IOperationResult>? OnAlert;

    public void ShowAlert(IOperationResult operationResult)
    {
        OnAlert?.Invoke(operationResult);
    }
}