namespace BechelServer.Services;

internal class BechdelDataServer
{
  private readonly ILogger<BechdelDataServer> _logger;
  private IEnumerable<FilmYear>? _data;

  public BechdelDataServer(ILogger<BechdelDataServer> logger)
  {
    _logger = logger;
  }

  public async Task<FilmYear?> LoadYearAsync(int id)
  {
    var data = await LoadAsync();
    var result = data?.Where(d => d.Year == id).FirstOrDefault();
    return result;
  }

  public async Task<IEnumerable<Film>?> LoadFilmsByResultAndYearAsync(bool succeeded, int year)
  {
    var data = await LoadAsync();

    var result = data?
      .Where(y => y.Year == year)
      .SelectMany(d => d.Films)
      .Where(f => f.Success == succeeded)
      .OrderBy(f => f.Title);

    return result;
  }

  public async Task<IEnumerable<Film>?> LoadFilmsByResultAsync(bool succeeded)
  {
    var data = await LoadAsync();
    
    var result = data?.SelectMany(d => d.Films)
      .Where(f => f.Success == succeeded)
      .OrderBy(f => f.Title);

    return result;
  }

  public async Task<IEnumerable<FilmYear>?> LoadAllYears()
  {
    var result = await LoadAsync();
    return result;
  }

  public async Task<IEnumerable<Film>?> LoadAllFilmsAsync()
  {
    var data = await LoadAsync();

    var result = data?
      .OrderBy(d => d.Year)
      .SelectMany(d => d.Films)
      .OrderBy(f => f.Title);

    return result;
  }

  protected async Task<IEnumerable<FilmYear>?> LoadAsync()
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
      _data = bechdelData?.Years;
      _logger.LogInformation("Loaded From Data Json");
    }

    return _data;
  }
}
