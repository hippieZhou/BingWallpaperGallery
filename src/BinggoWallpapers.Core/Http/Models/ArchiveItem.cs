using System.Text.Json.Serialization;

namespace BinggoWallpapers.Core.Http.Models;
public class ArchiveItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Sha { get; set; }
    public int Size { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; }
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; }
    public string Type { get; set; }
    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}

public class Links
{
    public string Self { get; set; }
    public string Git { get; set; }
    public string Html { get; set; }
}

