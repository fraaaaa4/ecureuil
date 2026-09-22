using System;
using System.Collections.Generic;
using Ecureuil.Core.Helpers;
using Ecureuil.Core.Models;

namespace Ecureuil.Core.Services {
  public class CatalogManager {

    // Converte index JSON in lista di AppModel
    public List<AppModel> ParseIndexJson(string jsonContent) {
      if (string.IsNullOrEmpty(jsonContent)) {
        return new List<AppModel>();
      }

      try {
        List<AppModel> apps = MiniJson.Deserialize<List<AppModel>>(jsonContent);
        if (apps == null) {
          return new List<AppModel>();
        }
        return apps;
      } catch (Exception ex) {
        Console.WriteLine("Can't parse the index json: " + ex.Message);
        return new List<AppModel>();
      }
    }

    // Converte source JSON in lista di SourceModel
    public List<SourceModel> ParseSourcesJson(string jsonContent) {
      if (string.IsNullOrEmpty(jsonContent)) {
        return new List<SourceModel>();
      }

      try {
        List<SourceModel> sources = MiniJson.Deserialize<List<SourceModel>>(jsonContent);
        if (sources == null) {
          return new List<SourceModel>();
        }
        return sources;
      } catch (Exception ex) {
        Console.WriteLine("Can't parse the source json: " + ex.Message);
        return new List<SourceModel>();
      }
    }

      public List<sourceDiscoverModel> ParseDiscoveryJson(string jsonContent)
      {
          if (string.IsNullOrEmpty(jsonContent))
          {
              return new List<sourceDiscoverModel>();
          }
          try
          {
              List<sourceDiscoverModel> sources = MiniJson.Deserialize<List<sourceDiscoverModel>>(jsonContent);
              if (sources == null)
              {
                  return new List<sourceDiscoverModel>();
              }
              return sources;
          }
          catch (Exception ex)
          {
              Console.WriteLine("Can't parse source JSON: " + ex.Message);
              return new List<sourceDiscoverModel>();
          }
      }
  }
}
