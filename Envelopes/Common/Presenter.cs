namespace Envelopes.Common {
    /// <summary>
    ///     Simple base class for presenters that connects the DataContext of a View object to a IViewModelBase replacing three
    ///     lines with a call to base class constructor.
    /// </summary>
    public abstract class Presenter {
        protected Presenter(IView view, IViewModelBase viewModel) {
            View = view;
            ViewModel = viewModel;
            View.DataContext = ViewModel;
        }

        private IView View { get; }
        private IViewModelBase ViewModel { get; }
    }
}