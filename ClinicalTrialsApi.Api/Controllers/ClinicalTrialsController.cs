using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using System.IO;
using System.Text;

namespace ClinicalTrialsApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalTrialsController : ControllerBase
    {
        private readonly IClinicalTrialService _trialService;
        private const int MaxFileSizeBytes = 1024 * 1024; // 1MB

        public ClinicalTrialsController(IClinicalTrialService trialService)
        {
            _trialService = trialService;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadTrial(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (file.Length > MaxFileSizeBytes)
                return BadRequest("File size exceeds the limit of 1MB");

            if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .json files are allowed");

            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            try
            {
                var trial = await _trialService.ProcessAndSaveTrialDataAsync(jsonContent);
                return CreatedAtAction(nameof(GetTrialById), new { id = trial.Id }, trial);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the trial data");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicalTrial>> GetTrialById(int id)
        {
            var trial = await _trialService.GetTrialByIdAsync(id);
            if (trial == null)
                return NotFound();

            return Ok(trial);
        }

        [HttpGet("trial/{trialId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicalTrial>> GetTrialByTrialId(string trialId)
        {
            var trial = await _trialService.GetTrialByTrialIdAsync(trialId);
            if (trial == null)
                return NotFound();

            return Ok(trial);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicalTrial>>> GetTrials([FromQuery] string status = null)
        {
            var trials = await _trialService.GetTrialsAsync(status);
            return Ok(trials);
        }
    }
} 