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
    /// Interaction logic for ProjectHeaderView.xaml
    /// </summary>
    public partial class ProjectHeaderView : UserControl
    {
        public ProjectHeaderView()
        {
            InitializeComponent();
        }

        public void SetProjectName(string projectName)
        {
            ProjectNameLabel.Content = projectName;
        }

        public void SetBracketsSize(int height)
        {
            CanvasBrac.Height = height;
            PathBrac.Height = height;
        }
    }
}
