using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Button> list;

        public MainWindow()
        {
            InitializeComponent();

            list = new List<Button>() { btn1,btn2,btn3,btn4,btn5,btn6,btn7,btn8,btn9};
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {
            Button current_btn = (Button)sender;
            current_btn.Content = Mark.UserMark;
            current_btn.IsEnabled = false;

            if (GameManager.UserStep(list, current_btn))
            {
                title.Content = "Ты выиграл!!!";
                SetUp(list,false,false);
            }
            else
            {
                switch (GameManager.BotStep(list))
                {
                    case BotReturn.Win: title.Content = "Ты проиграл((("; SetUp(list, false, false); break;
                    case BotReturn.Tip: title.Content = "Ничья <>"; break;
                    case BotReturn.Pass: break;
                }
                
            }

        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            x.IsEnabled = o.IsEnabled = true;
            x.Background = o.Background = Brushes.White;
            title.Content = "Крестики нолики";

            SetUp(list, false);
        }

        private void Choose(object sender, RoutedEventArgs e)
        {
            Button mark_btn = (Button)sender;

            GameManager.choose(

                (string)mark_btn.Content

                );
            x.IsEnabled = o.IsEnabled = false;

            //mark_btn.Background = new SolidColorBrush(Color.FromRgb(0x00,0xFF,0x7F));

            SetUp(list);


        }

        private void SetUp(List<Button> list,bool IsEnable = true, bool withContent = true)
        {
            foreach (Button btn in list)
            {
                btn.IsEnabled = IsEnable;

                if (!withContent) continue;
                btn.Content = "";
            }
        }
    }
}

static class Mark
{
    static public string UserMark;
    static public string BotMark;

}

enum BotReturn
{   
    Pass = 0,
    Win = 1,
    Tip = 2,
}


static class GameManager
{
    static private int X = 3;
    static private int Y = 3;
    static private string[][] field = new string[Y][];
    
    static public void choose(string mark)
    {
        (Mark.UserMark, Mark.BotMark) = (mark == "o") ? ("o", "x") : ("x", "o");

    }

    static public bool UserStep(List<Button> list, Button btn)
    {
        int level,x , y;
        level = x = y = 0;

        for (int i = 0; i < Y; i++)
        {
            string[] mid = new string[X];
            for(int j = 0; j < X; j++)
            {
                if(list[j + level].Equals(btn))
                {
                    y = i; x = j;
                }
                mid[j] = (string)list[j+level].Content;
            }
            field[i] = mid;
            level += Y;
        }
        return CheckWin(y, x, Mark.UserMark);
    }

    static private bool CheckWin(int y, int x, string mark)
    {
        if (field[y].Count(n => n == mark) == X) return true;

        if (field.Count(row => row[x] == mark) == X) return true;


        if (x == X / 2 && y == Y / 2) //если середина, то проверяем 2 диагонали
        {
            return CheckDiagonal(y, 0, mark) || CheckDiagonal(y, X - 1, mark);
        }
        return CheckDiagonal(y,x,mark);


    }

    static private bool CheckDiagonal(int y,int x,string mark)
    {
        if ((x == 0 || x == X - 1) && (y == 0 || y == Y - 1)) // диагональ когда x = 0  или x = X-1
        {
            int xdiag = (y == 0) ? x : X - 1 - x; 
            // если y = 0, то x = x (мы его не трогаем) иначе мы x = X-1
            //те мы его переварачиваем (ведь все равно с какой стороны идти по диагонали) (2,0)->(0,2)
            for (int ind = 0; ind < Y; ind++)
            {
                if (field[ind][Math.Abs(xdiag - ind)] != mark) return false; //проверяем по диагонали
            }
            return true;
        }
        return false;
    }

    static public BotReturn BotStep(List<Button> buttons)
    {
        List<Button> btns = new List<Button>();
        int UserBlock = -1;

        foreach (var button in buttons.Select((btn,ind)=> new {btn,ind}))
        {
            int y = button.ind / Y;
            int x = button.ind - y*Y;

            if(field[y][x] == "")
            {
                if (CheckWin(y, x, (field[y][x] = Mark.BotMark)))
                {
                    button.btn.Content = Mark.BotMark;
                    return BotReturn.Win;
                }
                else if (CheckWin(y, x, (field[y][x] = Mark.UserMark)))
                {
                    UserBlock = button.ind;
                    continue;

                }
                field[y][x] = "";
                btns.Add(button.btn);
            }
        }
        if (UserBlock != -1)
        {
            SetBotButton(buttons[UserBlock]);
            return BotReturn.Pass;
        }
        if (btns.Count == 0) return BotReturn.Tip;

        int index = (new Random()).Next(0, btns.Count);
        SetBotButton(btns[index]);

        return BotReturn.Pass;

    }

    static private void SetBotButton(Button btn)
    {
        btn.Content = Mark.BotMark;
        btn.IsEnabled = false;
    }
}

