<Query Kind="Expression">
  <Connection>
    <ID>43a4e350-a857-4c1a-a516-57605953ef5d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="no-strong-name">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <DisplayName>Dataverse Connection</DisplayName>
    <DriverData>
      <EnvironmentUrl>https://environment.crm.dynamics.com</EnvironmentUrl>
      <ApplicationId></ApplicationId>
      <ClientSecret></ClientSecret>
    </DriverData>
  </Connection>
  <Namespace>Microsoft.PowerPlatform.Cds.Client</Namespace>
  <Namespace>Microsoft.Xrm.Sdk.Metadata</Namespace>
</Query>

(from c in CustomControl
join cr in CustomControlResource on c.CustomControlId.Value equals cr.CustomControlId
join s in Solution on c.SolutionId equals s.Id
//join w in WebResource on cr.WebResourceId.Value equals w.Id
orderby c.CreatedOn descending
select new
{
c.Name,
c.CompatibleDataTypes,
c.ComponentState,
c.Version,
SolutionName = s.FriendlyName,
Manifest = XElement.Parse(c.Manifest)
//WebResourceName = w.Name,
//Content = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(w.Content))
}).Take(10)