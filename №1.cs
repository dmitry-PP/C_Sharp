using System;
using System.Collections.Generic;
namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int factorial(double num)
            {
                int res = 1;
                for (int start = 1; start <= num; ++start)
                {
                    res *= start;
                }
                return res;
            }
            var operations = new Dictionary<string, Action<List<double>>>()
            {
                ["1"] = (lst2) => Console.WriteLine($"{lst2[0]} + {lst2[1]} = {lst2[0] + lst2[1]}"),
                ["2"] = (lst2) => Console.WriteLine($"{lst2[0]} - {lst2[1]} = {lst2[0] - lst2[1]}"),
                ["3"] = (lst2) => Console.WriteLine($"{lst2[0]} * {lst2[1]} = {lst2[0] * lst2[1]}"),
                ["4"] = (lst2) => Console.WriteLine($"{lst2[0]} / {lst2[1]} = {lst2[0] / lst2[1]}"),
                ["5"] = (lst2) => Console.WriteLine($"{lst2[0]} ^ {lst2[1]} = {Math.Pow(lst2[0], lst2[1])}"),
                ["6"] = (lst1) => Console.WriteLine($"Корень числа {lst1[0]} = {Math.Sqrt(lst1[0])}"),
                ["7"] = (lst1) => Console.WriteLine($"1 процент от числа {lst1[0]} = {lst1[0] * 0.01}"),
                ["8"] = (lst1) => Console.WriteLine($"Факториал числа {lst1[0]} = {factorial(lst1[0])}")
            };
            void getMenu()
            {
                Console.WriteLine();
                Console.WriteLine("__________MENU__________");
                Console.WriteLine("1. Сложить 2 числа");
                Console.WriteLine("2. Вычесть первое из второго");
                Console.WriteLine("3. Перемножить два числа");
                Console.WriteLine("4. Разделить первое на второе");
                Console.WriteLine("5. Возвести в степень N первое число");
                Console.WriteLine("6. Найти квадратный корень из числа");
                Console.WriteLine("7. Найти 1 процент от числа");
                Console.WriteLine("8. Найти факториал из числа");
                Console.WriteLine("9. Выйти из программы");
                Console.WriteLine("________________________");
                Console.WriteLine();
            }
            string asked;
            do
            {
                getMenu();
                asked = Console.ReadLine();
                try
                {
                    switch (asked)
                    {
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                            {
                                List<double> lst = new List<double>();
                                Console.WriteLine("Введите первое число: ");
                                lst.Add(Convert.ToDouble(Console.ReadLine().Replace(".", ",")));
                                Console.WriteLine("Введите второе число: ");
                                lst.Add(Convert.ToDouble(Console.ReadLine().Replace(".", ",")));
                                operations[asked](lst);
                                break;
                            }
                        case "6":
                        case "7":
                        case "8":
                            {
                                Console.WriteLine("Введите число: ");
                                operations[asked](new List<double> { Convert.ToDouble(Console.ReadLine().Replace(".", ",")) });
                                break;
                            }
                    }
                }
                catch
                {
                    Console.WriteLine("Было введено не число. Выход");
                }
            }
            while (asked != "9");
        }
    }
}