+using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Ecureuil.Core.Models;

namespace Ecureuil.Core.Services {
  public class CatalogManager {
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
      PropertyNameCaseInsensitive = true
    };

    //converts index json into a list of AppModel objects
    public List<AppModel> ParseIndexJson(string jsonContent) {
      if (string.IsNullOrWhiteSpace(jsonContent)) return new List<AppModel>();

      try {
        var apps = JsonSerializer.Deserialize<List<AppModel>>(jsonContent, _jsonOptions);
        return apps ?? new List<AppModel>();
      } catch (Exception ex) {
        Console.WriteLine($"Can't parse the index json: {ex.Message}");
        return new List<AppModel>();
      }
    }

    //converts source json into a source model
    public List<SourceModel> ParseSourcesJson(string jsonContent) {
      if (string.IsNullOrWhiteSpace(jsonContent)) return new List<SourceModel>();

      try {
        var sources = JsonSerializer.Deserialize<List<SourceModel>>(jsonContent, _jsonOptions);
        return sources ?? new List<SourceModel>();
      } catch (Exception ex) {
        Console.WriteLine($"Can't parse the source json: {ex.Message}");
        return new List<SourceModel>();
      }
    }
  }
}
