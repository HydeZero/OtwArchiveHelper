namespace OTWArchiveHelper; // change this line to your namespace/namespace location for manual installation
using HtmlAgilityPack;
using System.Text;

public class OtwArchiveHelper
{
    public string ArchivePath { get; private set; }
    private HttpClient _archiveClient = new HttpClient();
    /// <summary>
    /// Initializes the OtwArchiveHelper with the specified archive path, app name, and app version.
    /// </summary>
    /// <param name="appName">The name of the app for user-agent purposes. Required.</param>
    /// <param name="appVersion">The version of the app for user-agent purposes</param>
    /// <param name="archivePath">The path to the OTW Archive instance you want to use. Defaults to "https://archiveofourown.org/". Needs "http://" or "https://" at the beginning.</param>
    /// <param name="timeout">The timeout for the HTTP client in seconds. Defaults to 30 seconds.</param>
    /// <exception cref="ArgumentException">Occurs when ArchivePath doesn't end with http:// or https://.</exception>
    /// <exception cref="Exception">This typically means there was a user-agent set failure. Make sure your app name and app version don't have weird characters.</exception>
    public OtwArchiveHelper(string appName, string appVersion, string archivePath = "https://archiveofourown.org/", int timeout = 30) // initialize archive url (otw-archive is open-source, so others can host it). default to AO3
    {
        if (archivePath.EndsWith("/") == false) // ensure the archive path ends with a slash
        {
            archivePath += "/";
        }

        if ((archivePath.Substring(0, 8) != "https://") && !(archivePath.Substring(0,7) == "http://")) // ensure the archive path starts with http or https
        {
            throw new ArgumentException("ArchivePath must start with http:// or https://");
        }
        ArchivePath = archivePath;
        _archiveClient.BaseAddress = new Uri(ArchivePath);
        _archiveClient.Timeout = TimeSpan.FromSeconds(timeout);
        _archiveClient.DefaultRequestVersion = new Version(2, 0); // who knew this one line speeds up the thing by a lot
        _archiveClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        bool useragentTest = _archiveClient.DefaultRequestHeaders.UserAgent.TryParseAdd($"{appName}/{appVersion}"); // set user agent to identify the app
        if (useragentTest == false)
        {
            throw new Exception("Failed to set User-Agent header for OtwArchiveHelper.");
        }
    }
    
    /// <summary>
    /// Tests connection to the archive.
    /// </summary>
    /// <returns>A HttpResponseMessage. Use .IsSuccessStatusCode on the result to check if the status code is OK (200 typically).</returns>
    /// <exception cref="ArgumentException">Occurs when ArchivePath is null/empty. This typically is a simple glitch and can be fixed by restarting the program.</exception>
    public async Task<HttpResponseMessage> TestConnection()
    {
        if (string.IsNullOrEmpty(ArchivePath))
        {
            throw new ArgumentException("ArchivePath cannot be null or empty.");
        }
        HttpResponseMessage testResults = await _archiveClient.GetAsync(ArchivePath); // try to get the root page of the archive
        return testResults; // if we get a response, the connection is successful
    }
    
    private async Task<List<Dictionary<string,List<string>>>> GetCanonTagPageBackend(string tag, int page) // async list of dictionaries with work information
    {
        if (string.IsNullOrEmpty(tag))
        {
            throw new ArgumentException("Fandom name cannot be null or empty.");
        }
        
        string url = $"{ArchivePath}tags/{Uri.EscapeDataString(tag)}/works?page={page}"; // construct the URL for the fandom page
        Task<HttpResponseMessage> fandomPageDownload = _archiveClient.GetAsync(url);

        HttpResponseMessage response = await fandomPageDownload;
        
                
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve fandom page: {response.ReasonPhrase}");
        }
        
        var fandomPage = new HtmlDocument();
        
        fandomPage.LoadHtml(await response.Content.ReadAsStringAsync());
        
