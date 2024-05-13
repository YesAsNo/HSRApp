using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HSRApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            GetItemsAsync();
        }
        
        public ObservableCollection<ImageFileInfo> Images { get; } = new ObservableCollection<ImageFileInfo>();

        public async Task GetItemsAsync()
        {
            StorageFolder MainFolder = Package.Current.InstalledLocation;
            StorageFolder ImgFolder = await MainFolder.GetFolderAsync("Assets\\Samples");
            var result = ImgFolder.CreateFileQueryWithOptions(new QueryOptions());
            IReadOnlyList<StorageFile> ImgExamples = await result.GetFilesAsync();

            foreach (StorageFile file in ImgExamples) {
                Images.Add(await LoadImageInfoAsync(file));
                    }
        }

        public async static Task<ImageFileInfo> LoadImageInfoAsync(StorageFile file)
        {
            var properties = await file.Properties.GetImagePropertiesAsync();
            ImageFileInfo info = new(properties, file, file.DisplayName, file.DisplayType);
            return info;
        }

        private void ImageGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs e) {
            if (e.InRecycleQueue)
            {
                var templateRoute = e.ItemContainer.ContentTemplateRoot as Grid;
                var image = templateRoute.FindName("ItemImage") as Image;
                image.Source = null;
            }

            if (e.Phase == 0)
            {
                e.RegisterUpdateCallback(ShowImage);
                e.Handled = true;
            }
        }

        private async void ShowImage(ListViewBase sender, ContainerContentChangingEventArgs e)
        {
            if (e.Phase == 1)
            {
                var templateRoute = e.ItemContainer.ContentTemplateRoot as Grid;
                var image = templateRoute.FindName("ItemImage") as Image;
                var item = e.Item as ImageFileInfo;
                image.Source = await item.GetImageThumbnailAsync();
            }
        }
    }
}
