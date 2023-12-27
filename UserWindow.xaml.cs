using Lab_5.Moduls;
using System.Windows;

namespace Lab_5
{

    public partial class UserWindow : Window
    {
        public Song Song { get; private set; }
        public UserWindow(Song song)
        {
            InitializeComponent();
            Song = song;
            DataContext = Song;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
