using System.Collections.Concurrent;
using Microsoft.JSInterop;
using Player;

namespace GameEngine;

public class IndexedDbService : IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _dbName = "attackResultsDb";
    private readonly string _storeName = "attackResults";
    private readonly string _counterStoreName = "idCounter";
    private readonly ConcurrentQueue<Func<Task>> _operationQueue = new ConcurrentQueue<Func<Task>>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _processingTask;
    private bool _isInitialized = false;
    private bool _isDisposed = false;
    public long CurrentMaxId = 0;

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _processingTask = ProcessOperationQueueAsync(_cts.Token);
    }

    public void Initialize()
    {
        if (_isInitialized)
            return;

        EnqueueOperation(async () =>
        {
            try
            {
                // Initialize the database with both stores
                await _jsRuntime.InvokeVoidAsync("indexedDBInterop.openDatabaseWithCounter", _dbName, _storeName,
                    _counterStoreName);

                // Retrieve the current max ID
                await InitializeMaxIdFromDatabaseAsync();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to initialize IndexedDB: {ex.Message}");
                throw;
            }
        });
    }

    private async Task InitializeMaxIdFromDatabaseAsync()
    {
        try
        {
            // Find the max ID from existing items first
            var results = await _jsRuntime.InvokeAsync<object[]>("indexedDBInterop.getAllItems", _dbName, _storeName);

            if (results != null && results.Length > 0)
            {
                long maxId = 0;
                foreach (var item in results)
                {
                    var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                        System.Text.Json.JsonSerializer.Serialize(item));

                    long itemId = jsonElement.GetProperty("id").GetInt64();
                    if (itemId > maxId)
                    {
                        maxId = itemId;
                    }
                }

                CurrentMaxId = maxId;
            }

            // Store the current max ID
            await _jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName, CurrentMaxId);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to initialize max ID: {ex.Message}");
            // Default to 0 if there's an error
            CurrentMaxId = 0;

            try
            {
                // Try to store the default value
                await _jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName,
                    CurrentMaxId);
            }
            catch
            {
                // Ignore if this fails too
            }
        }
    }

    private long GetNextId()
    {
        // Increment and return the next ID
        return Interlocked.Increment(ref CurrentMaxId);
    }

    private async Task UpdateStoredCounterAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName, CurrentMaxId);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to update stored counter: {ex.Message}");
        }
    }

    public bool Save(AttackResult attackResult)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        if (!_isInitialized)
            Initialize();

        EnqueueOperation(async () =>
        {
            try
            {
                // Auto-assign ID for new items (if ID is 0 or negative)
                if (attackResult.Id <= 0)
                {
                    attackResult.Id = GetNextId();
                    await UpdateStoredCounterAsync();
                }
                else if (attackResult.Id > CurrentMaxId)
                {
                    // Update max ID if a manually assigned ID is higher
                    CurrentMaxId = attackResult.Id;
                    await UpdateStoredCounterAsync();
                }

                // Serialize any complex properties if needed
                var serializedWeapons = attackResult.WeaponsStolen
                    .Select(kv => new { WeaponType = kv.Key.ToString(), Count = kv.Value })
                    .ToList();

                // Create a serializable object
                var dbRecord = new
                {
                    attackResult.Id,
                    attackResult.AttackSucceeded,
                    attackResult.MenLost,
                    attackResult.MoneyStolen,
                    attackResult.GuardsKilled,
                    attackResult.Success,
                    WeaponsStolen = serializedWeapons,
                    attackResult.Type,
                    attackResult.Attacker,
                    attackResult.Defender,
                };

                await _jsRuntime.InvokeVoidAsync("indexedDBInterop.saveItem", _dbName, _storeName, dbRecord);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to save attack result: {ex.Message}");
                throw;
            }
        });

        return true;
    }

    public bool Delete(long id)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        if (!_isInitialized)
            Initialize();

        EnqueueOperation(async () =>
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("indexedDBInterop.deleteItem", _dbName, _storeName, id);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to delete attack result: {ex.Message}");
                throw;
            }
        });

        return true;
    }

    public bool Clear()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        if (!_isInitialized)
            Initialize();

        EnqueueOperation(async () =>
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("indexedDBInterop.clearStore", _dbName, _storeName);

                // Reset the counter to 0 after clearing the store
                CurrentMaxId = 0;
                await UpdateStoredCounterAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to clear attack results: {ex.Message}");
                throw;
            }
        });

        return true;
    }

    public async Task<IEnumerable<AttackResult>> GetAll()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        if (!_isInitialized)
            Initialize();

        var tcs = new TaskCompletionSource<IEnumerable<AttackResult>>();

        // Fetch all items from IndexedDB
        var results = await _jsRuntime.InvokeAsync<object[]>("indexedDBInterop.getAllItems", _dbName, _storeName);

        // Convert to AttackResult objects
        var attackResults = new List<AttackResult>();
        foreach (var item in results)
        {
            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                System.Text.Json.JsonSerializer.Serialize(item));

            var attackResult = new AttackResult
            {
                Id = jsonElement.GetProperty("id").GetInt64(),
                AttackSucceeded = jsonElement.GetProperty("attackSucceeded").GetBoolean(),
                MenLost = jsonElement.GetProperty("menLost").GetInt64(),
                MoneyStolen = jsonElement.GetProperty("moneyStolen").GetInt64(),
                GuardsKilled = jsonElement.GetProperty("guardsKilled").GetInt64(),
                Success = jsonElement.GetProperty("success").GetBoolean(),
                Type = (MessageType)jsonElement.GetProperty("type").GetInt32(),
                Attacker = jsonElement.GetProperty("attacker").GetString(),
                Defender = jsonElement.GetProperty("defender").GetString(),
            };

            // Deserialize weapons stolen
            var weaponsElement = jsonElement.GetProperty("weaponsStolen");
            foreach (var weaponItem in weaponsElement.EnumerateArray())
            {
                var weaponType = Enum.Parse<Weapon>(weaponItem.GetProperty("weaponType").GetString());
                var count = weaponItem.GetProperty("count").GetInt64();
                attackResult.WeaponsStolen[weaponType] = count;
            }

            attackResults.Add(attackResult);
        }

        return attackResults;
    }

    public async Task<AttackResult> GetByIdAsync(long id)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        try
        {
            var result = await _jsRuntime.InvokeAsync<object>("indexedDBInterop.getItemById", _dbName, _storeName, id);

            if (result == null)
                return null;

            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                System.Text.Json.JsonSerializer.Serialize(result));

            var attackResult = new AttackResult
            {
                Id = jsonElement.GetProperty("id").GetInt64(),
                AttackSucceeded = jsonElement.GetProperty("attackSucceeded").GetBoolean(),
                MenLost = jsonElement.GetProperty("menLost").GetInt64(),
                MoneyStolen = jsonElement.GetProperty("moneyStolen").GetInt64(),
                GuardsKilled = jsonElement.GetProperty("guardsKilled").GetInt64(),
                Success = jsonElement.GetProperty("success").GetBoolean(),
                Type = (MessageType)jsonElement.GetProperty("type").GetInt32(),
                Attacker  = jsonElement.GetProperty("attacker").GetString(),
                Defender = jsonElement.GetProperty("defender").GetString(),
            };

            var weaponsElement = jsonElement.GetProperty("weaponsStolen");
            foreach (var weaponItem in weaponsElement.EnumerateArray())
            {
                var weaponType = Enum.Parse<Weapon>(weaponItem.GetProperty("weaponType").GetString());
                var count = weaponItem.GetProperty("count").GetInt64();
                attackResult.WeaponsStolen[weaponType] = count;
            }

            return attackResult;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to get attack result: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AttackResult>> GetRangeAsync(long startId, long count)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IndexedDbService));

        try
        {
            var results = await _jsRuntime.InvokeAsync<List<object>>("indexedDBInterop.getRange", _dbName, _storeName, startId, count);
            var attackResults = new List<AttackResult>();

            foreach (var result in results)
            {
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                    System.Text.Json.JsonSerializer.Serialize(result));

                var attackResult = new AttackResult
                {
                    Id = jsonElement.GetProperty("id").GetInt64(),
                    AttackSucceeded = jsonElement.GetProperty("attackSucceeded").GetBoolean(),
                    MenLost = jsonElement.GetProperty("menLost").GetInt64(),
                    MoneyStolen = jsonElement.GetProperty("moneyStolen").GetInt64(),
                    GuardsKilled = jsonElement.GetProperty("guardsKilled").GetInt64(),
                    Success = jsonElement.GetProperty("success").GetBoolean(),
                    Type = (MessageType)jsonElement.GetProperty("type").GetInt32(),
                    Attacker  = jsonElement.GetProperty("attacker").GetString(),
                    Defender = jsonElement.GetProperty("defender").GetString(),
                };

                var weaponsElement = jsonElement.GetProperty("weaponsStolen");
                foreach (var weaponItem in weaponsElement.EnumerateArray())
                {
                    var weaponType = Enum.Parse<Weapon>(weaponItem.GetProperty("weaponType").GetString());
                    var wCount = weaponItem.GetProperty("count").GetInt64();
                    attackResult.WeaponsStolen[weaponType] = wCount;
                }

                attackResults.Add(attackResult);
            }

            return attackResults;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to get attack results range: {ex.Message}");
            throw;
        }
    }

    private void EnqueueOperation(Func<Task> operation)
    {
        _operationQueue.Enqueue(operation);
    }

    private async Task ProcessOperationQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (_operationQueue.TryDequeue(out var operation))
                {
                    await operation();
                }
                else
                {
                    // If the queue is empty, wait a bit before checking again
                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing operation: {ex.Message}");
                // Continue processing other operations
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _cts.Cancel();
        _cts.Dispose();
        _semaphore.Dispose();

        // Wait for the processing task to complete
        try
        {
            _processingTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch
        {
            // Ignore exceptions during disposal
        }
    }
}