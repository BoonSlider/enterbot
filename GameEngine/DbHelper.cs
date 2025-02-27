
namespace GameEngine;

public class DbHelper
{
    public static AttackResultStorageService Db { get; set; }

    public DbHelper(AttackResultStorageService dbService)
    {
        Db = dbService;
    }
}