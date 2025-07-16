# Reference

## Setup

### Class Initialization
Example Usage:
```csharp
using OTWArchiveHelper;
// Other using/code statements...
var otwArchiveHelper = new OTWArchiveHelper("ExampleApp", "1.0.0");
```
Initializes the class with a user-agent made from the application name and version.

Parameters:
- `appName` [string]: The name of the application for user-agent purposes.
- `appVersion` [string]: The version of the application for user-agent purposes.
- `archivePath` [string]: The path to the OTW Archive instance you want to use. Defaults to "https://archiveofourown.org/". Needs "http://" or "https://" at the beginning.
- `timeout` [int]: The timeout for the HTTP requests in seconds. Defaults to 30 seconds.

Returns:
An instance of the `OTWArchiveHelper` class.


## Canonical Tag Page Functions

### GetTagPageList
Example Usage:
```csharp
var tagPage = OTWArchiveHelper.GetCanonTagPageList("Tag", 1);
// Other code...
works = await canonTagPage; // call when the canonTagPage is needed for processing.
```
Gets a tag page as a list of dictionaries with List(string) key values. Good for additional processing/tag searches.

Paramaters:
- `tag` [string]: The name of the tag to search through
- `page` [int]: The page number to retrieve (integer starting at 1 and ending at page limit. Will throw an error if exceeding page limit)

Returns:
A list of dictionaries of main tags/info of fanworks with the key values as lists (List<Dictionary<string, List<string>>>>).) The dictionary contains the following keys:
- `title`: The title of the fanwork
- `authors`: The author(s) of the fanwork
- `workId`: The ID of the fanwork
- `fandoms`: The fandom(s) of the fanwork
- `warnings`: The warnings of the fanwork
- `relationships`: The relationships of the fanwork
- `characters`: The characters of the fanwork
- `tags`: The tags of the fanwork
- `summary`: The summary of the fanwork
- `wordCount`: The word count of the fanwork

`title`, `workId`, `summary`, and `wordCount` always have only one value in the list, and can be extracted by this following code:
```csharp
string title = extractedData[indexOfWorkOnPage]["title"][0];
string workId = extractedData[indexOfWorkOnPage]["workId"][0];
string summary = extractedData[indexOfWorkOnPage]["summary"][0];
int wordCount = int.Parse(extractedData[indexOfWorkOnPage]["wordCount"][0]);
```
where `extractedData` is the list returned by the function, and `indexOfWorkOnPage` is the index of the work on the page (starting at 0).

`authors`, `fandoms`, `warnings`, `relationships`, `characters`, and `tags` can have multiple values, and each index can be extracted by the following code:
```csharp
List<string> authors = extractedData[indexOfWorkOnPage]["authors"];
List<string> fandoms = extractedData[indexOfWorkOnPage]["fandoms"];
List<string> warnings = extractedData[indexOfWorkOnPage]["warnings"];
List<string> relationships = extractedData[indexOfWorkOnPage]["relationships"];
List<string> characters = extractedData[indexOfWorkOnPage]["characters"];
List<string> tags = extractedData[indexOfWorkOnPage]["tags"];
```
where `extractedData` is the list returned by the function, and `indexOfWorkOnPage` is the index of the work on the page (starting at 0). This code makes a list for each multi-value key, which can then be parsed as needed.

### GetTagPageString
Example Usage:
```csharp
var canonTagPageString = OTWArchiveHelper.GetCanonTagPageString("Fandom Name", 1);
// Other code...
works = await canonTagPageString; // call when canonTagPageString is needed for display/processing.
```
Gets a tag page as a list of dictionaries with string key values. Good for just displaying the works for UI.

Parameters:
- `fandomName` [string]: The name of the fandom to search through
- `page` [int]: The page number to retrieve (integer starting at 1 and ending at page limit. Will throw an error if exceeding page limit)

Returns:
A Dictionary<string, string> of main tags/info of fanworks. The dictionary contains the following keys:
- `title`: The title of the fanwork
- `authors`: The author(s) of the fanwork
- `workId`: The ID of the fanwork
- `fandoms`: The fandom(s) of the fanwork
- `warnings`: The warnings of the fanwork
- `relationships`: The relationships of the fanwork
- `characters`: The characters of the fanwork
- `tags`: The tags of the fanwork
- `summary`: The summary of the fanwork
- `wordCount`: The word count of the fanwork

Unlike `GetTagPageList`, this function returns a single string for each key, rather than a list of strings. This is useful for displaying the works in a UI, as you can just get the value directly using `string NEEDEDITEM = extractedData["NEEDEDITEM"]`, where `NEEDEDITEM` is the item needed and `extractedData` is what the function returned.

## Work Functions

### GetWorkPageMarkdown
Example Usage:
```csharp
var workPageMarkdown = OTWArchiveHelper.GetWorkPageMarkdown("12345678901234567890");
// Other code...
workMarkdown = await workPageMarkdown; // call when the workPageMarkdown is needed for display/processing.
```

Gets the work page as a markdown string. This is useful for displaying the work in a markdown viewer or for further processing.

Parameters:
- workId [string]: The ID of the work to retrieve. This is the number in the URL of the work page. You can pass the `workId` output from `GetTagPageList` (index 0) or `GetTagPageString` to this function to get the work page.

Returns:
A string containing markdown of the work page. The markdown will be as shown:
```markdown
# {title}

**Author(s):** {authors}

## Summary
{summary, defaults to "No Summary Provided" if not found}

## Notes
{notes, defaults to "No Notes Provided" if not found}

{workText}
```

### GetWorkPageHtml
Example Usage:
```csharp
var workPageHtml = OTWArchiveHelper.GetWorkPageHtml("12345678901234567890");
// Other code...
workHtml = await workPageHtml; // call when the workPageHtml is needed for display/processing.
```
Gets the work page as an HTML string. This is useful for displaying the work in a web view or for further processing.

Parameters:
- workId [string]: The ID of the work to retrieve. This is the number in the URL of the work page. You can pass the `workId` output from `GetTagPageList` (index 0) or `GetTagPageString` to this function to get the work page.

Returns:
A string containing HTML of the work page. The HTML will be as shown:
```html
<html>
    <head>
        <title>{title}</title>
    </head>
    <body>
        <h1>{title}</h1>
        <p><strong>Author(s):</strong>{authors}</p>
        <h2>Summary</h2>
        <p>{summary, defaults to "No Summary Provided" if not found.}</p>
        <h2>Notes</h2>
        <p>{notes, defaults to "No Noted Provided" if not found.}</p>
        <h2>Content</h2>
        {content}
    </body>
</html>
```

## Other Functions

### TestConnection
Example Usage:
```csharp
var connectionTestAsync = OTWArchiveHelper.TestConnection();
// other code...
var connectionTest = await connectionTestAsync;

if (connectionTest.IsSuccessStatusCode)
{
	Console.WriteLine("Connection successful!");
}
else
{
	Console.WriteLine("Connection failed.");
	// other code can go here, such as stopping loading or notifying the user.
}
```

Tests the connection to the otwarchive instance. This is useful to ensure that the instance is reachable.

Returns: A `HttpResponseMessage` object that contains the info of the connection test. If the connection is successful, you can safely assume that the instance is reachable. If unsuccessful, you can let the person know connection failed.