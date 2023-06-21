using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ProjectsScheduler.Desktop.View
{
    /// <summary>
    /// Interaction logic for TasksSchedulingResultControl.xaml
    /// </summary>
    public partial class TasksSchedulingResultView : UserControl
    {
        public TasksSchedulingResultView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            var vm = (TasksSchedulingResultsViewModel)DataContext;

            // перестроить таблицу оси времени
            var timeMax = vm.TimeMax;
            SetGridSize(TimeAxis, 1, timeMax);

            for (var j = 0; j < timeMax; j++)
            {
                var timePointControl = GetDefaultCellControl();
                timePointControl.Content = j + 1;
                TimeAxis.Children.Add(timePointControl);
                Grid.SetRow(timePointControl, 0);
                Grid.SetColumn(timePointControl, j);
            }


            // перестроить таблицу тасков
            var tasksCount = vm.Projects.SelectMany(p => p.Tasks).Count();
            SetGridSize(TasksTimelineLeft, tasksCount, 2);
            SetGridSize(TasksTimeline, tasksCount, timeMax);

            // заполнить таблицу тасков
            var nextProjectRow = 0;
            for (int i = 0; i < vm.Projects.Count; i++)
            {
                // названия проектов
                var project = vm.Projects[i];
                var projectControl = GetProjectLegendControl(project.Name, project.Tasks.Count);
                TasksTimelineLeft.Children.Add(projectControl);
                Grid.SetRow(projectControl, nextProjectRow);
                Grid.SetColumn(projectControl, 0);
                Grid.SetRowSpan(projectControl, project.Tasks.Count);

                // таски
                for (int j = 0; j < project.Tasks.Count; j++)
                {
                    var task = project.Tasks[j];
                    var taskControl = GetDefaultLegendControl();
                    taskControl.Content = task.Name;
                    TasksTimelineLeft.Children.Add(taskControl);
                    Grid.SetRow(taskControl, nextProjectRow + j);
                    Grid.SetColumn(taskControl, 1);

                    // отрезок
                    var intervalControl = GetDefaultCellControl(task.Duration);
                    intervalControl.Background = new SolidColorBrush(task.ResourceColor);
                    TasksTimeline.Children.Add(intervalControl);
                    Grid.SetRow(intervalControl, nextProjectRow + j);
                    Grid.SetColumn(intervalControl, task.Start);
                    Grid.SetColumnSpan(intervalControl, task.Duration);
                }

                nextProjectRow += project.Tasks.Count;
            }


            // заполнить таблицу ресурсов
            var resourcesCount = vm.Resources.Count;
            SetGridSize(ResourceTimelineLeft, resourcesCount, 2);
            SetGridSize(ResourceTimeline, resourcesCount, timeMax);

            // header
            var headerControl = GetProjectLegendControl("Загруженность", vm.Resources.Count);
            ResourceTimelineLeft.Children.Add(headerControl);
            Grid.SetRow(headerControl, 0);
            Grid.SetColumn(headerControl, 0);
            Grid.SetRowSpan(headerControl, vm.Resources.Count);

            for (int i = 0; i < vm.Resources.Count; i++)
            {
                var resource = vm.Resources[i];

                var resourceControl = GetDefaultLegendControl();
                resourceControl.Content = resource.Name;
                ResourceTimelineLeft.Children.Add(resourceControl);
                Grid.SetRow(resourceControl, i);
                Grid.SetColumn(resourceControl, 1);


                // заполнить load
                for (int j = 0; j < resource.Load.Count; j++)
                {
                    var point = resource.Load[j];

                    var resourcePointControl = GetDefaultCellControl();
                    resourcePointControl.Background = new SolidColorBrush(resource.ResourceColor);
                    ResourceTimeline.Children.Add(resourcePointControl);
                    Grid.SetRow(resourcePointControl, i);
                    Grid.SetColumn(resourcePointControl, point);
                }
            }
        }

        private void SetGridSize(Grid grid, int rowsCount, int columnsCount)
        {
            grid.RowDefinitions.Clear();
            for (var j = 0; j < rowsCount; j++)
            {
                var row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid.RowDefinitions.Add(row);
            }

            grid.ColumnDefinitions.Clear();
            for (var j = 0; j < columnsCount; j++)
            {
                var col = new ColumnDefinition();
                col.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(col);
            }
        }

        private static int CellHeight = 40;

        private Label GetDefaultCellControl(int size = 1)
        {
            var control = new Label();
            control.Width = size * 40;
            control.Height = CellHeight;
            control.VerticalContentAlignment = VerticalAlignment.Center;
            control.HorizontalContentAlignment = HorizontalAlignment.Center;
            return control;
        }

        private FrameworkElement GetProjectLegendControl(string text, int size)
        {
            var control = new ProjectHeaderView();
            control.SetProjectName(text);
            control.Height = size * CellHeight;
            control.Width = 150;
            control.HorizontalAlignment = HorizontalAlignment.Right;
            control.SetBracketsSize(size * CellHeight - 5);
            return control;
        }

        private Label GetDefaultLegendControl()
        {
            var control = new Label();
            control.Width = 100;
            control.Height = CellHeight;
            control.VerticalContentAlignment = VerticalAlignment.Center;
            control.HorizontalContentAlignment = HorizontalAlignment.Left;
            return control;
        }
    }
}
