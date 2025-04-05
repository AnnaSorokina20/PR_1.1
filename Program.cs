using System;
using System.Globalization;
using System.Linq;

namespace Sorokina_Lab1_1
{
    class JordanIlimination
    { // Обчислення оберненої матриці та розв’язання СЛАР методом Жорданових виключень
        public static double[,] ComputeInverseAndSolve(double[,] matrix, double[] vector)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);
            double[,] workMatrix = (double[,])matrix.Clone();
            int pivotCount = 0;
            // Використовуємо стандартні мітки для рядків та стовпців
            string[] rowTags = Enumerable.Range(1, rowCount).Select(i => $"b{i}").ToArray();
            string[] colTags = Enumerable.Range(1, colCount).Select(i => $"x{i}").ToArray();

            Console.WriteLine("\n--- Розв’язання СЛАР за методом Жорданових виключень ---");
            Console.WriteLine("\nВектор правих частин:");
            for (int i = 0; i < rowCount; i++)
                Console.WriteLine(vector[i]);

            Console.WriteLine("\nПочаткова матриця коефіцієнтів:");
            ShowMatrixWithLabels(workMatrix, rowTags, colTags);

            for (int step = 0; step < Math.Min(rowCount, colCount); step++)
            {
                workMatrix = ExecuteJordanStep(workMatrix, step, ref pivotCount, rowTags, colTags);
            }

