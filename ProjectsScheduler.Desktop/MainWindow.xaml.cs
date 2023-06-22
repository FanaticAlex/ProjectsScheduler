using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Core.OrToolsSolver;
using ProjectsScheduler.Desktop.View;
using ProjectsScheduler.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace ProjectsScheduler.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = DataContext as MainWindowViewModel;
            vm.SolutionUpdated += Vm1_SolutionUpdated;
            ProjectsSetView.DataContext = vm.ProjectsSetViewModel;
        }

        private void Vm1_SolutionUpdated(object? sender, EventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            ResultView.DataContext = vm.ResultsViewModel;
            ResultView.Update();
        }
    }
}
