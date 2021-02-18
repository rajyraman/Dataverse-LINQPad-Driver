using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NY.Dataverse.LINQPadDriver
{
    public static class MetadataExtensions
    {
        public static (string RelationshipName, string PrimaryKey) RelationshipDetails(this List<EntityMetadata> entityMetadata, string mainEntity, string fromAttribute, string linkEntity, string toAttribute)
        {
            var linkEntityRelationshipName = (from e in entityMetadata
                                              where e.LogicalName == linkEntity
                                              select new
                                              {
                                                  RelationshipName =
                                                  e.OneToManyRelationships.SingleOrDefault(x => x.ReferencingEntity == mainEntity
                                                  && x.ReferencingAttribute == toAttribute && x.ReferencedAttribute == fromAttribute)?.ReferencingEntityNavigationPropertyName ??
                                                  e.ManyToOneRelationships.SingleOrDefault(x => x.ReferencedEntity == mainEntity
                                                  && x.ReferencedAttribute == toAttribute && x.ReferencingAttribute == fromAttribute)?.ReferencedEntityNavigationPropertyName,
                                                  PrimaryKey = e.PrimaryIdAttribute
                                              }).FirstOrDefault();
            return (linkEntityRelationshipName?.RelationshipName, linkEntityRelationshipName?.PrimaryKey) ;
        }
    }
}
