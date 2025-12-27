using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class Edge
{
    public int Beg, End, Weight;
    public Edge(int beg, int end, int weight)
    {
        Beg = beg;
        End = end;
        Weight = weight;
    }
}

class UnionFind
{
    private int[] parent;

    public UnionFind(int n)
    {
        parent = new int[n + 1];
        for (int i = 1; i <= n; i++)
            parent[i] = i;
    }

    public int Find(int x)
    {
        while (x != parent[x])
            x = parent[x];
        return x;
    }

    public void Union(int a, int b)
    {
        parent[Find(a)] = Find(b);
    }
}

class Algorytmy
{
    // ZADANIE 1 – LCS 
    public static void Zadanie1_LCS()
    {
        string inputFile = "in0301.txt";
        string outputFile = "out0301.txt";

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Brak pliku {inputFile}!");
            return;
        }

        string[] lines = File.ReadAllLines(inputFile);
        int n = int.Parse(lines[0]);

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            for (int i = 0; i < n; i++)
            {
                string s1 = lines[2 * i + 1].Trim();
                string s2 = lines[2 * i + 2].Trim();

                (int length, string subcode) = LongestCommonSubsequence(s1, s2);
                sw.WriteLine($"{length} {subcode}");
            }
        }

        Console.WriteLine($"Wyniki zapisano do pliku {outputFile}");
    }

    private static (int, string) LongestCommonSubsequence(string s1, string s2)
    {
        int m = s1.Length;
        int n = s2.Length;
        int[,] dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (s1[i - 1] == s2[j - 1])
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                else
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
            }
        }

        int x = m;
        int y = n;
        List<char> result = new List<char>();

        while (x > 0 && y > 0)
        {
            if (s1[x - 1] == s2[y - 1])
            {
                result.Add(s1[x - 1]);
                x--;
                y--;
            }
            else
            {
                int up = dp[x - 1, y];
                int left = dp[x, y - 1];

                if (up > left)
                {
                    x--;
                }
                else if (left > up)
                {
                    y--;
                }
                else
                {
                    string upStr = RecoverString(dp, s1, s2, x - 1, y);
                    string leftStr = RecoverString(dp, s1, s2, x, y - 1);

                    if (string.CompareOrdinal(upStr, leftStr) <= 0)
                        x--;
                    else
                        y--;
                }
            }
        }

        result.Reverse();
        return (dp[m, n], new string(result.ToArray()));
    }

    private static string RecoverString(int[,] dp, string s1, string s2, int x, int y)
    {
        List<char> r = new List<char>();

        while (x > 0 && y > 0)
        {
            if (s1[x - 1] == s2[y - 1])
            {
                r.Add(s1[x - 1]);
                x--;
                y--;
            }
            else
            {
                int up = dp[x - 1, y];
                int left = dp[x, y - 1];

                if (up > left)
                    x--;
                else if (left > up)
                    y--;
                else
                {
                    x--;
                }
            }
        }

        r.Reverse();
        return new string(r.ToArray());
    }

    // ZADANIE 3 – Kruskal
    public static void Zadanie3_Kruskal()
    {
        string inputFile = "in0303.txt";
        string outputFile = "out0303.txt";

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Brak pliku {inputFile}!");
            return;
        }

        string[] lines = File.ReadAllLines(inputFile);
        int n = int.Parse(lines[0]);

        List<Edge> edges = new List<Edge>();

        for (int i = 1; i <= n; i++)
        {
            string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < parts.Length; j += 2)
            {
                int v = int.Parse(parts[j]);
                int w = int.Parse(parts[j + 1]);
                if (i < v)
                    edges.Add(new Edge(i, v, w));
            }
        }

        edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));

        UnionFind UnionFind = new UnionFind(n);
        List<Edge> mst = new List<Edge>();
        int totalWeight = 0;


        foreach (var e in edges)
        {
            int a = e.Beg;
            int b = e.End;

            if (UnionFind.Find(a) != UnionFind.Find(b))
            {
                UnionFind.Union(a, b);
                mst.Add(e);
                totalWeight += e.Weight;

                if (mst.Count == n - 1)
                    break;
            }
        }

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            for (int i = 0; i < mst.Count; i++)
            {
                var e = mst[i];
                sw.Write($"{e.Beg} {e.End} [{e.Weight}]");
                if (i < mst.Count - 1)
                    sw.Write(", ");
            }
            sw.WriteLine();
            sw.WriteLine(totalWeight);
        }

        Console.WriteLine($"Wyniki zapisano do pliku {outputFile}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Wybierz zadanie do wykonania: ");
        Console.WriteLine("1 - Modyfikacja koloru kwiatu tulibajtu (LCS)");
        Console.WriteLine("3 - Algorytm Kruskala (MST)");
        Console.Write("Twój wybór: ");

        string? wybor = Console.ReadLine();

        if (wybor == "1")
            Algorytmy.Zadanie1_LCS();
        else if (wybor == "3")
            Algorytmy.Zadanie3_Kruskal();
        else
            Console.WriteLine("Nieprawidłowy wybór!");
    }
}