using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TestASPRouting;

public partial class DashedRoutingConvention : IControllerModelConvention
{
    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])")]
    private static partial Regex KebabReplacer();

    public void Apply(ControllerModel controller)
    {
        foreach (SelectorModel controllerSelector in controller.Selectors)
        {
            if (controllerSelector.AttributeRouteModel is null)
            {
                return;
            }

            controllerSelector.AttributeRouteModel.Template =
                controllerSelector.AttributeRouteModel.Template?.Replace(
                    "[controller]",
                    PascalToKebabCase(controller.ControllerName)
                );
        }

        foreach (ActionModel controllerAction in controller.Actions)
        {
            foreach (SelectorModel actionSelector in controllerAction.Selectors)
            {
                if (actionSelector.AttributeRouteModel is null)
                {
                    return;
                }

                actionSelector.AttributeRouteModel.Template =
                    actionSelector.AttributeRouteModel.Template?.Replace(
                        "[action]",
                        PascalToKebabCase(controllerAction.ActionName)
                    );
            }
        }
    }

    public static string PascalToKebabCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return KebabReplacer().Replace(value, "-$1").Trim().ToLower();
    }
}
