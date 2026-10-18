using System;
using System.Collections.Generic;
using Ecureuil.Core.Helpers;
using Ecureuil.Core.Models;

namespace Ecureuil.Core.Services {
  public class CatalogManager {

    private List<T> ParseList<T>(string jsonContent, string errorMsg) {
      if (string.IsNullOrEmpty(jsonContent)) return new List<T>();
      try {
        List<T> list = MiniJson.Deserialize<List<T>>(jsonContent);
        return list ?? new List<T>();
      } catch (Exception ex) {
        Console.WriteLine(errorMsg + ": " + ex.Message);
        return new List<T>();
      }
    }

    // Converte index JSON in lista di AppModel
    public List<AppModel> ParseIndexJson(string jsonContent) {
      return ParseList<AppModel>(jsonContent, "Can't parse the index json");
    }

    // Converte source JSON in lista di SourceModel
    public List<SourceModel> ParseSourcesJson(string jsonContent) {
      return ParseList<SourceModel>(jsonContent, "Can't parse the source json");
    }

    public List<sourceDiscoverModel> ParseDiscoveryJson(string jsonContent) {
      return ParseList<sourceDiscoverModel>(jsonContent, "Can't parse source JSON");
    }
  }
}
