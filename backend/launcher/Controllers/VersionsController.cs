using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.Debug;

namespace api.Controllers;

[ApiController]
[Route("versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionsStorage Storage;
    private readonly IDatabaseReader  Reader;
    private readonly IDatabaseWriter  Writer;
    
    private readonly ILocalLogger     Logger;

    public VersionsController(IVersionsStorage storage, IDatabaseReader reader, IDatabaseWriter writer, ILocalLogger logger)
    {
        Storage = storage;
        Reader  = reader;
        Writer  = writer;
        Logger  = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetVersionsInfo()
    {
        try
        {
            Logger.Message($"Versions requested");
            
            LocalVersionInfo[]  vis         = await Reader.ReadAllVersionsInfo();
            PublicVersionInfo[] pubVersions = new PublicVersionInfo[vis.Length];

            for (int i = 0; i < vis.Length; i++)
            {
                pubVersions[i] = vis[i].PublicInfo;
            }

            Logger.Message($"Sending {vis.Length} versions info");
            
            return Ok(pubVersions);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, "Failed getting versions");
        }
    }

    [HttpGet("totaldownloads")]
    public async Task<IActionResult> GetTotalDownloads()
    {
        try
        {
            LocalVersionInfo[] vis = await Reader.ReadAllVersionsInfo();

            int totalDownloads = 0;
            
            for (int i = 0; i < vis.Length; i++)
            {
                totalDownloads += vis[i].PublicInfo.Downloads;
            }

            return Ok(totalDownloads);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, "Failed getting total downloads");
        }
    }
    
    [HttpGet("{versionID}")]
    public async Task<IActionResult> GetVersionInfo(string versionID)
    {
        try
        {
            Logger.Message($"Version {versionID} requested");
            
            PublicVersionInfo ver = (await Reader.ReadVersionInfo(versionID)).PublicInfo;

            return Ok(ver);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }

    [HttpGet("files/windows/{versionID}")]
    public async Task<IActionResult> DownloadVersionWindows(string versionID) => await DownloadVersion(Storage.GetWindowsVersionFile, versionID, "win");

    [HttpGet("files/linux/{versionID}")]
    public async Task<IActionResult> DownloadVersionLinux(string versionID) => await DownloadVersion(Storage.GetLinuxVersionFile, versionID, "linux");
    
    private async Task<IActionResult> DownloadVersion(Func<string, Task<FileStream>> getFile, string versionID, string additionalTag)
    {
        try
        {
            Logger.Message($"{versionID} with additional tag {additionalTag} file request");
            
            FileStream stream = await getFile(versionID);
            Writer.IncreaseDownloadsCount(versionID);
            return File(stream, "application/zip", $"Deadays_{versionID}_{additionalTag}.zip");
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, $"Failed getting version: {versionID} with additional tag {additionalTag}");
        }
    }
}