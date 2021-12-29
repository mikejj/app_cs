// @nuget: HtmlAgilityPack

using HtmlAgilityPack;
class MyApp
{
    const string webUri = "https://www.cts-tradeit.cz";
    private struct Job
    {
        public string title;
        public string description;
    }
    private enum ExitCode
    {
        OK = 0,
        Error = 1
    }
    static void Main()
    {
        Console.WriteLine("Application is running, please wait...");

        const string jobsPageUri = webUri + "/kariera";

        try
        {
            List<string> jobsUri = GetJobsUri(jobsPageUri);

            if (jobsUri != null)
            {
                Console.WriteLine("Found {0} jobs...", jobsUri.Count);

                foreach (string jobUri in jobsUri)
                {
                    Job job = GetJob(jobUri);
                    SaveJob(job);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            Environment.Exit((int)ExitCode.Error);
        }

        Console.WriteLine("Done.");
        Environment.Exit((int)ExitCode.OK);
    }
    static List<string> GetJobsUri(string jobsPageUri)
    {
        HtmlWeb web;
        HtmlDocument doc;
        HtmlNodeCollection? nodes;
        List<string> jobsUri = new List<string>();

        web = new HtmlWeb();

        doc = web.Load(jobsPageUri);
        nodes = doc.DocumentNode.SelectNodes("//*[@class='shortcode']//*[@class='col']//a");

        if (nodes != null)
        {
            foreach (HtmlNode node in nodes)
            {
                jobsUri.Add(webUri + node.GetAttributeValue("href", ""));
            }
        }

        return jobsUri;
    }
    static Job GetJob(string jobUri)
    {
        HtmlWeb web;
        HtmlDocument doc;
        HtmlNode node, sibling;
        Job job = new Job();

        string jobDescription = String.Empty;

        web = new HtmlWeb();
        doc = web.Load(jobUri);
        node = doc.DocumentNode.SelectSingleNode("//h1");

        job.title = node.InnerText;

        Console.WriteLine("Job: {0}", job.title);

        node = doc.DocumentNode.SelectSingleNode("//h2[1]");

        sibling = node.NextSibling;

        while (sibling != null)
        {
            if (sibling.NodeType == HtmlNodeType.Element)
            {
                jobDescription = jobDescription + sibling.InnerText.Trim().Replace("&nbsp;", " ") + Environment.NewLine;
            }

            sibling = sibling.NextSibling;
        }

        job.description = jobDescription;

        return job;
    }
    static void SaveJob(Job job)
    {
        string fileName;

        fileName = job.title.Replace(".", "").Replace("/", " ").Replace(Path.DirectorySeparatorChar, ' ');
        fileName = fileName + ".txt";

        using (StreamWriter sw = new StreamWriter(fileName, append: false))
        {
            sw.Write(job.description);
        }

    }
}