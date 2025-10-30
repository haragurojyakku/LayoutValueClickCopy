using System.Windows;

using System.IO;
using System.Windows.Media.Imaging;

namespace LayoutValueClickCopy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "LayoutValueClickCopy";
            TrySetWindowIcon();
        }

        private void TrySetWindowIcon()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var icoPath = Path.Combine(baseDir, "Assets", "app.ico");
                var pngPath = Path.Combine(baseDir, "Assets", "app.png");

                string? path = null;
                if (File.Exists(icoPath)) path = icoPath;
                else if (File.Exists(pngPath)) path = pngPath;

                if (path != null)
                {
                    this.Icon = BitmapFrame.Create(new Uri(path, UriKind.Absolute));
                }
            }
            catch
            {
                // 失敗時はデフォルトアイコンのまま
            }
        }
    }
}

