using Abp.Dependency;
using GraphQL.Types;
using GraphQL.Utilities;
using SR.EscrowBaseWeb.Queries.Container;
using System;

namespace SR.EscrowBaseWeb.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IServiceProvider provider) :
            base(provider)
        {
            Query = provider.GetRequiredService<QueryContainer>();
        }
    }
}