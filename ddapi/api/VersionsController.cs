using Microsoft.AspNetCore.Mvc;

namespace api.Services;

[ApiController]
[Route("api/versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionsService VersionsService;

    public VersionsController(IVersionsService versionsService)
    {
        VersionsService = versionsService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetVersionsInfo()
    {
        try
        {
            VersionInfo[] vis = await VersionsService.GetVersions();
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
            
            VersionInfo ver = await VersionsService.GetVersion(versionID);

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
            FileStream stream = await VersionsService.GetVersionFile(versionID);
            return File(stream, "versions/zip", $"Deadays_{versionID}.zip");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }
}