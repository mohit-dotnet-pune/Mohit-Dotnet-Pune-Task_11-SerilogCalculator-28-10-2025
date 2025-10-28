using System;
using System.IO;
using Serilog;

namespace SerilogCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var logsPath = Path.Combine(projectDir, "Logs");

            // Ensure the Logs folder exists
            Directory.CreateDirectory(logsPath);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logsPath, "calculator.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("=== Application Started ===");

            try
            {
                Console.WriteLine("===== Serilog Powered Calculator =====");

                Console.Write("Enter first number: ");
                var input1 = Console.ReadLine();
                if (!double.TryParse(input1, out var num1))
                {
                    Log.Error("Invalid first number input: {Input}", input1);
                    throw new ArgumentException("Invalid number input!");
                }

                Console.Write("Enter operator (+, -, *, /): ");
                var op = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(op) || "+-*/".IndexOf(op) == -1)
                {
                    Log.Error("Invalid operator entered: {Operator}", op);
                    throw new ArgumentException("Invalid operator!");
                }

                Console.Write("Enter second number: ");
                var input2 = Console.ReadLine();
                if (!double.TryParse(input2, out var num2))
                {
                    Log.Error("Invalid second number input: {Input}", input2);
                    throw new ArgumentException("Invalid number input!");
                }

                Log.Information("Inputs received: Num1={Num1}, Num2={Num2}, Operator={Operator}", num1, num2, op);

                double result;
                switch (op)
                {
                    case "+": result = num1 + num2; break;
                    case "-": result = num1 - num2; break;
                    case "*": result = num1 * num2; break;
                    case "/":
                        if (num2 == 0)
                        {
                            Log.Error("Divide by zero error. Num1={Num1}, Num2={Num2}", num1, num2);
                            throw new DivideByZeroException("Cannot divide by zero!");
                        }
                        result = num1 / num2;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid operator!");
                }

                Console.WriteLine($"Result: {result}");
                Log.Information("Calculation successful: {Num1} {Operator} {Num2} = {Result}", num1, op, num2, result);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occurred during calculation");
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Log.Information("=== Application Ended ===");
                Log.CloseAndFlush();
            }
        }
    }
}
