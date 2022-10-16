using MainOutsiderService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainOutsiderService.Controllers
{
    [Route("Protos")]
    [ApiController]
    public class ProtosController : ControllerBase
    {
        private readonly ILogger<ProtosController> _logger;
        private readonly string baseDirectory;
        public ProtosController(IWebHostEnvironment webHost , ILogger<ProtosController> logger)
        {
            _logger = logger;
            baseDirectory = webHost.ContentRootPath;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            _logger.LogInformation("-> baseDirectory: "+ baseDirectory);
            return
            Ok(Directory.GetFiles($"{baseDirectory}/../SharedData/Protos")
            .Select(Path.GetFileName));
        }

        [HttpGet("{protoName}")]
        public async Task<ActionResult> GetFileContent(string protoName)
        {
            var filePath =
            $"{baseDirectory}/../SharedData/Protos/{protoName}";
            if (System.IO.File.Exists(filePath))
                return Content(await
                System.IO.File.ReadAllTextAsync(filePath));
            return NotFound();
        }
    }
}
