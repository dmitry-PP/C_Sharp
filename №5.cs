using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Menu.showMenu();
        }
    }
}


class Order
{
    public class SubPoint
    {
        public string name;
        public int price;
        public SubPoint(string nameArg, int priceArg)
        {
            name = nameArg;
            price = priceArg;
        }
    }

    static public int sum;
    static public string description = "";
    static List<SubPoint> subPoints;

    //static List<string> points = new List<string>() { "�����", "������", "����", "���������� ������", "�������", "�����" };
    static public Dictionary<string, List<SubPoint>> points_choices = new Dictionary<string, List<SubPoint>>
    {
        ["�����"] = new List<SubPoint> { new SubPoint("�������", 600), new SubPoint("����������", 200), new SubPoint("��������", 1000) },
        ["������"] = new List<SubPoint> { new SubPoint("25 ��", 1200), new SubPoint("28 ��", 1500), new SubPoint("30 ��", 2000) },
        ["����"] = new List<SubPoint> { new SubPoint("���������", 400), new SubPoint("�����������", 500), new SubPoint("���������", 700) },
        ["����������� ������"] = new List<SubPoint> { new SubPoint("3 �����", 300), new SubPoint("4 �����", 500), new SubPoint("5 ������", 600) },
        ["�������"] = new List<SubPoint> { new SubPoint("����������", 100), new SubPoint("�������", 400), new SubPoint("�������", 150) },
        ["�����"] = new List<SubPoint> { new SubPoint("��������", 120), new SubPoint("������", 200), new SubPoint("�������", 100) },
    };

    static public List<SubPoint> choosePoint(int index)
    {
        subPoints = Order.points_choices.Values.ToArray()[index];
        return subPoints;
    }

    static public void add(int index)
    {
        SubPoint subpoint = subPoints[index];
        sum += subpoint.price;
        description += $"{subpoint.name} - {subpoint.price}\n";
    }

    static public void Write()
    {
        if (description != "")
        {
            File.AppendAllText($@"{Environment.CurrentDirectory}\file.txt", description + $"______________________\n{DateTime.Now}  -  {sum}\n\n");
            clear();
        }
    }

    static private void clear()
    {
        sum = 0;
        description = "";
    }


}

static class Menu
{
    static int posCursor;
    static int limit;

    static public void showMenu()
    {
        ConsoleKey key;
        showPoints();
        do
        {

            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow) choosePoint(key);
            else if (key == ConsoleKey.Enter)
            {
                if (0 <= (posCursor - 2) && (posCursor - 2) < Order.points_choices.Count) showSubPoints();
                else if ((posCursor - 2) == Order.points_choices.Count) { Order.Write(); showPoints(); }
            }

        }
        while (key != ConsoleKey.Escape);
    }

    static void showPoints()
    {
        Console.Clear();
        Console.WriteLine("��������� � �������� ���� ����!!!");
        Console.WriteLine("_________________________________");
        foreach (string item in Order.points_choices.Keys)
        {
            Console.WriteLine($"  {item}");
        }
        Console.WriteLine("  <<��������>>");
        Console.WriteLine($"\n����: {Order.sum}");
        Console.WriteLine(Order.description);
        limit = Order.points_choices.Keys.Count;
        posCursor = limit + 3;//+1 <<��������>>
    }

    static void showSubPoints()
    {
        List<Order.SubPoint> sub = Order.choosePoint(posCursor - 2);

        Console.Clear();
        Console.WriteLine("�������� ���� �� ������� ����");
        Console.WriteLine("_____________________________");


        foreach (Order.SubPoint item in sub)
        {
            Console.WriteLine($"  {item.name} - {item.price}");

        }
        limit = sub.Count - 1;

        posCursor = limit + 3;//+1 limit
        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
            try
            {

                if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow) choosePoint(key);
                else if (key == ConsoleKey.Enter) { Order.add(posCursor - 2); break; }
            }
            catch { continue; }
        }
        while (key != ConsoleKey.Escape);
        showPoints();

    }

    static void choosePoint(ConsoleKey key)
    {
        Console.SetCursorPosition(0, posCursor);
        Console.WriteLine("  ");
        if (key == ConsoleKey.DownArrow)
        {
            if (posCursor >= 2 + limit) posCursor = 2;
            else ++posCursor;
        }
        else if (key == ConsoleKey.UpArrow)
        {
            if (posCursor == 2) posCursor = 2 + limit;
            else --posCursor;
        }
        Console.SetCursorPosition(0, posCursor);
        Console.WriteLine("->");
    }
}