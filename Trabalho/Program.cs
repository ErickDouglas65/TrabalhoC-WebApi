using Microsoft.EntityFrameworkCore;
using Trabalho;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Database;Trusted_Connection=True;")

);

//var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("Trabalho") ?? "Data Source=Trabalho.db";
//builder.Services.AddSqlite<AppDbContext>(connectionString);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

/*     CRUD Client*/
// Listar os clientes
app.MapGet("/client", async (AppDbContext db) =>
    await db.Clients.ToListAsync());

// Listar um cliente pelo Id
app.MapGet("/client/{id}", async (int id, AppDbContext db) =>
    await db.Clients.FindAsync(id)
        is Client client
            ? Results.Ok(client)
            : Results.NotFound());

// Adicionar um cliente
app.MapPost("/client", async (Client client, AppDbContext db) =>
{
    db.Clients.Add(client);
    await db.SaveChangesAsync();

    return Results.Created($"/client/{client.Id}", client);
});

// Atualizar o client
app.MapPut("/client/{id}", async (int id, Client inputClient, AppDbContext db) =>
{
    var client = await db.Clients.FindAsync(id);

    if (client is null) return Results.NotFound();

    client.Name = inputClient.Name;
    client.Cidade = inputClient.Cidade;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Apagar um cliente
app.MapDelete("/client/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Clients.FindAsync(id) is Client client)
    {
        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});



/*     CRUD Game*/
// Listar os jogos
app.MapGet("/game", async (AppDbContext db) =>
    await db.Games.ToListAsync());

// Listar um jogo pelo Id
app.MapGet("/game/{id}", async (int id, AppDbContext db) =>
    await db.Games.FindAsync(id)
        is Game game
            ? Results.Ok(game)
            : Results.NotFound());

// Adicionar um jogo
app.MapPost("/game", async (Game game, AppDbContext db) =>
{
    db.Games.Add(game);
    await db.SaveChangesAsync();

    return Results.Created($"/game/{game.Id}", game);
});

// Atualizar o jogo
app.MapPut("/game/{id}", async (int id, Game inputGame, AppDbContext db) =>
{
    var game = await db.Games.FindAsync(id);

    if (game is null) return Results.NotFound();

    game.Name = inputGame.Name;
    game.Genre = inputGame.Genre;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Apagar um jogo
app.MapDelete("/game/{id}", async (int id, AppDbContext db) =>
{
    var game = await db.Games.FindAsync(id);
    if (game == null)
    {
        return Results.NotFound();
    }

    if (game.IsRented)
    {
        return Results.BadRequest("O jogo está alugado, você não pode excluí-lo");
    }


    db.Games.Remove(game);
    await db.SaveChangesAsync();
    return Results.NoContent();

});

/*     Rental*/
// Locar um game
app.MapPost("/rental", async (AppDbContext db, Rental model) =>
{
    var client = await db.Clients.FindAsync(model.ClientId);
    var game = await db.Games.FindAsync(model.GameId);

    if (client == null || game == null)
    {
        return Results.NotFound();
    }   

    if (game.IsRented == true)
    {
        return Results.BadRequest("O jogo já está alugado.");
    }

    if (client.Age < GetAgeRatingMinimumAge(game.AgeRating))
    {
        return Results.BadRequest("Você não tem idade suficiente para alugar o jogo");
    }

    if (client.CurrentRentals >= client.MaxRentals)
    {
        return Results.BadRequest("Você atingiu o limite de aluguéis permitido");
    }

    if (!game.IsAvailable)
    {
        return Results.BadRequest("Este jogo não está disponível para aluguel no momento");
    }

    var rental = new Rental
    {
        RentalDate = DateTime.Now,
        ClientId = model.ClientId,
        GameId = model.GameId
    };

    db.Rentals.Add(rental);
    game.IsRented = true;
    client.CurrentRentals++;
    await db.SaveChangesAsync();

    return Results.Created($"/rental/{rental.Id}", rental);
});

// Devolver um game
app.MapPost("/return/{rentalId}", async (int rentalId, AppDbContext db) =>
{
    var rental = await db.Rentals.FindAsync(rentalId);

    if (rental == null)
    {
        return Results.NotFound();
    }

    var client = await db.Games.FindAsync(rental.ClientId);
    if (client == null || client.Id != rental.ClientId)
    {
        return Results.BadRequest("Você não tem permissão para devolver este jogo");
    }

    rental.ReturnDate = DateTime.Now;
    var game = await db.Games.FindAsync(rental.GameId);

    if (game != null && game.IsRented)
    {
        rental.ReturnDate = DateTime.Now;
        game.IsRented = false;

        await db.SaveChangesAsync();

        return Results.Ok("Jogo devolvido com suesso");
    }
    else
    {
        return Results.BadRequest("O jogo ja foi devolvido ou não esta alugado");
    }

});

int GetAgeRatingMinimumAge(string ageRating)
{
    if (ageRating == "Livre") return 0;
    if (ageRating == "10 anos") return 10;
    if (ageRating == "16 anos") return 16;
    if (ageRating == "18 anos") return 20;


    return 0;
}

app.Run();