#region Wiring Up
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
#endregion

void MapApis(WebApplication app)
{
  app.MapGet("api/years", async (BechdelDataService ds) =>
  {
    var result = await ds.LoadAllYears();
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{id:int}", async (BechdelDataService ds,    
    int id) =>
  {
    var result = await ds.LoadYearAsync(id);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{year:int}/failed", async (BechdelDataService ds, int year, int? page, int? pageSize) =>
  {
    if (ds is null) return Results.BadRequest();
    pageSize = pageSize ?? 50;
    FilmResult data = await ds.LoadFilmsByResultAndYearAsync(false, year, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (data.Results is null) Results.NotFound();
    return Results.Ok(data);
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{year:int}/passed", async (BechdelDataService ds, int year, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    FilmResult  data =await ds.LoadFilmsByResultAndYearAsync(true, year, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (data.Results is null)Results.NotFound();
    return Results.Ok(data);
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/films", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    FilmResult  data =await ds.LoadAllFilmsAsync(page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (data.Results is null)return Results.NotFound();
    return Results.Ok(data);
  }).WithTags("By Film").Produces(200).ProducesProblem(404);


  app.MapGet("api/films/failed", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    FilmResult  data =await ds.LoadFilmsByResultAsync(false, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (data.Results is null)return Results.NotFound();
    return Results.Ok(data);
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

  app.MapGet("api/films/passed", async (BechdelDataService ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    FilmResult  data =await ds.LoadFilmsByResultAsync(true, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (data.Results is null)return Results.NotFound();
    return Results.Ok(data);
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

}


void Register(IServiceCollection svc)
{
  svc.AddEndpointsApiExplorer();
  svc.AddSingleton<BechdelDataService>();

  svc.AddCors();

  
  svc.AddSwaggerGen(setup =>
  {
    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\BechdelDataServer.xml"));
    setup.IncludeXmlComments(path);
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
