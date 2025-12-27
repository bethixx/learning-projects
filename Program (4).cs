using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    // Zadanie 1a - sortowanie przez zliczanie
    static void Zadanie1a()
    {
        string inputFile = "In0201.txt";
        string outputFile = "Out0201.txt";

        string[] lines = File.ReadAllLines(inputFile);
        int n = int.Parse(lines[0]);
        int[] numbers = Array.ConvertAll(lines[1].Split(' '), int.Parse);

        int minValue = -10000;
        int maxValue = 10000;
        int range = maxValue - minValue + 1;

        int[] count = new int[range];

        foreach (int num in numbers)
            count[num - minValue]++;

        int index = 0;
        for (int i = 0; i < range; i++)
        {
            while (count[i] > 0)
            {
                numbers[index++] = i + minValue;
                count[i]--;
            }
        }

        File.WriteAllText(outputFile, string.Join(" ", numbers));
        Console.WriteLine("Zadanie 1a: Sortowanie zakończone.");
    }

    // Zadanie 2 - rysowanie trójkąta
    static void Zadanie2()
    {
        string inputFile = "In0202.txt";
        string outputFile = "Out0202.txt";

        int n = int.Parse(File.ReadAllLines(inputFile)[0]);

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            sw.WriteLine("n=" + n);
            DrawTriangle(sw, n, 1);
        }

        Console.WriteLine("Zadanie 2: Trójkąt zapisany.");
    }

    static void DrawTriangle(StreamWriter sw, int baseWidth, int currentRow)
    {
        int totalRows = (baseWidth + 1) / 2;

        if (currentRow > totalRows)
            return;

        int stars = 2 * currentRow - 1;
        int spaces = (baseWidth - stars) / 2;

        sw.WriteLine(new string(' ', spaces) + new string('*', stars));

        DrawTriangle(sw, baseWidth, currentRow + 1);
    }


    // Zadanie 3 - ciąg Fibonacciego
    static void Zadanie3()
    {
        string inputFile = "In0203.txt";
        string outputFile = "Out0203.txt";

        int n = int.Parse(File.ReadAllLines(inputFile)[0]);

        List<int> fibList = new List<int>();
        GenerateFibonacci(0, 1, n, fibList);

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            sw.WriteLine("n=" + n);
            sw.WriteLine(string.Join(", ", fibList));
        }

        Console.WriteLine("Zadanie 3: Ciąg Fibonacciego zapisany.");
    }

    static void GenerateFibonacci(int a, int b, int limit, List<int> fibList)
    {
        if (a >= limit) return;

        fibList.Add(a);
        GenerateFibonacci(b, a + b, limit, fibList);
    }

    // Zadanie 4 - magiczna kostka
    static int n;
    static string T;
    static int tIndex = 0;

    static char GetNextChar()
    {
        char c = T[tIndex];
        tIndex = (tIndex + 1) % T.Length;
        return c;
    }

    static void Zadanie4()
    {
        string inputFile = "In0204.txt";
        string outputFile = "Out0204.txt";

        string[] lines = File.ReadAllLines(inputFile);
        n = int.Parse(lines[0]);
        T = lines[1];
        tIndex = 0;

        char[,,] cube = new char[n, n, n];

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            sw.WriteLine($"n={n}, T={T}");

            for (int tabIdx = 0; tabIdx < n; tabIdx++)
            {
                char[,] tab = new char[n, n];
                FillSquare(tab, tabIdx % 2 == 0);

                for (int r = 0; r < n; r++)
                    for (int c = 0; c < n; c++)
                        cube[tabIdx, r, c] = tab[r, c];

                sw.WriteLine($"Tablica {tabIdx}");
                for (int r = 0; r < n; r++)
                {
                    sw.Write("[");
                    for (int c = 0; c < n; c++)
                        sw.Write(c == n - 1 ? tab[r, c].ToString() : tab[r, c] + ", ");
                    sw.WriteLine("]");
                }
            }
        }

        Console.WriteLine("Zadanie 4: Kostka zapisana.");
    }

    static void FillSquare(char[,] tab, bool evenOrder)
    {
        List<(int r, int c)> coords = new List<(int r, int c)>();
        GenerateSpiralCoords(coords, 0, n - 1, 0, n - 1);

        if (!evenOrder)
            coords.Reverse();

        foreach (var (r, c) in coords)
            tab[r, c] = GetNextChar();
    }

    static void GenerateSpiralCoords(List<(int r, int c)> coords, int rowStart, int rowEnd, int colStart, int colEnd)
    {
        if (rowStart > rowEnd || colStart > colEnd) return;

        for (int c = colStart; c <= colEnd; c++)
            coords.Add((rowStart, c));
        for (int r = rowStart + 1; r <= rowEnd; r++)
            coords.Add((r, colEnd));
        if (rowEnd > rowStart)
            for (int c = colEnd - 1; c >= colStart; c--)
                coords.Add((rowEnd, c));
        if (colEnd > colStart)
            for (int r = rowEnd - 1; r > rowStart; r--)
                coords.Add((r, colStart));

        GenerateSpiralCoords(coords, rowStart + 1, rowEnd - 1, colStart + 1, colEnd - 1);
    }

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Wybierz zadanie do uruchomienia:");
            Console.WriteLine("1 - Zadanie 1a (Sortowanie)");
            Console.WriteLine("2 - Zadanie 2 (Trójkąt)");
            Console.WriteLine("3 - Zadanie 3 (Ciąg Fibonacciego)");
            Console.WriteLine("4 - Zadanie 4 (Magiczna kostka)");
            Console.WriteLine("0 - Wyjście");
            Console.Write("Twój wybór: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Zadanie1a();
                    break;
                case "2":
                    Zadanie2();
                    break;
                case "3":
                    Zadanie3();
                    break;
                case "4":
                    Zadanie4();
                    break;
                case "0":
                    Console.WriteLine("Koniec programu.");
                    return;
                default:
                    Console.WriteLine("Niepoprawny wybór, spróbuj ponownie.");
                    break;
            }
            Console.WriteLine();
        }
    }
}
