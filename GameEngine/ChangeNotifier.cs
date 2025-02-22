namespace GameEngine;

public class ChangeNotifier
{
    public event Action? OnChange;
    public event Func<Task>? OnChangeAsync;

    public async Task Notify()
    {
        OnChange?.Invoke();
        if (OnChangeAsync != null)
        {
            var handlers = OnChangeAsync.GetInvocationList()
                .Cast<Func<Task>>();

            foreach (var handler in handlers)
            {
                await handler(); // Ensuring each subscriber runs asynchronously
            }
        }
    }
}