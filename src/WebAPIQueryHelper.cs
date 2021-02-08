using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using LINQPad;

namespace NY.Dataverse.LINQPadDriver
{
    public class WebAPIQueryHelper
    {
        public static Hyperlinq BuildWebApiUrl(CdsServiceClient DataverseClient, string query)
        {

            var hasLevel2LinkEntity = false;
            var expand = "";
            var filter = "";

            var url = $"{DataverseClient.ConnectedOrgPublishedEndpoints[EndpointType.WebApplication]}api/data/v{DataverseClient.ConnectedOrgVersion.Major}.{DataverseClient.ConnectedOrgVersion.Minor}/";
            var fetchXml = XElement.Parse(query);
            var entityElement = fetchXml.Element(FetchAttributes.Entity);
            var entityMetadata = new List<EntityMetadata>();
            var mainEntity = DataverseClient.GetEntityMetadata(entityElement.Attribute(FetchAttributes.Name).Value, EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships);
            entityMetadata.Add(mainEntity);
            entityMetadata.AddRange(fetchXml.Descendants(FetchAttributes.LinkEntity).Select(x => DataverseClient.GetEntityMetadata(x.Attribute(FetchAttributes.Name).Value, EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships)).ToList());
            var entityName = entityMetadata[0].EntitySetName;

            url += $"{entityName}?";

            var top = fetchXml.Attribute(FetchAttributes.Top)?.Value;
            var count = fetchXml.Attribute(FetchAttributes.Count)?.Value;
            var mainAttributes = entityElement.Elements(FetchAttributes.Attribute).Select(x => x.Attribute(FetchAttributes.Name).Value);
            var mainSortings = entityElement.Elements(FetchAttributes.Order).Select(x => (name: x.Attribute(FetchAttributes.Attribute).Value, descending: x.Attribute(FetchAttributes.Descending)?.Value, ascending: x.Attribute(FetchAttributes.Ascending)?.Value));
            var linkEntities = entityElement.Elements(FetchAttributes.LinkEntity);
            var filters = entityElement.Descendants(FetchAttributes.Filter);
            var topLevelFilter = filters.Count() > 1 ? filters.Skip(1) : filters;

            if (linkEntities.Any())
            {
                expand = BuildExpands(linkEntities, entityMetadata, mainAttributes);
                hasLevel2LinkEntity = linkEntities.Any(l => l.Element(FetchAttributes.LinkEntity) != null);
            }
            if (topLevelFilter?.Elements().Any() == true)
                filter = BuildFilter(filters.Reverse(), mainAttributes, linkEntities, entityMetadata, expand);
            var correctedMainAttributes = from a in mainAttributes
                                          join a1 in mainEntity.Attributes on a equals a1.LogicalName
                                          select (a1.AttributeType == AttributeTypeCode.Lookup || a1.AttributeType == AttributeTypeCode.Customer || a1.AttributeType == AttributeTypeCode.Owner) ? $"_{a}_value" : a;
            if (mainAttributes.Any())
                url += $"$select={string.Join(",", correctedMainAttributes)}";
            if (!string.IsNullOrEmpty(expand))
                url += expand;
            if (!string.IsNullOrEmpty(filter))
            {
                url += filter;
                url += $" and ({BuildLinkEntityJoinConditions(mainEntity.LogicalName, linkEntities, entityMetadata)})";
            }
            else
            {
                if (linkEntities.Any())
                {
                    filter = $"{(mainAttributes.Any() || !string.IsNullOrEmpty(expand) ? "&" : "")}$filter=({BuildLinkEntityJoinConditions(mainEntity.LogicalName, linkEntities, entityMetadata)})";
                    url += filter;
                }
            }
            if (count != null || top != null)
                url += ($"{(mainAttributes.Any() || !string.IsNullOrEmpty(expand) || !string.IsNullOrEmpty(filter) ? "&" : "")}$top={top ?? count}");
            if (mainSortings.Any())
            {
                var orders = string.Join(",", mainSortings.Select(o => $"{o.name}{(o.ascending ?? (o.descending != null ? " desc" : " asc"))}"));
                url += ($"{(mainAttributes.Any() || !string.IsNullOrEmpty(expand) || !string.IsNullOrEmpty(filter) || !string.IsNullOrEmpty(top) || !string.IsNullOrEmpty(count) ? "&" : "")}$orderby={orders}");
            }
            return !hasLevel2LinkEntity ? new Hyperlinq(url).Dump("WebAPI URL") : null;
        }

