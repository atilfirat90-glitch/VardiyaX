using Microsoft.EntityFrameworkCore;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Tests;

public static class TestDbContextFactory
{
    public static ShiftCraftDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ShiftCraftDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ShiftCraftDbContext(options);
    }
}