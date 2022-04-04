using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using BechdelDataServer.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BechdelDataServer.Tests;

public class YearTests : BaseFilmTests
{
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
  public async void CanGetYearFailed() => await TestFilms("/api/years/2013/failed");

  [Fact]
  public async void CanGetYearPassed() => await TestFilms("/api/years/2013/passed");

  [Fact]
  public async void CanGetYearFailedMaxPage() => await TestFilmsMax("/api/years/2013/failed");

  [Fact]
  public async void CanGetYearPassedMaxPage() => await TestFilmsMax("/api/years/2013/passed");
}
