using System.Windows;

namespace Plugin_DbRestorerConfig.Plugin_ExecutionOrder
{
    /// <summary>
    /// Interaction logic for RenameExecutionOrderView.xaml
    /// </summary>
    public partial class ExecutionOrderNameView 
    {
        public ExecutionOrderNameView(string originalName)
        {
            InitializeComponent();
            txtName.Text = originalName;
            Loaded += (sender, args) => txtName.Focus();
        }

        public string NewName { get; private set; }

        private void BtnOkClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                return;
            }
            NewName = txtName.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
