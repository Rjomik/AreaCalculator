using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AreaCalculator
{
    public class Calculator
    {
        static SortedDictionary<string, Figure> supportedEquestions = new SortedDictionary<string, Figure>();

        #region Task requests
        static Calculator()
        {
            var cube = new Figure("Circle");
            cube.AddFigureDefinition("PI*[r]*[r]", ("r", "Radius"));
            supportedEquestions.Add(cube.Name, cube);

            var triangle = new Figure("Triangle");
            triangle.AddFigureDefinition("sqrt(([a]+[b]+[c])/2 * (([a]+[b]+[c])/2 - [a]) * (([a]+[b]+[c])/2 - [b]) * (([a]+[b]+[c])/2 - [c]))", ("a", "Second side"), ("b", "First side"), ("c", "Third side"));
            supportedEquestions.Add(triangle.Name, triangle);
        }
        #endregion

        public static List<Figure> GetSupportedFigures()
        {
            return new List<Figure>(supportedEquestions.Values);
        }

        public static void AddCustomFigureDefinition(string figureName, string areaEquation, params (string Label, string Description)[] defintionParameters)
        {
            if (!supportedEquestions.TryGetValue(figureName, out var existing))
            {
                existing = new Figure(figureName);
                supportedEquestions.Add(figureName, existing);
            }
            existing.AddFigureDefinition(areaEquation, defintionParameters);
        }
    }

    public class FigureDefintionToAreaEquation
    {
        public Dictionary<string, string> ParametersDescriptions;
        public string AreaEquation;

        public FigureDefintionToAreaEquation(string areaEquation, params (string Name, string Description)[] defintionParameters)
        {
            AreaEquation = areaEquation;
            ParametersDescriptions = defintionParameters.ToDictionary(x => x.Name, x => x.Description);
        }

        public float Calculate(params (string ParameterName, float ParameterValue)[] values)
        {
            string parsedEquation = AreaEquation;

            foreach (var i in values.ToList().OrderByDescending(x => x.ParameterName.Length))
            {
                parsedEquation = parsedEquation.Replace($"[{i.ParameterName}]", $"({i.ParameterValue})");
            }
            return Convert.ToSingle( Z.Expressions.Eval.Execute(parsedEquation));
        }
    }

    public class Figure
    {
        public string Name;
        public List<FigureDefintionToAreaEquation> FigureDefintionToAreaEquations;


        internal Figure(string name)
        {
            this.Name = name;
            FigureDefintionToAreaEquations = new List<FigureDefintionToAreaEquation>();
        }

        public void AddFigureDefinition(string areaEquation, params (string Name, string Description)[] defintionParameters)
        {
            FigureDefintionToAreaEquations.Add(new FigureDefintionToAreaEquation(areaEquation, defintionParameters));
        }
    }
}