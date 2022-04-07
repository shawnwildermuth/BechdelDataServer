namespace BechdelDataServer.Services;

internal class BechdelDataService
{
  private readonly ILogger<BechdelDataService> _logger;
  private readonly IConfiguration _config;
  private IEnumerable<Film>? _data;

  public BechdelDataService(ILogger<BechdelDataService> logger, IConfiguration config)
  {
    _logger = logger;
    _config = config;
  }

  public async Task<FilmResult> LoadFilmsByResultAndYearAsync(
    bool succeeded,
    int year,
    int page,
    int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return FilmResult.Default;

    IOrderedEnumerable<Film> qry = data
      .Where(y => y.Year == year)
      .Where(f => f.Passed == succeeded)
      .OrderBy(f => f.Title);

    return await GetFilmResult(qry, page, pageSize);
  }

  public async Task<FilmResult> LoadFilmsByResultAsync(
    bool succeeded,
    int page,
    int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return FilmResult.Default;

    IOrderedEnumerable<Film> qry = data
      .Where(f => f.Passed == succeeded)
      .OrderBy(f => f.Title);

    return await GetFilmResult(qry, page, pageSize);
  }

  public async Task<FilmResult> LoadAllFilmsAsync(int page, int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return FilmResult.Default;

    IOrderedEnumerable<Film> qry = data.OrderBy(d => d.Year)
                                       .ThenBy(f => f.Title);

    return await GetFilmResult(qry, page, pageSize);
  }

  async Task<FilmResult> GetFilmResult(IOrderedEnumerable<Film> query, int page, int pageSize)
  {
    var count = query.Count();
    var pageCount = (int)Math.Ceiling(((double)count / (double)pageSize));
    var results = query.Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();

    await LoadTmdbInfo(results);

    return new FilmResult(count, pageCount, page, results);
  }

  async Task LoadTmdbInfo(IEnumerable<Film> results)
  {
    string tmdbKey = _config["tmdb:apikey"];
    foreach (var film in results)
    {
      if (!film.InfoLoaded)
      {
        using var client = new HttpClient();

        var url = $"https://api.themoviedb.org/3/find/{film.IMDBId}?api_key={tmdbKey}&external_source=imdb_id";
        var result = await client.GetFromJsonAsync<TmdbResult>(url);
        if (result is not null && result.movie_results.Any())
        {
          var imdb = result.movie_results.First();
          film.InfoLoaded = true;
          film.PosterUrl = $"https://image.tmdb.org/t/p/w200{imdb.poster_path}";
          film.Overview = imdb.overview;
          film.Rating = imdb.vote_average;
        }
      }
    }
  }

  protected async Task<IEnumerable<Film>?> LoadAsync()
  {
    if (_data is null)
    {
      var opts = new JsonSerializerOptions()
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      _logger.LogInformation("Loading From Data Json");
      var json = await File.ReadAllTextAsync("./bechdel.json");
      var bechdelData = JsonSerializer.Deserialize<BechdelFilms>(json, opts);
      _data = MapToFilms(bechdelData?.Years);
      _logger.LogInformation("Loaded From Data Json");
    }

    return _data;
  }

  internal async Task ProcessJson()
  {
    var data = await LoadAsync();
    if (data is not null)
    {
      Console.WriteLine("Loading The Movie Database Data");
      await LoadTmdbInfo(data);
      var json = JsonSerializer.Serialize(data);
      File.WriteAllText(@"c:\users\shawn\desktop\bechdel-films.json", json);
    }
  }

  private IEnumerable<Film> MapToFilms(IEnumerable<FilmYear>? years)
  {
    var result = new List<Film>();

    if (years is null) return result;

    foreach (var year in years)
    {
      var films = year.Films.ToArray();

      foreach (var film in films)
      {
        var newFilm = new Film()
        {
          Year = year.Year,
          Title = film.Title,
          IMDBId = film.IMDBId,
          Passed = film.Success,
          Budget = film.Budget,
          DomesticGross = film.DomesticGross,
          InternationalGross = film.InternationalGross
        };

        switch (film.ResultReason)
        {
          case "ok":
          case "ok-disagree":
            newFilm.Reason = "Passed";
            break;
          case "dubious":
          case "dubious-disagree":
            newFilm.Reason = "Passed, but some users were skeptical of the result";
            break;
          case "notalk":
          case "notalk-disagree":
            newFilm.Reason = "Women do not talk";
            break;
          case "men":
          case "men-disagree":
            newFilm.Reason = "Women talk to each other about men";
            break;
          case "nowomen":
          case "nowomen-disagree":
            newFilm.Reason = "Less than two women in the film";
            break;
          default:
            newFilm.Reason = "Unknown reason";
            break;
        }

        result.Add(newFilm);
      }
    }

    return result;
  }
}