        private static string BuildFilter(IEnumerable<XElement> topLevelFilter, IEnumerable<string> mainAttributes, IEnumerable<XElement> linkEntities, List<EntityMetadata> entityMetadata, string expand)
        {
            var filter = "";
            var conditionsList = new List<string>();
            var groupedConditions = new List<string>();
            var i = 1;
            var filterCount = topLevelFilter.Count();
            if (filterCount > 1)
                topLevelFilter = topLevelFilter.Take(filterCount - 1);
            foreach (var f in topLevelFilter)
            {
                var filterType = f.Attribute(FetchAttributes.Type)?.Value ?? "and";
                var conditions = BuildConditions(filterType, f.Elements(FetchAttributes.Condition), linkEntities, entityMetadata);
                if (!string.IsNullOrEmpty(conditions) && !f.Elements(FetchAttributes.Filter).Any())
                {
                    conditionsList.Add(conditions);
                }
                if (conditionsList.Any() && (i == topLevelFilter.Count() || f.Elements(FetchAttributes.Filter).Any()))
                {
                    if (!string.IsNullOrEmpty(conditions))
                    {
                        if (f.Elements(FetchAttributes.Filter).Any())
                            filter += ($"({conditions} {filterType} (" + string.Join($" {filterType} ", conditionsList)) + "))";
                        else
                            filter += conditions;
                        groupedConditions.Add(($"({conditions} {filterType} (" + string.Join($" {filterType} ", conditionsList)) + "))");
                        conditionsList.Clear();
                    }
                    else
                    {
                        groupedConditions.AddRange(conditionsList);
                        groupedConditions.Reverse();
                        filter = string.Join($" {filterType} ", groupedConditions);
                        groupedConditions.Clear();
                    }
                }
                i++;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                filter = $"{(mainAttributes.Any() || !string.IsNullOrEmpty(expand) ? "&" : "")}$filter=({filter})";
            }
            return filter;
        }

