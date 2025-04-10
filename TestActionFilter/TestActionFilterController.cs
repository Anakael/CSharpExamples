using System.Reflection;
using Dumpify;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestActionFilter;

[Route("api/[controller]")]
[ApiController]
public class TestActionFilter : ControllerBase
{
    [HttpPut]
    [UseValidationFilter]
    public void Test(
        [FromQuery, Validate] LolModelAnother filter,
        [FromBody, Validate] LolModel model
    ) { }
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

public class UseValidationFilter: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Type validatorBaseType = typeof(IValidator<>);
        IEnumerable<ControllerParameterDescriptor> parameters = context
            .ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>()
            .Where(x => x.ParameterInfo.IsDefined(typeof(ValidateAttribute)));
        foreach (ControllerParameterDescriptor param in parameters)
        {
            Type validatorType = validatorBaseType.MakeGenericType(param.ParameterType);

            object? service = context.HttpContext.RequestServices.GetService(validatorType);
            if (service is null)
            {
                continue;
            }

            object? paramValue = context.ActionArguments[param.Name];
            ValidationResult result = (ValidationResult)
                validatorType
                    .GetMethod(nameof(IValidator.Validate))!
                    .Invoke(service, [paramValue])!;
            if (result.IsValid)
            {
                continue;
            }

            Results.ValidationProblem(result.ToDictionary()).ExecuteAsync(context.HttpContext);
            context.Result = new EmptyResult();
        }
    }

    public override void OnActionExecuted(ActionExecutedContext context) { }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class ValidateAttribute : Attribute;
