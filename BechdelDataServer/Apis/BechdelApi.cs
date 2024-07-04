using MinimalApis.Discovery;
using static Microsoft.AspNetCore.Http.Results;

namespace BechdelDataServer.Apis;

public class BechdelApi : IApi
{
  public void Register(IEndpointRouteBuilder app)
  {
    app.MapGet("/api/films", GetAllFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getAllFilms")
      .WithDescription("Gets all the Films")
      .WithOpenApi();

    app.MapGet("/api/film", async (BechdelDataService sc) =>
    {
      var result = await sc.LoadAllFilmsAsync(1, 1);
      return Ok(result.Results?.First());
    })
      .Produces<Film>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getFilm")
      .WithDescription("Get's a film")
      .WithOpenApi();

    app.MapGet("/api/films/{year:int}", GetFilmByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithOpenApi();

    app.MapGet("/api/films/failed", GetFailedFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithOpenApi();

    app.MapGet("/api/films/passed", GetPassedFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithOpenApi();

    app.MapGet("/api/films/failed/{year:int}", GetFailedFilmsByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithOpenApi();

    app.MapGet("/api/films/passed/{year:int}", GetPassedFilmsByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithOpenApi();

    app.MapGet("api/years", GetYears)
      .Produces<int[]>()
      .Produces<string[]>(200)
      .ProducesProblem(500)
      .WithOpenApi();

  }

  public static async Task<IResult> GetAllFilms(BechdelDataService ds, int? page, int? pageSize)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadAllFilmsAsync(pageNumber, pagerTake);
    if (data.Results is null) return NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetFilmByYear(BechdelDataService ds, int? page, int? pageSize, int year)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadAllFilmsByYearAsync(pageNumber, pagerTake, year);
    if (data.Results is null) return NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetFailedFilms(BechdelDataService ds, int? page, int? pageSize)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAsync(false, pageNumber, pagerTake);
    if (data.Results is null) return NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetPassedFilms(BechdelDataService ds, int? page, int? pageSize)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAsync(true, pageNumber, pagerTake);
    if (data.Results is null) return NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetFailedFilmsByYear(BechdelDataService ds, int year, int? page, int? pageSize)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAndYearAsync(false, year, pageNumber, pagerTake);
    if (data.Results is null) NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetPassedFilmsByYear(BechdelDataService ds, int year, int? page, int? pageSize)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAndYearAsync(true, year, pageNumber, pagerTake);
    if (data.Results is null) NotFound();
    return Ok(data);
  }

  public static async Task<IResult> GetYears(BechdelDataService ds)
  {
    if (ds is null) return Problem("Server Error", statusCode: 500);
    var data = await ds.LoadFilmYears();
    if (data is null) NotFound();
    return Ok(data);
  }
}
