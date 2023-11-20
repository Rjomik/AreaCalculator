// See https://aka.ms/new-console-template for more information
using AreaCalculator;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
while (true)
{
    try
    {
        Console.WriteLine("What to do?\n1 - Calculate area for existing figures\n2 - Add new figure");
        int mode = Convert.ToInt32(Console.ReadLine());
        switch (mode)
        {
            case 1:
                Console.WriteLine("Pick a figure");
                var figures = Calculator.GetSupportedFigures();
                int index = 1;
                foreach (var i in figures)
                {
                    Console.WriteLine($"{index++} - {i.Name}");
                }
                mode = Convert.ToInt32(Console.ReadLine());
                mode--;
                if (mode < 0 || mode >= figures.Count)
                    continue;

                var targetFigure = figures[mode];

                FigureDefintionToAreaEquation targetParameters;

                if (targetFigure.FigureDefintionToAreaEquations.Count > 1)
                {
                    Console.WriteLine("Pick a definition parameters");
                    index = 1;
                    foreach (var i in targetFigure.FigureDefintionToAreaEquations)
                    {
                        Console.WriteLine($"{index++}:");
                        Console.WriteLine($" Parameters:");
                        foreach (var j in i.ParametersDescriptions)
                            Console.WriteLine($"  {j.Key} - {j.Value}");
                    }

                    mode = Convert.ToInt32(Console.ReadLine());
                    mode--;

                    if (mode < 0 || mode >= targetFigure.FigureDefintionToAreaEquations.Count)
                        continue;

                    targetParameters = targetFigure.FigureDefintionToAreaEquations[mode];
                }
                else
                {
                    targetParameters = targetFigure.FigureDefintionToAreaEquations.First();
                    Console.WriteLine($" Parameters:");
                    foreach (var j in targetParameters.ParametersDescriptions)
                        Console.WriteLine($"  {j.Key} - {j.Value}");
                 
                }

               
                (string Name, float Value)[] values = new (string Name, float Value)[targetParameters.ParametersDescriptions.Count];
                index = 0;
                foreach (var i in targetParameters.ParametersDescriptions)
                {
                    Console.Write($"{i.Key} = ");
                    values[index++] = (i.Key, Convert.ToSingle(Console.ReadLine()));
                }

                Console.WriteLine($"Area is {targetParameters.Calculate(values)}");

                break;
            case 2:
                Console.Write("Write figure name: ");
                string figureName = Console.ReadLine();
                Console.Write("Write number of parameters: ");
                int nParameters = Convert.ToInt32(Console.ReadLine());
                (string Name, string Descriptions)[] parameters = new (string Name, string Descriptions)[nParameters];
                for(int i = 0; i < nParameters; i++)
                {
                    Console.Write("Parameter Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Parameter Description (May be skipped): ");
                    parameters[i] = (name, Console.ReadLine());
                }
                Console.Write("Type Area equation (parameters should be quoted into '[]' e.g. sqrt(PI*10+[a]): ");
                string areaEquation = Console.ReadLine();
                Calculator.AddCustomFigureDefinition(figureName, areaEquation, parameters);
                break;
            default:
                continue;
        }
    }
    catch
    {

    }
}