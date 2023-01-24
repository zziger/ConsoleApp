namespace ConsoleApp
{
    using System;

    internal class Program
    {
        private static void Main()
        {
            var reader = new DataReader();
            reader.ImportAndPrintData("data.csv");

            Console.ReadLine();
        }
    }
}
