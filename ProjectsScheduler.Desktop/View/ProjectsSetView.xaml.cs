using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectsScheduler.Desktop.View
{
    /// <summary>
    /// Interaction logic for ProjectsSetView.xaml
    /// </summary>
    public partial class ProjectsSetView : UserControl
    {
        public ProjectsSetView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Properties.Children.Clear();
            if (e.NewValue is Node)
            {
                var nodeOriginal = ((Node)e.NewValue).Original;
                var vm = (ProjectsSetViewModel)DataContext;
                UserControl properiesView = null;

                if (nodeOriginal is ProjectViewModel)
                    properiesView = new ProjectProperiesView();

                if (nodeOriginal is TaskViewModel)
                    properiesView = new TaskPropertiesView();

                if (nodeOriginal is ResourceViewModel)
                    properiesView = new ResourcePropertiesView();

                if (nodeOriginal is SubResourceViewModel)
                    properiesView = new SubResourcePropertiesView();

                if (properiesView == null)
                    return;

                properiesView.DataContext = nodeOriginal;
                Properties.Children.Add(properiesView);
                vm.SelectedObject = nodeOriginal;
            }
        }
    }
}
