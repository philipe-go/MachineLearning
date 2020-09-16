using System;
using HelloMachineLearningML.Model;

namespace HelloMachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Machine Learning!\n");
            Console.WriteLine("Enter your text below:");

            // Add input data
            var input = new ModelInput();
            input.SentimentText = Console.ReadLine();

            // Load model and predict output of sample data
            ModelOutput result = ConsumeModel.Predict(input);
            Console.WriteLine($"Text: {input.SentimentText}\nIs Toxic: {result.Prediction}");
        }
    }
}
