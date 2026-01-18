using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Scrima.OData.AspNetCore;

internal class QueryOptionsBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType.IsODataQuery())
        {
            return new BinderTypeModelBinder(typeof(ODataQueryModelBinder));
        }

        return null;
    }
}
