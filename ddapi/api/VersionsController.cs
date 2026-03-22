using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("api/versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionsStorage Storage;
    private readonly IDatabaseReader  Reader;
    
    public VersionsController(IVersionsStorage storage, IDatabaseReader reader)
    {
        Storage = storage;
        Reader = reader;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetVersionsInfo()
    {
        try
        {
            VersionInfo[] vis = await Reader.ReadAllVersionsInfo();
            Console.WriteLine($"Versions requested. Sending {vis.Length} versions info");
            return Ok(vis);
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
            
            VersionInfo ver = await Reader.ReadVersionInfo(versionID);

            return Ok(ver);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }

    [HttpGet("download/{versionID}")]
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
