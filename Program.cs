using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    // ZADANIE 1: Symbol Newtona
    static long Silnia(int n, ref int licznik)
    {
        long wynik = 1;
        for (int i = 2; i <= n; i++)
        {
            wynik *= i;
            licznik++;
        }
        return wynik;
    }

    static long SN1(int n, int k, out int licz)
    {
        licz = 0;

        int nk = n - k;

        long nf = Silnia(n, ref licz);
        long kf = Silnia(k, ref licz);
        long nkf = Silnia(nk, ref licz);

        licz++;

        return nf / (kf * nkf);
    }

    static long SN3(int n, int k, out int licz, string prefix = "", List<string> drzewo = null)
    {
        if (drzewo != null)
            drzewo.Add($"{prefix}SN({n}, {k})");

        licz = 1;

        if (k == 0 || k == n)
        {
            return 1;
        }

        int licz1, licz2;
        long a = SN3(n - 1, k - 1, out licz1, prefix + "  ", drzewo);
        long b = SN3(n - 1, k, out licz2, prefix + "  ", drzewo);

        licz += licz1 + licz2;
        return a + b;
    }

    static void ZadanieSymbolNewtona()
    {
        string input = "In0101.txt";
        string output = "Out0101.txt";
        string[] data = File.ReadAllLines(input);
        string[] parts = data[0].Split(' ');
        int n = int.Parse(parts[0]);
        int k = int.Parse(parts[1]);

        int licz1;
        long sn1 = SN1(n, k, out licz1);

        int licz3;
        List<string> drzewoRekurencji = new List<string>();
        long sn3 = SN3(n, k, out licz3, "", drzewoRekurencji);

        List<string> outputLines = new List<string>
    {
        $"n={n} k={k}",
        $"SN1 = {sn1}, licz = {licz1}",
        $"SN3 = {sn3}, licz = {licz3}",
        $"Drzewo wywołań SN3:"
    };
        outputLines.AddRange(drzewoRekurencji);

        File.WriteAllLines(output, outputLines);
        Console.WriteLine("Wynik zapisano do Out0101.txt");
    }

    // ZADANIE 2: Sejf Króla
    static void ZadanieSejfKrola()
    {
        string inputPath = "In0104.txt";
        string outputPath = "Out0104.txt";

        string[] lines = File.ReadAllLines(inputPath);
        string[] firstLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(firstLine[0]);
        int m = int.Parse(firstLine[1]);

        int[] pokrycie = new int[n + 2];

        List<int> par = new List<int>();

        for (int i = 1; i <= m; i++)
        {
            string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int x1 = int.Parse(parts[0]);
            int y1 = int.Parse(parts[1]);
            int x2 = int.Parse(parts[2]);
            int y2 = int.Parse(parts[3]);

            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            if (y1 == y2)
            {
                if (y1 >= 0 && y1 < n)
                {
                    par.Add(y1);
                }
            }
            else
            {
                if (minY >= 0 && minY < n)
                {
                    pokrycie[minY]++;
                    if (maxY <= n)
                        pokrycie[maxY]--;
                }
            }
        }

        int[] suma = new int[n + 2];
        suma[0] = pokrycie[0];
        for (int i = 1; i <= n + 1; i++)
            suma[i] = suma[i - 1] + pokrycie[i];

        List<(int, int)> bezpiecznePasma = new List<(int, int)>();
        int start = -1;

        for (int y = 0; y <= n; y++)
        {
            if (y == n || suma[y] != 0)
            {
                if (start != -1)
                {
                    bezpiecznePasma.Add((start, y));
                    start = -1;
                }
            }
            else
            {
                if (par.Contains(y))
                {
                    par.Remove(y);
                    bezpiecznePasma.Add((start, y));
                    start = y;
                }
                if (start == -1)
                {
                    start = y;
                }
            }
        }

        List<string> outputLines = new List<string>();

        foreach (var (y1, y2) in bezpiecznePasma)
            outputLines.Add($"{y1} {y2}");

        outputLines.Add($"liczba bezpiecznych pasm: {bezpiecznePasma.Count}");

        File.WriteAllLines(outputPath, outputLines);

        Console.WriteLine("Wynik zapisano do Out0104.txt");
    }

    static void ZadanieSejfKrola_Naiwny()
    {
        string inputPath = "In0104.txt";
        string outputPath = "Out0104.txt";

        string[] lines = File.ReadAllLines(inputPath);
        string[] firstLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(firstLine[0]);
        int m = int.Parse(firstLine[1]);

        int licznik = 0;

        int[] pokrycie = new int[n + 1];
        List<int> par = new List<int>();

        for (int i = 1; i <= m; i++)
        {
            string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int x1 = int.Parse(parts[0]);
            int y1 = int.Parse(parts[1]);
            int x2 = int.Parse(parts[2]);
            int y2 = int.Parse(parts[3]);

            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            if (y1 == y2)
            {
                if (y1 >= 0 && y1 < n)
                {
                    par.Add(y1);
                }
            }
            else
            {
                int a = Math.Max(minY, 0);
                int b = Math.Min(maxY - 1, n - 1);
                if (a <= b)
                {
                    for (int y = a; y <= b; y++)
                    {
                        pokrycie[y]++;
                        licznik++;
                    }
                }
            }
        }

        int[] suma = new int[n + 1];
        for (int y = 0; y < n; y++)
        {
            suma[y] = pokrycie[y];
            licznik++;
        }
        suma[n] = 0;

        List<(int, int)> bezpiecznePasma = new List<(int, int)>();
        int start = -1;

        for (int y = 0; y <= n; y++)
        {
            licznik++;

            if (y == n || suma[y] != 0)
            {
                if (start != -1)
                {
                    bezpiecznePasma.Add((start, y));
                    start = -1;
                }
            }
            else
            {
                if (par.Contains(y))
                {
                    int idx = par.IndexOf(y);
                    if (idx >= 0)
                        par.RemoveAt(idx);

                    bezpiecznePasma.Add((start, y));
                    start = y;
                }
                if (start == -1)
                {
                    start = y;
                }
            }
        }

        List<string> outputLines = new List<string>();

        foreach (var (y1, y2) in bezpiecznePasma)
            outputLines.Add($"{y1} {y2}");

        outputLines.Add($"liczba bezpiecznych pasm: {bezpiecznePasma.Count}");
        outputLines.Add($"licznik operacji: {licznik}");

        File.WriteAllLines(outputPath, outputLines);

        Console.WriteLine("Wynik zapisano do Out0104.txt");
    }

    // ZADANIE 3: Prezes Bajt
    static void ZadaniePrezesBajt()
    {
        string inputPath = "In0103.txt";
        string outputPath = "Out0103.txt";

        string[] lines = File.ReadAllLines(inputPath);
        int n = int.Parse(lines[0]);
        int[] dane = Array.ConvertAll(lines[1].Split(' '), int.Parse);

        int maxSum = dane[0];
        int currentSum = dane[0];
        int start = 0, end = 0, tempStart = 0;

        for (int i = 1; i < n; i++)
        {
            if (currentSum < 0)
            {
                currentSum = dane[i];
                tempStart = i;
            }
            else
            {
                currentSum += dane[i];
            }

            if (currentSum > maxSum)
            {
                maxSum = currentSum;
                start = tempStart;
                end = i;
            }
        }

        File.WriteAllText(outputPath, $"{start + 1}, {end + 1}, {maxSum}");
        Console.WriteLine("Wynik zapisano do Out0103.txt");
    }

    static void ZadaniePrezesBajt_Naiwny()
    {
        string inputPath = "In0103.txt";
        string outputPath = "Out0103.txt";

        string[] lines = File.ReadAllLines(inputPath);
        int n = int.Parse(lines[0]);
        int[] dane = Array.ConvertAll(lines[1].Split(' '), int.Parse);

        int maxSum = int.MinValue;
        int start = 0, end = 0;
        int licznik = 0;

        for (int i = 0; i < n; i++)
        {
            int suma = 0;
            for (int j = i; j < n; j++)
            {
                suma += dane[j];
                licznik++;
                if (suma > maxSum)
                {
                    maxSum = suma;
                    start = i;
                    end = j;
                }
            }
        }

        File.WriteAllText(outputPath, $"{start + 1}, {end + 1}, {maxSum}, licznik={licznik}");
        Console.WriteLine("Wynik zapisano do Out0103.txt");
    }

    static void Main()
    {
        Console.WriteLine("Wybierz zadanie do uruchomienia:");
        Console.WriteLine("1 - Symbol Newtona (In0101.txt → Out0101.txt)");
        Console.WriteLine("2 - Sejf Króla (In0104.txt → Out0104.txt)");
        Console.WriteLine("2n - Sejf Króla – naiwny (In0104.txt → Out0104.txt)");
        Console.WriteLine("3 - Prezes Bajt (In0103.txt → Out0103.txt)");
        Console.WriteLine("3n - Prezes Bajt – naiwny (In0103.txt → Out0103.txt)");
        Console.Write("Twój wybór: ");
        string wybor = Console.ReadLine();

        switch (wybor)
        {
            case "1": ZadanieSymbolNewtona(); break;
            case "2": ZadanieSejfKrola(); break;
            case "2n": ZadanieSejfKrola_Naiwny(); break;
            case "3": ZadaniePrezesBajt(); break;
            case "3n": ZadaniePrezesBajt_Naiwny(); break;
            default: Console.WriteLine("Nieprawidłowy wybór."); break;
        }
    }
}