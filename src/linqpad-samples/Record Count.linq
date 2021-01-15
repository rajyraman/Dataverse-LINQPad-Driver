<Query Kind="Statements">
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
  <Namespace>Microsoft.Crm.Sdk.Messages</Namespace>
</Query>

var entities = (from e in DataverseClient.GetAllEntityMetadata()
where e.IsCustomizable.Value == true &&
e.DataProviderId == null &&
e.LogicalName != LINQPad.User.Entities.UserQuery.EntityLogicalName &&
e.LogicalName != LINQPad.User.Entities.UserQueryVisualization.EntityLogicalName
select e.LogicalName).ToArray();
((RetrieveTotalRecordCountResponse)this.DataverseClient.Execute(
new RetrieveTotalRecordCountRequest {
EntityNames = entities
})).EntityRecordCountCollection.OrderByDescending(x => x.Value).Dump("Record Count");