<Query Kind="Statements">
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
  <Namespace>Microsoft.Crm.Sdk.Messages</Namespace>
</Query>

var entities = (from e in DataverseClient.GetAllEntityMetadata()
where e.IsCustomizable.Value == true &&
e.DataProviderId == null &&
e.LogicalName != LINQPad.User.Entities.UserQuery.EntityLogicalName &&
e.LogicalName != LINQPad.User.Entities.UserQueryVisualization.EntityLogicalName
select e.LogicalName).ToArray();
((Microsoft.Crm.Sdk.Messages.RetrieveTotalRecordCountResponse)this.DataverseClient.Execute(
new Microsoft.Crm.Sdk.Messages.RetrieveTotalRecordCountRequest
{
EntityNames = entities
})).EntityRecordCountCollection.Chart(x => x.Key, x => x.Value, LINQPad.Util.SeriesType.Pie).Dump("Record Count")