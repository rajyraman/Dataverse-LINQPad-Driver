<Query Kind="Expression">
  <Connection>
    <ID>43a4e350-a857-4c1a-a516-57605953ef5d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="NY.Dataverse.LINQPadDriver" PublicKeyToken="1f402b3ef4c25058">NY.Dataverse.LINQPadDriver.DynamicDriver</Driver>
    <DisplayName>Dataverse Connection</DisplayName>
    <DriverData>
      <EnvironmentUrl>https://crm.crm6.dynamics.com</EnvironmentUrl>
      <ApplicationId></ApplicationId>
      <ClientSecret></ClientSecret>
      <ConnectionName>Dataverse</ConnectionName>
    </DriverData>
  </Connection>
  <Namespace>Microsoft.PowerPlatform.Cds.Client</Namespace>
  <Namespace>Microsoft.Xrm.Sdk.Metadata</Namespace>
</Query>

(from m in SdkMessageProcessingStep
join f in SdkMessageFilter on m.SdkMessageFilterId.Id equals f.SdkMessageFilterId
join s in SdkMessage on f.SdkMessageId.Id equals s.SdkMessageId
join p in PluginType on m.PluginTypeId.Id equals p.PluginTypeId
where f.IsCustomProcessingStepAllowed
&& !m.IsHidden
&& m.CustomizationLevel == 1
select new { EntityName = f.PrimaryObjectTypeCode, Message = s.Name, Rank = m.Rank, Stage = m.Stage, p.AssemblyName, PluginName = p.Name, StepName = m.Name, StepDescription = m.Description, m.StatusCode, m.FilteringAttributes }).ToList().OrderBy(x => x.EntityName).ThenBy(x => x.PluginName)