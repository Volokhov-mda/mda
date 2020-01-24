using System;
using System.IO;
using System.Collections.Generic;

namespace calculator
{
    delegate double MathOperation(double a, double b);

    class Program
    {
        // Файл, в котором записаны все выражения.
        const string expressionsFile = "../../../expressions.txt";

        // Файл, в котором записаны заданные заранее результаты выражений.
        const string checkerFile = "../../../expressions_checker.txt";

        // Файл, в котором записаны результаты выражений, вычесленные программой.
        const string answersFile = "../../../answers.txt";

        // Файл, в котором записаны результаты сравнения каждого результата выражений из файла checkerFile и answersFile.
        const string resultsFile = "../../../results.txt";

        // Словарь, в котором хранятся арифметические операции.
        static Dictionary<String, MathOperation> operations = new Dictionary<String, MathOperation>();

        static Program()
        {
            // Заполнение словаря арифметическими операциями.
            operations = new Dictionary<string, MathOperation>();
            operations.Add("+", (x, y) => { return x + y; });
            operations.Add("-", (x, y) => { return x - y; });
            operations.Add("*", (x, y) => { return x * y; });
            operations.Add("/", (x, y) => { return x / y; });
            operations.Add("^", (x, y) => { return Math.Pow(x, y); });
        }

        /// <summary>
        /// Метод, возвращающий результат данного выражения.
        /// </summary>
        /// <param name="expr">Выражение</param>
        /// <returns></returns>
        static double Calculate(string expr)
        {
            // Разбиение выражения на операнды и операцию.
            string[] arguments = expr.Split();

            double operandA = double.Parse(arguments[0]);
            string operatorExpr = arguments[1];
            double operandB = double.Parse(arguments[2]);

            // Результат выражения, вычесленный при помощи словаря.
            return operations[operatorExpr](operandA, operandB);
        }

        /// <summary>
        /// Метод, очищающий или создающий файлы по переданным директориям.
        /// </summary>
        /// <param name="paths">Директории файлов, которые требуется очистить/создать</param>
        static void ClearFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                try
                {
                    File.WriteAllText(path, "");
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File Not Found Exception: ", e.Message);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Input/Output Exception: ", e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Unauthorized Access Exception: ", e.Message);
                }
                catch (System.Security.SecurityException e)
                {
                    Console.WriteLine("Security Exception: ", e.Message);
                }
            }
        }

        /// <summary>
        /// Метод, вычисляющий значения каждого выражения, записанного в файл, и записывающий их в новый файл.
        /// </summary>
        /// <param name="expressionsFile">Директория файла, в котором хранятся выражения</param>
        /// <param name="answersFile">Директория файла, в который записываются значения выражений</param>
        static void CalculateFileExpressions(string expressionsFile, string answersFile)
        {
            // Массив выражений.
            string[] arrOfExprs = File.ReadAllLines(expressionsFile);

            foreach (var expr in arrOfExprs)
            {
                try
                {
                    // Запись в файл answersFile результатов выражений (каждый результат записывается с новой строчки).
                    File.AppendAllText(answersFile, $"{Calculate(expr):f3}" + Environment.NewLine);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File Not Found Exception: ", e.Message);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Input/Output Exception: ", e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Unauthorized Access Exception: ", e.Message);
                }
                catch (System.Security.SecurityException e)
                {
                    Console.WriteLine("Security Exception: ", e.Message);
                }
            }
        }

        /// <summary>
        /// Метод, сравнивающий значения выражений, и записывающий результаты сравнений в файл.
        /// </summary>
        /// <param name="answersFile">Первый файл со значениями выражений</param>
        /// <param name="checkerFile">Второй файл со значениями выражений</param>
        /// <param name="resultsFile">Файл, в который записываются результаты сравнений</param>
        static void CheckResults(string answersFile, string checkerFile, string resultsFile)
        {
            try
            {
                // Инициализация массивов заданных заранне результатов выражений и
                // результатов выражений, вычесленных программой.
                string[] checks = File.ReadAllLines(checkerFile);
                string[] answers = File.ReadAllLines(answersFile);

                // Счетчик несовпадающих ответов.
                int wrongAnswers = 0;

                // Проверка, одинаковое ли количество результатов веражений в двух файлах.
                if (answers.Length != checks.Length)
                {
                    Console.WriteLine(
                        "Impossible to check the answers: different number of elements in answers and checker files." + Environment.NewLine +
                        $"There are {answers.Length} elements in first file and {checks.Length} int the second file.");
                }
                else
                {
                    for (int i = 0; i < answers.Length; i++)
                    {
                        // Запись результата сравнения в файл resultsFile.
                        if (answers[i] == checks[i]) { File.AppendAllText(resultsFile, "OK" + Environment.NewLine); }
                        else { File.AppendAllText(resultsFile, "Error" + Environment.NewLine); ++wrongAnswers; }
                    }

                    File.AppendAllText(resultsFile, wrongAnswers + Environment.NewLine);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File Not Found Exception: ", e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine("Input/Output Exception: ", e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unauthorized Access Exception: ", e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Security Exception: ", e.Message);
            }
        }

        static void Main(string[] args)
        {
            // Очистка/создание файлов.
            ClearFiles(answersFile, resultsFile);

            // Вычисление значений выражений и их запись в новый файл.
            CalculateFileExpressions(expressionsFile, answersFile);

            // Сравнение посчитанных результатов с результатами, предложенными в файле checkerFile.
            // Запись результатов сравнения в новый файл.
            CheckResults(answersFile, checkerFile, resultsFile);
        }
    }
}
