using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Envelopes.Models;

namespace Envelopes.Common {
    public interface IItemsViewModelBase<T> : IViewModelBase {
        public T SelectedItem { get; set; }
        public void AddItem(T item);
        public bool RemoveItem(T item);
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ObservableCollection<T> ItemList { get; }
    }


    public class ItemsViewModelBase<T> : NotifyPropertyChanged, IItemsViewModelBase<T> {
        private event PropertyChangedEventHandler ItemPropertyChanged;
        private event PropertyChangedEventHandler SelectedItemChanged;
        private event NotifyCollectionChangedEventHandler ItemsCollectionsChanged;


        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }

        private T selectedItem;

        public T SelectedItem {
            get => selectedItem;
            set { SetPropertyValue(ref selectedItem, value, nameof(SelectedItem)); }
        }

        private ObservableCollection<T> itemsList;

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

        public void AddItem(T item) {
            if (!(item is Model modelItem)) return;
            modelItem.PropertyChanged += OnItemPropertyChanged;
            ItemList.Add(item);
        }

        public bool RemoveItem(T item) {
            if (!(item is Model modelItem)) return false;
            modelItem.PropertyChanged -= OnItemPropertyChanged;
            return ItemList.Remove(item);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            ItemPropertyChanged?.Invoke(sender, e);
        }

        private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            ItemsCollectionsChanged?.Invoke(sender, e);
        }
    }
}