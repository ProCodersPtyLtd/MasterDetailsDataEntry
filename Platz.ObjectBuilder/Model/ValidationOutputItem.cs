
using Platz.SqlForms;

namespace Platz.ObjectBuilder;

public class ValidationOutputItem
{
    public string Message { get; set; }
    public string Location { get; set; }
    public ValidationLocationType LocationType { get; set; }
    public ValidationResultTypes Type { get; set; }
}

public enum ValidationLocationType
{
    Project,
    Schema,
    Query,
    Form,
    Flow
}
