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