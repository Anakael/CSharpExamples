using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestActionFilter;

[Route("api/[controller]")]
[ApiController]
public class TestActionFilter : ControllerBase
{
    [HttpPut]
    [Validate<LolModel>]
    [Validate<LolModelAnother>]
    public void Test([FromQuery] LolModelAnother filter, [FromBody] LolModel model) { }
}

public record LolModel(int LolInt, string LolString);

public record LolModelAnother(string SomeFilter);

public class LolModelValidator : AbstractValidator<LolModel>
{
    public LolModelValidator()
    {
        RuleFor(x => x.LolInt).GreaterThan(10);
        RuleFor(x => x.LolString).NotEmpty();
    }
}

public class LolModelAnotherValidator : AbstractValidator<LolModelAnother>
{
    public LolModelAnotherValidator()
    {
        RuleFor(x => x.SomeFilter).NotEmpty();
    }
}

public class ValidationFilterService<T>(IValidator<T> validator) : IActionFilter
    where T : class
{
    private readonly IValidator<T> _validator = validator;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (
            context.ActionArguments.Values.FirstOrDefault(x => x?.GetType() == typeof(T))
            is not T argument
        )
        {
            throw new Exception("No parameter to validate");
        }

        ValidationResult result = _validator.Validate(argument);
        if (result.IsValid)
        {
            return;
        }

        Results.ValidationProblem(result.ToDictionary()).ExecuteAsync(context.HttpContext);
        context.Result = new EmptyResult();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

public class ValidateAttribute<T> : ServiceFilterAttribute<ValidationFilterService<T>>
    where T : class;
