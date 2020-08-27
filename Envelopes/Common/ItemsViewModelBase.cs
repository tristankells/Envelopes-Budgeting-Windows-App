using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace Envelopes.Common {
    /// <summary>
    ///     Base class for our view models storing a collection of one type of item. Grids, list of items etc...
    /// </summary>
    public interface IItemsViewModelBase<T> : IViewModelBase {
        /// <summary>
        ///     Collection of items for the View to bind to.
        /// </summary>
        public ObservableCollection<T> ItemList { get; }

        /// <summary>
        ///     The currently selected item. Used to determine which item to delete.
        /// </summary>
        public T SelectedItem { get; set; }

        /// <summary>
        ///     The add item command. Bind to the "Add" function of an user control.
        /// </summary>
        public ICommand AddItemCommand { get; set; }

        /// <summary>
        ///     The remove item command. Bind to the "Remove" function of an user control.
        /// </summary>
        public ICommand DeleteItemCommand { get; set; }

        /// <summary>
        ///     Add an item to the view model's collection.
        /// </summary>
        public void AddItem(T item);

        /// <summary>
        ///     Add an item to the view model's collection.
        /// </summary>
        public void AddRange(IEnumerable<T> items);

        /// <summary>
        ///     Remove an item from the view model's collection.
        /// </summary>
        public bool RemoveItem(T item);
    }

    /// <summary>
    ///     Base class for our ViewModels storing a collection of one type of item. Grids, list of items etc...
    /// </summary>
    public abstract class ItemsViewModelBase<T> : NotifyPropertyChanged, IItemsViewModelBase<T> {
        #region Fields

        private T selectedItem;
        private ObservableCollection<T> itemsList;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }

        public T SelectedItem {
            get => selectedItem;
            set => SetPropertyValue(ref selectedItem, value, nameof(SelectedItem));
        }

        public ObservableCollection<T> ItemList {
            get => itemsList ??= ItemList = new ObservableCollection<T>();

            private set {
                if (itemsList != null) {
                    itemsList.CollectionChanged -= OnItemCollectionChanged;
                }

                itemsList = value;
                itemsList.CollectionChanged += OnItemCollectionChanged;

                OnPropertyChanged(nameof(ItemList));
            }
        }

        #endregion

        #region Abstract/Virtual Methods

        /// <summary>
        ///     A virtual class, override to handle PropertyChanged event from items in the ViewModel collection.
        /// </summary>
        protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
        }

        /// <summary>
        ///     A virtual class, override to handle CollectionChanged fired by the ViewModel collection.
        /// </summary>
        protected virtual void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        }

        #endregion

        #region Methods

        public void AddItem(T item) {
            if (!(item is Model modelItem)) return;
            modelItem.PropertyChanged += OnItemPropertyChanged;
            ItemList.Add(item);
        }

        public void AddRange(IEnumerable<T> items) {
            foreach (T item in items) {
                AddItem(item);
            }
        }

        public bool RemoveItem(T item) {
            if (!(item is Model modelItem)) return false;
            modelItem.PropertyChanged -= OnItemPropertyChanged;
            return ItemList.Remove(item);
        }

        #endregion
    }
}