        private static string BuildExpands(IEnumerable<XElement> linkEntities, List<EntityMetadata> entityMetadata, IEnumerable<string> mainAttributes)
        {
            var expand = string.Join(",", linkEntities.Select(l =>
            {
                var linkAttributes = l.Elements(FetchAttributes.Attribute);
                if (linkAttributes.Any())
                {
                    var linkEntityMetadata = entityMetadata.SingleOrDefault(e => e.LogicalName == l.Attribute(FetchAttributes.Name).Value);

                    var relationshipName = linkEntityMetadata?.OneToManyRelationships
                                          ?.Where(x => x.ReferencingEntity == entityMetadata[0].LogicalName && x.ReferencingAttribute == l.Attribute(FetchAttributes.To).Value).FirstOrDefault();
                    return relationshipName != null ? $@"{relationshipName?.ReferencingEntityNavigationPropertyName}($select={string.Join(",", linkAttributes.Select(a => {
                        var attributeName = a.Attribute(FetchAttributes.Name).Value;
                        var attributeType = linkEntityMetadata.Attributes.Single(x => x.LogicalName == attributeName).AttributeType;
                        return attributeType == AttributeTypeCode.Customer || attributeType == AttributeTypeCode.Lookup || attributeType == AttributeTypeCode.Owner ? $"_{attributeName}_value" : attributeName;
                    }))})" : "";
                }
                return "";
            }).ToList());
            if (!string.IsNullOrEmpty(expand))
            {
                expand = $"{(mainAttributes.Any() ? "&" : "")}$expand=" + expand;
            }
            return expand;
        }

        private static string BuildConditions(string filterType, IEnumerable<XElement> conditions, IEnumerable<XElement> linkEntities, List<EntityMetadata> entityMetadata)
        {
            var andConditions = string.Join($" {filterType} ", conditions.Select(c =>
            {
                var conditionOperator = c.Attribute(FetchAttributes.Operator)?.Value;
                var conditionEntity = c.Attribute(FetchAttributes.EntityName)?.Value;
                var linkEntityFieldName = "";
                var linkEntityName = "";
                var linkEntityRelationshipName = "";
                AttributeMetadata attributeMetadata = null;
                var likeFunction = "contains";

                if (linkEntities.Any())
                {
                    var linkEntity = linkEntities.SingleOrDefault(e => e.Attribute(FetchAttributes.Alias).Value == conditionEntity);
                    linkEntityFieldName = linkEntity?.Attribute(FetchAttributes.To).Value ?? "";
                    linkEntityName = linkEntity?.Attribute(FetchAttributes.Name).Value ?? "";
                    linkEntityRelationshipName = (from e in entityMetadata
                                                  where e.LogicalName == linkEntityName
                                                  select e.OneToManyRelationships.Single(x => x.ReferencingEntity == entityMetadata[0].LogicalName && x.ReferencingAttribute == linkEntityFieldName)).FirstOrDefault()?.ReferencingEntityNavigationPropertyName;
                }
                var attributeValue = c.Attribute(FetchAttributes.AttributeValue)?.Value ?? "";
                var attributeName = c.Attribute(FetchAttributes.Attribute)?.Value;
                if (string.IsNullOrEmpty(attributeName)) return "";

                if (string.IsNullOrEmpty(linkEntityName))
                {
                    attributeMetadata = (from e in entityMetadata
                                         where e.LogicalName == entityMetadata[0].LogicalName
                                         select e.Attributes.First(a => a.LogicalName == attributeName)).First();
                }
                else
                {
                    attributeMetadata = (from e in entityMetadata
                                         where e.LogicalName == linkEntityName
                                         select e.Attributes.First(a => a.LogicalName == attributeName)).First();
                }

                if (attributeValue.EndsWith("%") && !attributeValue.StartsWith("%"))
                {
                    likeFunction = "startswith";
                    attributeValue = attributeValue.Substring(0, attributeValue.Length - 1);
                }
                else if (!attributeValue.EndsWith("%") && attributeValue.StartsWith("%"))
                {
                    likeFunction = "endswith";
                    attributeValue = attributeValue.Substring(1);
                }
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    switch (attributeMetadata.AttributeType)
                    {
                        case AttributeTypeCode.String:
                            attributeValue = $"'{attributeValue}'";
                            break;
                        case AttributeTypeCode.Boolean:
                            attributeValue = attributeValue == "1" ? "true" : "false";
                            break;
                        case AttributeTypeCode.DateTime:
                            attributeValue = DateTime.Parse(attributeValue).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                            break;
                    }
                }
                switch (conditionOperator)
                {
                    case "eq":
                    case "ne":
                    case "lt":
                    case "le":
                    case "gt":
                    case "ge":
                        return string.IsNullOrEmpty(linkEntityFieldName) ? $"{attributeName} {conditionOperator} {attributeValue}" : $"{linkEntityRelationshipName}/{attributeName} {conditionOperator} {attributeValue}";
                    case "null":
                        return string.IsNullOrEmpty(linkEntityFieldName) ? $"{attributeName} eq null" : $"{linkEntityRelationshipName}/{attributeName} eq null";
                    case "not-null":
                        return string.IsNullOrEmpty(linkEntityFieldName) ? $"{attributeName} ne null" : $"{linkEntityRelationshipName}/{attributeName} ne null";
                    case "not-like":
                        return string.IsNullOrEmpty(linkEntityFieldName) ? $"{likeFunction}({attributeName},{attributeValue})" : $"not {likeFunction}({linkEntityRelationshipName}/{attributeName},{attributeValue})";
                    case "like":
                        return string.IsNullOrEmpty(linkEntityFieldName) ? $"{likeFunction}({attributeName},{attributeValue})" : $"{likeFunction}({linkEntityRelationshipName}/{attributeName},{attributeValue})";
                    default:
                        return "";
                }
            }));
            return !string.IsNullOrEmpty(andConditions) ? $"({andConditions})" : "";
        }
        static string BuildLinkEntityJoinConditions(string mainEntity, IEnumerable<XElement> linkEntities, List<EntityMetadata> entityMetadata)
        {
            var linkPrimaryAttributeConditions = string.Join(" and ", linkEntities.Select(en =>
            {
                var linkEntityFieldName = en.Attribute(FetchAttributes.To).Value ?? "";
                var linkEntityName = en.Attribute(FetchAttributes.Name).Value ?? "";
                var linkEntityRelationshipName = (from e in entityMetadata
                                                  where e.LogicalName == linkEntityName
                                                  select new { RelationshipName = e.OneToManyRelationships.Single(x => x.ReferencingEntity == entityMetadata[0].LogicalName && x.ReferencingAttribute == linkEntityFieldName).ReferencingEntityNavigationPropertyName, PrimaryKey = e.PrimaryIdAttribute }).FirstOrDefault();
                return $"{linkEntityRelationshipName.RelationshipName}/{linkEntityRelationshipName.PrimaryKey} ne null";
            }));
            return linkPrimaryAttributeConditions;
        }
    }
}
