<Query Kind="Expression">
  <Connection>
    <ID>43a4e350-a857-4c1a-a516-57605953ef5d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="1f402b3ef4c25058">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <DisplayName>Dataverse Connection</DisplayName>
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
  <Namespace>Microsoft.PowerPlatform.Cds.Client</Namespace>
  <Namespace>Microsoft.Xrm.Sdk.Metadata</Namespace>
</Query>

(from s in SavedQuery
where s.QueryType == 0 && s.ReturnedTypeCode == LINQPad.User.Entities.Account.EntityLogicalName
orderby s.CreatedOn
select new { EntityName = s.ReturnedTypeCode, CreatedBy = s.CreatedBy.Name, CreatedOn = s.CreatedOn  }).Take(1)