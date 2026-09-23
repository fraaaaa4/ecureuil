# Ecureuil source code
This is the source code repository of Ecureuil.

Ecureuil is a repository manager, primarily built for Windows RT, for dynamically downloading apps to an RT device. Content gets dynamically refreshed by the app from your selected Ecureuil sources.

Since it works on RT, it also works on x86 versions of Windows, starting from Windows 2000 onwards. It gets offered in a .NET 2.0 and .NET 4.5 version, both are identical in terms of functionalities.

This repository contains the source code for the C#/WinForms app. Details on how to configure your Ecureuil source, with example JSONs, are in my other repository.

## How it works
Ecureuil needs two parts: a GitHub repository as a source, which can be made by anyone, and the C# frontend (or CLI interface). Details for how a Ecureuil source repository should be made, if you want to make one, is available on my other repository: ecureuil-source-example.

The app on your device works like so:
- firstly, it checks for saved caches on the device itself, so it doesn't needlessly have to download stuff from the sources. If there aren't, it downloads the `source.json` and `source-index.json` from the sources you've added
- when downloading an app, it firstly checks the Compatibility property of the app; if your Windows version is there then you can proceed. Then it checks for the FileType property, and automatically chooses what's the best way to install it; additionally, you can create also a start menu shortcut
- once installed, it gets registered as an installed app in Ecureuil, so it keeps track of that too

## How it's subdivided
It's a Visual Studio 2012 project in C#, made in WinForms for .NET Framework 4.5, the one included in Windows RT 8.1. In this section there's a brief description of each of the main files included in any of the folders.

.NET 2.0 version is a Visual Studio 2005 project in C#. I suggest the 2.0 version for Windows 2000/XP/Vista, while 4.5 mainly for RT and more modern versions of Windows.

### Models
This folder contains all the JSON structures translated into C# objects. These are divided into three:
- **AppModel**: represents a single app that's inside a repository. So, an app JSON that is in the folder of your source
- **InstallationRecord**: repredents an installation of an app from Ecureuil
- **SourceModel**: represents a source you've added to the app. So, the sources.json file of your source
Data between them should be linked, and should use the same values. For example, if you want to retrieve a certain app you installed, InstallationRecord will have an AppId and SourceId; that AppId and SourceId will correspond to an AppModel, extracted from the URL JSON from a SourceModel.

### Services
This folder contains everything that the app actually does.
- **CacheManager**: loads and saves a cache of your sources, and the apps inside them, inside your AppData folder. Cached data stays there until the user decides to delete them
- **CatalogManager**: parses the app index and source JSONs to transform them into their relative models.
- **DownloadManager**: does the download of the JSONs from repositories, and downloads files
- **InstallationRegistry**: manages loading the installer registry from disk and saving it, as well as inserting and removing installer entries from its registry
- **InstallerService**: installs the app to the given file path, usually a new folder in Program Files; if it's a ZIP it extracts its content, if it's an exe it copies it, otherwise if it's a setup it just executes it. Also, for ZIP and exes, it creates a new shortcut for ig in the start menu

### Compatibility
Compatibility for each app must be specified by the uploader; you must specify all versions that are supported (e.g. if an app supports both 8.0 and 8.1, you must specify both of them. A method inside automatically verifies your NT version, and checks if it's compatible. Your architecture gets verified too.
