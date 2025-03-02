using System.Text.Json;
using Microsoft.JSInterop;
using Player;

namespace GameEngine;

public class IndexedDbService(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly string _dbName = "attackResultsDb";
    private readonly string _storeName = "attackResults";
    private readonly string _counterStoreName = "idCounter";
    private bool _isInitialized;
    public long CurrentMaxId;

    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            // Initialize the database with both stores
            await jsRuntime.InvokeVoidAsync("indexedDBInterop.openDatabaseWithCounter", _dbName, _storeName, _counterStoreName);

            // Retrieve the current max ID
            await InitializeMaxIdFromDatabaseAsync();

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to initialize IndexedDB: {ex.Message}");
            throw;
        }
    }

    private async Task InitializeMaxIdFromDatabaseAsync()
    {
        try
        {
            // Find the max ID from existing items first
            var results = await jsRuntime.InvokeAsync<object[]>("indexedDBInterop.getAllItems", _dbName, _storeName);

            if (results.Length > 0)
            {
                long maxId = 0;
                foreach (var item in results)
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(item));
                    long itemId = jsonElement.GetProperty("id").GetInt64();
                    if (itemId > maxId)
                    {
                        maxId = itemId;
                    }
                }

                CurrentMaxId = maxId;
            }

            // Store the current max ID
            await jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName, CurrentMaxId);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to initialize max ID: {ex.Message}");
            // Default to 0 if there's an error
            CurrentMaxId = 0;

            try
            {
                // Try to store the default value
                await jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName, CurrentMaxId);
            }
            catch
            {
                // Ignore if this fails too
            }
        }
    }

    private long GetNextId()
    {
        return Interlocked.Increment(ref CurrentMaxId);
    }

    private async Task UpdateStoredCounterAsync()
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("indexedDBInterop.saveCounter", _dbName, _counterStoreName, CurrentMaxId);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to update stored counter: {ex.Message}");
        }
    }

    public async Task<bool> SaveAsync(AttackResult attackResult)
    {
        if (!_isInitialized)
            await InitializeAsync();

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
                attackResult.TurnNumber,
                attackResult.FameChange,
            };

            await jsRuntime.InvokeVoidAsync("indexedDBInterop.saveItem", _dbName, _storeName, dbRecord);
            return true;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to save attack result: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        if (!_isInitialized)
            await InitializeAsync();

        try
        {
            await jsRuntime.InvokeVoidAsync("indexedDBInterop.deleteItem", _dbName, _storeName, id);
            return true;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to delete attack result: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> ClearAsync()
    {
        if (!_isInitialized)
            await InitializeAsync();

        try
        {
            await jsRuntime.InvokeVoidAsync("indexedDBInterop.clearStore", _dbName, _storeName);

            // Reset the counter to 0 after clearing the store
            CurrentMaxId = 0;
            await UpdateStoredCounterAsync();
            return true;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to clear attack results: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<AttackResult>> GetAllAsync()
    {
        if (!_isInitialized)
            await InitializeAsync();

        // Fetch all items from IndexedDB
        var results = await jsRuntime.InvokeAsync<object[]>("indexedDBInterop.getAllItems", _dbName, _storeName);

        // Convert to AttackResult objects
        var attackResults = new List<AttackResult>();
        foreach (var item in results)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(item));
            attackResults.Add(DeserializeAttackResult(jsonElement));
        }

        return attackResults;
    }

    // public async Task<AttackResult> GetByIdAsync(long id)
    // {
    //     if (!_isInitialized)
    //         await InitializeAsync();
    //
    //     try
    //     {
    //         var result = await _jsRuntime.InvokeAsync<object>("indexedDBInterop.getItemById", _dbName, _storeName, id);
    //
    //         if (result == null)
    //             return null;
    //
    //         var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(result));
    //         return DeserializeAttackResult(jsonElement);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.Error.WriteLine($"Failed to get attack result: {ex.Message}");
    //         throw;
    //     }
    // }

    public async Task<List<AttackResult>> GetRangeAsync(long startId, long count)
    {
        if (!_isInitialized)
            await InitializeAsync();

        try
        {
            var results = await jsRuntime.InvokeAsync<List<object>>("indexedDBInterop.getRange", _dbName, _storeName, startId, count);
            var attackResults = new List<AttackResult>();

            foreach (var result in results)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(result));
                attackResults.Add(DeserializeAttackResult(jsonElement));
            }

            return attackResults;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to get attack results range: {ex.Message}");
            throw;
        }
    }

    private AttackResult DeserializeAttackResult(JsonElement jsonElement)
    {
        var attackResult = new AttackResult
        {
            Id = jsonElement.GetProperty("id").GetInt64(),
            AttackSucceeded = jsonElement.GetProperty("attackSucceeded").GetBoolean(),
            MenLost = jsonElement.GetProperty("menLost").GetInt64(),
            MoneyStolen = jsonElement.GetProperty("moneyStolen").GetInt64(),
            GuardsKilled = jsonElement.GetProperty("guardsKilled").GetInt64(),
            Success = jsonElement.GetProperty("success").GetBoolean(),
            Type = (MessageType)jsonElement.GetProperty("type").GetInt32(),
            Attacker = jsonElement.GetProperty("attacker").GetString()!,
            Defender = jsonElement.GetProperty("defender").GetString()!,
            TurnNumber = jsonElement.GetProperty("turnNumber").GetInt64(),
            FameChange= jsonElement.GetProperty("fameChange").GetInt64(),
        };

        // Deserialize weapons stolen
        var weaponsElement = jsonElement.GetProperty("weaponsStolen");
        foreach (var weaponItem in weaponsElement.EnumerateArray())
        {
            var weaponType = Enum.Parse<Weapon>(weaponItem.GetProperty("weaponType").GetString()!);
            var count = weaponItem.GetProperty("count").GetInt64();
            attackResult.WeaponsStolen[weaponType] = count;
        }

        return attackResult;
    }

    public ValueTask DisposeAsync()
    {
        // Nothing specific to clean up with async disposal
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}