#region Wiring Up
var builder = WebApplication.CreateBuilder(args);

bool isTesting = builder.Configuration.GetValue<bool>("IsTesting", true);

Register(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors(cfg =>
{
  cfg.WithMethods("GET");
  cfg.AllowAnyHeader();
  cfg.AllowAnyOrigin();
});

app.UseSwagger();

MapApis(app);

app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bechdel Test Results v1");
  c.RoutePrefix = string.Empty;
});

app.Run();

return 0;
#endregion

void MapApis(WebApplication app)
{
  app.MapGet("api/films", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadAllFilmsAsync(pageNumber, pagerTake);
    if (data.Results is null) return Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/films/{year:int}", async (BechdelDataService ds, int? page, int? pageSize, int year) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadAllFilmsByYearAsync(pageNumber, pagerTake, year);
    if (data.Results is null) return Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/films/failed", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAsync(false, pageNumber, pagerTake);
    if (data.Results is null) return Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/films/passed", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAsync(true, pageNumber, pagerTake);
    if (data.Results is null) return Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/films/failed/{year:int}", async (BechdelDataService ds, int year, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAndYearAsync(false, year, pageNumber, pagerTake);
    if (data.Results is null) Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/films/passed/{year:int}", async (BechdelDataService ds, int year, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    int pageNumber = page ?? 1;
    int pagerTake = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAndYearAsync(true, year, pageNumber, pagerTake);
    if (data.Results is null) Results.NotFound();
    return Results.Ok(data);
  }).Produces<IEnumerable<Film>>(contentType: "application/json").Produces(404).ProducesProblem(500);

  app.MapGet("api/years", async (BechdelDataService ds) =>
  {
    if (ds is null) return Results.Problem("Server Error", statusCode: 500);
    var data = await ds.LoadFilmYears();
    if (data is null) Results.NotFound();
    return Results.Ok(data);
  }).Produces<int[]>(contentType: "application/json").Produces<string[]>(200).ProducesProblem(500);

}


void Register(IServiceCollection svc)
{
  svc.AddEndpointsApiExplorer();
  svc.AddSingleton<BechdelDataService>();

  svc.AddCors();

  svc.AddSwaggerGen(setup =>
  {
    if (!isTesting)
    {
      var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"BechdelDataServer.xml"));
      setup.IncludeXmlComments(path);
    }
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
      Description = "Bechdel Test API using data from FiveThirtyEight.com",
      Title = "Bechdel Test API",
      Version = "v1"
    });
  });
}

// To enable access to the Top Level Class
public partial class Program { }
