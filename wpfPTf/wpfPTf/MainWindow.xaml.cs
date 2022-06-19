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
        bool? _getPhonefromRigthSide = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            btnProcess.IsEnabled = false;
            tcMain.IsEnabled = false;
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
                _getPhonefromRigthSide = getPhonefromRigthSide.IsChecked;
                try
                {
                    using (StreamReader r = new StreamReader(new BufferedStream(File.OpenRead(openFileDialog.FileName), fileBuff)))
                    {

                        while (r.EndOfStream != true)
                        {

                            string curLine = await r.ReadLineAsync();


                            if (tcMain.TabIndex == 0)
                            {
                                if (curLine.Length == maxLineLen)
                                {
                                    _resultlist.Add(curLine.Substring(maxLineLen - cutLen, cutLen));
                                }
                            }
                            else
                            {
                                parseF2(curLine);
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
                    tcMain.IsEnabled = true;
                }
            }

        }

        private void parseF2(string line)
        {
            var data = line.Split('_');

            if (data.Length > 3)
            {
                _resultlist.Add(data[2]);

                if (_getPhonefromRigthSide == true && long.TryParse(data[5], out long val))
                    _resultlist.Add(data[5]);
            }
        }

    }
}
