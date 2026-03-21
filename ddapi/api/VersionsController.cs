using Microsoft.AspNetCore.Mvc;

namespace api.Services;

[ApiController]
[Route("api/versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionsStorage versionsStorage;
    private readonly IDatabaseWriter  Writer;
    private readonly IDatabaseReader  Reader;
    
    public VersionsController(IVersionsStorage versionsStorage, IDatabaseWriter writer, IDatabaseReader reader)
    {
        this.versionsStorage = versionsStorage;
        Writer = writer;
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
            FileStream stream = await versionsStorage.GetVersionFile(versionID);
            return File(stream, "zip", $"Deadays_{versionID}.zip");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed getting version: {versionID}");
        }
    }
    
    private async Task<IActionResult> EditVersion(string versionID, Action<VersionInfo> editInfo)
    {
        try
        {
            VersionInfo info = await Reader.ReadVersionInfo(versionID);
            
            editInfo.Invoke(info);
            
            await Writer.EditVersion(versionID, info);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed editing version: {e}");
            return StatusCode(500, $"Failed editing version: {e}");
        }
    }
    
    [HttpPost("edit_id")]
    public async Task<IActionResult> EditVersionID(string versionID, string newVersionID)
        => await EditVersion(versionID, (i) => i.ID = newVersionID);
    
    [HttpPost("edit_name")]
    public async Task<IActionResult> EditVersionName(string versionID, string newName)
        => await EditVersion(versionID, (i) => i.Name = newName);

    
    [HttpPost("edit_tag")]
    public async Task<IActionResult> EditVersionTag(string versionID, string newTag)
        => await EditVersion(versionID, (i) => i.Tag = newTag);

    
    [HttpPost("edit_changelog")]
    public async Task<IActionResult> EditVersionChangelog(string versionID, string newChangelog)
        => await EditVersion(versionID, (i) => i.Changelog = newChangelog);
    
    [HttpPost("upload")]
    public async Task<IActionResult> PostVersion(string versionID, string name, string tag, IFormFile file, string changelog)
    {
        if (!file.FileName.EndsWith(".zip") && !file.FileName.EndsWith(".tar.gz"))
        {
            return StatusCode(500, $"Invalid file extension");
        }

        try
        {
            string path = await versionsStorage.WriteVersionOnDisk(file, versionID, tag);

            Writer.RegisterVersion(new VersionInfo(versionID, path, name, tag, changelog, DateTime.Today.Date.ToString()));
            
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed uploading version: {e}");
            return StatusCode(500, $"Failed uploading version: {e}");
        }
    }
    
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteVersion(string versionID)
    {
        try
        {
            await Writer.DeleteVersion(versionID);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Failed deleting version: {e}");
        }
    }
}