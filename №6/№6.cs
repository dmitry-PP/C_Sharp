using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    internal class Program
    {
        static int pos_x = 0;
        static int pos_y = 0;
        static string[] body_file;

        private static void get_file_body(List<Cake> lst)
        {
            List<string> midd = new List<string> { };
            for(int i = 0; i < lst.Count; i++)
            {
                foreach (var item in lst[i].convert())
                {
                    midd.Add(item);
                }
            }
            body_file = midd.ToArray();
            
        }

        static void Main(string[] args)
        {
            string path = Console.ReadLine();
            
            if (Path.GetExtension(path) == ".json")
            {
                List<Cake> result = JsonConvert.DeserializeObject<List<Cake>>(File.ReadAllText(path));
                get_file_body(result);
            }
            else if (Path.GetExtension(path) == ".xml")
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<Cake>));
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    List<Cake> result = xml.Deserialize(fs) as List<Cake>;
                    get_file_body(result);
                }
            }
            else if (Path.GetExtension(path) == ".txt")
            {
                body_file = File.ReadAllLines(path);
            }
            if (File.Exists(path))
            {
                foreach (string item in body_file)
                {
                    Console.WriteLine(item);
                }


                pos_y = body_file.Length-1; pos_x = body_file[pos_y].Length;
                
                Console.SetCursorPosition(pos_x , pos_y +1);
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);

                    setPos(key,path);

                }
                while (key.Key != ConsoleKey.Escape);
                
            }
            else Console.WriteLine("none");
        }
        static void setPos(ConsoleKeyInfo key,string path)
        {
            ConsoleKey Key = key.Key;
            switch (Key)
            {
                case ConsoleKey.DownArrow:
                    {
                        if (pos_y != body_file.Length - 1)
                        {
                            pos_y += 1;
                            pos_x = body_file[pos_y].Length;
                        }
                        break;
                    }
                case ConsoleKey.UpArrow:
                    {
                        if (pos_y != 0)
                        {
                            pos_y -= 1;
                            pos_x = body_file[pos_y].Length;
                        }
                            
                        break;
                    }
                case ConsoleKey.LeftArrow:
                    {
                        if (pos_x != 0)
                            pos_x -= 1;
                        else if (pos_y != 0)
                        {
                            pos_y -= 1;
                            pos_x = body_file[pos_y].Length;
                        }
                        break;
                    }
                case ConsoleKey.RightArrow:
                    {
                        if (pos_x!= body_file[pos_y].Length)
                        {
                            pos_x += 1;
                        }
                        else if (pos_y != body_file.Length - 1)
                        {
                            pos_y += 1;
                            pos_x = 0;
                        }
                        break;
                    }
                case ConsoleKey.Backspace:
                    {
                        if (pos_x != 0)
                        {

                            string part = body_file[pos_y].Substring(pos_x);

                            pos_x-=1;
                            Console.SetCursorPosition(0, pos_y+1);
                            ClearLine(pos_y+1);
                            body_file[pos_y] = body_file[pos_y].Substring(0,pos_x) + part;
                            Console.WriteLine(body_file[pos_y]);
                            
                        }
                        
                        else if (pos_y != 0)
                        {
                            pos_y -= 1;
                            pos_x = body_file[pos_y].Length;
                        }


                        break;
                    }
                case ConsoleKey.F1:
                    {
                        Console.Clear();
                        string new_path = Console.ReadLine();
                        int j = 0;
                        List<Cake> list = new List<Cake>();
                        for(int i = 0; i <= ((body_file.Length-1)/3); i++)
                        {
                            list.Add(new Cake(body_file[j], Convert.ToInt32(body_file[j + 1]), Convert.ToInt32(body_file[j+2])));
                            j+=3;
                        }
                        if (Path.GetExtension(new_path)==".json")
                        {
                            string json = JsonConvert.SerializeObject(list);
                            File.WriteAllText(new_path, json);
                        }
                        else if (Path.GetExtension(new_path) == ".xml")
                        {
                            XmlSerializer xml = new XmlSerializer(typeof(List<Cake>));
                            using (FileStream fs = new FileStream(new_path, FileMode.OpenOrCreate))
                            {
                                xml.Serialize(fs, list);
                            }

                        }
                        else if(Path.GetExtension(new_path) == ".txt")
                        {
                            File.WriteAllText(new_path, string.Join("\n", body_file));
                        }
                        break;
                    }
                default:
                    {
                          
                        if(pos_x== body_file[pos_y].Length)
                        {
                            body_file[pos_y] = body_file[pos_y]+ key.KeyChar.ToString();
                            Console.WriteLine(key.KeyChar.ToString());
                        }
                        else
                        {
                            body_file[pos_y] = body_file[pos_y].Insert(pos_x, key.KeyChar.ToString());
                            Console.WriteLine( body_file[pos_y].Substring(pos_x));
                        }

                        pos_x++;
                        break;
                    }
            }
            Console.SetCursorPosition (pos_x , pos_y+1);
        }
        
        private static void ClearLine(int line)
        {
            Console.MoveBufferArea(0, line, Console.BufferWidth, 1, Console.BufferWidth, line, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }
    }
}


public class Cake
{
    public string name;
    public int weight;
    public int price;
    public Cake() { }
    public Cake(string name, int weight, int price)
    {
        this.name = name;
        this.weight = weight;
        this.price = price;
    }

    public string[] convert()
    {
        return new string[3] { this.name, Convert.ToString(this.weight), Convert.ToString(this.price) };
    }
}
