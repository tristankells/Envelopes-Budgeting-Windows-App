using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Envelopes.Common;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes
{
    public interface IMainWindowViewModel : IViewModel
    {
        public FrameworkElement CurrentPage { get; set; }
        public ICommand NavigateToTransactionsPageCommand { get; set; }
        public ICommand NavigateToBudgetPageCommand { get; set; }
    }
    public class MainWindowViewModel : NotifyPropertyChanged, IMainWindowViewModel
    {
        #region Fields
        private FrameworkElement currentPage;


        #endregion

        public ICommand NavigateToTransactionsPageCommand { get; set; }
        public ICommand NavigateToBudgetPageCommand { get; set; }

        public FrameworkElement CurrentPage {
            get => currentPage;
            set => SetPropertyValue(ref currentPage, value, nameof(CurrentPage));
        }
    }
}
