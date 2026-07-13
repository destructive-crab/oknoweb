using Debug;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Submit;

[ApiController]
[Route("")]
public sealed class SubmissionsController : ControllerBase
{
    private readonly IDatabaseWriter Writer;
    private readonly IDatabaseReader Reader;
    private readonly ILocalLogger Logger;

    public SubmissionsController(IDatabaseWriter writer, IDatabaseReader reader, ILocalLogger logger)
    {
        Writer = writer;
        Reader = reader;
        Logger = logger;
    }

    [HttpPost("new")]
    public async Task<IActionResult> PostNewSubmission([FromForm] string name, [FromForm] string contact, [FromForm] string link, [FromForm] string additionalInfo)
    {
        try
        {
            Logger.Message($"NewS: {name} {contact} {additionalInfo} {link}");
            PrivateSubmit info = new PrivateSubmit(contact, await PublicSubmit.GenerateID(Reader, Logger),
                PublicSubmit.UNVERIFIED_STATUS, name, link, additionalInfo, DateTime.Now.ToString("dd/MM/yyyy"), "none");

            await Writer.WriteSubmission(info);

            return Ok(info);
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("submissions_verified_count")]
    public async Task<IActionResult> GetVerifiedSubmissionsCount()
    {
        try
        {
            PrivateSubmit[] all = await Reader.ReadAll();
            List<PublicSubmit> pubAll = new();

            for (var i = 0; i < all.Length; i++)
            {
                if (all[i].Status == PublicSubmit.REVIEWED_STATUS || all[i].Status == PublicSubmit.PENDING_STATUS)
                {
                    pubAll.Add(all[i]);
                }
            }

            return Ok(pubAll.Count);
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    
    [HttpGet("submissions_unverified_count")]
    public async Task<IActionResult> GetUnverifiedSubmissionsCount()
    {
        try
        {
            PrivateSubmit[] all = await Reader.ReadAll();
            List<PublicSubmit> pubAll = new();

            for (var i = 0; i < all.Length; i++)
            {
                if (all[i].Status == PublicSubmit.UNVERIFIED_STATUS)
                {
                    pubAll.Add(all[i]);
                }
            }

            return Ok(pubAll.Count);
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("submissions")]
    public async Task<IActionResult> GetAllSubmissions()
    {
        try
        {
            PrivateSubmit[] all = await Reader.ReadAll();
            List<PublicSubmit> pubAll = new();

            for (var i = 0; i < all.Length; i++)
            {
                if (all[i].Status == PublicSubmit.REVIEWED_STATUS || all[i].Status == PublicSubmit.PENDING_STATUS)
                {
                    pubAll.Add(all[i]);
                }
            }

            return Ok(pubAll);
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("submissions/{subid}")]
    public async Task<IActionResult> GetSubmission(string subid)
    {
        try
        {
            if (!(await Reader.HasSubmission(subid)))
            {
                return BadRequest($"No submission with {subid} ID");
            }

            return Ok(await Reader.Read(subid));
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    //admin

    [Authorize]
    [HttpGet("panel/login")]
    public async Task<IActionResult> Login()
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("panel/submissions")]
    public async Task<IActionResult> GetAllPrivate()
    {
        try
        {
            return Ok(await Reader.ReadAll());
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("panel/submissions/reject/{subid}")]
    public async Task<IActionResult> RejectSubmission(string subid)
    {
        try
        {
            await Writer.DeleteSubmission(subid);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("panel/submissions/pend/{subid}")]
    public async Task<IActionResult> PendSubmission(string subid)
    {
        try
        {
            await Writer.MarkAsPending(subid);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("panel/submissions/edit/{id}")]
    public async Task<IActionResult> EditSubmission([FromForm] string name, [FromForm] string contact, [FromForm] string link, [FromForm] string additionalInfo, [FromForm] string date)
    {
        try
        {
            var submitInfo = await Reader.Read(id);
            
            submitInfo.Name = name;
            submitInfo.Contact = contact;
            submitInfo.Link = link;
            submitInfo.AdditionalInfo = additionalInfo;
            submitInfo.Date = date;
            
            await Writer.WriteSubmission(submitInfo);
            
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [Authorize]
    [HttpPost("panel/submissions/review/set/{subid}")]
    public async Task<IActionResult> ReviewSubmit(string subid, [FromForm] string reviewLink)
    {
        try
        {
            await Writer.MarkAsReviewed(subid, reviewLink);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("panel/submissions/review/remove/{subid}")]
    public async Task<IActionResult> RemoveSubmitReview(string subid)
    {
        try
        {
            await Writer.MarkAsPending(subid);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }
}
