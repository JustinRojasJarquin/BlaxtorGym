using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;

public static class DatabaseInitializer
{
    public static async Task InicializarAsync()
    {
        await using var context = new AppDbContext();

        await context.Database.EnsureCreatedAsync();
    }
}