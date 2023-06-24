﻿using ProjectsScheduler.Core.InputData;
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
                if (((Node)e.NewValue).Original is ProjectViewModel)
                {
                    var view = new ProjectProperiesView();
                    var projectVM = ((Node)e.NewValue).Original;
                    view.DataContext = projectVM;
                    Properties.Children.Add(view);

                    var vm = (ProjectsSetViewModel)DataContext;
                    vm.SelectedObject = projectVM;
                }
            }
            else
            {
                if (e.NewValue is TaskViewModel)
                {
                    var view = new TaskPropertiesView();
                    var taskVM = e.NewValue;
                    view.DataContext = taskVM;
                    Properties.Children.Add(view);

                    var vm = (ProjectsSetViewModel)DataContext;
                    vm.SelectedObject = taskVM;
                }

                if (e.NewValue is ResourceViewModel)
                {
                    var view = new ResourcePropertiesView();
                    var resourceVM = e.NewValue;
                    view.DataContext = resourceVM;
                    Properties.Children.Add(view);

                    var vm = (ProjectsSetViewModel)DataContext;
                    vm.SelectedObject = resourceVM;
                }
            }
        }
    }
}