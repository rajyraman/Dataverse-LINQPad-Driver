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
      <AuthenticationType>OAuth</AuthenticationType>
      <EnvironmentUrl>https://instance.crm.dynamics.com</EnvironmentUrl>
      <ApplicationId>51f81489-12ee-4a9e-aaae-a2591f45987d</ApplicationId>
      <UserName></UserName>
      <ConnectionName>Dataverse</ConnectionName>
    </DriverData>
</Query>

(from s in SavedQuery
 where s.QueryType == 0 && s.ReturnedTypeCode == AccountTable.EntityLogicalName
 orderby s.CreatedOn
 select new { EntityName = s.ReturnedTypeCode, CreatedBy = s.CreatedBy.Name, CreatedOn = s.CreatedOn }).Take(1)