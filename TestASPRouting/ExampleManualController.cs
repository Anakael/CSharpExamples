namespace TestASPRouting;

[Route("api/ManualRouting")]
[ApiController]
public class ExampleManualController : ControllerBase
{
    [HttpGet("LolMethod")]
    public string LolMethod() => "LOL";

    [HttpGet("test-lol-method")]
    public string OtherLolMethod() => "LOL";

    [HttpGet("directLolMethod")]
    public string DirectLolMethod() => "LOL";
}
