using D2Hunt.App.Infrastructure.Helpers;

namespace D2Hunt.App.Data.ValidationRules;

public class HotIpValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        var stringValue = value as string;
        if (string.IsNullOrEmpty(stringValue))
        {
            return new ValidationResult(true, null);
        }

        var result = stringValue.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).All(x => StringHelper.IsValidIpAddress(x));
        if (result)
        {
            Log.ForContext<HotIpValidationRule>().LogInfo("Validation successful for TextBox with value: {Value}", stringValue);
            return new ValidationResult(true, null);
        }

        
        return new ValidationResult(false, "Not a valid IP");
    }

}