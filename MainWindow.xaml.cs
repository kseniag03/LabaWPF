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

namespace LabaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int listNum = default;
        private Dictionary<RichTextBox, FileInfo> textBoxInfo;

        public MainWindow()
        {
            InitializeComponent();
            textBoxInfo = new();
        }        

        private void CreateNewTab_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox textBox = new();
            textBox.Background = Brushes.LightSteelBlue;
            textBox.Foreground = Brushes.Snow;
            TextBlock textBloc = new();
            textBloc.Text = $"File {++listNum}";
            textBloc.Foreground = Brushes.Snow;

            tabControl.Items.Add(new TabItem
            {                
                Header = textBloc, Background = Brushes.LightSteelBlue, Content = textBox
            });
        }

        private void Tab1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(addTabButton))
            {
                CreateNewTab_Click(sender, e);
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|RichText files (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName == String.Empty) return;
                FileInfo fileInfo = new(openFileDialog.FileName);
                RichTextBox textBox = new();
                TextBlock textBloc = new();
                textBox.Background = Brushes.LightSteelBlue;
                textBox.Foreground = Brushes.Snow;
                textBloc.Text = fileInfo.Name;
                textBloc.Foreground = Brushes.Snow;
                TabItem item = new TabItem
                {
                    Header = textBloc, Background = Brushes.LightSteelBlue, Content = textBox
                };
                tabControl.Items.Add(item);
                tabControl.SelectedItem = item;
                try
                {
                    TextRange text = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        if (System.IO.Path.GetExtension(openFileDialog.FileName).Equals(".rtf"))
                        {
                            text.Load(fs, DataFormats.Rtf);
                        }
                        else
                        {
                            text.Load(fs, DataFormats.Text); text.Load(fs, DataFormats.Text);
                        }
                        textBoxInfo.Add(textBox, fileInfo);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error");
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox textBox = (tabControl.SelectedItem as TabItem).Content as RichTextBox;
            if (textBox == null) return;
            if (!textBoxInfo.ContainsKey(textBox)) return;
            FileInfo fileInfo = textBoxInfo[textBox];
            try
            {
                TextRange text = new(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                using (FileStream fs = File.Create(fileInfo.FullName))
                {
                    if (System.IO.Path.GetExtension(fileInfo.FullName).Equals(".rtf"))
                    {
                        text.Save(fs, DataFormats.Rtf);
                    }
                    else
                    {
                        text.Save(fs, DataFormats.Text);
                    }
                }
            }            
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox textBox = (tabControl.SelectedItem as TabItem).Content as RichTextBox;
            if (textBox == null) return;
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|RichText files (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new(saveFileDialog.FileName);
                try
                {
                    TextRange text = new(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                    using (FileStream fs = File.Create(saveFileDialog.FileName))
                    {
                        if (System.IO.Path.GetExtension(saveFileDialog.FileName).Equals(".rtf"))
                        {
                            text.Save(fs, DataFormats.Rtf);
                        }
                        else
                        {
                            text.Save(fs, DataFormats.Text);
                        }
                        TextBlock textBloc = new();
                        textBloc.Text = fileInfo.Name;
                        textBloc.Foreground = Brushes.Snow;
                        textBoxInfo[textBox] = fileInfo;
                        (tabControl.SelectedItem as TabItem).Header = textBloc;
                    }
                }                
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error");
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
