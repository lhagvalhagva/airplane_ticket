using System;

namespace CalculatorLibrary
{
    public class ExtendedCalculator
    {
        private double memoryValue = 0;

        public string Calculate(double firstNumber, double secondNumber, char operation)
        {
            try
            {
                return operation switch
                {
                    '+' => (firstNumber + secondNumber).ToString(),
                    '-' => (firstNumber - secondNumber).ToString(),
                    '*' => (firstNumber * secondNumber).ToString(),
                    '/' => secondNumber == 0
                           ? "Error"
                           : (firstNumber / secondNumber).ToString(),
                    _ => "Error"
                };
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public void MemorySave(double value)
        {
            memoryValue = value;
        }

        public double MemoryRecall()
        {
            return memoryValue;
        }

        public void MemoryClear()
        {
            memoryValue = 0;
        }
    }
}
