using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Lab_5.Moduls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Lab_5
{
    public partial class MainWindow : Window
    {
        AppContext db = new AppContext();
        bool show = false;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(db.Songs.Local.ToObservableCollection());
            if (view == null)
            {
                MessageBox.Show("view is null");
            }
            else
            {
                view.Filter = UserFilter;
            }
        }

        // при загрузке окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            db.Database.EnsureCreated();
            // загружаем данные из БД
            db.Songs.Load();
            // и устанавливаем данные в качестве контекста
            DataContext = db.Songs.Local.ToObservableCollection();
        }

        // добавление
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UserWindow UserWindow = new UserWindow(new Song());
            if (UserWindow.ShowDialog() == true)
            {
                if (!Already_In(UserWindow.Song))
                {
                    Song Song = UserWindow.Song;
                    db.Songs.Add(Song);
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Данная композиция уже есть в списке");
                }
            }

        }
        // редактирование
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Song? user = songsList.SelectedItem as Song;
            // если ни одного объекта не выделено, выходим
            if (user is null) return;

            UserWindow UserWindow = new UserWindow(new Song
            {
                Id = user.Id,
                Singer = user.Singer,
                SongTitle = user.SongTitle
            });

            if (UserWindow.ShowDialog() == true)
            {
                // получаем измененный объект
                user = db.Songs.Find(UserWindow.Song.Id);
                if (user != null && !Already_In(UserWindow.Song))
                {
                    user.Singer = UserWindow.Song.Singer;
                    user.SongTitle = UserWindow.Song.SongTitle;
                    db.SaveChanges();
                    songsList.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Данная композиция уже есть в списке или вы ничего не ввели");
                }
            }
        }
        // удаление
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Song? user = songsList.SelectedItem as Song;
            // если ни одного объекта не выделено, выходим
            if (user is null) return;
            db.Songs.Remove(user);
            db.SaveChanges();
        }

        private void songsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            if (show == false)
            {
                filtersPanel.Visibility = Visibility.Visible;
                show = true;
            } else
            {
                filtersPanel.Visibility = Visibility.Hidden;
                show = false;
            }
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(songsList.ItemsSource).Refresh();
        }
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;

            if (Song_Check.IsChecked == true)
                return ((item as Song).SongTitle.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            return ((item as Song).Singer.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /*private void TXTFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(songsList.ItemsSource).Refresh();
        }*/

        private bool Already_In(Song item)
        {
            if (db.Songs.Where(x => (x.Singer == item.Singer && x.SongTitle == item.SongTitle)).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}