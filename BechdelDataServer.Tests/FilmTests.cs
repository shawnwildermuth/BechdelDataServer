using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BechdelDataServer.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BechdelDataServer.Tests
{
  public class FilmTests : BaseFilmTests
  {


    [Fact]
    public async void CanGetFilms() => await TestFilms("/api/films");

    [Fact]
    public async void CanGetFilmsMax() => await TestFilmsMax("/api/films");

    [Fact]
    public async void CanGetFilmsPassed() => await TestFilms("/api/films/passed");

    [Fact]
    public async void CanGetFilmsPassedMax() => await TestFilmsMax("/api/films/passed");

    [Fact]
    public async void CanGetFilmsFailed() => await TestFilms("/api/films/failed");

    [Fact]
    public async void CanGetFilmsFailedMax() => await TestFilmsMax("/api/films/failed");

    [Fact]
    public async void CanGetFilmsPassedByYear() => await TestFilms("/api/films/passed/2013");

    [Fact]
    public async void CanGetFilmsPassedByYearMax() => await TestFilmsMax("/api/films/passed/2013");

    [Fact]
    public async void CanGetFilmsFailedByYear() => await TestFilms("/api/films/failed/2013");

    [Fact]
    public async void CanGetFilmsFailedByYearMax() => await TestFilmsMax("/api/films/failed/2013");

  }
}
