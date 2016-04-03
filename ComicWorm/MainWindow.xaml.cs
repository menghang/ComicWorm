using System;
using System.Windows;
using System.Windows.Controls;
using ComicModels;
using ComicWormCore;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ComicWorm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel view;
        private CustomDialog comicDialog;
        private DownloadTaskManager taskManager;
        private Database database;

        public MainWindow()
        {
            this.view = new MainWindowViewModel();
            this.database = new Database();
            this.view.Comics = database.DatabaseInitialize();
            Console.SetOut(new RedirectedTextWriter(this.view));
            this.DataContext = this.view;

            InitializeComponent();

            this.comicDialog = this.Resources["ComicDialog"] as CustomDialog;
        }

        private async void AddComic_Click(object sender, RoutedEventArgs e)
        {
            this.comicDialog.Title = "添加Comic";
            await this.ShowMetroDialogAsync(this.comicDialog);

            Button buttonComicDialogOK = this.comicDialog.FindChild<Button>("buttonComicDialogOK");
            Button buttonComicDialogCancel = this.comicDialog.FindChild<Button>("buttonComicDialogCancel");
            TextBox textBoxComicName = this.comicDialog.FindChild<TextBox>("textBoxComicName");
            TextBox textBoxComicUrl = this.comicDialog.FindChild<TextBox>("textBoxComicUrl");

            textBoxComicName.Text = "";
            textBoxComicUrl.Text = "";

            RoutedEventHandler addComicDialogButtonOKClick = null;
            RoutedEventHandler addComicDialogButtonCancelClick = null;
            addComicDialogButtonOKClick = async (o, args) =>
             {
                 buttonComicDialogOK.Click -= addComicDialogButtonOKClick;
                 buttonComicDialogCancel.Click -= addComicDialogButtonCancelClick;

                 ComicModel comic = new ComicModel();
                 comic.Name = textBoxComicName.Text;
                 comic.Url = textBoxComicUrl.Text;
                 comic.Selected = false;
                 if (this.view.Comics.Contains(comic))
                 {
                     this.view.Comics.Remove(comic);
                     this.database.RemoveComic(comic);
                 }

                 this.view.Comics.Add(comic);
                 this.database.AddComic(comic);

                 await this.HideMetroDialogAsync(this.comicDialog);
             };
            addComicDialogButtonCancelClick = async (o, args) =>
            {
                buttonComicDialogOK.Click -= addComicDialogButtonOKClick;
                buttonComicDialogCancel.Click -= addComicDialogButtonCancelClick;

                await this.HideMetroDialogAsync(this.comicDialog);
            };

            buttonComicDialogOK.Click += addComicDialogButtonOKClick;
            buttonComicDialogCancel.Click += addComicDialogButtonCancelClick;
        }

        private void DeleteComic_Click(object sender, RoutedEventArgs e)
        {
            ComicModel comic = (sender as DataGrid).SelectedItem as ComicModel;
            if (comic == null)
            {
                return;
            }
            this.view.Comics.Remove(comic);
            this.database.RemoveComic(comic);
        }

        private async void GetChapters_Click(object sender, RoutedEventArgs e)
        {
            this.taskManager = new DownloadTaskManager();

            foreach (ComicModel comic in this.view.Comics)
            {
                if (comic.Selected)
                {
                    DownloadModel dm = null;
                    try
                    {
                        dm = new DownloadModel(comic);
                    }
                    catch (DownloadModel.NoAnalysisModelException)
                    {
                        continue;
                    }
                    this.taskManager.AddDownloadTask(dm);
                }
            }

            this.view.Busy = true;
            await this.taskManager.StartGetChaptersTasks();
            this.view.Busy = false;
            await this.ShowMessageAsync("获取Chapter信息", "获取Chapter信息完毕", MessageDialogStyle.Affirmative);
        }

        private async void GetPages_Click(object sender, RoutedEventArgs e)
        {
            this.view.Busy = true;
            await this.taskManager.StartGetPagesTasks();
            this.view.Busy = false;
            await this.ShowMessageAsync("获取Page信息", "获取Page信息完毕", MessageDialogStyle.Affirmative);
        }

        private async void DownloadComics_Click(object sender, RoutedEventArgs e)
        {
            this.view.Busy = true;
            await this.taskManager.StartDownloadComicsTasks();
            this.view.Busy = false;
            await this.ShowMessageAsync("下载Comic", "下载Comic完毕", MessageDialogStyle.Affirmative);
        }

        private void SelectAllChapter_Click(object sender, RoutedEventArgs e)
        {
            foreach(ChapterModel chapter in this.view.Chapters)
            {
                chapter.Selected = true;
            }
        }

        private void Comics_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComicModel comic = (sender as DataGrid).SelectedItem as ComicModel;
            if (comic != null)
            {
                this.view.Chapters.Clear();
                foreach(ChapterModel chapter in comic.Chapters)
                {
                    this.view.Chapters.Add(chapter);
                }
            }
        }

        private void Chapters_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChapterModel chapter = (sender as DataGrid).SelectedItem as ChapterModel;
            if (chapter != null)
            {
                this.view.Pages.Clear();
                foreach (PageModel page in chapter.Pages)
                {
                    this.view.Pages.Add(page);
                }
            }
        }
    }
}
