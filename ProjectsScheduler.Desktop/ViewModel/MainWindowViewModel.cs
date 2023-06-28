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
        public event EventHandler SolutionUpdated;

        public ICommand RunCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddProjectCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand RemoveTaskCommand { get; set; }
        public ICommand AddResourceCommand { get; set; }

        public ProjectsSetViewModel ProjectsSetViewModel { get; set; }

        public ResultsViewModel ResultsViewModel { get; set; }

        public ProjectsSet ProjectsSet { get; set; }

        public Result Result { get; set; }

        public MainWindowViewModel()
        {
            RunCommand = new RelayCommand(Run);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            AddProjectCommand = new RelayCommand(AddProject);
            AddTaskCommand = new RelayCommand(AddTask, CanExecuteAddTaskCommand);
            RemoveTaskCommand = new RelayCommand(RemoveTask, CanExecuteRemoveTaskCommand);
            AddResourceCommand = new RelayCommand(AddResource);

            ProjectsSetViewModel = new ProjectsSetViewModel();
            ResultsViewModel = new ResultsViewModel();

            Load("TestProjectsSet.json");
        }

        private void Run(object? parameter)
        {
            try
            {
                var solver = new ProjectSchedulerProblemSolver();
                Result = solver.Solve(ProjectsSet);
                switch (Result.Status)
                {
                    case Status.Optimal:
                        {
                            break;
                        }
                    case Status.Stopped:
                        {
                            MessageBox.Show("Процесс решения был остановлен из за длительного выполнения. Решение может быть не оптимальным.");
                            break;
                        }
                    case Status.Unknown:
                        {
                            MessageBox.Show("Процесс решения был остановлен из за длительного выполнения. Решение может быть не валидным.");
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Решение не найдено.");
                            return;
                        }
                }

                ResultsViewModel.SetData(Result, ProjectsSet);
                SolutionUpdated?.Invoke(this, null);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void Save(object? parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "NewConfiguration";
            saveFileDialog.Filter = "Json (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                var json = JsonSerializer.Serialize(ProjectsSet);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void AddProject(object? parameter)
        {
            var newName = "Новый проект";
            while (ProjectsSet.ProjectList.Select(p => p.Name).Contains(newName))
            {
                newName = "Новый_" + newName;
            }

            var newProject = new Project(newName);
            ProjectsSet.ProjectList.Add(newProject);
            ProjectsSetViewModel.SetProjectSet(ProjectsSet);
        }

        private void AddTask(object? parameter)
        {
            var projectVM = (ProjectsSetViewModel.SelectedObject as ProjectViewModel);
            var project = ProjectsSet.ProjectList.Single(p => p.Name == projectVM.Name);
            var defaultResource = ProjectsSet.Resources.FirstOrDefault();
            var newTask = new ProjectTask(1,defaultResource.Name);
            project.Tasks.Add(newTask);
            ProjectsSetViewModel.SetProjectSet(ProjectsSet);
        }

        private bool CanExecuteAddTaskCommand(object? parameter)
        {
            return (ProjectsSetViewModel.SelectedObject is ProjectViewModel);
        }

        private void RemoveTask(object? parameter)
        {
            var taskVM = (ProjectsSetViewModel.SelectedObject as TaskViewModel);
            foreach(var project in ProjectsSet.ProjectList)
            {
                foreach(var task in project.Tasks.ToList())
                {
                    if (task.ID == taskVM.ProjectTask.ID)
                    {
                        var taskToRemove = task;
                        project.Tasks.Remove(taskToRemove);
                        ProjectsSetViewModel.SetProjectSet(ProjectsSet);
                        return;
                    }
                }
            }
        }

        private bool CanExecuteRemoveTaskCommand(object? parameter)
        {
            return (ProjectsSetViewModel.SelectedObject is TaskViewModel);
        }

        private void AddResource(object? parameter)
        {
            var newName = "Новый ресурс";
            while (ProjectsSet.Resources.Select(r => r.Name).Contains(newName))
            {
                newName = "Новый_" + newName;
            }

            var newResource = new ProjectResource() { Name = newName, MaxParallelTasks = 1 };
            ProjectsSet.Resources.Add(newResource);
            ProjectsSetViewModel.SetProjectSet(ProjectsSet);
        }
    }
}
