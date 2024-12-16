using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;

public class LuckyNumbersProcessor
{
    public static void Main()
    {
        // Отримання поточного каталогу програми
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        // Пошук шляху до папки ResultExecution (на 4 рівні вище)
        string targetDir = Path.Combine(currentDir, "..", "..", "..", "ResultExecution");

        // Визначення файлів вводу та виводу
        string inputFile = Path.Combine(targetDir, "INPUT.TXT");
        string outputFile = Path.Combine(targetDir, "OUTPUT.TXT");

        // Зчитування вхідних даних
        string rawInput = File.ReadAllText(inputFile);
        BigInteger maxNumber = BigInteger.Parse(rawInput.Trim());

        // Підрахунок "щасливих чисел"
        int luckyCount = CalculateLuckyCount(maxNumber);

        // Запис результату у файл
        File.WriteAllText(outputFile, luckyCount.ToString());
    }

    public static int CalculateLuckyCount(BigInteger limit)
    {
        // Генерація всіх "щасливих чисел" до зазначеного обмеження
        List<BigInteger> luckyNums = GenerateLuckyNumbersUpTo(limit);
        return luckyNums.Count;
    }

    public static List<BigInteger> GenerateLuckyNumbersUpTo(BigInteger maxLimit)
    {
        var luckyNumsList = new List<BigInteger>();
        RecursivelyBuildNumbers(string.Empty, maxLimit, luckyNumsList);
        return luckyNumsList;
    }

    private static void RecursivelyBuildNumbers(string currentNumber, BigInteger maxLimit, List<BigInteger> results)
    {
        if (!string.IsNullOrEmpty(currentNumber))
        {
            BigInteger parsedNumber = BigInteger.Parse(currentNumber);
            if (parsedNumber > maxLimit)
                return;

            results.Add(parsedNumber);
        }

        // Генеруємо нові "щасливі числа" шляхом додавання 4 або 7
        if (currentNumber.Length == 0 || BigInteger.Parse(currentNumber) <= maxLimit)
        {
            RecursivelyBuildNumbers(currentNumber + "4", maxLimit, results);
            RecursivelyBuildNumbers(currentNumber + "7", maxLimit, results);
        }
    }

    public static bool CheckIfLucky(BigInteger candidate)
    {
        // Перевіряємо, чи складається число лише з цифр 4 та 7
        while (candidate > 0)
        {
            BigInteger digit = candidate % 10;
            if (digit != 4 && digit != 7)
                return false;

            candidate /= 10;
        }
        return true;
    }
}
