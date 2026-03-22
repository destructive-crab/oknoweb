using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;

[ApiController]
[Authorize]
[Route("panel")]
public sealed class PanelController : ControllerBase
{
    private readonly IVersionsStorage Storage;
    private readonly IDatabaseWriter Writer;
    private readonly IDatabaseReader Reader;

    public PanelController(IVersionsStorage storage, IDatabaseWriter writer, IDatabaseReader reader)
    {
	Storage = storage;
	Writer  = writer;
	Reader  = reader;
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
            string path = await Storage.WriteVersionOnDisk(file, versionID, tag);

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
            await Writer .DeleteVersion(versionID);
	    await Storage.DeleteVersionFile(versionID);
	    
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed deleting {versionID}  " + e.ToString());
            return StatusCode(500, $"Failed deleting version: {e}");
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
}
