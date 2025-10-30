using System;
using System.Linq;
using System.Windows;

namespace LayoutValueClickCopy.Services
{
    public static class LocalizationService
    {
        private const string DictionaryPrefix = "Resources/Strings.";

        public static void SetLanguage(string culture)
        {
            if (Application.Current?.Resources == null) return;
            var md = Application.Current.Resources.MergedDictionaries;

            // 既存の文字列辞書を除去
            var toRemove = md.Where(d => d.Source != null && d.Source.OriginalString.StartsWith("Resources/Strings.")).ToList();
            foreach (var d in toRemove)
                md.Remove(d);

            var path = culture switch
            {
                "en" => "Resources/Strings.en.xaml",
                _ => "Resources/Strings.ja.xaml"
            };

            md.Add(new ResourceDictionary { Source = new Uri(path, UriKind.Relative) });
        }

        public static string GetString(string key)
        {
            if (Application.Current?.Resources.Contains(key) == true)
            {
                var val = Application.Current.Resources[key]?.ToString();
                if (!string.IsNullOrEmpty(val)) return val!;
            }
            return key; // フォールバック
        }
    }
}
