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
            triangle.figureDefintionToAreaEquations.Add(new TriangleDefinition("sqrt(([a]+[b]+[c])/2 * (([a]+[b]+[c])/2 - [a]) * (([a]+[b]+[c])/2 - [b]) * (([a]+[b]+[c])/2 - [c]))", ("a", "Second side"), ("b", "First side"), ("c", "Third side")));
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

    internal class TriangleDefinition : FigureDefintionToAreaEquation
    {
        public TriangleDefinition(string areaEquation, params (string Name, string Description)[] defintionParameters) : base(areaEquation,defintionParameters)
        {

        }

        public override float Calculate(params (string ParameterName, float ParameterValue)[] values)
        {
            var sorted = values.Select(x=>x.ParameterValue).ToList();
            sorted.Sort();
            if (Math.Pow(sorted[0], 2) + Math.Pow(sorted[1], 2) == Math.Pow(sorted[2], 2))
                return (sorted[0] * sorted[1]) / 2;
            else
                return base.Calculate(values);
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

        public virtual float Calculate(params (string ParameterName, float ParameterValue)[] values)
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
        internal List<FigureDefintionToAreaEquation> figureDefintionToAreaEquations;


        internal Figure(string name)
        {
            this.Name = name;
            figureDefintionToAreaEquations = new List<FigureDefintionToAreaEquation>();
        }

        public void AddFigureDefinition(string areaEquation, params (string Name, string Description)[] defintionParameters)
        {
            figureDefintionToAreaEquations.Add(new FigureDefintionToAreaEquation(areaEquation, defintionParameters));
        }

        public List<FigureDefintionToAreaEquation> FigureDefintionToAreaEquations { get => new List<FigureDefintionToAreaEquation>(figureDefintionToAreaEquations); }
    }
}