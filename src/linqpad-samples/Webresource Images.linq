<Query Kind="Expression">
  <Connection>
    <ID>45c2b9d0-74b3-44f5-a479-d9004e2fe864</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="1f402b3ef4c25058">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <Persist>true</Persist>
    <DriverData>
      <EnvironmentUrl>https://crm.crm6.dynamics.com</EnvironmentUrl>
      <ApplicationId></ApplicationId>
      <ClientSecret></ClientSecret>
      <ConnectionName>Dataverse</ConnectionName>
    </DriverData>
  </Connection>
  <Namespace>Microsoft.Xrm.Sdk.Discovery</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
</Query>

(from w in WebResource
where w.WebResourceType == WebResource_WebResourceType.JPG_format ||
w.WebResourceType == WebResource_WebResourceType.ICO_format ||
w.WebResourceType == WebResource_WebResourceType.GIF_format ||
w.WebResourceType == WebResource_WebResourceType.PNG_format ||
w.WebResourceType == WebResource_WebResourceType.Vector_format_SVG
select new
{
Name = new Hyperlinq($"{DataverseClient.ConnectedOrgPublishedEndpoints[Microsoft.Xrm.Sdk.Discovery.EndpointType.WebApplication]}WebResources/{w.Name}", w.Name),
Image = w.WebResourceType == WebResource_WebResourceType.Vector_format_SVG ? new LINQPad.Controls.Svg(Encoding.ASCII.GetString(Convert.FromBase64String(w.Content)), 100, 100, null) : Util.Image(Convert.FromBase64String(w.Content))
}).Take(100)