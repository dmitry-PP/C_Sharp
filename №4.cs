using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Program
    {
        static Dictionary<string, RecordDate> recordsDate = new Dictionary<string, RecordDate>(); 
        static DateTime currentDate = DateTime.Now;
        static int current_recordDateLength = 0;
        static int posCursor = 3;
        static void Main(string[] args)
        {
            ConsoleKey key;
            Menu();
            do
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow) choosePoint(key);
                else if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow) changeDate( key);
                else if (key == ConsoleKey.Enter)
                {
                    Console.Clear();

                    if (posCursor == 2 + current_recordDateLength) createRecordDate();
                    else getRecord(posCursor - 2);

                    Menu();

                }

            }
            while (key != ConsoleKey.Escape);
        }

        static void Menu()
        {
            try
            {
                Console.Clear();
                RecordDate rec_data = recordsDate[currentDate.ToShortDateString()];
                currentDate = rec_data.date;
                current_recordDateLength = rec_data.records.Count;
                PrintRecord(currentDate, rec_data.records);
            }
            catch
            {
                PrintRecord(currentDate);
                current_recordDateLength = 0;

            }
        }


        static void PrintRecord(DateTime date, List<RecordDate.Record> lst = null)
        {
            Console.WriteLine(date.ToShortDateString());
            Console.WriteLine("------------------");
            if (lst != null)
            {
                for (int i = 0; i < lst.Count; ++i)
                {
                    Console.WriteLine($"  {i + 1}. " + lst[i].name);
                }
            }
            Console.WriteLine("  +Добавить запись");
        }

        static void choosePoint(ConsoleKey key)
        {
            Console.SetCursorPosition(0, posCursor);
            Console.WriteLine("  ");
            if (key == ConsoleKey.DownArrow)
            {
                if (posCursor >= 2 + current_recordDateLength) posCursor = 2;
                else ++posCursor;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                if (posCursor == 2) posCursor = 2 + current_recordDateLength;
                else --posCursor;
            }

            Console.SetCursorPosition(0, posCursor);
            Console.WriteLine("->");
        }

        static void changeDate( ConsoleKey key)
        {
            if (key == ConsoleKey.LeftArrow)
            {
                currentDate = currentDate.AddDays(-1);
            }
            else if (key == ConsoleKey.RightArrow)
            {
                currentDate = currentDate.AddDays(1);
            }
            Menu();
        }

        static void createRecordDate()
        {
            Console.WriteLine(currentDate);
            Console.WriteLine("Введите название записи");
            string name = Console.ReadLine();
            Console.WriteLine("Введите описание записи");
            string desc = Console.ReadLine();
            
            if (recordsDate.ContainsKey(currentDate.ToShortDateString()))
            {
                RecordDate currentRD = recordsDate[currentDate.ToShortDateString()];
                currentRD.createRecord(currentDate, name, desc);
            }
            else
            {
                RecordDate recordDate = new RecordDate();
                recordDate.createRecord(currentDate, name, desc);
                recordsDate.Add(currentDate.ToShortDateString(),recordDate);
            }
            
        }
        static void getRecord(int index)
        {
            try
            {
                RecordDate rd = recordsDate[currentDate.ToShortDateString()];
                rd.showRecord(index);
                Console.ReadKey(true);
            }
            catch { }
        }

    }
}


class RecordDate
{
    public class Record
    {
        public string name;
        public string desc;

        public Record(string nameARG, string descARG)
        {
            name = nameARG;
            desc = descARG;
        }
    }

    public DateTime date;
    public List<Record> records = new List<Record>();

    public Record createRecord(DateTime dateARG, string nameARG, string descARG)
    {
        date = dateARG;
        Record rec = new Record(nameARG, descARG);
        records.Add(rec);
        return rec;
    }
    public void showRecord(int index)
    {
        Record rec = records[index];
        Console.WriteLine(rec.name);
        Console.WriteLine("------------------");
        Console.WriteLine($"Описание: {rec.desc}");
        Console.WriteLine($"Дата: {date}");
    }
}