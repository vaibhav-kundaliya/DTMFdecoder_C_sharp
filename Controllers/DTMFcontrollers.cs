using Microsoft.AspNetCore.Mvc;
using DtmfDetection; // Assuming this is where your DTMF detection logic is
using DtmfDetection.NAudio;
using NAudio.Wave;
using DTMF_decoder_c_sharp.Models;


namespace DTMF_decoder_c_sharp.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DtmfController : ControllerBase
    {
        [HttpPost]
        public IActionResult DecodeDtmf([FromForm] ThresholdInputModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("Invalid file.");

            try
            {
                // Save the uploaded file temporarily
                var tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }

                // Use the saved file path to create an AudioFileReader
                using var audioFile = new AudioFileReader(tempFilePath);
                var myConfig = Config.Default.WithThreshold(model.Threshold);
                var dtmfs = audioFile.DtmfChanges(config: myConfig);
                string decodedString = "";
                foreach (var dtmf in dtmfs){
                    if(dtmf.IsStart)
                        // Console.Write(dtmf.Key.ToSymbol());
                        decodedString += dtmf.Key.ToSymbol().ToString();
                } 


                System.IO.File.Delete(tempFilePath);
                return Ok(decodedString);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
