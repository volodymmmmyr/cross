using System;
using System.Collections.Generic;
using System.IO;

public static class PathfindingProgram
{
    private static char[,]? _grid;
    private static int _gridSize;
    private static (int Row, int Col) _start;
    private static (int Row, int Col) _end;

    public static void Main(string[] args)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string resultPath = Path.Combine(baseDir, "..", "..", "..", "ResultExecution");

        string inputFilePath = Path.Combine(resultPath, "INPUT.TXT");
        string outputFilePath = Path.Combine(resultPath, "OUTPUT.TXT");

        string[] inputLines;

        try
        {
            inputLines = File.ReadAllLines(inputFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read the input file: {ex.Message}");
            return;
        }

        if (ProcessInput(inputLines, out var resultGrid))
        {
            try
            {
                File.WriteAllText(outputFilePath, "Yes\n" + string.Join("\n", resultGrid));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to the output file: {ex.Message}");
            }
        }
        else
        {
            try
            {
                File.WriteAllText(outputFilePath, "No");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to the output file: {ex.Message}");
            }
        }
    }

    private static bool ProcessInput(string[] input, out string[] outputGrid)
    {
        _gridSize = int.Parse(input[0]);
        _grid = new char[_gridSize, _gridSize];

        _start = (-1, -1);
        _end = (-1, -1);

        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                _grid[i, j] = input[i + 1][j];
                if (_grid[i, j] == '@')
                {
                    _start = (i, j);
                }
                else if (_grid[i, j] == 'X')
                {
                    _end = (i, j);
                }
            }
        }

        if (_start == (-1, -1) || _end == (-1, -1))
        {
            outputGrid = Array.Empty<string>();
            return false;
        }

        bool pathFound = FindShortestPath();

        outputGrid = new string[_gridSize];
        for (int i = 0; i < _gridSize; i++)
        {
            var row = new char[_gridSize];
            for (int j = 0; j < _gridSize; j++)
            {
                row[j] = _grid[i, j];
            }
            outputGrid[i] = new string(row);
        }

        return pathFound;
    }

    private static bool FindShortestPath()
    {
        var directions = new (int RowOffset, int ColOffset)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1)
        };

        var distances = new int[_gridSize, _gridSize];
        var previous = new (int Row, int Col)?[_gridSize, _gridSize];
        var priorityQueue = new SortedSet<(int Distance, int Row, int Col)>(Comparer<(int, int, int)>.Create((a, b) =>
        {
            int comparison = a.Distance.CompareTo(b.Distance);
            if (comparison == 0)
            {
                comparison = a.Row.CompareTo(b.Row);
                if (comparison == 0)
                {
                    comparison = a.Col.CompareTo(b.Col);
                }
            }
            return comparison;
        }));

        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }

        distances[_start.Row, _start.Col] = 0;
        priorityQueue.Add((0, _start.Row, _start.Col));

        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentRow, currentCol) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            if (currentRow == _end.Row && currentCol == _end.Col)
            {
                TracePath(previous, currentRow, currentCol);
                return true;
            }

            foreach (var (rowOffset, colOffset) in directions)
            {
                int newRow = currentRow + rowOffset;
                int newCol = currentCol + colOffset;

                if (newRow >= 0 && newCol >= 0 && newRow < _gridSize && newCol < _gridSize && (_grid![newRow, newCol] == '.' || _grid[newRow, newCol] == 'X'))
                {
                    int newDistance = currentDistance + 1;
                    if (newDistance < distances[newRow, newCol])
                    {
                        priorityQueue.Remove((distances[newRow, newCol], newRow, newCol));
                        distances[newRow, newCol] = newDistance;
                        previous[newRow, newCol] = (currentRow, currentCol);
                        priorityQueue.Add((newDistance, newRow, newCol));
                    }
                }
            }
        }

        return false;
    }

    private static void TracePath((int Row, int Col)?[,] previous, int row, int col)
    {
        while (previous[row, col] != null)
        {
            _grid![row, col] = '+';
            var (prevRow, prevCol) = previous[row, col]!.Value;
            row = prevRow;
            col = prevCol;
        }
        _grid![_start.Row, _start.Col] = '@';
    }
}
