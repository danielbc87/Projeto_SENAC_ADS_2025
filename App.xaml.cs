namespace Health_Organizer
{
    public partial class App : Application
    {
        public static Database Database { get; private set; }

        public App()
        {
            InitializeComponent();
            Database = new Database();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}