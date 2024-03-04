using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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


        object lockedObj = new object();
        AutoResetEvent _event = new AutoResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();

        }


        private void _update_from_code(double Value)
        {
            /*
                Говорим, что обновление происходит непосредственно из кода,
                а не из GUI. Тем самым игнорирую событие music_ValueChanged
            */

            isProgram = true;

            music.Value = Value;

            isProgram = false;
        }

        private void openBTN_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {

                files = Directory.GetFiles(dialog.FileName, "*.mp3");

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

            media.Source = new Uri(filename, UriKind.RelativeOrAbsolute);
            media.Volume = volume.Value;
            media.Pause();//загружаем и сразу ставим на паузу, чтобы синхронизироваться с потоком 



        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            music.Maximum = media.NaturalDuration.TimeSpan.Ticks;
            end.Text = media.NaturalDuration.TimeSpan.ToString("hh\\:mm\\:ss");

            if (history.Count == 50) history.RemoveAt(49); //Максимальная длина истории прослушивание = 50 песням


            string item = media.Source.ToString();

            if (history.Contains(item)) history.Remove(item); //Если такой трек уже был занесен в список, то поднимаем его наверх

            history.Insert(0,item);

            StartMusicTime();
        }

        private void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw e.ErrorException;
        }

        private void StartMusicTime()
        {
            /*Запускаем поток, отвечающий за таймер продолжительности трека и движения ползунка вслед за музыкой*/

            (long Value, long Ticks) = (TimeSpan.Parse(start.Text).Ticks, media.NaturalDuration.TimeSpan.Ticks);
            //Value знвчение устанавливаем получая, то время на которое поставили ползунок, чтобы не было никаких перескоков

            thread = new Thread(_ =>
            {

                long second = (new TimeSpan(0, 0, 0, 1)).Ticks;

                //Таймер не должен заходить за границу времени продолжительности трека
                while (Value + second <= Ticks)
                {
                    lock (lockedObj) //Получаем блок на поток
                    {

                        //Ждем пока истечет время, чтобы обновить таймер с ползунком (acquire = false).
                        //Если же сигнал разморозки происходит раньше, то мы переводим данный поток в режим ожидания (acquire = true)
                        //Пока другой поток(основной) не выполнит операцию: снятие с паузы или перестановка ползунка
                        bool acquire = _event.WaitOne(1000);
                        

                        if (!acquire)
                        {

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Value = TimeSpan.Parse(start.Text).Ticks + second;

                                //Получает текущую позицию пройденных тиков и добавлем к ним 1 секунду, присваивая полученное значение в переменную
                                //А затем на основе этого значения обновлем таймер и позицию курсора слайдера
                                start.Text = (new TimeSpan(Value)).ToString("hh\\:mm\\:ss");

                                if (!dragStarted)
                                {
                                    _update_from_code(Value);
                                }
                            });
                        }
                        else
                        {
                            //Замораживаем поток, до тех пор, пока основной поток не уведомит о своей готовности

                            Monitor.Wait(lockedObj);
                            _event.Reset();
                        }

                    }

                }
                try
                {
                    Thread.Sleep((new TimeSpan(Ticks - Value)).Milliseconds); //Ожидаем оставшееся время
                }
                catch (ArgumentOutOfRangeException)
                {
                    //если пользователь переносит ползунок в конец, то у потока может быть
                    //недостоверное значение, так как он в процессе получает тек позицию медиа и добавляет к ней 1с
                    //Из-за этого Value становиться больше, чем продолжительность аудио
                }


                Application.Current.Dispatcher.Invoke(() => _next(repeat));

            }
            );
            thread.IsBackground = true;
            media.Play();
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
            dragStarted = false;

            //Снимаем другой поток с ожидания и блокируем
            _event.Set();

            lock (lockedObj)
            {
                //Меняем значение времени и медиа, где пользователь остановил ползунок
                //И размораживает другой поток

                start.Text = new TimeSpan(Convert.ToInt64(music.Value)).ToString("hh\\:mm\\:ss");
                media.Position = TimeSpan.Parse(start.Text);
                Monitor.Pulse(lockedObj);
            }

        }

        private void playBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CheckStatusNull(false)) return;

            pause = !pause;
            if (pause)
            {

                playBTN.Content = "Пауза";

                //Снимаем другой поток с ожидания и блокируем, а медиа ставим на паузу
                _event.Set();
                media.Pause();
            }
            else
            {
                lock (lockedObj)
                {
                    playBTN.Content = "Играть";
                    start.Text = media.Position.ToString("hh\\:mm\\:ss");

                    _update_from_code(media.Position.Ticks);

                    //Снимаем паузу медиа и размораживаем другой поток
                    media.Play();
                    Monitor.Pulse(lockedObj);
                }
            }  
        }

        private void forwordBTN_Click(object sender, RoutedEventArgs e)
        {
            _next();
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CheckStatusNull()) return;


            if (musiclist.SelectedIndex == 0) musiclist.SelectedIndex = files.Length - 1;
            else musiclist.SelectedIndex -= 1;

        }

        private void _next(bool rpt = false)
        {
            if (CheckStatusNull()) return;


            if ((musiclist.SelectedIndex + 1) == files.Length && !rpt) musiclist.SelectedIndex = 0;
            else if (!rpt) musiclist.SelectedIndex += 1;
            else Sound(files[musiclist.SelectedIndex]);


        }

        private void randomBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CheckStatusNull()) return;

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

            if (repeat)
            {
                repaetBTN.Content += "🔁";
            }
            else repaetBTN.Content = repaetBTN.Content.ToString().Replace("🔁", "");

        }


        private void musiclist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CheckStatusNull()) return;

            Sound(files[musiclist.SelectedIndex]);
        }

        private void historyBTN_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1(history);
            window.ShowDialog();

            int ind;

            if (window.choose != null && (ind = musiclist.Items.IndexOf(window.choose)) != -1)
            {
                musiclist.SelectedIndex = ind;
            }
        }

        private bool CheckStatusNull(bool killThread = true)
        {
            if(
                killThread && 
                thread != null && 
                thread.IsAlive

                ) thread.Abort();

            return files == null;
        }
    }
}
