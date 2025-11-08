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
                    return;
                }

                // ファイルが無い場合は、WPF リソース（pack URI）からの読み込みを試す
                try
                {
                    var packPng = new Uri("pack://application:,,,/Assets/app.png", UriKind.Absolute);
                    this.Icon = BitmapFrame.Create(packPng);
                    return;
                }
                catch
                {
                    // リソース未埋め込み等で失敗した場合は握りつぶし
                }
            }
            catch
            {
                // 失敗時はデフォルトアイコンのまま
            }
        }
    }
}

