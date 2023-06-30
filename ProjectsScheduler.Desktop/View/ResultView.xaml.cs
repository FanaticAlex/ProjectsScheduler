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

            // хэдеры
            InitGrid(HeaderTasks, 1, 2, new GridLength(1, GridUnitType.Star), GridLength.Auto);
            var progectsHeader = GetDefaultLegendControl("Проекты");
            HeaderTasks.Children.Add(progectsHeader);
            Grid.SetRow(progectsHeader, 0);
            Grid.SetColumn(progectsHeader, 0);
            var tasksHeader = GetDefaultLegendControl("Таски");
            HeaderTasks.Children.Add(tasksHeader);
            Grid.SetRow(tasksHeader, 0);
            Grid.SetColumn(tasksHeader, 1);

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
                    // название
                    var task = project.Tasks[j];
                    var taskControl = GetDefaultLegendControl(task.Name);
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

                    // дэдлайн
                    if (project.Deadline != null)
                    {
                        var deadlineControl = GetDeadlineControl();
                        TasksTimeline.Children.Add(deadlineControl);
                        Grid.SetRow(deadlineControl, nextProjectRow + j);
                        Grid.SetColumn(deadlineControl, project.Deadline.Value);
                    }
                }

                nextProjectRow += project.Tasks.Count;
            }


            // заполнить таблицу ресурсов
            var resourcesCount = vm.Resources.SelectMany(r => r.SubResources).Count();
            InitGrid(ResourceTimelineLeft, resourcesCount, 2, GridLength.Auto, GridLength.Auto); // названия строк
            InitGrid(ResourceTimeline, resourcesCount, timeMax, new GridLength(CellWidth), new GridLength(CellHeight)); // значения

            var nextResourceRow = 0;
            for (int i = 0; i < vm.Resources.Count; i++)
            {
                // названия ресурсов
                var resource = vm.Resources[i];
                var headerControl = GetProjectLegendControl(resource.Name, resource.SubResources.Count);
                ResourceTimelineLeft.Children.Add(headerControl);
                Grid.SetRow(headerControl, nextResourceRow);
                Grid.SetColumn(headerControl, 0);
                Grid.SetRowSpan(headerControl, resource.SubResources.Count);

                for (int j = 0; j < resource.Resource.SubResources.Count; j++)
                {
                    var subResource = resource.SubResources[j];

                    // Название субресурса
                    var subResourceNameControl = GetDefaultLegendControl(subResource.Name);
                    ResourceTimelineLeft.Children.Add(subResourceNameControl);
                    Grid.SetRow(subResourceNameControl, nextResourceRow + j);
                    Grid.SetColumn(subResourceNameControl, 1);

                    // отпуска
                    for (int v = 0; v < subResource.SubResource.Vacations.Count(); v++)
                    {
                        var vacation = subResource.SubResource.Vacations[v];
                        var vacationControl = GetVacationControl();
                        ResourceTimeline.Children.Add(vacationControl);
                        Grid.SetRow(vacationControl, nextResourceRow + j);
                        Grid.SetColumn(vacationControl, vacation - 1);
                    }

                    // загруженность ресурса
                    for (int k = 0; k < timeMax; k++)
                    {
                        var resourcePointControl = GetDefaultCellControl();
                        if (subResource.Load.Contains(k))
                            resourcePointControl.Background = new SolidColorBrush(resource.ResourceColor);

                        ResourceTimeline.Children.Add(resourcePointControl);
                        Grid.SetRow(resourcePointControl, nextResourceRow + j);
                        Grid.SetColumn(resourcePointControl, k);
                    }
                }

                nextResourceRow += resource.SubResources.Count;
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

        private Image GetDeadlineControl()
        {
            var control = new Image();
            control.Source = new BitmapImage(new Uri("/Resources/DeadlineImage.png", UriKind.Relative));
            control.Width = CellWidth;
            control.Height = CellHeight;
            return control;
        }

        private Image GetVacationControl()
        {
            var control = new Image();
            control.Source = new BitmapImage(new Uri("/Resources/VacationImage.png", UriKind.Relative));
            control.Width = CellWidth;
            control.Height = CellHeight;
            return control;
        }

        private FrameworkElement GetProjectLegendControl(string text, int size)
        {
            var control = new ProjectHeaderView();
            control.SetProjectName(text);
            control.Height = size * CellHeight;
            control.FontSize = FontSize;
            control.HorizontalAlignment = HorizontalAlignment.Right;
            control.SetBracketsSize(size * CellHeight - 5);
            return control;
        }

        private Label GetDefaultLegendControl(string text)
        {
            var control = new Label();
            control.Content = text;
            //control.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(100,100,100));
            control.Height = CellHeight;
            control.FontSize = FontSize;
            control.HorizontalAlignment= HorizontalAlignment.Center;
            control.VerticalContentAlignment = VerticalAlignment.Center;
            control.HorizontalContentAlignment = HorizontalAlignment.Center;
            return control;
        }
    }
}
