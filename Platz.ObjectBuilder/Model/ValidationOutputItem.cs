
using Platz.SqlForms;

namespace Platz.ObjectBuilder;

public class ValidationOutputItem
{
    public string Message { get; set; }
    public string Location { get; set; }
    public string RuleName { get; set; }
    public ValidationLocationType LocationType { get; set; }
    public ValidationResultTypes Type { get; set; }

    public ValidationOutputItem()
    { }

    public ValidationOutputItem(RuleValidationResult src, ValidationLocationType locationType)
    {
        Message = src.Message;
        Location = src.Location;
        RuleName = src.RuleName;
        Type = src.Type;
        LocationType = locationType;
    }
}

public enum ValidationLocationType
{
    Project,
    Schema,
    Query,
    Form,
    Flow
}
