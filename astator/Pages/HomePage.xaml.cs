using Android.Views;
using astator.Views;

namespace astator.Pages
{
    public partial class HomePage : ContentPage
    {
        private string rootDir = string.Empty;

        public HomePage()
        {
            InitializeComponent();

            try
            {
                this.rootDir = Android.OS.Environment.GetExternalStoragePublicDirectory("astator").ToString();

                if (!Directory.Exists(this.rootDir))
                {
                    Directory.CreateDirectory(this.rootDir);
                }

                var scriptDir = Path.Combine(this.rootDir, "脚本");

                if (!Directory.Exists(scriptDir))
                {
                    Directory.CreateDirectory(scriptDir);
                }

                UpdateDirTbs(scriptDir);
            }
            catch { }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainActivity.Instance.OnKeyDownCallback = OnKeyDown;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainActivity.Instance.OnKeyDownCallback = null;
        }

        private void ShowFiles(string directory)
        {
            this.FilesLayout.Children.Clear();
            var dirs = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var name = Path.GetFileName(dir);
                var info = $"{new DirectoryInfo(dir).LastWriteTime:yyyy/MM/dd HH:mm}";

                var card = new CustomPathCard
                {
                    Tag = dir,
                    PathName = name,
                    PathInfo = info,
                    TypeImageSource = "dir.png",
                };

                card.Clicked += Dir_Clicked;

                this.FilesLayout.Children.Add(card);
            }

            var files = Directory.EnumerateFiles(directory, "", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var info = $"{new DirectoryInfo(file).LastWriteTime:yyyy/MM/dd HH:mm}";
                var icon = file.EndsWith(".cs") ? "_script" : file.EndsWith(".csproj") ? "_csproj" : file.EndsWith("xml") ? "_xml" : string.Empty;

                var card = new CustomPathCard
                {
                    Tag = file,
                    PathName = name,
                    PathInfo = info,
                    TypeImageSource = $"file{icon}.png",
                };
                this.FilesLayout.Children.Add(card);
            }
        }

        private void Dir_Clicked(object sender, EventArgs e)
        {
            var card = sender as CustomPathCard;
            var directory = card.Tag as string;
            UpdateDirTbs(directory);
        }

        private void DirTb_Clicked(object sender, EventArgs e)
        {
            var dir = sender as CustomLabelButton;
            UpdateDirTbs(dir.Tag as string);
        }


        private void UpdateDirTbs(string dir)
        {
            this.DirTbLayout.Clear();
            var dirs = Path.GetRelativePath(this.rootDir, dir).Split(Path.DirectorySeparatorChar);
            for (var i = Math.Max(0, dirs.Length - 3); i < dirs.Length; i++)
            {
                var tb = new CustomLabelButton
                {
                    Text = dirs[i],
                    TextColor = Color.Parse("#888888"),
                    Tag = Path.Combine(this.rootDir, string.Join('/', dirs[0..(i + 1)])),
                };

                tb.Clicked += DirTb_Clicked;
                this.DirTbLayout.Add(tb);

                var img = new Image
                {
                    HeightRequest = 18,
                    WidthRequest = 18,
                    Margin = 0,
                    VerticalOptions = LayoutOptions.Center,
                    Source = ImageSource.FromFile("right.png")
                };

                this.DirTbLayout.Add(img);

                if (i == dirs.Length - 1)
                {
                    tb.TextColor = Color.Parse("#4a4a4d");
                    ShowFiles(tb.Tag as string);
                }
            }
        }

        private bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                var count = this.DirTbLayout.Children.Count / 2;
                if (count > 1)
                {
                    UpdateDirTbs((this.DirTbLayout.Children[(count - 2) * 2] as CustomLabelButton).Tag as string);
                    return true;
                }
            }
            return false;
        }

    }
}