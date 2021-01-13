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

from a in this.DataverseClient.GetAllAttributesForEntity(LINQPad.User.Entities.Account.EntityLogicalName)
where a.RequiredLevel.Value != AttributeRequiredLevel.SystemRequired
orderby a.LogicalName
select new {a.LogicalName, a.SchemaName, a.AttributeType}