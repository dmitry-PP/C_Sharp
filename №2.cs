using System;
using System.Collections.Generic;


namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            void guessRandom()
            {
                Random rnd = new Random();
                int number = rnd.Next(1,101);
                Console.WriteLine("Угадайте число от 0 до 100: ");
                int answer = Convert.ToInt32(Console.ReadLine());

                
                while (number!=answer)
                {
                    if (answer>number) {Console.WriteLine($"{answer} больше загаданного");}
                    else { Console.WriteLine($"{answer} меньше загаданного"); }
                    answer = Convert.ToInt32(Console.ReadLine());
                }
                Console.WriteLine("Вы угадали!!!");

            }

            void matrixMultiply()
            {
                List<List<string>>  list = new List<List<string>>();
                for(int i = 1; i < 10; i++)
                {
                    List<string> numbers = new List<string>();
                    for(int j = 1; j < 10; j++)
                    {
                        numbers.Add(Convert.ToString(j*i));
                    }
                    list.Add(numbers);
                }
                foreach (List<string> item in list)
                {
                    string line = "";
                    foreach (string item1 in item)
                    {
                        line = line +item1 + ((item1.Length == 1) ? "      " : "     ");
                    }
                    Console.WriteLine(line);
                }

            }
            void division()
            {
                Console.WriteLine("Для выхода нажмите enter");
                
                while (true){
                    string number = Console.ReadLine();
                    string line = "";
                    if (number == "") break;
                    for (int i = 1; i <= Convert.ToInt32(number); i++)
                    {
                        if (Convert.ToInt32(number) % i == 0)
                        {
                            line += Convert.ToString(i)+"    ";
                        }
                    }
                    Console.WriteLine(line.TrimEnd());
                }
                

            }

            while (true)
            {
                Console.WriteLine("\nВыберите программу:\n"+
                    "1. Игра \"Угадай число\"\n"+
                    "2. Таблица умножения\n"+
                    "3. Вывод делителей числа\n"+
                    "(Для выхода нажмите enter)");
                string asked = Console.ReadLine();
                try
                {
                    switch (asked)
                    {
                        case "1":
                            {
                                guessRandom();
                                break;
                            }
                        case "2":
                            {
                                matrixMultiply();
                                break;
                            }
                        case "3":
                            {
                                division();
                                break;
                            }
                        case "": return;

                    }
                }
                catch { Console.WriteLine("Возникла ошибка, попробуйте еще раз"); }
            }
        }
    }
}
