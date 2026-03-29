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
    
    private readonly ILocalLogger     Logger;

    public VersionsController(IVersionsStorage storage, IDatabaseReader reader, ILocalLogger logger)
    {
        Storage = storage;
        Reader = reader;
        Logger = logger;
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

    [HttpGet("files/{versionID}")]
    public async Task<IActionResult> DownloadVersion(string versionID)
    {
        try
        {
            Logger.Message($"{versionID} file request");
            
            FileStream stream = await Storage.GetVersionFile(versionID);
            return File(stream, "application/zip", $"Deadays_{versionID}.zip");
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }
}