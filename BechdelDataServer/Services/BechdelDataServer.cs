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

  public async Task<IEnumerable<FilmYear>?> LoadAllYears()
  {
    var data = await LoadAsync();

    return data;
  }

  public async Task<(int count, int pageCount, int currentPage, IEnumerable<Film>? results)>
    LoadFilmsByResultAndYearAsync(bool succeeded, int year, int page, int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return (0, 0, 0, null);

    IOrderedEnumerable<Film> qry = data
      .Where(y => y.Year == year)
      .SelectMany(d => d.Films)
      .Where(f => f.Success == succeeded)
      .OrderBy(f => f.Title);

    return GetCountAndData(qry, page, pageSize);
  }

  public async Task<(int count, int pageCount, int currentPage, IEnumerable<Film>? results)>
    LoadFilmsByResultAsync(bool succeeded, int page, int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return (0, 0, 0, null);

    IOrderedEnumerable<Film> qry = data.SelectMany(d => d.Films)
      .Where(f => f.Success == succeeded)
      .OrderBy(f => f.Title);

    return GetCountAndData(qry, page, pageSize);
  }

  public async Task<(int count, int pageCount, int currentPage, IEnumerable<Film>? results)> LoadAllFilmsAsync(int page, int pageSize)
  {
    var data = await LoadAsync();

    if (data is null) return (0, 0, 0, null);

    IOrderedEnumerable<Film> qry = data.OrderBy(d => d.Year)
                                       .SelectMany(d => d.Films)
                                       .OrderBy(f => f.Title);

    return GetCountAndData(qry, page, pageSize);
  }

  (int count, int pageCount, int currentPage, IEnumerable<Film>? results) GetCountAndData(IOrderedEnumerable<Film> query, int page, int pageSize)
  {
    var count = query.Count();
    var pageCount = (int)Math.Ceiling((double)(count / pageSize));
    var results = query.Skip(page * pageSize)
                     .Take(pageSize)
                     .ToList();

    return (count, pageCount, page, results);
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
