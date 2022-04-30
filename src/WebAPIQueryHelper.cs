using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using LINQPad;
using System.Diagnostics;
using MarkMpn.FetchXmlToWebAPI;

namespace NY.Dataverse.LINQPadDriver
{
    public class WebAPIQueryHelper
    {
        public static string GetWebApiUrl(ServiceClient DataverseClient, string query)
        {
            var url = $"{DataverseClient.ConnectedOrgPublishedEndpoints[EndpointType.WebApplication]}api/data/v{DataverseClient.ConnectedOrgVersion.Major}.{DataverseClient.ConnectedOrgVersion.Minor}";
            var fetchXml = XElement.Parse(query);
            var entityElement = fetchXml.Element(FetchAttributes.Entity);
            var entityMetadata = new List<EntityMetadata>();
            var mainEntity = DataverseClient.GetEntityMetadata(entityElement.Attribute(FetchAttributes.Name).Value, EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships);
            entityMetadata.Add(mainEntity);

            if(fetchXml.Descendants(FetchAttributes.LinkEntity).Any())
                entityMetadata.AddRange(fetchXml.Descendants(FetchAttributes.LinkEntity).Select(x => DataverseClient.GetEntityMetadata(x.Attribute(FetchAttributes.Name).Value, EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships)).ToList());

            var converter = new FetchXmlToWebAPIConverter(new LINQPadMetadataProvider(entityMetadata), url);
            var webApiUrl = converter.ConvertFetchXmlToWebAPI(query);
            return webApiUrl;
        }
    }
}
