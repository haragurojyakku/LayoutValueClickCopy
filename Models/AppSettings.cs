namespace LayoutValueClickCopy.Models
{
    public sealed class AppSettings
    {
        public string? SpacingText { get; set; }
        public int SelectedTabIndex { get; set; } = 0;
        public string SelectedLanguage { get; set; } = "ja";
    }
}
