using LayoutValueClickCopy.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LayoutValueClickCopy.Services;

namespace LayoutValueClickCopy.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private const double GoldenRatio = 1.618;
        private const int DefaultVertical = 1080;
        private const int DefaultHorizontal = 1920;

        private string _spacingText = string.Empty;
        private string _statusMessage = string.Empty;
        private Brush _statusBrush = Brushes.Gray;
    private int _selectedTabIndex;
    private string _selectedLanguage = "ja";

    public ObservableCollection<int> WidthValues { get; } = new();
    public ObservableCollection<int> HeightValues { get; } = new();
    public ObservableCollection<int> WidthDecrements { get; } = new();
    public ObservableCollection<int> HeightDecrements { get; } = new();
    public ObservableCollection<int> MultipleOfTwoValues { get; } = new();
    public ObservableCollection<ApproxValue> WidthApproxValues { get; } = new();
    public ObservableCollection<ApproxValue> HeightApproxValues { get; } = new();
        public IReadOnlyList<string> DisplaySets { get; } = new[] { "黄金比", "近似", "2の累乗" };
        public IReadOnlyList<LangOption> Languages { get; } = new[]
        {
            new LangOption("ja", "日本語"),
            new LangOption("en", "English"),
        };

        public string SpacingText
        {
            get => _spacingText;
            set { if (_spacingText != value) { _spacingText = value; OnPropertyChanged(); } }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set { if (_statusMessage != value) { _statusMessage = value; OnPropertyChanged(); } }
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            private set { if (_statusBrush != value) { _statusBrush = value; OnPropertyChanged(); } }
        }

        // タブ切替（0: 黄金比, 1: 近似, 2: 2の累乗）
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { if (_selectedTabIndex != value) { _selectedTabIndex = value; OnPropertyChanged(); SaveSettings(); } }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value && !string.IsNullOrWhiteSpace(value))
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                    LocalizationService.SetLanguage(_selectedLanguage);
                    // 表示を即時反映するため、ステータス文言はリフレッシュされるタイミングで適用
                    SaveSettings();
                }
            }
        }

        public ICommand RecalculateCommand { get; }
        public ICommand CopyValueCommand { get; }

        public MainWindowViewModel()
        {
            RecalculateCommand = new RelayCommand(_ => Recalculate());
            CopyValueCommand = new RelayCommand(CopyValue);

            // 設定読み込み
            var s = SettingsService.Load();
            _selectedLanguage = string.IsNullOrWhiteSpace(s.SelectedLanguage) ? "ja" : s.SelectedLanguage;
            LocalizationService.SetLanguage(_selectedLanguage);

            _selectedTabIndex = s.SelectedTabIndex;
            _spacingText = s.SpacingText ?? string.Empty;

            // 初期描画
            int spacing = 0;
            int.TryParse(_spacingText, out spacing);
            BuildValues(spacing);
            OnPropertyChanged(nameof(SpacingText));
            OnPropertyChanged(nameof(SelectedTabIndex));
            OnPropertyChanged(nameof(SelectedLanguage));
        }

        private void Recalculate()
        {
            if (int.TryParse(SpacingText, out var spacing) && spacing >= 0)
            {
                BuildValues(spacing);
                SaveSettings();
            }
            else
            {
                StatusMessage = LocalizationService.GetString("Msg_InvalidSpacing");
                StatusBrush = Brushes.Red;
            }
        }

        private void CopyValue(object? parameter)
        {
            try
            {
                var text = parameter switch
                {
                    null => string.Empty,
                    string s => s,
                    int i => i.ToString(),
                    _ => parameter.ToString() ?? string.Empty
                };

                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text);
                    var fmt = LocalizationService.GetString("Msg_Copied");
                    StatusMessage = string.Format(fmt, text);
                    StatusBrush = Brushes.Green;
                }
            }
            catch
            {
                // クリップボード例外は無視（他アプリがロック等）
            }
        }

        private void BuildValues(int spacing)
        {
            WidthValues.Clear();
            HeightValues.Clear();
            WidthDecrements.Clear();
            HeightDecrements.Clear();
            MultipleOfTwoValues.Clear();
            WidthApproxValues.Clear();
            HeightApproxValues.Clear();

            var initialVertical = Math.Max(1, DefaultVertical - spacing);
            var initialHorizontal = Math.Max(1, DefaultHorizontal - spacing);

            // 減少値の先頭は0
            HeightDecrements.Add(0);
            WidthDecrements.Add(0);

            int v = initialVertical;
            int h = initialHorizontal;
            int prevV = initialVertical;
            int prevH = initialHorizontal;

            while (v > 1 && h > 1)
            {
                WidthValues.Add(h);
                HeightValues.Add(v);

                // 近似（二進スケールの加算で10%以内・最大3要素）
                var wApprox = ApproximateBinarySum(h);
                if (wApprox != null)
                    WidthApproxValues.Add(wApprox);

                var vApprox = ApproximateBinarySum(v);
                if (vApprox != null)
                    HeightApproxValues.Add(vApprox);

                if (prevV != v)
                {
                    HeightDecrements.Add(prevV - v);
                }

                if (prevH != h)
                {
                    WidthDecrements.Add(prevH - h);
                }

                prevV = v;
                prevH = h;
                v = (int)(v / GoldenRatio);
                h = (int)(h / GoldenRatio);
            }

            int t = 4096;
            while (t >= 1)
            {
                MultipleOfTwoValues.Add(t);
                t /= 2;
            }
        }

        // 10%以内、最大3要素、2の累乗の和での近似を求める
        private ApproxValue? ApproximateBinarySum(int target)
        {
            if (target <= 0) return null;

            // 候補の2の累乗（1～4096）
            var powers = new List<int>();
            for (int p = 1; p <= 4096; p <<= 1)
                powers.Add(p);

            double maxError = target * 0.10; // 10%

            ApproxValue? best = null;
            int bestAbsErr = int.MaxValue;
            int bestCount = int.MaxValue;
            bool bestIsOver = false;

            // 単体（上下2段の条件は単体なら常に満たす）
            foreach (var a in powers)
            {
                int sum = a;
                int err = Math.Abs(sum - target);
                bool within = err <= maxError;
                if (!within) continue;
                UpdateBest(sum, new[] { a });
            }

            // 2要素
            for (int i = 0; i < powers.Count; i++)
            {
                for (int j = i + 1; j < powers.Count; j++)
                {
                    int a = powers[i], b = powers[j];
                    // 上下2段の制約
                    int L = Math.Max(a, b);
                    int minAllowed = Math.Max(1, L / 4);
                    int maxAllowed = L * 4;
                    if (a < minAllowed || a > maxAllowed || b < minAllowed || b > maxAllowed)
                        continue;

                    int sum = a + b;
                    int err = Math.Abs(sum - target);
                    if (err <= maxError)
                        UpdateBest(sum, new[] { a, b });
                }
            }

            // 3要素
            for (int i = 0; i < powers.Count; i++)
            {
                for (int j = i + 1; j < powers.Count; j++)
                {
                    for (int k = j + 1; k < powers.Count; k++)
                    {
                        int a = powers[i], b = powers[j], c = powers[k];
                        // 上下2段の制約
                        int L = Math.Max(a, Math.Max(b, c));
                        int minAllowed = Math.Max(1, L / 4);
                        int maxAllowed = L * 4;
                        if (a < minAllowed || a > maxAllowed ||
                            b < minAllowed || b > maxAllowed ||
                            c < minAllowed || c > maxAllowed)
                            continue;

                        int sum = a + b + c;
                        int err = Math.Abs(sum - target);
                        if (err <= maxError)
                            UpdateBest(sum, new[] { a, b, c });
                    }
                }
            }

            return best;

            void UpdateBest(int sum, int[] parts)
            {
                int err = Math.Abs(sum - target);
                int count = parts.Length;
                bool isOver = sum > target;

                // 優先順位: 誤差が小さい -> 要素数が少ない -> アンダー優先
                if (err < bestAbsErr ||
                    (err == bestAbsErr && count < bestCount) ||
                    (err == bestAbsErr && count == bestCount && bestIsOver && !isOver))
                {
                    bestAbsErr = err;
                    bestCount = count;
                    bestIsOver = isOver;
                    string display = parts.Length == 1
                        ? $"{sum} ({parts[0]})"
                        : $"{sum} ({string.Join("+", parts.OrderByDescending(x => x))})";
                    best = new ApproxValue(sum, display);
                }
            }
        }

        public sealed class ApproxValue
        {
            public int Value { get; }
            public string Display { get; }
            public ApproxValue(int value, string display)
            {
                Value = value;
                Display = display;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SaveSettings()
        {
            SettingsService.Save(new Models.AppSettings
            {
                SpacingText = this.SpacingText,
                SelectedTabIndex = this.SelectedTabIndex,
                SelectedLanguage = this.SelectedLanguage
            });
        }

        public sealed record LangOption(string Code, string Display);
    }
}
