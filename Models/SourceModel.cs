using System;
using System.Collections.Generic;

namespace Ecureuil.Core.Models {

  //equivalent of your source json
  public class SourceModel {
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public List<string> categories { get; set; }
    public string url { get; set; }
    public string originalURL { get; set; }
    public string iconUrl { get; set; }
    public string author { get; set; }
    public List<string> authorLink { get; set; }
    public DateTime? dateCreated { get; set; }
    public bool isLocal { get; set; }

    //local - various properties
    public bool isEnabled { get; set; }
    public DateTime? lastUpdated { get; set; }
    public string catalogLocation { get; set; }
    public bool isNotOld { get; set; }
  }
}
