using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;

namespace JitDumpAnalyser
{
    internal class WinUIFileSelector : IFileSelector
    {
        private readonly Window window;

        public WinUIFileSelector(Window window)
        {
            this.window = window;
        }

        public async Task<string> SelectFileAsync()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
            };

            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".log");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return null;
            }

            return file.Path;
        }
    }
}
