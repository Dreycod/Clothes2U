using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Helpers;

public static class DbContextHelper
{
    public static Clothes2UDbContext GetInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        
        var context = new Clothes2UDbContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        return context;
    }
    
    
}