using System;
using System.Text;
using Scrima.OData.Parsers;

namespace Scrima.OData;

/// <summary>
/// A class which contains the raw request values.
/// </summary>
public sealed class ODataRawQueryOptions
{
    public const string SelectParamName = "$select";
    public const string FilterParamName = "$filter";
    public const string OrderByParamName = "$orderby";
    public const string SkipParamName = "$skip";
    public const string TopParamName = "$top";
    public const string SearchParamName = "$search";
    public const string SkipTokenParamName = "$skiptoken";
    public const string CountParamName = "$count";

    /// <summary>
    /// Initialises a new instance of the <see cref="ODataRawQueryOptions"/> class.
    /// </summary>
    /// <param name="rawQuery">The raw query.</param>
    /// <exception cref="ArgumentNullException">Thrown if raw query is null.</exception>
    public static ODataRawQueryOptions ParseRawQuery(string rawQuery)
    {
        const string SelectFullParam = SelectParamName + "=";
        const string FilterFullParam = FilterParamName + "=";
        const string OrderByFullParam = OrderByParamName + "=";
        const string SkipFullParam = SkipParamName + "=";
        const string TopFullParam = TopParamName + "=";
        const string SearchFullParam = SearchParamName + "=";
        const string SkipTokenFullParam = SkipTokenParamName + "=";
        const string CountFullParam = CountParamName + "=";

        var options = new ODataRawQueryOptions();

        ArgumentNullException.ThrowIfNull(rawQuery);

        // Any + signs we want in the data should have been encoded as %2B,
        // so do the replace first otherwise we replace legitemate + signs!
        rawQuery = rawQuery.Replace('+', ' ');

        if (rawQuery.Length > 0)
        {
            // Drop the ?
            var query = rawQuery.StartsWith("?") ? rawQuery.Substring(1) : rawQuery;

            var queryOptions = query.Split(SplitCharacter.Ampersand, StringSplitOptions.RemoveEmptyEntries);

            foreach (var queryOption in queryOptions)
            {
                // Decode the chunks to prevent splitting the query on an '&' which is actually part of a string value
                var rawQueryOption = Uri.UnescapeDataString(queryOption);

                if (rawQueryOption.StartsWith(SelectFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != SelectFullParam.Length)
                    {
                        options.Select = rawQueryOption.Substring(SelectFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(FilterFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != FilterFullParam.Length)
                    {
                        options.Filter = rawQueryOption.Substring(FilterFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(OrderByFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != OrderByFullParam.Length)
                    {
                        options.OrderBy = rawQueryOption.Substring(OrderByFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(SkipFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != SkipFullParam.Length)
                    {
                        options.Skip = rawQueryOption.Substring(SkipFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(TopFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != TopFullParam.Length)
                    {
                        options.Top = rawQueryOption.Substring(TopFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(SearchFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != SearchFullParam.Length)
                    {
                        options.Search = rawQueryOption.Substring(SearchFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(SkipTokenFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != SkipTokenFullParam.Length)
                    {
                        options.SkipToken = rawQueryOption.Substring(SkipTokenFullParam.Length);
                    }
                }
                else if (rawQueryOption.StartsWith(CountFullParam, StringComparison.Ordinal))
                {
                    if (rawQueryOption.Length != CountFullParam.Length)
                    {
                        options.Count = rawQueryOption.Substring(CountFullParam.Length);
                    }
                }
            }
        }

        return options;
    }

    /// <summary>
    /// Gets the raw $count query value from the incoming request Uri if specified.
    /// </summary>
    public string Count { get; set; }

    /// <summary>
    /// Gets the raw $select query value from the incoming request Uri if specified.
    /// </summary>
    public string Select { get; set; }

    /// <summary>
    /// Gets the raw $filter query value from the incoming request Uri if specified.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Gets the raw $orderby query value from the incoming request Uri if specified.
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    /// Gets the raw $search query value from the incoming request Uri if specified.
    /// </summary>
    public string Search { get; set; }

    /// <summary>
    /// Gets the raw $skip query value from the incoming request Uri if specified.
    /// </summary>
    public string Skip { get; set; }

    /// <summary>
    /// Gets the raw $skip token query value from the incoming request Uri if specified.
    /// </summary>
    public string SkipToken { get; set; }

    /// <summary>
    /// Gets the raw $top query value from the incoming request Uri if specified.
    /// </summary>
    public string Top { get; set; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var builder = new StringBuilder();
            
        if (Select != null)
        {
            builder.Append(SelectParamName);
            builder.Append('=');
            builder.Append(Select);
            builder.Append('&');
        }
        
        if (Filter != null)
        {
            builder.Append(FilterParamName);
            builder.Append('=');
            builder.Append(Filter);
            builder.Append('&');
        }

        if (OrderBy != null)
        {
            builder.Append(OrderByParamName);
            builder.Append('=');
            builder.Append(OrderBy);
            builder.Append('&');
        }

        if (Skip != null)
        {
            builder.Append(SkipParamName);
            builder.Append('=');
            builder.Append(Skip);
            builder.Append('&');
        }

        if (Top != null)
        {
            builder.Append(TopParamName);
            builder.Append('=');
            builder.Append(Top);
            builder.Append('&');
        }

        if (Search != null)
        {
            builder.Append(SearchParamName);
            builder.Append('=');
            builder.Append(Search);
            builder.Append('&');
        }

        if (SkipToken != null)
        {
            builder.Append(SkipTokenParamName);
            builder.Append('=');
            builder.Append(SkipToken);
            builder.Append('&');
        }

        if (Count != null)
        {
            builder.Append(CountParamName);
            builder.Append('=');
            builder.Append(Count);
            builder.Append('&');
        }

        return builder.ToString(0, builder.Length > 0 ? builder.Length - 1 : builder.Length);
    }
}
