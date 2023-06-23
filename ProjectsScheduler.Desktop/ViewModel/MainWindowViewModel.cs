using Microsoft.Win32;
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
using System.Windows;
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
        public ICommand LoadCommand { get; set; }

        public ProjectsSetViewModel ProjectsSetViewModel { get; set; }

        public ResultsViewModel ResultsViewModel { get; set; }

        public ProjectsSet ProjectsSet { get; set; }

        public Result Result { get; set; }

        public MainWindowViewModel()
        {
            RunCommand = new RelayCommand(Run);
            LoadCommand = new RelayCommand(Load);

            ProjectsSetViewModel = new ProjectsSetViewModel();
            ResultsViewModel = new ResultsViewModel();

            Load("TestProjectsSet.json");
        }

        private void Run(object? parameter)
        {
            var solver = new ProjectSchedulerProblemSolver();
            Result = solver.Solve(ProjectsSet);
            ResultsViewModel.SetData(Result, ProjectsSet);
            SolutionUpdated?.Invoke(this, null);
            SelectedTabIndex = 1;
        }

        private void Load(object? parameter)
        {
            var filename = (string)parameter;
            if (parameter == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Json files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    filename = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                var json = File.ReadAllText(filename);
                ProjectsSet = JsonSerializer.Deserialize<ProjectsSet>(json);
                ProjectsSetViewModel.SetProjectSet(ProjectsSet);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Загрузка файла не удалась, возможно файл имеет неверный формат.");
            }
        }
    }
}
