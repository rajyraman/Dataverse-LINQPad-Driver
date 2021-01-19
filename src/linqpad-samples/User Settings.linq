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
  </Connection>
</Query>

(from u in UserSettings
join s in SystemUser on u.SystemUserId.Value equals s.Id
join t in TimeZoneDefinition on u.TimeZoneCode equals t.TimeZoneCode
select new
{
	u.Id,
	s.FullName,
	u.DataValidationModeForExportToExcel,
	u.DateFormatString,
	u.TimeFormatString,
	u.HomepageArea,
	u.HomepageLayout,
	u.HomepageSubarea,
	u.PagingLimit,
	t.StandardName
}).ToList().OrderBy(x => x.FullName)