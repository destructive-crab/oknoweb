using Debug;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Submit;

[ApiController]
[Route("")]
public sealed class SubmissionsController : ControllerBase
{
    private readonly IDatabaseWriter  Writer;
    private readonly IDatabaseReader  Reader;
    private readonly ILocalLogger     Logger;

    public SubmissionsController(IDatabaseWriter writer, IDatabaseReader reader, ILocalLogger logger)
    {
	    Writer  = writer;
	    Reader  = reader;
        Logger  = logger;
    }

    [HttpPost("new")]
    public async Task<IActionResult> PostNewSubmission(string name, string contact, string link, string additionalInfo)
    {
        try
        {
            PrivateSubmit info = new PrivateSubmit(contact, await PublicSubmit.GenerateID(Reader, Logger),
                PublicSubmit.UNVERIFIED_STATUS, name, link, additionalInfo, DateTime.Now.ToString("dd/MM/yyyy"));

            await Writer.WriteSubmission(info);

            return Ok();
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
    public async Task<IActionResult> GetAllPrivate(string subid)
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
    [HttpPost("panel/submissions/review/{subid}")]
    public async Task<IActionResult> MarkSubmissionAsReview(string subid, string videoLink)
    {
        try
        {
            await Writer.MarkAsReviewed(subid, videoLink);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return StatusCode(500, "Internal Server Error");
        }
    }
}