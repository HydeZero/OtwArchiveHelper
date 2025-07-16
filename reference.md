# Reference

## Canonical Tag Page Functions

### GetCanonTagPageList
Example Usage:
```csharp
var canonTagPage = OTWArchiveHelper.GetCanonTagPageList("Tag", 1);
// Other code...
works = await canonTagPage; // call when the canonTagPage is needed for processing.
```
Gets a canonical tag page as a list of dictionaries with List(string) key values. Good for additional processing/tag searches.

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

### GetCanonTagPageString
Example Usage:
```csharp
var canonTagPageString = OTWArchiveHelper.GetCanonTagPageString("Fandom Name", 1);
// Other code...
works = await canonTagPageString; // call when canonTagPageString is needed for display/processing.
```
Gets the fandom page as a list of dictionaries with string key values. Good for just displaying the works for UI.

Parameters:
- `fandomName` [string]: The name of the fandom to search through
- `page` [int]: The page number to retrieve (integer starting at 1 and ending at page limit. Will throw an error if exceeding page limit)

Returns:
TODO: Make returns

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