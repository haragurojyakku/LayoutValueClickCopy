using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GoldenRatioApp
{
    public partial class MainWindow : Window
    {
        // 黄金比の定義
        private const double GoldenRatio = 1.618;

        // 基準となる解像度（例えばフルHD）
        private readonly int initialVerticalResolution = 1080;
        private readonly int initialHorizontalResolution = 1920;

        public MainWindow()
        {
            InitializeComponent();
            CreateButtons();
        }

        // クリップボードにコピーするメソッド（軽量化）
        private void CopyToClipboard(string text)
        {
            // コピー成功のメッセージを表示
            StatusMessage.Text = $"Copied to clipboard: {text}";
            StatusMessage.Foreground = new SolidColorBrush(Colors.Green); // 成功時の色
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Clipboard.SetText(text);
                });
            }
            catch
            {
                // エラー処理をスキップ
            }
        }

        // ボタンを動的に作成するメソッド（黄金比と2の倍数に基づく解像度）
        private void CreateButtons()
        {
            // 黄金比で縮小した解像度ボタンを生成
            CreateGoldenRatioResolutionButtons(VerticalResolutionPanel, HorizontalResolutionPanel);

            // 2の倍数で縮小した解像度ボタンを生成
            CreateMultipleOfTwoButtons(MultipleOfTwoPanel);
        }

        // 黄金比に基づく解像度を計算し、ボタンを生成
        private void CreateGoldenRatioResolutionButtons(Panel verticalPanel, Panel horizontalPanel)
        {
            int verticalResolution = initialVerticalResolution;
            int horizontalResolution = initialHorizontalResolution;

            while (verticalResolution > 1 && horizontalResolution > 1)
            {
                // ローカル変数を使って値をキャプチャ
                int capturedVerticalResolution = verticalResolution;
                int capturedHorizontalResolution = horizontalResolution;

                // ボタンを作成してパネルに追加（FontSizeを大きく設定）
                Button verticalButton = new Button
                {
                    Content = $"{capturedVerticalResolution}",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontSize = 16 // フォントサイズを大きく設定
                };
                Button horizontalButton = new Button
                {
                    Content = $"{capturedHorizontalResolution}",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontSize = 16 // フォントサイズを大きく設定
                };

                // クリック時にクリップボードにコピー
                verticalButton.Click += (sender, e) => CopyToClipboard(capturedVerticalResolution.ToString());
                horizontalButton.Click += (sender, e) => CopyToClipboard(capturedHorizontalResolution.ToString());

                // ボタンをパネルに追加
                verticalPanel.Children.Add(verticalButton);
                horizontalPanel.Children.Add(horizontalButton);

                // 黄金比で次の解像度を計算（小数点以下は切り捨て）
                verticalResolution = (int)(verticalResolution / GoldenRatio);
                horizontalResolution = (int)(horizontalResolution / GoldenRatio);
            }
        }

        // 2の倍数に基づく解像度を計算し、ボタンを生成
        private void CreateMultipleOfTwoButtons(Panel targetPanel)
        {
            int resolution = 1024; // 初期解像度

            while (resolution >= 1)
            {
                int capturedResolution = resolution; // クリック時の値をキャプチャ

                Button button = new Button
                {
                    Content = $"{capturedResolution}",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontSize = 16 // フォントサイズを大きく設定
                };

                // クリック時にクリップボードにコピー
                button.Click += (sender, e) => CopyToClipboard(capturedResolution.ToString());

                // ボタンをパネルに追加
                targetPanel.Children.Add(button);

                // 解像度を2で割る
                resolution /= 2;
            }
        }
    }
}
