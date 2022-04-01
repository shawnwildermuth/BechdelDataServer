#region Wiring Up
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Register(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

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

  app.MapGet("api/years/{year:int}/failed", async (BechdelDataServer ds, int year) =>
  {
    var result = await ds.LoadFilmsByResultAndYearAsync(false, year);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/years/{year:int}/passed", async (BechdelDataServer ds, int year) =>
  {
    var result = await ds.LoadFilmsByResultAndYearAsync(true, year);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Year").Produces(200).ProducesProblem(404);

  app.MapGet("api/films", async (BechdelDataServer ds) =>
  {
    var result = await ds.LoadAllFilmsAsync();
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Film").Produces(200).ProducesProblem(404);


  app.MapGet("api/films/failed", async (BechdelDataServer ds) =>
  {
    var result = await ds.LoadFilmsByResultAsync(false);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

  app.MapGet("api/films/passed", async (BechdelDataServer ds) =>
  {
    var result = await ds.LoadFilmsByResultAsync(true);
    if (result is null) Results.NotFound();
    return result;
  }).WithTags("By Film").Produces(200).ProducesProblem(404);

}


void Register(IServiceCollection svc)
{
  svc.AddEndpointsApiExplorer();
  svc.AddScoped<BechdelDataServer>();

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

