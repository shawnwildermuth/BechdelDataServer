using MinimalApis.Discovery;
using static Microsoft.AspNetCore.Http.Results;

namespace BechdelDataServer.Apis;

public class BechdelApi : IApi
{
  public void Register(IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/films")
      .WithOpenApi();

    group.MapGet("", GetAllFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getAllFilms")
      .WithDescription("Gets all the Films")
      .WithOpenApi();

    group.MapGet(@"{imdbId:regex(tt\\d{{7,9}})}", GetFilm)
      .Produces<Film>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getFilm")
      .WithDescription("Get's a film")
      .WithOpenApi();

    group.MapGet("{year:int}", GetFilmByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getFilmsByYear")
      .WithDescription("Gets Films by Year")
      .WithOpenApi();

    group.MapGet("failed", GetFailedFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getFailedFilms")
      .WithDescription("Gets All Failed Films")
      .WithOpenApi();

    group.MapGet("passed", GetPassedFilms)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getPassedFilms")
      .WithDescription("Gets All Passed Films")
      .WithOpenApi();

    group.MapGet("failed/{year:int}", GetFailedFilmsByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getFailedFilmsByYear")
      .WithDescription("Gets Failed Films by Year")
      .WithOpenApi();

    group.MapGet("passed/{year:int}", GetPassedFilmsByYear)
      .Produces<FilmResult>()
      .Produces(404)
      .ProducesProblem(500)
      .WithName("getPassedFilmsByYear")
      .WithDescription("Gets Passed Films by Year")
      .WithOpenApi();

    app.MapGet("api/years", GetYears)
      .Produces<int[]>()
      .Produces<string[]>(200)
      .ProducesProblem(500)
      .WithName("getYears")
      .WithDescription("Gets all the years that encompass the film library.")
      .WithOpenApi();

  }

  public async Task<IResult> GetFilm(BechdelDataService sc, string imdbId)
  {
    var result = await sc.FindFilmByImdbId(imdbId);
    if (result is null) return NotFound();
    return Ok(result);
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
