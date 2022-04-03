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
  app.MapGet("api/years", async (BechdelDataServer ds) =>
  {
    var result = await ds.LoadAllYears();
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{id:int}", async (BechdelDataServer ds,    
    int id) =>
  {
    var result = await ds.LoadYearAsync(id);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{year:int}/failed", async (BechdelDataServer ds, int year, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    var (count, pageCount, currentPage, results) = await ds.LoadFilmsByResultAndYearAsync(false, year, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (results is null) Results.NotFound();
    return Results.Ok(new { count, pageCount, currentPage, results });
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{year:int}/passed", async (BechdelDataServer ds, int year, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    var (count, pageCount, currentPage, results) = await ds.LoadFilmsByResultAndYearAsync(true, year, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (results is null) Results.NotFound();
    return Results.Ok(new { count, pageCount, currentPage, results });
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/films", async (BechdelDataServer ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    var (count, pageCount, currentPage, results) = await ds.LoadAllFilmsAsync(page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (results is null) return Results.NotFound();
    return Results.Ok(new { count, pageCount, currentPage, results });
  }).WithTags("By Film").Produces(200).ProducesProblem(404);


  app.MapGet("api/films/failed", async (BechdelDataServer ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    var (count, pageCount, currentPage, results) = await ds.LoadFilmsByResultAsync(false, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (results is null) return Results.NotFound();
    return Results.Ok(new { count, pageCount, currentPage, results });
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

  app.MapGet("api/films/passed", async (BechdelDataServer ds, int? page, int? pageSize) =>
  {
    pageSize = pageSize ?? 50;
    var (count, pageCount, currentPage, results) = await ds.LoadFilmsByResultAsync(true, page.GetValueOrDefault(), pageSize.GetValueOrDefault());
    if (results is null) return Results.NotFound();
    return Results.Ok(new { count, pageCount, currentPage, results });
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

}


void Register(IServiceCollection svc)
{
  svc.AddEndpointsApiExplorer();
  svc.AddSingleton<BechdelDataServer>();

  svc.AddCors();

  svc.AddSwaggerGen(setup =>
  {
    var path = Path.Combine(AppContext.BaseDirectory, "BechdelDataServer.xml");
    setup.IncludeXmlComments(path);
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
      Description = "Bechdel Test API using data from FiveThirtyEight.com",
      Title = "Bechdel Test API",
      Version = "v1"
    });
  });


}

