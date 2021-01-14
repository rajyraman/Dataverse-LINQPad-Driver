using LINQPad.Extensibility.DataContext;
using Microsoft.PowerPlatform.Cds.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NY.Dataverse.LINQPadDriver
{
	/// <summary>
	/// Wrapper to read/write connection properties. This acts as our ViewModel - we will bind to it in ConnectionDialog.xaml.
	/// </summary>
	class ConnectionProperties
	{
		public IConnectionInfo ConnectionInfo { get; private set; }

		XElement DriverData => ConnectionInfo.DriverData;

		public ConnectionProperties (IConnectionInfo cxInfo)
		{
			ConnectionInfo = cxInfo;
		}

		public string ConnectionString
		{
			get
			{
				switch (AuthenticationType)
				{
					case AuthenticationType.ClientSecret:
						return $"AuthType=ClientSecret; Url={EnvironmentUrl}; ClientId={ApplicationId}; ClientSecret={ClientSecret}";
					case AuthenticationType.Certificate:
						return $"AuthType=Certificate; Url={EnvironmentUrl}; ClientId={ApplicationId}; Thumbprint={CertificateThumbprint}";
					default:
						return "";
				}
			}
		}

		public string ApplicationId
		{
			get => (string)DriverData.Element("ApplicationId") ?? "";
			set => DriverData.SetElementValue("ApplicationId", value);
		}

		public string ClientSecret
		{
			get 
			{ 
				var clientSecret = (string)DriverData.Element("ClientSecret");
				return string.IsNullOrEmpty(clientSecret) ? "" : ConnectionInfo.Decrypt(clientSecret);
			}
			set => DriverData.SetElementValue("ClientSecret", ConnectionInfo.Encrypt(value));
		}
		public string CertificateThumbprint
		{
			get
			{
				var thumbprint = (string)DriverData.Element("CertificateThumbprint");
				return string.IsNullOrEmpty(thumbprint) ? "" : ConnectionInfo.Decrypt(thumbprint);
			}
			set => DriverData.SetElementValue("CertificateThumbprint", ConnectionInfo.Encrypt(value));
		}
		public string EnvironmentUrl
		{
			get => (string)DriverData.Element("EnvironmentUrl") ?? "";
			set => DriverData.SetElementValue("EnvironmentUrl", value);
		}

		public AuthenticationType AuthenticationType
		{
			get => (AuthenticationType?)(int?)DriverData.Element("AuthenticationType") ?? AuthenticationType.ClientSecret;
			set => DriverData.SetElementValue("AuthenticationType", (int)value);
		}
		public string ConnectionName
		{
			get => (string)DriverData.Element("ConnectionName") ?? "";
			set => DriverData.SetElementValue("ConnectionName", value);
		}

		public CdsServiceClient GetCdsClient()
		{
			return new CdsServiceClient(this.ConnectionString);
		}
	}
}