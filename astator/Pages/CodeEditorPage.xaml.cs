using astator.Core.Script;

namespace astator
{
    public partial class CodeEditorPage : ContentPage
    {
        private readonly string path = string.Empty;
        public CodeEditorPage(string path)
        {
            this.path = path;
            InitializeComponent();

            this.Header.Text = Path.GetFileName(path);
            this.editor.Text = File.ReadAllText(path);

            if (!path.EndsWith(".cs"))
            {
                this.editor.LineNumberEnabled = false;
                this.editor.Padding = new Thickness(10, 0, 10, 0);
            }
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                var text = this.editor.GetText();
                File.WriteAllText(this.path, text);
                Globals.Toast("文件已保存");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Globals.Toast("文件保存失败: " + ex.ToString());
            }
        }
    }
}