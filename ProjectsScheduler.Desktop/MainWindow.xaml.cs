using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Core.OrToolsSolver;
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

            var json = File.ReadAllText("TestProjectsSet.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(json);

            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            var vm = (TasksSchedulingResultsViewModel)ResultView.DataContext;
            vm.SetData(result, projectSet);
            ResultView.Update();
            ResultView.UpdateLayout();
        }
    }
}
