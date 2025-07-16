using OTWArchiveHelper;

OtwArchiveHelper archiveHelper = new OtwArchiveHelper("OTWArchiveHelperDemo", "0.5.0");

Console.WriteLine("Testing connection to the archive...");

var connectionTestRequest = archiveHelper.TestConnection();

Console.WriteLine("Connection started. Awaiting response...");
var connectionTest = await connectionTestRequest;

if (connectionTest.IsSuccessStatusCode)
{
    Console.WriteLine("Connection successful!");
}

Console.WriteLine("tag:");
string fandomName = Console.ReadLine();
int pageNum = 1;
while (true)
{
    var tagPageResponse = archiveHelper.GetTagPageString(fandomName, pageNum);
    Console.WriteLine("Getting tag...");
    var works = await tagPageResponse;
    foreach (var work in works)
    {
        Console.WriteLine($"[{works.IndexOf(work)}]: {work["title"]}");
        Console.WriteLine(work["authors"]);
        Console.WriteLine(work["workId"]);
        Console.WriteLine(work["fandoms"]);
        Console.WriteLine(work["warnings"]);
        Console.WriteLine(work["relationships"]);
        Console.WriteLine(work["characters"]);
        Console.WriteLine(work["tags"]);
        Console.WriteLine(work["summary"]);
        Console.WriteLine(work["wordCount"]);
        Console.WriteLine("========================================");
    }

    while (true)
    {
        Console.WriteLine("Type the work you want to read (or type 'next' to go to the next page):");
        var request = Console.ReadLine();
        if (request?.ToLower() == "next")
        {
            pageNum++;
            break;
        }
        try
        {
            int.TryParse(request, out int workIndex);
            if ((workIndex >= 0) && (workIndex < works.Count))
            {
                while (true)
                {
                    var work = works[workIndex];
                    Console.WriteLine($"Reading work: {work["title"]} by {work["authors"]}");
                    var workContent = await archiveHelper.GetWorkPageMarkdown(work["workId"]);
                    Console.WriteLine("Work content received. Displaying...");
                    Console.WriteLine(workContent[0]);
                    if (workContent[1] != "-1")
                    {
                        Console.WriteLine("Go to next chapter? (y/n)");
                        var nextChapterResponse = Console.ReadLine();
                        if (nextChapterResponse?.ToLower() == "y")
                        {
                            var nextChapterContent = await archiveHelper.GetWorkPageMarkdown(work["workId"], workContent[1]);
                            Console.WriteLine("Next chapter content received. Displaying...");
                            Console.WriteLine(nextChapterContent[0]);
                        }
                    }
                }
                
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Invalid work index");
        }
    }
}