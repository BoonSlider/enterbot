
namespace GameEngine;

public class DbHelper
{
    public static IndexedDbService Db { get; set; }

    public DbHelper(IndexedDbService dbService)
    {
        Db = dbService;
    }
}