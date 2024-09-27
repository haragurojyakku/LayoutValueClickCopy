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
        private int initialVerticalResolution = 1080;
        private int initialHorizontalResolution = 1920;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "LayoutValueClickCopy"; // Windowの名前を変更
            CreateButtons();
        }

        // クリップボードにコピーするメソッド（軽量化）
        private void CopyToClipboard(string text)
        {
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

        private void CreateButtons()
        {
            // ボタンパネルをクリアして、再計算用にリセット
            HorizontalResolutionPanel.Children.Clear();
            VerticalResolutionPanel.Children.Clear();
            WidthDecrementPanel.Children.Clear();
            HeightDecrementPanel.Children.Clear();
            MultipleOfTwoPanel.Children.Clear();

            // 各列にタイトルを追加
            AddTitle(HorizontalResolutionPanel, "Width");
            AddTitle(VerticalResolutionPanel, "Height");
            AddTitle(WidthDecrementPanel, "Width Decrement");
            AddTitle(HeightDecrementPanel, "Height Decrement");
            AddTitle(MultipleOfTwoPanel, "Multiple of Two");

            // 再生成するボタン群を生成
            CreateGoldenRatioResolutionButtons(VerticalResolutionPanel, HorizontalResolutionPanel);
            CreateMultipleOfTwoButtons(MultipleOfTwoPanel);
        }

        // タイトル用のTextBlockを追加するメソッド
        private void AddTitle(Panel panel, string title)
        {
            TextBlock titleBlock = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            panel.Children.Add(titleBlock);
        }


        // 再計算ボタンのクリックイベントハンドラ
        private void RecalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // ユーザー入力のスペーシング値を取得
            if (int.TryParse(SpacingInput.Text, out int spacingValue))
            {
                // 縦と横の最大解像度からスペーシング値を引く
                initialVerticalResolution = Math.Max(1, 1080 - spacingValue);
                initialHorizontalResolution = Math.Max(1, 1920 - spacingValue);

                // 新しい値に基づいてボタンを再生成
                CreateButtons();
            }
            else
            {
                // 無効な入力の場合のメッセージ
                StatusMessage.Text = "Invalid spacing value";
                StatusMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        // 黄金比に基づく解像度を計算し、ボタンを生成
        private void CreateGoldenRatioResolutionButtons(Panel verticalPanel, Panel horizontalPanel)
        {
            int verticalResolution = initialVerticalResolution;
            int horizontalResolution = initialHorizontalResolution;
            int previousVerticalResolution = initialVerticalResolution;
            int previousHorizontalResolution = initialHorizontalResolution;

            // 減少値列に0を追加（最初の減少値は0）
            Button initialVerticalDecrementButton = new Button
            {
                Content = "0",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5),
                FontSize = 16
            };
            Button initialHorizontalDecrementButton = new Button
            {
                Content = "0",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5),
                FontSize = 16
            };

            initialVerticalDecrementButton.Click += (sender, e) => CopyToClipboard("0");
            initialHorizontalDecrementButton.Click += (sender, e) => CopyToClipboard("0");

            HeightDecrementPanel.Children.Add(initialVerticalDecrementButton);
            WidthDecrementPanel.Children.Add(initialHorizontalDecrementButton);

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
                    FontSize = 16
                };
                Button horizontalButton = new Button
                {
                    Content = $"{capturedHorizontalResolution}",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontSize = 16
                };

                verticalButton.Click += (sender, e) => CopyToClipboard(capturedVerticalResolution.ToString());
                horizontalButton.Click += (sender, e) => CopyToClipboard(capturedHorizontalResolution.ToString());

                verticalPanel.Children.Add(verticalButton);
                horizontalPanel.Children.Add(horizontalButton);

                // 減少値を計算
                if (previousVerticalResolution != capturedVerticalResolution)
                {
                    int verticalDecrement = previousVerticalResolution - capturedVerticalResolution;
                    Button verticalDecrementButton = new Button
                    {
                        Content = $"-{verticalDecrement}",
                        Width = 100,
                        Height = 30,
                        Margin = new Thickness(5),
                        FontSize = 16
                    };
                    verticalDecrementButton.Click += (sender, e) => CopyToClipboard(verticalDecrement.ToString());
                    HeightDecrementPanel.Children.Add(verticalDecrementButton);
                }

                if (previousHorizontalResolution != capturedHorizontalResolution)
                {
                    int horizontalDecrement = previousHorizontalResolution - capturedHorizontalResolution;
                    Button horizontalDecrementButton = new Button
                    {
                        Content = $"-{horizontalDecrement}",
                        Width = 100,
                        Height = 30,
                        Margin = new Thickness(5),
                        FontSize = 16
                    };
                    horizontalDecrementButton.Click += (sender, e) => CopyToClipboard(horizontalDecrement.ToString());
                    WidthDecrementPanel.Children.Add(horizontalDecrementButton);
                }

                // 黄金比で次の解像度を計算（小数点以下は切り捨て）
                previousVerticalResolution = capturedVerticalResolution;
                previousHorizontalResolution = capturedHorizontalResolution;
                verticalResolution = (int)(verticalResolution / GoldenRatio);
                horizontalResolution = (int)(horizontalResolution / GoldenRatio);
            }
        }

        // 2の倍数に基づく解像度を計算し、ボタンを生成
        private void CreateMultipleOfTwoButtons(Panel targetPanel)
        {
            int resolution = 4096; // 初期解像度

            while (resolution >= 1)
            {
                int capturedResolution = resolution; // クリック時の値をキャプチャ

                Button button = new Button
                {
                    Content = $"{capturedResolution}",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontSize = 16
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

