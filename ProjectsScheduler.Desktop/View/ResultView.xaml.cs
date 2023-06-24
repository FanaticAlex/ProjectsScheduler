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
    /// Interaction logic for ResultControl.xaml
    /// </summary>
    public partial class ResultView : UserControl
    {
        public ResultView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            var vm = (ResultsViewModel)DataContext;

            // перестроить таблицу оси времени
            var timeMax = vm.TimeMax;
            InitGrid(TimeAxis, 1, timeMax, new GridLength(CellWidth), new GridLength(CellHeight));

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
            InitGrid(TasksTimelineLeft, tasksCount, 2, GridLength.Auto, GridLength.Auto);
            InitGrid(TasksTimeline, tasksCount, timeMax, new GridLength(CellWidth), new GridLength(CellHeight));

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
                    intervalControl.Background = new SolidColorBrush(task.Resource.ResourceColor);
                    TasksTimeline.Children.Add(intervalControl);
                    Grid.SetRow(intervalControl, nextProjectRow + j);
                    Grid.SetColumn(intervalControl, task.Start.Value);
                    Grid.SetColumnSpan(intervalControl, task.Duration);
                }

                nextProjectRow += project.Tasks.Count;
            }


            // заполнить таблицу ресурсов
            var resourcesCount = vm.Resources.Count;
            InitGrid(ResourceTimelineLeft, resourcesCount, 2, GridLength.Auto, GridLength.Auto);
            InitGrid(ResourceTimeline, resourcesCount, timeMax, new GridLength(CellWidth), new GridLength(CellHeight));

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


                // заполнить загруженность/мощьность
                for (int j = 0; j < timeMax; j++)
                {
                    var resourcePointControl = GetDefaultCellControl();
                    resourcePointControl.Content = resource.GetMaxParallelTask(j);

                    if (resource.Load.Contains(j))
                        resourcePointControl.Background = new SolidColorBrush(resource.ResourceColor);

                    ResourceTimeline.Children.Add(resourcePointControl);
                    Grid.SetRow(resourcePointControl, i);
                    Grid.SetColumn(resourcePointControl, j);
                }
            }
        }

        private void InitGrid(Grid grid, int rowsCount, int columnsCount, GridLength width, GridLength height)
        {
            grid.Children.Clear();

            grid.RowDefinitions.Clear();
            for (var j = 0; j < rowsCount; j++)
            {
                var row = new RowDefinition();
                row.Height = height;
                grid.RowDefinitions.Add(row);
            }

            grid.ColumnDefinitions.Clear();
            for (var j = 0; j < columnsCount; j++)
            {
                var col = new ColumnDefinition();
                col.Width = width;
                grid.ColumnDefinitions.Add(col);
            }
        }

        private static int CellHeight = 30;
        private static int CellWidth = 30;
        private static int FontSize = 10;

        private Label GetDefaultCellControl(int size = 1)
        {
            var control = new Label();
            control.Width = size * CellWidth;
            control.Height = CellHeight;
            control.FontSize = FontSize;
            control.VerticalContentAlignment = VerticalAlignment.Center;
            control.HorizontalContentAlignment = HorizontalAlignment.Center;
            return control;
        }

        private FrameworkElement GetProjectLegendControl(string text, int size)
        {
            var control = new ProjectHeaderView();
            control.SetProjectName(text);
            control.Height = size * CellHeight;
            //control.Width = 150;
            control.FontSize = FontSize;
            control.HorizontalAlignment = HorizontalAlignment.Right;
            control.SetBracketsSize(size * CellHeight - 5);
            return control;
        }

        private Label GetDefaultLegendControl()
        {
            var control = new Label();
            //control.Width = 100;
            control.Height = CellHeight;
            control.FontSize = FontSize;
            control.VerticalContentAlignment = VerticalAlignment.Center;
            control.HorizontalContentAlignment = HorizontalAlignment.Left;
            return control;
        }
    }
}
