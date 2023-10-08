using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Program
    {
        static double[][] frequency = new double[][] {
                new []{16.35, 17.32, 18.35, 19.45, 20.6, 21.83, 23.12, 24.5, 25.96, 27.5, 29.14, 30.87},//0
                new []{32.7, 34.65, 36.71, 38.89, 41.2, 43.65, 46.25, 49.0, 51.91, 55.0, 58.27, 61.74},//1
                new []{65.41, 69.3, 73.42, 77.78, 82.41, 87.31, 92.5, 98.0, 103.8, 110.0, 116.5, 123.5},//2
                new []{130.8, 138.6, 146.8, 155.6, 164.8, 174.6, 185.0, 196.0, 207.7, 220.0, 233.1, 246.9},//3
                new []{261.6, 277.2, 293.7, 311.1, 329.6, 349.2, 370.0, 392.0, 415.3, 440.0, 466.2, 493.9},//4
                new []{523.3, 554.4, 587.3, 622.3, 659.3, 698.5, 740.0, 784.0, 830.6, 880.0, 932.3, 987.8},//5
                new []{1047.0, 1109.0, 1175.0, 1245.0, 1319.0, 1397.0, 1480.0, 1568.0, 1661.0, 1760.0, 1865, 1976},//6
                new []{2093.0, 2217.0, 2349.0, 2489.0, 2637.0, 2794.0, 2960.0, 3136.0, 3322.0, 3520.0, 3729.0, 3951.0},//7
                new []{4186.0, 4435.0, 4699.0, 4978.0, 5274.0, 5588.0, 5920.0, 6272.0, 6645.0, 7040.0, 7459.0, 7902.0},//8
                };

        static Dictionary<ConsoleKey, int> notes = new Dictionary<ConsoleKey, int>()
        { [ConsoleKey.A] = 0, [ConsoleKey.Q] = 1, [ConsoleKey.S] = 2, [ConsoleKey.W] = 3, [ConsoleKey.D] = 4, [ConsoleKey.F] = 5, [ConsoleKey.E] = 6, [ConsoleKey.G] = 7, [ConsoleKey.R] = 8, [ConsoleKey.H] = 9, [ConsoleKey.T] = 10, [ConsoleKey.J] = 11 };

        static Dictionary<ConsoleKey, int> octaves = new Dictionary<ConsoleKey, int>()
        { [ConsoleKey.F1] = 0, [ConsoleKey.F2] = 1, [ConsoleKey.F3] = 2, [ConsoleKey.F4] = 3, [ConsoleKey.F5] = 4, [ConsoleKey.F6] = 5, [ConsoleKey.F7] = 6, [ConsoleKey.F8] = 7, [ConsoleKey.F9] = 8 };

        static void Main(string[] args)
        {
            ConsoleKey key; int octave = 8;
            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                try
                {
                    if (notes.ContainsKey(key)) Play(key,octave);
                    else if (octaves.ContainsKey(key)) octave = octaves[key];
                }
                catch { }
            }
        }
        static void Play(ConsoleKey key,int octave)
        {
            Console.Beep(Convert.ToInt32(Math.Round(changeOctave(octave)[notes[key]])), 200);
        }
        static double[] changeOctave(int octave)
        {
            return frequency[octave];
        }
    }
}











/*namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[] frequency = {16.35, 17.32, 18.35, 19.45, 20.6, 21.83, 23.12, 24.5, 25.96, 27.5, 29.14, 30.87}; // C C# D Eb E F F# G G# A Bb B

            Dictionary<ConsoleKey, int> notes = new Dictionary<ConsoleKey, int>()
            { [ConsoleKey.A] = 0, [ConsoleKey.Q] = 1, [ConsoleKey.S] = 2, [ConsoleKey.W] = 3, [ConsoleKey.D] = 4, [ConsoleKey.F] = 5, [ConsoleKey.E] = 6, [ConsoleKey.G] = 7, [ConsoleKey.R] = 8, [ConsoleKey.H] = 9, [ConsoleKey.T] = 10, [ConsoleKey.J] = 11 };

            Dictionary<ConsoleKey, int> octaves = new Dictionary<ConsoleKey, int>()
            { [ConsoleKey.F1] = 0, [ConsoleKey.F2] = 1, [ConsoleKey.F3] = 2, [ConsoleKey.F4] = 3, [ConsoleKey.F5] = 4, [ConsoleKey.F6] = 5, [ConsoleKey.F7] = 6, [ConsoleKey.F8] = 7, [ConsoleKey.F9]=8};

            ConsoleKey key;
            int octave = 0;
            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                try
                {
                    if (notes.ContainsKey(key))  Console.Beep(Convert.ToInt32(Math.Round(frequency[notes[key]] * (Math.Pow(2, octave ))) ), 200);//На каждом новом уровне частота ноты увеличивается в 2^(номер октавы)

                    else if (octaves.ContainsKey(key))  octave = octaves[key];
                }
                catch { }
            }
        }
    }
}*/