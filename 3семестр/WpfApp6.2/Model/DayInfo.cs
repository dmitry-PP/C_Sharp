using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp6._2.Model
{
    public class DayInfo
    {
        public List<Choice> choices = new List<Choice>() 
        {
            new Choice("Гимнастика","Картинка 1"),
            new Choice("Приседания","Картинка 2"),
            new Choice("Отжимания","Картинка 3"),
            new Choice("Пресс","Картинка 4"),
            new Choice("ХЗ","Картинка 5"),
        };
        public DateTime date;

        public DayInfo(DateTime date)
        {
            this.date = date;
        }


    }

    public class Choice
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Selected { get; set; }

        public Choice(string name, string image, bool selected = false)
        {
            Name = name;
            Image = image;
            Selected = selected;
        }
    }
}
