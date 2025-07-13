namespace TestASPRouting;

[Route("api/[controller]")]
[ApiController]
public class ExampleManagementController : ControllerBase
{
    [HttpGet("[action]")]
    public string LolMethod() => "LOL";

    [HttpGet("test/[action]")]
    public string OtherLolMethod() => "LOL";

    [HttpGet("direct")]
    public string DirectLolMethod() => "LOL";
}
