using System;
using System.Threading.Tasks;

namespace AsyncDelegateExample
{
    // Определяем делегат, принимающий два параметра int и возвращающий int
    delegate Task<int> MatrixDelegate(int rows, int cols);

    class Program
    {
        static async Task<int> MatrixOperation(int rows, int cols, IProgress<int> progress)
        {
            // Создаем матрицу целых случайных чисел
            int[,] matrix = new int[rows, cols];
            Random rnd = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rnd.Next(1, 100);
                }
            }

            // Вычисляем максимальное и минимальное значения
            int max = int.MinValue;
            int min = int.MaxValue;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j] > max)
                    {
                        max = matrix[i, j];
                    }
                    if (matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                    }
                }

                // Обновляем информацию о ходе выполнения операции
                if (progress != null)
                {
                    int percentComplete = (i + 1) * 100 / rows;
                    progress.Report(percentComplete);
                }

                // Делаем паузу в 100 миллисекунд, чтобы не перегружать процессор
                await Task.Delay(100);
            }

            // Возвращаем разницу между максимальным и минимальным значениями
            return max - min;
        }

        static async Task Main(string[] args)
        {
            // Создаем экземпляр делегата и передаем в него метод MatrixOperation
            MatrixDelegate matrixDelegate = new MatrixDelegate(MatrixOperation);

            // Создаем объект прогресса, чтобы выводить информацию о ходе выполнения операции
            Progress<int> progress = new Progress<int>();
            progress.ProgressChanged += (sender, percentComplete) =>
            {
                Console.CursorLeft = 0;
                Console.Write($"Calculating... {percentComplete}%");
            };

            // Вызываем метод асинхронно и ждем его завершения
            Task<int> task = matrixDelegate.Invoke(5, 5, progress);

            // Ожидаем завершения операции с тайм-аутом в 10 секунд
            if (await Task.WhenAny(task, Task.Delay(10000)) != task)
            {
                Console.WriteLine("\nOperation timed out!");
            }
            else
            {
                Console.WriteLine($"\nResult: {await task}");
            }
        }
    }
}