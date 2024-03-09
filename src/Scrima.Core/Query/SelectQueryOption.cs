using Scrima.Core.Query.Expressions;

namespace Scrima.Core.Query;

/// <summary>
/// A class containing deserialised values from the $select query option.
/// </summary>
public sealed class SelectQueryOption
{
    /// <summary>
    /// Initialises a new instance of the <see cref="SelectQueryOption" /> class.
    /// </summary>
    public SelectQueryOption(QueryNode expression, bool isStarSelect = false)
    {
        IsStarSelect = isStarSelect;
        Expression = expression;
    }

    /// <summary>
    /// Gets the expression.
    /// </summary>
    public QueryNode Expression { get; }

    public bool IsStarSelect { get; }
    
    public override string ToString()
    {
        var expression =  Expression?.ToString() ?? "<none>";

        return $"Select={expression}";
    }
}
