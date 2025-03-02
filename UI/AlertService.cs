using Player;

namespace UI;

public class AlertService
{
    public event Action<IOperationResult>? OnAlert;

    public void ShowAlert(IOperationResult operationResult)
    {
        OnAlert?.Invoke(operationResult);
    }
}