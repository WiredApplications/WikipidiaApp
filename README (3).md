# WikiForms – Desktop Wikipedia Browser & Search Manager

A Windows Forms desktop application that allows users to search Wikipedia articles, view summaries and images, listen to content using speech synthesis, and manage a persistent list of favourite searches.

---

## Table of Contents
- [About](#about)
- [Getting Started](#getting-started)
- [Prerequisites](#prerequisites)
- [Build and Run](#build-and-run)
- [Features](#features)
- [Using the Wikipedia API](#using-the-wikipedia-api)
- [Speech Synthesis](#speech-synthesis)
- [Favourite Searches](#favourite-searches)
- [Screenshots](#screenshots)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [License](#license)

---

## About

**WikiForms** is a desktop application built in C# using Windows Forms that provides an interactive way to browse Wikipedia content.

Users can search for articles in multiple languages, read concise summaries, view associated images, listen to article text through text-to-speech, and store favourite searches in a remote database for later access.

The project demonstrates API integration, asynchronous HTTP communication, speech synthesis, graphical user interface design, and persistent data storage.

---

## Getting Started

You can run the project directly in Visual Studio without additional tools beyond the included dependencies.

---

## Prerequisites

Before opening the project, ensure you have:

- Windows operating system
- Visual Studio 2022 (recommended)
- .NET Framework or .NET 6+
- Active internet connection

### Speech Synthesis

For text-to-speech functionality, the following Windows voices should be installed:

- Greek: Microsoft Stefanos
- English: Microsoft David Desktop

---

## Build and Run

1. Clone or download the repository.
2. Open the solution file (`.sln`) in Visual Studio.
3. Restore NuGet packages if required.
4. Build the solution.
5. Press **F5** to run the application.

---

## Features

- Wikipedia article search using keywords
- Multi-language support (Greek, English, Japanese, Russian)
- Display of article title and summary text
- Dynamic loading of article images
- Placeholder image when no thumbnail is available
- Text-to-speech playback of article summaries
- Persistent favourite search management
- Double-click loading of saved searches
- Windows Forms graphical user interface

---

## Using the Wikipedia API

The application consumes the Wikipedia REST API summary endpoint:

```
https://{language}.wikipedia.org/api/rest_v1/page/summary/{term}
```

For each search query, the application retrieves:

- Article title
- Summary text (extract)
- Thumbnail image (if available)

If an article is not found or a request fails, a descriptive error message is displayed.

---

## Speech Synthesis

Speech synthesis is implemented using `System.Speech.Synthesis`.

- Speech playback is triggered using the **Ανάγνωση** button.
- The active voice is automatically selected based on the chosen language.
- Any ongoing speech playback is cancelled before starting a new request.

---

## Favourite Searches

Favourite searches are stored and managed using **Supabase** via its REST API.

Supported operations include:

- Saving a search term with its language
- Displaying saved searches in a DataGridView
- Loading a saved search by double-clicking
- Deleting selected favourite entries

All database operations are performed using HTTP requests.

---

## Screenshots

Place your screenshots in a `screenshots/` directory and update the paths below.

### Main Screen
![Main Screen](screenshots/main-screen.png)

### Search Results
![Search Results](screenshots/search-results.png)

### Favourite Searches
![Favourite Searches](screenshots/favourites.png)

---

## Project Structure

```
Wikipidia_Exercise/
├── Form1.cs
├── Form1.Designer.cs
├── WikiSummary.cs
├── Supabase.cs
├── FavSearch.cs
├── Resources/
└── Program.cs
```

---

## Technologies Used

- C#
- Windows Forms (WinForms)
- .NET
- Wikipedia REST API
- Supabase
- Newtonsoft.Json
- System.Speech

---

## License

This project was developed for educational and demonstration purposes.
It is not licensed for commercial use without permission from the author.
