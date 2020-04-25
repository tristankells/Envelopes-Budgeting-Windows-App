﻿using System.Reflection;
using System.Windows;
using Envelopes.Data;
using Ninject;

namespace Envelopes {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            var kernel = new StandardKernel();
            new Bootstrapper(kernel).Load();
            kernel.Load(Assembly.GetExecutingAssembly());
            var mainWindowPresenter = kernel.Get<IMainWindowPresenter>();
            mainWindowPresenter.MainWindow.Show();
        }
    }
}