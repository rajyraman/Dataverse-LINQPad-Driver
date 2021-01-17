<Query Kind="Expression">
  <Connection>
    <ID>45c2b9d0-74b3-44f5-a479-d9004e2fe864</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="1f402b3ef4c25058">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <Persist>true</Persist>
    <DriverData>
      <CertificateThumbprint></CertificateThumbprint>
      <ClientSecret></ClientSecret>
      <AuthenticationType></AuthenticationType>
      <EnvironmentUrl>https://instance.crm.dynamics.com</EnvironmentUrl>
      <ApplicationId></ApplicationId>
      <UserName></UserName>
      <ConnectionName>Dataverse</ConnectionName>
    </DriverData>
  </Connection>
  <Namespace>Microsoft.Xrm.Sdk.Metadata</Namespace>
</Query>

from a in this.DataverseClient.GetAllAttributesForEntity(LINQPad.User.Entities.Account.EntityLogicalName)
where a.RequiredLevel.Value != Microsoft.Xrm.Sdk.Metadata.AttributeRequiredLevel.SystemRequired
orderby a.LogicalName
select new { a.LogicalName, a.SchemaName, a.AttributeType }