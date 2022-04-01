namespace BechelServer.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class BechdelFilms
{
  public string Source { get; set; }
  public IEnumerable<FilmYear> Years { get; set; }
}

/// <summary>
/// A Collection of Films by Year
/// </summary>
public class FilmYear
{
  /// <summary>
  /// Year the Film Was Released
  /// </summary>
  /// <example>2013</example>
  public int Year { get; set; }

  /// <summary>
  /// Collection of Films for this year
  /// </summary>
  public IEnumerable<Film> Films { get; set; }
}

/// <summary>
/// Represents an individual film results from the Bechdel test.
/// </summary>
public class Film
{
  /// <summary>
  /// Film Title
  /// </summary>
  public string Title { get; set; }

  /// <summary>
  /// IMDB Id to be used to create links
  /// </summary>
  public string IMDBId { get; set; }

  /// <summary>
  /// Reason for a result.
  /// </summary>
  /// <example>ok</example>
  /// <example>notalk</example>
  public string ResultReason { get; set; }

  /// <summary>
  /// Simple success or failure of the test.
  /// </summary>
  public bool Success { get; set; }

  /// <summary>
  /// Film Budget
  /// </summary>
  public int Budget { get; set; }

  /// <summary>
  /// Domestic Box Office Receipts
  /// </summary>
  public int DomesticGross { get; set; }

  /// <summary>
  /// International Box Office Receipts
  /// </summary>
  public int InternationalGross { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
