using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpfPTf
{

    public partial class MainWindow : Window
    {
        private List<string> _resultlist = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            btnProcess.IsEnabled = false;
            pbVal.Visibility = Visibility.Visible;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "файлы|*.rpt;*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                _resultlist.Clear();

                const int fileBuff = 1024 * 1024;
                int maxLineLen = Int32.Parse(tbMaxLineLen.Text);
                int cutLen = Int32.Parse(tbCutLent.Text);
                try
                {
                    using (StreamReader r = new StreamReader(new BufferedStream(File.OpenRead(openFileDialog.FileName), fileBuff)))
                    {

                        while (r.EndOfStream != true)
                        {

                            string curLine = await r.ReadLineAsync();

                            if (curLine.Length == maxLineLen)
                            {
                                _resultlist.Add(curLine.Substring(maxLineLen - cutLen, cutLen));

                            }

                        }

                    }
                    string newFileName = System.IO.Path.GetDirectoryName(openFileDialog.FileName) + "\\" +
                        System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "_res.txt";

                    await File.WriteAllLinesAsync(newFileName, _resultlist.Distinct().ToList());

                    MessageBox.Show($"Обработка окончена!\nРезультат сохранён в {newFileName}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($" Ошибка:{ex.Message}", "Ошибка обработки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    pbVal.Visibility = Visibility.Collapsed;
                    btnProcess.IsEnabled = true;
                }
            }

        }
    }
}
