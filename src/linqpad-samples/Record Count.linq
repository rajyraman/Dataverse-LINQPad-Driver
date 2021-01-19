<Query Kind="Statements">
  <Connection>
    <ID>45c2b9d0-74b3-44f5-a479-d9004e2fe864</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="1f402b3ef4c25058">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <Persist>true</Persist>
    <DriverData>
      <CertificateThumbprint></CertificateThumbprint>
      <ClientSecret></ClientSecret>
      <AuthenticationType>OAuth</AuthenticationType>
      <EnvironmentUrl>https://instance.crm.dynamics.com</EnvironmentUrl>
      <ApplicationId>51f81489-12ee-4a9e-aaae-a2591f45987d</ApplicationId>
      <UserName></UserName>
      <ConnectionName>Dataverse</ConnectionName>
    </DriverData>
  </Connection>
  <Namespace>Microsoft.Crm.Sdk.Messages</Namespace>
</Query>

var entities = (from e in DataverseClient.GetAllEntityMetadata()
where e.IsCustomizable.Value == true &&
e.DataProviderId == null &&
e.LogicalName != UserQueryTable.EntityLogicalName &&
e.LogicalName != UserQueryVisualizationTable.EntityLogicalName
select e.LogicalName).ToArray();
((RetrieveTotalRecordCountResponse)this.DataverseClient.Execute(
new RetrieveTotalRecordCountRequest
{
EntityNames = entities
})).EntityRecordCountCollection.Chart(x => x.Key, x => x.Value, LINQPad.Util.SeriesType.Pie).Dump("Record Count");