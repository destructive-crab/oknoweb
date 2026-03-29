using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionsStorage Storage;
    private readonly IDatabaseReader  Reader;
    
    public VersionsController(IVersionsStorage storage, IDatabaseReader reader)
    {
        Storage = storage;
        Reader = reader;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetVersionsInfo()
    {
        try
        {
            LocalVersionInfo[]  vis         = await Reader.ReadAllVersionsInfo();
            PublicVersionInfo[] pubVersions = new PublicVersionInfo[vis.Length];

            for (int i = 0; i < vis.Length; i++)
            {
                pubVersions[i] = vis[i].PublicInfo;
            }

            Console.WriteLine($"Versions requested. Sending {vis.Length} versions info");
            return Ok(pubVersions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Failed gettings versions");
        }
    }


    [HttpGet("{versionID}")]
    public async Task<IActionResult> GetVersionInfo(string versionID)
    {
        try
        {
            Console.WriteLine($"Version {versionID} requested");
            
            PublicVersionInfo ver = (await Reader.ReadVersionInfo(versionID)).PublicInfo;

            return Ok(ver);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }

    [HttpGet("files/{versionID}")]
    public async Task<IActionResult> DownloadVersion(string versionID)
    {
        try
        {
            Console.WriteLine($"{versionID} file request");
            FileStream stream = await Storage.GetVersionFile(versionID);
            return File(stream, "application/zip", $"Deadays_{versionID}.zip");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }
}