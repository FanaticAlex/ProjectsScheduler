using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Core.OrToolsSolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectsScheduler.Desktop.ViewModel
{
    class MainWindowViewModel : BaseVewModel
    {
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _selectedTabIndex = value; OnPropertyChanged(nameof(SelectedTabIndex)); }
        }

        public event EventHandler SolutionUpdated;

        public ICommand RunCommand { get; set; }

        public ProjectsSetViewModel ProjectsSetViewModel { get; set; }

        public ResultsViewModel ResultsViewModel { get; set; }

        public ProjectsSet ProjectsSet { get; set; }

        public Result Result { get; set; }

        public MainWindowViewModel()
        {
            RunCommand = new RelayCommand(Run);

            var json = File.ReadAllText("TestProjectsSet.json");
            ProjectsSet = JsonSerializer.Deserialize<ProjectsSet>(json);

            ProjectsSetViewModel = new ProjectsSetViewModel();
            ProjectsSetViewModel.SetProjectSet(ProjectsSet);
        }

        private void Run(object? parameter)
        {
            var solver = new ProjectSchedulerProblemSolver();
            Result = solver.Solve(ProjectsSet);
            ResultsViewModel = new ResultsViewModel();
            ResultsViewModel.SetData(Result, ProjectsSet);

            SolutionUpdated?.Invoke(this, null);
            SelectedTabIndex = 1;
        }
    }
}
