using System;
using System.IO;
using System.Collections.Generic;

namespace calculator
{
    public delegate double MathOperation(double a, double b);

    public delegate void ErrorNotificationType(string message);

    public class Calculator
    {
        // Словарь, в котором хранятся арифметические операции.
        static Dictionary<String, MathOperation> operations = new Dictionary<String, MathOperation>();

        // Событие, сообщяющее о возникших ошибках при записи файла.
        public event ErrorNotificationType ErrorNotification;
        // Событие, сообщяющее о возникших ошибках в вычислениях.
        public event ErrorNotificationType CalculatingError;

        /// <summary>
        /// Метод, вычисляющий значения каждого выражения, записанного в файл, и записывающий их в новый файл.
        /// </summary>
        /// <param name="expressionsFile">Директория файла, в котором хранятся выражения</param>
        /// <param name="answersFile">Директория файла, в который записываются значения выражений</param>
        public void CalculateFileExpressions(string expressionsFile, string answersFile)
        {
            // Массив выражений.
            string[] arrOfExprs = File.ReadAllLines(expressionsFile);

            Calculator calculator = new Calculator(operations);

            foreach (var expr in arrOfExprs)
            {
                // Разбиение выражения на операнды и операцию.
                string[] arguments = expr.Split();

                double operandA = double.Parse(arguments[0]);
                string operatorExpr = arguments[1];
                double operandB = double.Parse(arguments[2]);

                try
                {
                    // Запись в файл answersFile результатов выражений (каждый результат записывается с новой строчки).
                    File.AppendAllText(answersFile, $"{operations[operatorExpr](operandA, operandB):f3}" + Environment.NewLine);
                }
                catch (KeyNotFoundException)
                {
                    CalculatingError("неверный оператор" + Environment.NewLine);
                }
                catch (DivideByZeroException)
                {
                    CalculatingError("bruh" + Environment.NewLine);
                }
                catch (NotFiniteNumberException)
                {
                    CalculatingError("не число" + Environment.NewLine);
                }
                catch (FileNotFoundException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (IOException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (System.Security.SecurityException e)
                {
                    ErrorNotification(e.Message);
                }
            }
        }

        /// <summary>
        /// Метод, сравнивающий значения выражений, и записывающий результаты сравнений в файл.
        /// </summary>
        /// <param name="answersFile">Первый файл со значениями выражений</param>
        /// <param name="checkerFile">Второй файл со значениями выражений</param>
        /// <param name="resultsFile">Файл, в который записываются результаты сравнений</param>
        public void CheckResults(string answersFile, string checkerFile, string resultsFile)
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
                    Console.WriteLine(Environment.NewLine +
                        "Impossible to check the answers: different number of elements in answers and checker files." + Environment.NewLine +
                        $"There are {answers.Length} elements in first file and {checks.Length} in the second file.");
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
                ErrorNotification(e.Message);
            }
            catch (IOException e)
            {
                ErrorNotification(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification(e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification(e.Message);
            }
        }

        /// <summary>
        /// Метод, очищающий или создающий файлы по переданным директориям.
        /// </summary>
        /// <param name="paths">Директории файлов, которые требуется очистить/создать</param>
        public void ClearFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                try
                {
                    File.WriteAllText(path, "");
                }
                catch (FileNotFoundException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (IOException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    ErrorNotification(e.Message);
                }
                catch (System.Security.SecurityException e)
                {
                    ErrorNotification(e.Message);
                }
            }
        }


        // Конструктор.
        public Calculator(Dictionary<string, MathOperation> mathOperations)
        {
            operations = mathOperations;
        }
    }

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

        /// <summary>
        /// Метод, выводящий в консоль сообщение об ошибке.
        /// </summary>
        /// <param name="message">Сообщение, выводящееся в консоль</param>
        public static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}: {message}");
        }

        /// <summary>
        /// Записывает в файл answersFile сообщение message.
        /// </summary>
        /// <param name="message">Сообщение, записываемое в файл</param
        public static void ResultErrorHandler(string message)
        {
            File.AppendAllText(answersFile, "неверный оператор");
        }

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

        static void Main(string[] args)
        {
            Calculator calculator = new Calculator(operations);

            // Определение событий.
            calculator.ErrorNotification += ConsoleErrorHandler;
            calculator.CalculatingError += ResultErrorHandler;

            // Очистка/создание файлов.
            calculator.ClearFiles(answersFile, resultsFile);

            // Вычисление значений выражений и их запись в новый файл.
            calculator.CalculateFileExpressions(expressionsFile, answersFile);

            // Сравнение посчитанных результатов с результатами, предложенными в файле checkerFile.
            // Запись результатов сравнения в новый файл.
            calculator.CheckResults(answersFile, checkerFile, resultsFile);
        }
    }
}
