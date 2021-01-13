using LINQPad.Extensibility.DataContext;
using System.Windows;

namespace NY.Dataverse.LINQPadDriver
{
	public partial class ConnectionDialog : Window
	{
		IConnectionInfo _cxInfo;

		public ConnectionDialog (IConnectionInfo cxInfo)
		{
			_cxInfo = cxInfo;

			// ConnectionProperties is your view-model.
			DataContext = new ConnectionProperties (cxInfo);

			InitializeComponent ();
		}

		void btnOK_Click (object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}