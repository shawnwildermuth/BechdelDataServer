using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using BechdelDataServer.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BechdelDataServer.Tests;

public class YearTests : IDisposable
{
  private readonly WebApplicationFactory<Program> _app;
  private readonly HttpClient _client;

  public YearTests()
  {
    _app = new WebApplicationFactory<Program>();

    _client = _app.CreateClient();
  }

  [Fact]
  public async void CanGetAllYears()
  {
    var response = await _client.GetAsync("/api/years");

    Assert.True(response.IsSuccessStatusCode);

    var films = await response.Content.ReadFromJsonAsync<IEnumerable<FilmYear>>();

    Assert.NotNull(films);
    Assert.True(films?.Any(f => f.Year == 2013));

  }

  [Fact]
  public async void CanGetYear()
  {
    var response = await _client.GetAsync("/api/years/2013");

    Assert.True(response.IsSuccessStatusCode);

    var film = await response.Content.ReadFromJsonAsync<FilmYear>();

    Assert.NotNull(film);
    Assert.True(film?.Year == 2013);
    Assert.True(film?.Films.Any());
  }

  [Fact]
  public async void CanGetYearFailed()
  {
    var response = await _client.GetAsync("/api/years/2013/failed");

    Assert.True(response.IsSuccessStatusCode);

    var result = await response.Content.ReadFromJsonAsync<FilmResult>();

    Assert.NotNull(result);
    Assert.True(result?.Count > 0);
    Assert.True(result?.PageCount > 0);
    Assert.True(result?.Results?.Any());
  }

  [Fact]
  public async void CanGetYearPassed()
  {
    var response = await _client.GetAsync("/api/years/2013/passed");

    Assert.True(response.IsSuccessStatusCode);

    var result = await response.Content.ReadFromJsonAsync<FilmResult>();

    Assert.NotNull(result);
    Assert.True(result?.Count > 0);
    Assert.True(result?.PageCount > 0);
    Assert.True(result?.Results?.Any());
  }

  public void Dispose()
  {
    _app.Dispose();
  }
}
