using System;
using System.IO;

public class TileCovering
{
    // Метод для підрахунку кількості способів покриття області розміром m x n плитками
    public static long CalculateTileWays(int m, int n)
    {
        long[] ways = new long[n + 1];
        ways[0] = 1;

        for (int i = 1; i <= n; i++)
        {
            if (i >= m)
            {
                ways[i] += ways[i - m];
            }
            if (i >= 1)
            {
                ways[i] += ways[i - 1];
            }
        }

        return ways[n];
    }

    // Метод для перевірки коректності вхідних даних
    public static bool ValidateInput(int m, int n)
    {
        return m >= 2 && m <= 50 && n >= 2 && n <= 50 && m <= n;
    }

    public static void Main()
    {
        // Отримання шляху до поточної директорії
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // Формування шляху до папки ResultExecution
        string resultExecutionDir = Path.Combine(currentDirectory, "..", "..", "..", "ResultExecution");

        // Визначення шляхів для вхідного та вихідного файлів
        string inputFilePath = Path.Combine(resultExecutionDir, "INPUT.TXT");
        string outputFilePath = Path.Combine(resultExecutionDir, "OUTPUT.TXT");

        try
        {
            // Читання вхідних даних
            string inputContent = File.ReadAllText(inputFilePath);
            string[] inputParts = inputContent.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Парсинг значень m та n
            int m = int.Parse(inputParts[0]);
            int n = int.Parse(inputParts[1]);

            // Перевірка валідності вхідних даних
            if (!ValidateInput(m, n))
            {
                throw new ArgumentException("Некоректні вхідні дані");
            }

            // Обчислення результату
            long numberOfWays = CalculateTileWays(m, n);

            // Запис результату у вихідний файл
            File.WriteAllText(outputFilePath, numberOfWays.ToString());
        }
        catch (Exception)
        {
            // Запис помилки у випадку виключення
            File.WriteAllText(outputFilePath, "0");
        }
    }
}
