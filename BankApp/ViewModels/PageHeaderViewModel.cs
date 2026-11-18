namespace BankApp.ViewModels
{
    // ViewModel for the shared welcome card 
    public class PageHeaderViewModel
    {
        public string Kicker { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public bool ShowDate { get; set; }
        public string ActionPage { get; set; } = "";
        public string ActionText { get; set; } = "";
        public string ActionIcon { get; set; } = "";

        public bool Compact { get; set; } = true;
    }
}