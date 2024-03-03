using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Path = System.IO.Path;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
   

    public partial class MainWindow : Window
    {
        private Thread thread;
        private List<string> history = new List<string>();
        private string[] files;
        private bool dragStarted = false;
        private bool isProgram = false;
        private bool pause = false;
        private bool repeat = false;
        private bool random = false;
        

        public MainWindow()
        {
            InitializeComponent();

        }
        private void _update_from_code(double Value)
        {
            isProgram = true;

            music.Value = Value;

            isProgram = false;
        }

        private void openBTN_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if(result == CommonFileDialogResult.Ok)
            {

                files = Directory.GetFiles(dialog.FileName,"*.mp3");

                musiclist.ItemsSource = files.Select(file => Path.GetFileNameWithoutExtension(file));
                musiclist.SelectedIndex = 0;
                
                
            }
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Volume = volume.Value;

        }

        private void Sound(string filename)
        {

            _update_from_code(0);

            start.Text = "00:00:00";
            media.Close();
            Console.WriteLine("sound "+filename);

            media.Source = new Uri(filename, UriKind.RelativeOrAbsolute);
            media.Volume = volume.Value;
            media.Pause();//загружаем и сразу ставим на паузу, чтобы синхронизироваться с потоком 
            


        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            music.Maximum = media.NaturalDuration.TimeSpan.Ticks;
            end.Text = media.NaturalDuration.TimeSpan.ToString("hh\\:mm\\:ss");

            if(history.Count == 50) history.RemoveAt(0);

            string item = media.Source.ToString();

            if(history.Contains(item)) history.Remove(item);


            history.Add(item);

            Console.WriteLine("Started media opened");
            StartMusicTime();
        }

        private void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw e.ErrorException;
        }

        private void StartMusicTime()
        {
             (long Value, long Ticks) = (TimeSpan.Parse(start.Text).Ticks, media.NaturalDuration.TimeSpan.Ticks);
            //Value знвчение устанавливаем получая, то время на которое поставили ползунок, чтобы не было никаких перескоков

            thread = new Thread(_ =>
            {

                long second = (new TimeSpan(0, 0, 0, 1)).Ticks;

                while (Value + second <= Ticks)
                {
                    
                    Thread.Sleep(1000);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Value = media.Position.Ticks + second;
                        
                        //Получает текущую позицию пройденных тиков и добавлем к ним 1 секунду, присваивая полученное значение в переменную
                        //А затем на основе этого значения обновлем таймер и позицию курсора слайдера
                        start.Text = (new TimeSpan(Value)).ToString("hh\\:mm\\:ss");

                        if (!dragStarted)
                        {
                            _update_from_code(Value);
                        }
                    });
                    

                    Console.WriteLine($"{Value}  {Ticks}");
                    
                    
                }
                try
                {
                    Thread.Sleep((new TimeSpan(Ticks - Value)).Milliseconds); //Ожидаем оставшееся время
                }
                catch(ArgumentOutOfRangeException)
                {
                    //если пользователь переносит ползунок в конец, то у потока может быть
                    //недостоверное значение, так как он в процессе получает тек позицию медиа и добавляет к ней 1с
                    //Из-за этого Value становиться больше, чем продолжительность аудио
                }
                
                
                Application.Current.Dispatcher.Invoke(() => _next(repeat) );

            }
            );
            thread.IsBackground = true;
            media.Play(); //проигрываем
            thread.Start();
            
        }

        private void music_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (!dragStarted && !isProgram)
            {
                if (e.NewValue == music.Maximum)
                {
                    _next(repeat);
                    return;
                }

 
                Mouse.Capture(music);
                dragStarted = true;
            }

        }
        private void music_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStarted)
            {
                double mousePosition = e.GetPosition(music).X;
                double value = mousePosition / music.ActualWidth * (music.Maximum - music.Minimum) + music.Minimum;
                music.Value = value;
            }
        }

        private void music_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStarted = true;
        }

        private void music_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            music.ReleaseMouseCapture();

            start.Text = new TimeSpan(Convert.ToInt64(music.Value)).ToString("hh\\:mm\\:ss");
            media.Position = TimeSpan.Parse(start.Text);

            dragStarted = false;

        }

        private void playBTN_Click(object sender, RoutedEventArgs e)
        {
            pause = !pause;

            if (pause)
            {
                playBTN.Content = "Пауза";
                thread.Abort();
                media.Pause();
            }
            else
            {
                playBTN.Content = "Играть";
                start.Text = media.Position.ToString("hh\\:mm\\:ss");

                _update_from_code(media.Position.Ticks);

                StartMusicTime();
            }
        }

        private void forwordBTN_Click(object sender, RoutedEventArgs e)
        {
            _next();
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            if (thread.IsAlive) thread.Abort();
            
            if (musiclist.SelectedIndex == 0) musiclist.SelectedIndex = files.Length-1;
            else musiclist.SelectedIndex -= 1;

            

        }

        private void _next(bool rpt = false)
        {
            if (thread.IsAlive) thread.Abort();

            if ((musiclist.SelectedIndex + 1) == files.Length && !rpt) musiclist.SelectedIndex = 0;
            else if (!rpt) musiclist.SelectedIndex += 1;
            else Sound(files[musiclist.SelectedIndex]);
            

        }

        private void randomBTN_Click(object sender, RoutedEventArgs e)
        {
            random = !random;

            if (random)
            {
                Random rand = new Random();

                for (int i = files.Length - 1; i > 0; i--)
                {
                    int j = rand.Next(i + 1);
                    string temp = files[i];
                    files[i] = files[j];
                    files[j] = temp;
                }
                
            }
            else Array.Sort(files);

            musiclist.ItemsSource = files.Select(file => Path.GetFileNameWithoutExtension(file));
            musiclist.Items.Refresh();
            musiclist.SelectedIndex = 0;

        }

        private void repaetBTN_Click(object sender, RoutedEventArgs e)
        {
            repeat = !repeat;

            if(repeat)
            {
                repaetBTN.Content += "🔁";
            }
            else repaetBTN.Content = repaetBTN.Content.ToString().Replace("🔁", "");

        }

        private void ChooseMusic(string filename)
        {
            if(thread != null) thread.Abort();
            Console.WriteLine(filename);
            Sound(files[musiclist.SelectedIndex]);
        }

        private void musiclist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChooseMusic(musiclist.SelectedItem.ToString());
        }

        private void historyBTN_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1(history);
            window.ShowDialog();

            int ind;

            if(window.choose != null && (ind = musiclist.Items.IndexOf(window.choose)) != -1)
            {
                musiclist.SelectedIndex = ind;
            }
        }
    }
}