            Console.WriteLine("\nОдержана обернена матриця:");
            DisplayMatrix(workMatrix);
            Console.WriteLine($"\nОбчислений ранг матриці: {pivotCount}");
            SolveLinearSystem(workMatrix, vector);
            return workMatrix;
        }

        // Розв’язання СЛАР за допомогою отриманої оберненої матриці
        public static void SolveLinearSystem(double[,] invMatrix, double[] b)
        {
            int r = invMatrix.GetLength(0);
            int c = invMatrix.GetLength(1);
            Console.WriteLine("\nОбчислення розв’язків рівнянь:");
            for (int i = 0; i < r; i++)
            {
                double result = 0;
                string expr = "";
                for (int j = 0; j < c; j++)
                {
                    double term = invMatrix[i, j] * b[j];
                    expr += $"{invMatrix[i, j]:F2}*{b[j]:F2} + ";
                    result += term;
                }
                if (expr.Length >= 3)
                    expr = expr.Substring(0, expr.Length - 3); // видаляємо останній " + "
                Console.WriteLine($"x{i + 1} = {expr} = {result:F2}");
            }
        }

        // Виконання одного кроку Жорданових виключень
        public static double[,] ExecuteJordanStep(double[,] mat, int current, ref int counter, string[] rowTags, string[] colTags)
        {
            int r = mat.GetLength(0);
            int c = mat.GetLength(1);
            double pivot = mat[current, current];
            if (Math.Abs(pivot) < 1e-9)
            {
                Console.WriteLine($"Розв'язувальний елемент на позиції ({current + 1}, {current + 1}) є занадто малим, крок пропущено.");
                return mat;
            }

            double[,] newMat = new double[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (i == current)
                        newMat[i, j] = (j == current) ? 1 : -mat[i, j];
                    else if (j == current)
                        newMat[i, j] = mat[i, j];
                    else
                        newMat[i, j] = mat[i, j] * pivot - mat[i, current] * mat[current, j];
                }
            }
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    newMat[i, j] /= pivot;

            Console.WriteLine($"\nКрок {current + 1}");
            Console.WriteLine($"Розв'язувальний-елемент: A[{current + 1}, {current + 1}] = {pivot:F2}");

            // Обмін мітками для рядків та стовпців
            string temp = rowTags[current];
            rowTags[current] = colTags[current];
            colTags[current] = temp;
            counter++;

            Console.WriteLine("\nПоточна матриця після виключення:");
            ShowMatrixWithLabels(newMat, rowTags, colTags);
            return newMat;
        }

        // Обчислення оберненої матриці (без розв’язання СЛАР)
        public static double[,] InvertMatrix(double[,] matrix, string[] labels)
        {
            int r = matrix.GetLength(0);
            int c = matrix.GetLength(1);
            double[,] workMat = (double[,])matrix.Clone();
            int count = 0;

            string[] rowTags = labels.ToArray();
            string[] colTags = Enumerable.Range(1, c).Select(i => $"x{i}").ToArray();

            Console.WriteLine("\n--- Обчислення оберненої матриці ---");
            Console.WriteLine("\nПочаткова матриця:");
            ShowMatrixWithLabels(workMat, rowTags, colTags);

            for (int s = 0; s < Math.Min(r, c); s++)
            {
                workMat = ExecuteJordanStep(workMat, s, ref count, rowTags, colTags);
            }

            Console.WriteLine("\nОдержана обернена матриця:");
            DisplayMatrix(workMat);
            //Console.WriteLine($"\nРанг матриці: {count}");
            return workMat;
        }

        // Обчислення рангу матриці
        public static double[,] CalculateRank(double[,] matrix, string[] labels)
        {
            int r = matrix.GetLength(0);
            int c = matrix.GetLength(1);
            double[,] workMat = (double[,])matrix.Clone();
            int rank = 0;

            string[] rowTags = labels.ToArray();
            string[] colTags = Enumerable.Range(1, c).Select(i => $"x{i}").ToArray();

            Console.WriteLine("\n--- Обчислення рангу матриці ---");
            Console.WriteLine("\nПочаткова матриця:");
            ShowMatrixWithLabels(workMat, rowTags, colTags);

            for (int s = 0; s < Math.Min(r, c); s++)
            {
                workMat = ExecuteJordanStep(workMat, s, ref rank, rowTags, colTags);
                Console.WriteLine($"Кількість обчислених кроків: {rank}");
            }

            Console.WriteLine("\nОдержана матриця:");
            DisplayMatrix(workMat);
            Console.WriteLine($"\nРанг матриці: {rank}");
            return workMat;
        }

        // Вивід матриці з мітками
        public static void ShowMatrixWithLabels(double[,] mat, string[] rowTags, string[] colTags)
        {
            int r = mat.GetLength(0);
            int c = mat.GetLength(1);
            Console.Write("\t");
            for (int j = 0; j < c; j++)
                Console.Write($"{colTags[j],8}");
            Console.WriteLine();
            for (int i = 0; i < r; i++)
            {
                Console.Write($"{rowTags[i],8}");
                for (int j = 0; j < c; j++)
                    Console.Write($"{mat[i, j],8:F2} ");
                Console.WriteLine();
            }
        }

        // Простий вивід матриці
        public static void DisplayMatrix(double[,] mat)
        {
            int r = mat.GetLength(0);
            int c = mat.GetLength(1);
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                    Console.Write($"{mat[i, j],8:F2} ");
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Пошук оберненої матриці (метод Жорданових виключень)");
                Console.WriteLine("2. Обчислення рангу довільної матриці");
                Console.WriteLine("3. Розв’язання СЛАР за допомогою оберненої матриці (1-й спосіб)");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Некоректне значення, спробуйте ще раз.");
                    continue;
                }

                if (option == 0)
                {
                    Console.WriteLine("Завершення програми.");
                    break;
                }
                else if (option == 1)
                {
                    // Для пошуку оберненої матриці потрібна квадратна матриця
                    double[,] squareMatrix = InputSquareMatrix();
                    string[] defaultLabels = Enumerable.Range(1, squareMatrix.GetLength(0)).Select(i => $"b{i}").ToArray();
                    JordanIlimination.InvertMatrix(squareMatrix, defaultLabels);
                }
                else if (option == 2)
                {
                    // Обчислення рангу для довільної матриці
                    double[,] anyMatrix = InputMatrix();
                    string[] defaultLabels = Enumerable.Range(1, anyMatrix.GetLength(0)).Select(i => $"b{i}").ToArray();
                    JordanIlimination.CalculateRank(anyMatrix, defaultLabels);
                }
                else if (option == 3)
                {
                    // Розв’язання СЛАР: потрібна квадратна матриця та вектор правих частин
                    double[,] squareMatrix = InputSquareMatrix();
                    int n = squareMatrix.GetLength(0);
                    double[] rhs = InputVector(n);
                    JordanIlimination.ComputeInverseAndSolve(squareMatrix, rhs);
                }
                else
                {
                    Console.WriteLine("Невірний вибір, спробуйте ще раз.");
                }
            }
        }

        // Введення квадратної матриці з перевіркою розміру рядка
        static double[,] InputSquareMatrix()
        {
            Console.Write("Введіть розмір квадратної матриці: ");
            int n;
            while (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.Write("Некоректний розмір, спробуйте ще раз: ");
            }
            double[,] matrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                while (true)
                {
                    Console.Write($"Введіть елементи рядка {i + 1} (через пробіл): ");
                    string line = Console.ReadLine();
                    double[] row = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => double.Parse(s))
                                       .ToArray();
                    if (row.Length != n)
                    {
                        Console.WriteLine("Кількість елементів має дорівнювати розміру матриці. Спробуйте ще раз.");
                        continue;
                    }
                    for (int j = 0; j < n; j++)
                        matrix[i, j] = row[j];
                    break;
                }
            }
            return matrix;
        }

        // Введення довільної матриці (кількість рядків та стовпців)
        static double[,] InputMatrix()
        {
            Console.Write("Введіть кількість рядків: ");
            int rows;
            while (!int.TryParse(Console.ReadLine(), out rows) || rows <= 0)
            {
                Console.Write("Некоректне значення, спробуйте ще раз: ");
            }
            Console.Write("Введіть кількість стовпців: ");
            int cols;
            while (!int.TryParse(Console.ReadLine(), out cols) || cols <= 0)
            {
                Console.Write("Некоректне значення, спробуйте ще раз: ");
            }
            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                while (true)
                {
                    Console.Write($"Введіть елементи рядка {i + 1} (через пробіл): ");
                    string line = Console.ReadLine();
                    double[] row = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => double.Parse(s))
                                       .ToArray();
                    if (row.Length != cols)
                    {
                        Console.WriteLine("Кількість елементів має дорівнювати числу стовпців. Спробуйте ще раз.");
                        continue;
                    }
                    for (int j = 0; j < cols; j++)
                        matrix[i, j] = row[j];
                    break;
                }
            }
            return matrix;
        }

        // Введення вектора правих частин із перевіркою кількості елементів
        static double[] InputVector(int size)
        {
            double[] vector = new double[size];
            while (true)
            {
                Console.Write($"Введіть {size} елементів вектора (через пробіл): ");
                string line = Console.ReadLine();
                double[] v = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => double.Parse(s))
                                 .ToArray();
                if (v.Length != size)
                {
                    Console.WriteLine("Невірна кількість елементів, спробуйте ще раз.");
                    continue;
                }
                vector = v;
                break;
            }
            return vector;
        }

    }


}


