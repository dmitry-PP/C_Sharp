using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
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
using Path = System.IO.Path;

namespace WpfApp2
{
    
    public partial class MainWindow : Window
    {
        Manager manager;

        public MainWindow()
        {
            

            InitializeComponent();

            manager = new Manager(listbox);

            calendar.SelectedDate = DateTime.Now;
            manager.SetListBox(calendar.SelectedDate);
            


        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            manager.DeleteRecord(calendar.SelectedDate,(listbox.SelectedItem as Record).Name);
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            _CheckRecord((record) =>
            {
                manager.SetRecord(record);
            });

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {

            _CheckRecord((record) =>
            {
                manager.UpdateRecord(record, (listbox.SelectedItem as Record).Name);
            });

            
        }

        private void calendar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
            manager.SetListBox(calendar.SelectedDate);

            name_tb.Text = desc_tb.Text = "";
            delete.IsEnabled = save.IsEnabled = false;
            
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete.IsEnabled = save.IsEnabled = true;

            Record selected = listbox.SelectedItem as Record;

            if(selected != null )
            {
                name_tb.Text = selected.Name;
                desc_tb.Text = selected.Description;
            }

        }

        private void _CheckRecord(Action<Record> action)
        {
            if (name_tb.Text == "" || desc_tb.Text == "")
            {
                error.Content = "Название/Описание не может быть пустым!";
            }
            else if (manager.GetIndex(calendar.SelectedDate, name_tb.Text) != -1)
            {
                error.Content = "Такое название уже присутствует!";
            }
            else
            {
                error.Content = "";

                Record record = new Record(
                    name_tb.Text,
                    desc_tb.Text,
                    calendar.SelectedDate
                );


                action(record);
            }
        }
    }
}

class Record
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? Date { get; set; }

    public Record(string _Name, string _Description, DateTime? _Date)
    {
        Name = _Name;
        Description = _Description;
        Date = _Date;
    }

}

class Manager
{
    private ListBox listbox;
    private Dictionary<string, List<Record>> records;
    private string path;

    public Manager(ListBox lb)
    {
        listbox = lb;

        path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Saver.json"
            );

        records = ( File.Exists(path) ) ? Deserializer<Dictionary<string, List<Record>>>() : new Dictionary<string, List<Record>>();



    }

    public void Serializer<T>(T records)
    {
        string json = JsonConvert.SerializeObject(records);

        File.WriteAllText(path,json);

    }

    public T Deserializer<T>()
    {
        string json = File.ReadAllText(path);
        T data = JsonConvert.DeserializeObject<T>(json);

        return data;
    }

    public int GetIndex(DateTime? key, string value)
    {
        if (!records.ContainsKey(key.ToString())) return -1;

        int index = records[key.ToString()].FindIndex(x => x.Name == value); 

        return index;
    }

    public void SetListBox(DateTime? date)
    {
        try
        {
            Console.WriteLine(records[date.ToString()].Count);
            listbox.ItemsSource = records[date.ToString()];
            listbox.Items.Refresh();
        }
        catch (KeyNotFoundException) { listbox.ItemsSource = null; }
    }

    public void SetRecord(Record record)
    {
        string key = record.Date.ToString();

        if(!records.ContainsKey(key)) records[key] = new List<Record>() { record};

        else records[key].Add(record);


        _update(record.Date);

    }

    public void UpdateRecord(Record record, string Name)
    {

        int index = GetIndex(record.Date, Name);


        Record upRecord = records[record.Date.ToString()][index];

        upRecord.Name = record.Name;
        upRecord.Description = record.Description;

        _update(record.Date);
    }

    public void DeleteRecord(DateTime? Date,string Name)
    {
        int index = GetIndex(Date, Name);

        records[Date.ToString()].RemoveAt(index);

        _update(Date);
    }

    private void _update(DateTime? Date)
    {
        Serializer(records);
        SetListBox(Date);
    }
        
}