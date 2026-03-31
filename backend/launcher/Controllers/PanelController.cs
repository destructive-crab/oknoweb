using api.Debug;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;

[ApiController]
[Authorize]
[Route("versions")]
public sealed class PanelController : ControllerBase
{
    private readonly IVersionsStorage Storage;
    private readonly IDatabaseWriter  Writer;
    private readonly IDatabaseReader  Reader;
    private readonly ILocalLogger     Logger;

    public PanelController(IVersionsStorage storage, IDatabaseWriter writer, IDatabaseReader reader, ILocalLogger logger)
    {
	    Storage = storage;
	    Writer  = writer;
	    Reader  = reader;
        Logger  = logger;
    }

    [HttpPost("{versionID}/id")]
    public async Task<IActionResult> EditVersionID(string versionID, [FromForm] string newVersionID)
        => await EditVersion(versionID, (i) => i.PublicInfo.ID = newVersionID);
    
    [HttpPost("{versionID}/name")]
    public async Task<IActionResult> EditVersionName(string versionID, [FromForm] string newName)
        => await EditVersion(versionID, (i) => i.PublicInfo.Name = newName);

    [HttpPost("{versionID}/tag")]
    public async Task<IActionResult> EditVersionTag(string versionID, [FromForm] string newTag)
        => await EditVersion(versionID, (i) => i.PublicInfo.Tag = newTag);

    [HttpPost("versions/{versionID}/changelog")]
    public async Task<IActionResult> EditVersionChangelog(string versionID, [FromForm] string newChangelog)
        => await EditVersion(versionID, (i) => i.PublicInfo.Changelog = newChangelog);
    
    [HttpPost("files/{versionID}")]
    public async Task<IActionResult> EditVersionFile(string versionID, [FromForm] IFormFile file)
    {
        try
        {
            LocalVersionInfo? info = await Reader.ReadVersionInfo(versionID);
            
            if (info == null)
            {
                return StatusCode(400, $"Invalid ID {versionID}");
            }
            
            info.Path = await Storage.WriteVersionOnDisk(file, versionID, info.PublicInfo.Tag);

            await Writer.EditVersion(versionID, info);
            
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return StatusCode(500, "Failed uploading new version file");
        }
    }

    [HttpPost("{versionID}")]
    public async Task<IActionResult> PostVersion(string versionID, [FromForm] string name, [FromForm] string tag, [FromForm] IFormFile file, [FromForm] string changelog)
    {
        if (!file.FileName.EndsWith(".zip") && !file.FileName.EndsWith(".tar.gz"))
        {
            return StatusCode(400, $"Invalid file extension {file.FileName}");
        }

        try
        {
            LocalVersionInfo? versionInfo = await Reader.ReadVersionInfo(versionID);
            if (versionInfo != null)
            {
                versionInfo.PublicInfo.Name      = name;
                versionInfo.PublicInfo.Tag       = tag;
                versionInfo.PublicInfo.Changelog = changelog;

                
                
                return Ok();
            }
            
            string path = await Storage.WriteVersionOnDisk(file, versionID, tag);

            Writer.RegisterVersion(new LocalVersionInfo(new(versionID, name, tag, changelog, DateTime.Today.Date.ToString()),  path));
            
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error($"Failed uploading version: {e}");
            return StatusCode(500, $"Failed uploading version: {e}");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVersion(string id)
    {
        try
        {
            await Storage.DeleteVersionFile(id);
            await Writer .DeleteVersion(id);

            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error($"Failed deleting {id} " + e);
            return StatusCode(500, $"Failed deleting version: {e}");
        }
    }
    
    private async Task<IActionResult> EditVersion(string versionID, Action<LocalVersionInfo> editInfo)
    {
        try
        {
            LocalVersionInfo? info = await Reader.ReadVersionInfo(versionID);

            if (info == null)
            {
                return StatusCode(400, $"Invalid ID: {versionID}");    
            }
            
            editInfo.Invoke(info);
            
            await Writer.EditVersion(info.PublicInfo.ID, info);
            
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error($"Failed editing version: {e}");
            
            return StatusCode(500, $"Failed editing version: {e}");
        }
    }
}
