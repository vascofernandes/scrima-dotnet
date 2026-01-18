using System;
using Scrima.Core.Query;

namespace Scrima.OData;

public interface IoDataRawQueryParser
{
    QueryOptions ParseOptions(Type itemType, ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null);
}