        var workListNode = fandomPage.DocumentNode.SelectSingleNode("/html/body/div[@id='outer']/div[@id='inner']/div[@id='main']/ol[@class='work index group']");

        var result = new List<Dictionary<string, List<string>>>();
        
        foreach (var work in workListNode.ChildNodes)
        {
            if (work.Name != "li") continue; // skip non-work nodes
            
            Dictionary<string, List<string>> workList = new Dictionary<string, List<string>>();
            // dictionary is Title, Author, WorkID, Fandoms, Warnings, Relationships, Characters, Tags, Summary, Word Count
            // Extracting work details
            List<string> title = new List<string>();
            title.Add(work.SelectSingleNode(".//div[@class='header module']/h4[@class='heading']/a")?.InnerText.Trim() ?? "No Title Provided"); // works can have no title, so if the title tag isn't found, assume "No Title Provided"
            List<string> authors = new List<string>();
            foreach (var author in work.SelectNodes(".//div[@class='header module']/h4[@class='heading']/a[@rel='author']"))
            {
                authors.Add(author.InnerText.Trim());
            }
            List<string> workId = new List<string>();
            workId.Add(work.SelectSingleNode(".//div[@class='header module']/h4[@class='heading']/a").GetAttributeValue("href", "/404").Split('/').Last()); // get the last part of the href, which is the work ID
            List<string> fandoms = new List<string>();
            foreach(var fandom in work.SelectNodes(".//div/h5/a[@class='tag']"))
            {
                fandoms.Add(fandom.InnerText.Trim());
            }
            List<string> warnings = new List<string>();
            foreach (var warning in work.SelectNodes(".//ul[@class='tags commas']/li[@class='warnings']/strong/a[@class='tag']")) // works require a warning tag, so this should always return at least one
            {
                warnings.Add(warning.InnerText.Trim());
            }
            List<string> relationships = new List<string>();
            var relationshipNodes = work.SelectNodes(".//ul[@class='tags commas']/li[@class='relationships']/a[@class='tag']");
            if (relationshipNodes != null)
            {
                foreach (var relationship in relationshipNodes)
                {
                    relationships.Add(relationship.InnerText.Trim());
                }
            }
            else
            {
                relationships.Add("No Relationship Tags"); // if no relationship tags are found, assume "No Relationship Tags"
            }
            List<string> characters = new List<string>();
            var characterNodes = work.SelectNodes(".//ul[@class='tags commas']/li[@class='characters']/a[@class='tag']");
            if (characterNodes != null)
            {
                foreach (var character in characterNodes)
                {
                    characters.Add(character.InnerText.Trim());
                }
            }
            else
            {
                characters.Add("No Character Tags");
            }

            List<string> tags = new List<string>();
            var tagNodes = work.SelectNodes(".//ul[@class='tags commas']/li[@class='freeforms']/a[@class='tag']");
            if (tagNodes != null)
            {
                foreach (var tagNode in tagNodes)
                {
                    tags.Add(tagNode.InnerText.Trim()); // works can have no- yeah you get the idea by now
                }
            }
            else
            {
                tags.Add("No Freeform Tags");
            }

            List<string> summary = new List<string>();
            summary.Add(work.SelectSingleNode(".//blockquote[@class='userstuff summary']/p")?.InnerText.Trim() ?? "No Summary Provided"); // works can have no summary, so if the summary tag isn't found, assume "No Summary Provided"
            List<string> wordCount = new List<string>();
            wordCount.Add(work.SelectSingleNode(".//dl[@class='stats']/dd[@class='words']").InnerText.Trim()); // there always will be a word count.
            
            // compiling the dictionary
            workList = new Dictionary<string, List<string>>()
            {
                { "title", title },
                { "authors", authors },
                { "workId", workId },
                { "fandoms", fandoms },
                { "warnings", warnings },
                { "relationships", relationships },
                { "characters", characters },
                { "tags", tags },
                { "summary", summary },
                { "wordCount", wordCount }
            };
            
            result.Add(workList); // add the work dictionary to the result list
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets the canonical tag page page as a list of dictionaries with List(string) key values. Good for additional processing/tag searches.
    /// </summary>
    /// <param name="tag">The name of the tag to search through.</param>
    /// <param name="page">The page number to search through.</param>
    /// <returns>A dictionary of main tags/info of fanworks with the key values as lists.</returns>
    /// <exception cref="ArgumentException">Tag name cannot be null or empty.</exception>
    public async Task<List<Dictionary<string, List<string>>>> GetCanonTagPageList(string tag, int page = 1)
    {
        if (string.IsNullOrEmpty(tag))
        {
            throw new ArgumentException("Fandom name cannot be null or empty.");
        }
        
                
        try
        {
            return await GetCanonTagPageBackend(tag, page);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the canonical tag page: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Gets the canonical tag page as a list of dictionaries with string key values. Good for just displaying the info
    /// </summary>
    /// <param name="tag">The name of the tag to search through.</param>
    /// <param name="page">The page number to search through.</param>
    /// <returns>A dictionary of main tags/info of fanworks with the key values as lists.</returns>
    /// <exception cref="ArgumentException">Tag name cannot be null or empty.</exception>
    public async Task<List<Dictionary<string, string>>> GetCanonTagPageString(string tag, int page = 1)
    {
        if (string.IsNullOrEmpty(tag))
        {
            throw new ArgumentException("Fandom name cannot be null or empty.");
        }
        
                
        var preResult = await GetCanonTagPageBackend(tag, page);
        
        var result = new List<Dictionary<string, string>>();
        
        var workList = new Dictionary<string, string>();
        
        foreach (var work in preResult)
        {
            workList = new Dictionary<string, string>()
            {
                { "title", work["title"][0] },
                { "authors", string.Join(", ", work["authors"]) },
                { "workId", work["workId"][0] },
                { "fandoms", string.Join(", ", work["fandoms"]) },
                { "warnings", string.Join(", ", work["warnings"]) },
                { "relationships", string.Join(", ", work["relationships"]).Replace("&amp;", "&") },
                { "characters", string.Join(", ", work["characters"]) },
                { "tags", string.Join(", ", work["tags"]) },
                { "summary", work["summary"][0] },
                { "wordCount", work["wordCount"][0] }
            };
            result.Add(workList); // add the work dictionary to the result list
        }
        
        return result;
    }

    private async Task<Dictionary<string, string>> GetWorkPageBackend(string workId)
    {
        if (string.IsNullOrEmpty(workId))
        {
            ArgumentException ex = new ArgumentException("Work ID cannot be null or empty.");
        }
        string url = $"{ArchivePath}works/{workId}"; // construct the URL for the work page
        
        Task<HttpResponseMessage> workPageDownload = _archiveClient.GetAsync(url);
        
        HttpResponseMessage response = await workPageDownload;
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve work page: {response.ReasonPhrase}");
        }
        var workPage = new HtmlDocument();
        workPage.LoadHtml(await response.Content.ReadAsStringAsync());
        var workContent = workPage.DocumentNode.SelectSingleNode("/html/body/div[@id='outer']/div[@id='inner']/div[@id='main']/div[@id='workskin']");
        if (workContent == null)
        {
            throw new Exception("Work content not found. The work may not exist or the ID is incorrect.");
        }
        // initialize variables for work details
        string title = "";
        string authors = "";
        string summary = "";
        string notes = "";
        string text = "";
        
        title = workContent.SelectSingleNode(".//div[@class='preface group']/h2[@class='title heading']").InnerText.Trim();
        try
        {
            authors = string.Join(", ",
                workContent.SelectNodes(".//div[@class='preface group']/h3[@class='byline heading']/a[@rel='author']")
                    .Select(a => a.InnerText.Trim()));
        }
        catch (NullReferenceException)
        {
            authors = "Anonymous"; // if no authors are found, assume "Anonymous"
        }

        try
        {
            summary = workContent.SelectSingleNode(".//div[@class='preface group']/div[@class='summary module']/blockquote[@class='userstuff']").InnerText.Trim(); // summaries can have multiple lines so catch all lines
        }
        catch (NullReferenceException)
        {
            summary = "No Summary Provided"; // if no summary is found, assume "No Summary Provided"
        }

        try
        {
            notes = workContent.SelectSingleNode(".//div[@class='preface group']/div[@class='notes module']/blockquote[@class='userstuff']").InnerText.Trim(); // notes can have multiple lines so catch all lines
        }
        catch (NullReferenceException)
        {
            notes = "No Notes Provided"; // if no notes are found, assume "No Notes Provided"
        }

        try
        {
            text = workContent.SelectSingleNode(".//div[@id='chapters']/div[@class='userstuff']").InnerHtml.Trim(); // get html for different implementations of text
        }
        catch (NullReferenceException)
        {
            throw new Exception("No Content Found. The work may not exist or the ID is incorrect.");
        }
        
        Dictionary<string, string> work = new Dictionary<string, string>();
        
        work = new Dictionary<string, string>()
        {
            { "title", title },
            { "authors", authors },
            { "summary", summary },
            { "notes", notes },
            { "text", text }
        };

        return work;
    }
    
    private static string ConvertHtmlToMarkdown(string htmlContent)
    {
        if (string.IsNullOrEmpty(htmlContent))
        {
            throw new ArgumentException("HTML content cannot be null or empty.");
        }
        
        var converter = new ReverseMarkdown.Converter();

        return converter.Convert(htmlContent); // Convert HTML to Markdown using ReverseMarkdown
    }
    
    /// <summary>
    /// Gets a work page as a Markdown string.
    /// </summary>
    /// <param name="workId">The id of the work to grab.</param>
    /// <returns>A string containing work contents in markdown.</returns>
    /// <exception cref="ArgumentException">Work ID can't be null or empty.</exception>
    public async Task<string> GetWorkPageMarkdown(string workId)
    {
        if (string.IsNullOrEmpty(workId))
        {
            throw new ArgumentException("Work ID cannot be null or empty.");
        }
        
        var work = await GetWorkPageBackend(workId);
        
        work["text"] = ConvertHtmlToMarkdown(work["text"]); // convert HTML to Markdown
        
        string markdown = $"# {work["title"]}\n\n" +
                          $"**Authors:** {work["authors"]}\n\n" +
                          $"## Summary\n{work["summary"]}\n\n" +
                          $"## Notes\n{work["notes"]}\n\n" +
                          $"{work["text"]}"; // this is the text of the work, which is in Markdown format
        
        return markdown;
    }
    
    /// <summary>
    /// Gets a work page as an HTML file.
    /// </summary>
    /// <param name="workId">The id of the work to grab.</param>
    /// <returns>A string containing work contents in HTML.</returns>
    /// <exception cref="ArgumentException">Work ID can't be null or empty.</exception>
    public async Task<string> GetWorkPageHtml(string workId)
    {
        // construct a basic html page with the work content.
        if (string.IsNullOrEmpty(workId))
        {
            throw new ArgumentException("Work ID cannot be null or empty.");
        }
        
        var work = await GetWorkPageBackend(workId);
        
        string htmlResult = $"<html>\n" +
                            $"<head>\n" +
                            $"<title>{work["title"]}</title>\n" +
                            $"</head>\n" +
                            $"<body>\n" +
                            $"<h1>{work["title"]}</h1>\n" +
                            $"<p><strong>Authors:</strong> {work["authors"]}</p>\n" +
                            $"<h2>Summary</h2>\n" +
                            $"<p>{work["summary"]}</p>\n" +
                            $"<h2>Notes</h2>\n" +
                            $"<p>{work["notes"]}</p>\n" +
                            $"<h2>Content</h2>\n" +
                            $"{work["text"]}\n" + // this is the text of the work, which is in html format already. no modification is needed.
                            $"</body>\n" +
                            $"</html>";
        return htmlResult; // return the html result
    }
}