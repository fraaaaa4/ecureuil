using System;
using System.Collections.Generic;

namespace Ecureuil.Core.Models {

  // Modello sorgente equivalente a sources.json
  public class SourceModel {
    private string _id;
    private string _name;
    private string _description;
    private List<string> _categories;
    private string _url;
    private string _originalURL;
    private string _iconUrl;
    private string _author;
    private List<string> _authorLink;
    private DateTime? _dateCreated;
    private bool _isLocal;

    // Proprieta locali
    private bool _isEnabled = true;
    private DateTime? _lastUpdated;
    private string _catalogLocation;
    private bool _isNotOld;

    public SourceModel() {
      _isEnabled = true;
      _categories = new List<string>();
      _authorLink = new List<string>();
    }

      public SourceModel(string id, string name, string author, string url, string iconURL)
      {
          _id = id; _name = name; _author = author; _url = url; _iconUrl = iconURL;
      }

      public SourceModel(string id, string name, string url, string originalURL, bool isEnabled,
          DateTime? dateCreated, string author, string description)
      {
          _id = id; _name = name; _url = url; _originalURL = originalURL; _isEnabled = isEnabled;
          _dateCreated = dateCreated; _author = author; _description = description;
      }

    public string id {
      get { return _id; }
      set { _id = value; }
    }

    public string name {
      get { return _name; }
      set { _name = value; }
    }

    public string description {
      get { return _description; }
      set { _description = value; }
    }

    public List<string> categories {
      get { return _categories; }
      set { _categories = value; }
    }

    public string url {
      get { return _url; }
      set { _url = value; }
    }

    public string originalURL {
      get { return _originalURL; }
      set { _originalURL = value; }
    }

    public string iconUrl {
      get { return _iconUrl; }
      set { _iconUrl = value; }
    }

    public string author {
      get { return _author; }
      set { _author = value; }
    }

    public List<string> authorLink {
      get { return _authorLink; }
      set { _authorLink = value; }
    }

    public DateTime? dateCreated {
      get { return _dateCreated; }
      set { _dateCreated = value; }
    }

    public bool isLocal {
      get { return _isLocal; }
      set { _isLocal = value; }
    }

    public bool isEnabled {
      get { return _isEnabled; }
      set { _isEnabled = value; }
    }

    public DateTime? lastUpdated {
      get { return _lastUpdated; }
      set { _lastUpdated = value; }
    }

    public string catalogLocation {
      get { return _catalogLocation; }
      set { _catalogLocation = value; }
    }

    public bool isNotOld {
      get { return _isNotOld; }
      set { _isNotOld = value; }
    }
  }

    public class sourceDiscoverModel
    {
        private string _id;
        private string _name;
        private string _url;
        private string _author;
        private string _iconURL;
        private SourceModel _source;

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string name {
            get { return _name; }
            set { _name = value; }
        }

        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string iconURL
        {
            get { return _iconURL; }
            set { _iconURL = value; }
        }

        public SourceModel source
        {
            get { return _source; }
            set { _source = value; }
        }
    }
}
