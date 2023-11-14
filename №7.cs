using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;


namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("                                   Выбирете соотвесвующую папку или файл");
            Console.WriteLine("______________________________________________________________________________________________________________________");
            ArrowMenu.Listener();
            //foreach (string arg in Directory.GetFiles("C:\\")) { Console.WriteLine(arg); }
            //Console.ReadKey();

        }
    }
}
static class Manager
{
    static private string path = "";
    static private IEnumerable<string> getAll(string _path)
    {
        path = _path;
        return Directory.GetDirectories(path).Concat(Directory.GetFiles(path));
    }

    static public bool IsFile(string filepath)
    {
        FileAttributes attr = File.GetAttributes(filepath);
        return !attr.HasFlag(FileAttributes.Directory);
    }

    static public string ABSPath(string _path)
    {
        return Path.Combine(path, _path);
    }

    static public string[] ReturnParent()
    {
        try
        {
            DirectoryInfo dinfo = Directory.GetParent(path);
            if (dinfo == null) path = "";
            else path = dinfo.FullName;
        }
        catch { }
          
        return Show(path);
    }

    static public string[] Show(string _path = "")
    {
        Console.Clear();
        Console.WriteLine("                                   Выбирете соотвесвующую папку или файл");
        Console.WriteLine("______________________________________________________________________________________________________________________");
        if (_path == "")
        {
            Console.WriteLine($"  {{0,5}}   {{1,6}}   {{2,6}}", "Диски","Свободно","Всего");
            List<string> drivesName = new List<string> { };
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                drivesName.Add(drive.Name);
                Console.WriteLine("  {0,5}  |  {1,5}  | {2,5}", drive.Name, Convert.ToString(drive.TotalFreeSpace / 1024/1024/1024), Convert.ToString(drive.TotalSize / 1024/1024/1024)); 
            }
            return drivesName.ToArray();
        }

        IEnumerable<string> files = getAll(_path);

        Console.WriteLine($"  \t{{0,{-files.Max(f => f.Length/2)}}}\t{{1,8}}\t {{2,5}}","Название","Дата создания","Тип");
        foreach(string file in files)
        {   
            DateTime time; string ext="";

            if (!IsFile(file)) {
                time = Directory.GetCreationTime(file); 
            }

            else {
                time = File.GetCreationTime(file);
                ext = Path.GetExtension(file);
            }
            Console.WriteLine($"  {{0,{-files.Max(f => f.Length)}}}  |  {{1,19}}  | {{2,5}} |", Path.GetFileName(file), time,ext);
        }
        return files.ToArray();
    }
}

static class ArrowMenu
{
    static private int start = 2;
    static private int position = start;
    static private Dictionary<ConsoleKey, Action<int>> arrows = new Dictionary<ConsoleKey, Action<int>>
    {
       [ ConsoleKey.UpArrow ]= (length)=>
        {
            if (position <= start+1)
            {
                position = length + start;
            }
            else position--;
        },
        [ConsoleKey.DownArrow ]= (length)=>
        {
            if (position == length + start)
            {
                position = start + 1;
            }
            else position++;
        }
            
      
    };

    static public void Listener()
    {
        string[] files = Manager.Show();
        while (true)
        {

            ConsoleKey key = Console.ReadKey(true).Key;
            
            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                    {
                        int current_pos = position;
                        
                        arrows[key](files.Length);
                        Console.SetCursorPosition(0, current_pos);
                        Console.WriteLine("  ");
                        Console.SetCursorPosition(0, position);
                        Console.WriteLine("->");
                        break; 
                    }
                case ConsoleKey.Enter:
                    {
                        try
                        {
                            string path = files[position - start - 1];

                            if (Manager.IsFile(Manager.ABSPath(path)))
                            {
                                Process.Start(new ProcessStartInfo { FileName = Manager.ABSPath(path), UseShellExecute = true });
                                break;
                            }

                            files = Manager.Show(path);
                            position = start;
                            continue;
                        }
                        catch { break; }
                    }
                case ConsoleKey.Backspace:
                    { 
                    files = Manager.ReturnParent();
                    position = start;
                    break;
                    }
                case ConsoleKey.Escape: {
                        return;
                    }
            }
            
        }
    }
}
