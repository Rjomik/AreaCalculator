using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AreaCalculator
{
    /// <summary>
    /// Main class for creating figures definitions which can be used afterwards for quick area calculations
    /// </summary>
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

        /// <summary>
        /// Gets the list of all supported figures and how they can be defined
        /// </summary>
        /// <returns></returns>
        public static List<Figure> GetSupportedFigures()
        {
            return new List<Figure>(supportedEquestions.Values);
        }

        /// <summary>
        /// Add a new figure definition for which you know how to calculate an area
        /// </summary>
        /// <param name="figureName">Type a name, if it already exists you will add a new definition to a figure (e.g. rectangle can be defined by verticies or 1 side)</param>
        /// <param name="areaEquation">Equation to caculate an area. You may use 'Math.*' functions. All parameters must be quoted into '[]' e.g. sqrt(PI*10+[a])</param>
        /// <param name="defintionParameters">Tuples of parameters with defintions that needs to be provided to calculate an area</param>
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
        public TriangleDefinition(string areaEquation, params (string Name, string Description)[] defintionParameters) : base(areaEquation, defintionParameters)
        {

        }

        public override float Calculate(params (string ParameterName, float ParameterValue)[] values)
        {
            var sorted = values.Select(x => x.ParameterValue).ToList();
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

        /// <summary>
        /// Calculates an area using provided parameters
        /// </summary>
        /// <param name="values">List of parameters, careful, names must match the defintion</param>
        public virtual float Calculate(params (string ParameterName, float ParameterValue)[] values)
        {
            string parsedEquation = AreaEquation;

            foreach (var i in values.ToList().OrderByDescending(x => x.ParameterName.Length))
            {
                parsedEquation = parsedEquation.Replace($"[{i.ParameterName}]", $"({i.ParameterValue})");
            }
            return Convert.ToSingle(Z.Expressions.Eval.Execute(parsedEquation));
        }
    }

    /// <summary>
    /// Its a figure defintion, here you can access all possible ways to define it and get an area
    /// </summary>
    public class Figure
    {
        public string Name;
        internal List<FigureDefintionToAreaEquation> figureDefintionToAreaEquations;


        internal Figure(string name)
        {
            this.Name = name;
            figureDefintionToAreaEquations = new List<FigureDefintionToAreaEquation>();
        }

        internal void AddFigureDefinition(string areaEquation, params (string Name, string Description)[] defintionParameters)
        {
            figureDefintionToAreaEquations.Add(new FigureDefintionToAreaEquation(areaEquation, defintionParameters));
        }

        /// <summary>
        /// List of available ways to get an area
        /// </summary>
        public List<FigureDefintionToAreaEquation> FigureDefintionToAreaEquations { get => new List<FigureDefintionToAreaEquation>(figureDefintionToAreaEquations); }
    }
}