using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Extensions.FilterExpression
{
    public static class FilterExpressionExtensions
    {
        public static Microsoft.Xrm.Sdk.Query.FilterExpression Equal(this Microsoft.Xrm.Sdk.Query.FilterExpression ex, string attrName, object value)
        {
            ex.AddCondition(new Microsoft.Xrm.Sdk.Query.ConditionExpression(attrName, Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, value));
            return ex;
        }

        public static Microsoft.Xrm.Sdk.Query.FilterExpression IsNull(this Microsoft.Xrm.Sdk.Query.FilterExpression ex, string attrName)
        {
            ex.AddCondition(new Microsoft.Xrm.Sdk.Query.ConditionExpression(attrName, Microsoft.Xrm.Sdk.Query.ConditionOperator.Null));
            return ex;
        }

        public static Microsoft.Xrm.Sdk.Query.LinkEntity Inner(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string alias, string toEntity, string fromAttr, string toAttr)
        {
            var next = new Microsoft.Xrm.Sdk.Query.LinkEntity(query.EntityName, toEntity, fromAttr, toAttr, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);
            next.EntityAlias = alias;
            query.LinkEntities.Add(next);
            return next;
        }

        public static Microsoft.Xrm.Sdk.Query.LinkEntity Inner(this Microsoft.Xrm.Sdk.Query.LinkEntity link, string alias, string toEntity, string fromAttr, string toAttr)
        {
            var next = new Microsoft.Xrm.Sdk.Query.LinkEntity(link.LinkToEntityName, toEntity, fromAttr, toAttr, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);
            next.EntityAlias = alias;
            link.LinkEntities.Add(next);
            return next;
        }
    }
}
