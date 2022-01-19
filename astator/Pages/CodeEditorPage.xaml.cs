using System;
using astator.Core;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace astator
{
    public partial class CodeEditorPage : ContentPage
    {
        private string path = string.Empty;
        public CodeEditorPage(string path)
        {
            this.path = path;
            InitializeComponent();

            this.Header.Text = Path.GetFileName(path);
            this.editor.Text = File.ReadAllText(path);

            if (!path.EndsWith(".cs"))
            {
                this.editor.LineNumberEnabled = false;
            }
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                var text = this.editor.GetText();
                File.WriteAllText(path, text);
                Globals.Toast("文件已保存");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Globals.Toast("文件保存失败: " + ex.ToString());
            }
        }
    }
}