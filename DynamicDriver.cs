using LINQPad;
using LINQPad.Extensibility.DataContext;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace NY.CommonDataService.LINQPadDriver
{
	public class DynamicDriver : DynamicDataContextDriver
	{
		static DynamicDriver _driverInstance;
		static CdsServiceClient _cdsClient;
		static XElement _fetchXml;

#if DEBUG
		static DynamicDriver()
		{
			// Uncomment the following code to attach to Visual Studio's debugger when an exception is thrown:
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
			{
				if (args.Exception.StackTrace.Contains("NY.CommonDataService.LINQPadDriver"))
					Debugger.Launch();
			};
		}
#endif   
		public override string Name => "Dataverse LINQPad Driver";

		public override string Author => "Natraj Yegnaraman";
		public override string GetConnectionDescription(IConnectionInfo connectionInfo) => new ConnectionProperties(connectionInfo).EnvironmentUrl;

		public override bool ShowConnectionDialog (IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
			=> new ConnectionDialog (cxInfo).ShowDialog() == true;
		public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
		{
			return new string[]
				{
					"Microsoft.PowerPlatform.Cds.Client.dll",
					"Microsoft.Cds.Sdk.dll",
					"Microsoft.Cds.Sdk.Proxy.dll"
				};
		}

		public override List<ExplorerItem> GetSchemaAndBuildAssembly (
			IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
		{
#if DEBUG
			Debugger.Launch();
#endif
			var connectionProperties = new ConnectionProperties(cxInfo);
			List<ExplorerItem> explorerItems = new List<ExplorerItem>();
			var client = new CdsServiceClient(connectionProperties.ConnectionString);
			if (client.IsReady)
			{
				var entityMetadata = client.GetAllEntityMetadata(filter: EntityFilters.Attributes | EntityFilters.Entity | EntityFilters.Relationships).Where(x => x.IsPrivate == false).OrderBy(x=>x.LogicalName).ToList();
				var code = new CDSTemplate(entityMetadata) { Namespace = nameSpace, TypeName = typeName }.TransformText();
#if DEBUG
				File.WriteAllText(Path.Combine(GetContentFolder(), "LINQPad.EarlyBound.cs"), code);
#endif
				Compile(code, assemblyToBuild.CodeBase, cxInfo);
				foreach (var entity in entityMetadata)
				{
					var attributes = entity.Attributes
					.Where(x => x.IsLogical == false && x.AttributeType != AttributeTypeCode.Virtual && x.AttributeType != AttributeTypeCode.CalendarRules)
					.OrderBy(x => x.LogicalName)
					.Select(a => new ExplorerItem($"{a.SchemaName} ({a.AttributeType})", ExplorerItemKind.Parameter, ExplorerIcon.Column)
					{
						Icon = a.IsPrimaryId == true ? ExplorerIcon.Key : ExplorerIcon.Column,
						Tag = a.LogicalName
					}).ToList();
					ExplorerItem item = new ExplorerItem(entity.SchemaName, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						Children = attributes,
						Tag = entity.LogicalName
					};
					explorerItems.Add(item);
				}

				foreach (var entity in entityMetadata)
				{
					var source = explorerItems.FirstOrDefault(e => e.Kind == ExplorerItemKind.QueryableObject && (string)e.Tag == entity.LogicalName);
					foreach (var oneToMany in entity.OneToManyRelationships)
					{
						var target = explorerItems.FirstOrDefault(e => e.Kind == ExplorerItemKind.QueryableObject && (string)e.Tag == oneToMany.ReferencingEntity);
						if (target != null)
						{
							source?.Children.Add(new ExplorerItem(oneToMany.SchemaName, ExplorerItemKind.CollectionLink, ExplorerIcon.OneToMany)
							{
								HyperlinkTarget = target,
								ToolTipText = oneToMany.ReferencingAttribute
							});
						}
					}

					foreach (var manyToOne in entity.ManyToOneRelationships)
					{
						var targetEntity = explorerItems.FirstOrDefault(e => e.Kind == ExplorerItemKind.QueryableObject && (string)e.Tag == manyToOne.ReferencedEntity);
						var targetAttribute = targetEntity?.Children.FirstOrDefault(e => e.Kind == ExplorerItemKind.Parameter && (string)e.Tag == manyToOne.ReferencedAttribute);
						if (targetAttribute != null)
						{
							source?.Children.Add(new ExplorerItem(manyToOne.SchemaName, ExplorerItemKind.ReferenceLink, ExplorerIcon.ManyToOne)
							{
								HyperlinkTarget = targetAttribute,
								ToolTipText = manyToOne.ReferencingAttribute
							});
						}
					}
				}
			}
			return explorerItems;
		}

		public override void InitializeContext(IConnectionInfo cxInfo, object context,
												QueryExecutionManager executionManager)
		{
			_driverInstance = this;
			base.InitializeContext(cxInfo, context, executionManager);
		}

		public override bool AreRepositoriesEquivalent(IConnectionInfo r1, IConnectionInfo r2)
		{
			return Equals(r1.DriverData.Element("EnvironmentUrl"), r2.DriverData.Element("EnvironmentUrl"));
		}

		public override object[] GetContextConstructorArguments(IConnectionInfo cxInfo)
		{
			var connectionProperties = new ConnectionProperties(cxInfo);
			var cdsServiceClient = _cdsClient ?? connectionProperties.GetCdsClient();
			if (_cdsClient == null || !_cdsClient.Equals(cdsServiceClient))
			{
				_cdsClient = cdsServiceClient;
			}
			return new object[]
			{
				cdsServiceClient
			};
		}
		public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo connectionInfo) => new[]
		{
			new ParameterDescriptor("cdsServiceClient", typeof(CdsServiceClient).FullName)
		};

		static void Compile(string cSharpSourceCode, string outputFile, IConnectionInfo cxInfo)
		{
			var customAssemblies = new[]{
				typeof(CdsServiceClient).Assembly.Location,
				typeof(EntityReference).Assembly.Location,
				typeof(AddAppComponentsRequest).Assembly.Location,
			};
			var assembliesToReference = GetCoreFxReferenceAssemblies().Concat(customAssemblies);
			// CompileSource is a static helper method to compile C# source code using LINQPad's built-in Roslyn libraries.
			// If you prefer, you can add a NuGet reference to the Roslyn libraries and use them directly.
			var compileResult = CompileSource(new CompilationInput
			{
				FilePathsToReference = assembliesToReference.ToArray(),
				OutputPath = outputFile,
				SourceCode = new[] { cSharpSourceCode }
			});
			if (compileResult.Errors.Length > 0)
				throw new Exception("Cannot compile typed context: " + compileResult.Errors[0]);
		}

		//public override void PreprocessObjectToWrite(ref object objectToWrite, ObjectGraphInfo info)
		//{
		//	if(objectToWrite is IQueryable)
		//	{
		//		var linqQuery = (IQueryable)objectToWrite;
		//		var query = (QueryExpression)(linqQuery).Provider.GetType().InvokeMember("GetQueryExpression", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, linqQuery.Provider, new List<object> { linqQuery.Expression, false, false, null, null, null }.ToArray());
		//		var expressionToFetchXmlRequest = new QueryExpressionToFetchXmlRequest
		//		{
		//			Query = query
		//		};

		//		var organizationResponse = (QueryExpressionToFetchXmlResponse)_cdsClient.Execute(expressionToFetchXmlRequest);
		//		_fetchXml = XElement.Parse(organizationResponse.FetchXml);
		//	}
		//}
	}
}