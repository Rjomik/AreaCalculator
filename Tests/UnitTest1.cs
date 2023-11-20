using AreaCalculator;

namespace Tests
{
    public class UnitTest1
    {
        static string scenarios = "scenarios.txt";
        public static List<object[]> trianglesInputOutput = new List<object[]>();
        static UnitTest1()
        {
            var lines = File.ReadAllLines(scenarios);
            foreach(var i in lines)
            {
                var items = i.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x=>Convert.ToSingle(x)).ToList();
                trianglesInputOutput.Add(new object[] { items });
            }
        }

        [Theory]
        [MemberData(nameof(trianglesInputOutput))]
        public void Test1(List<float> inputOutput)
        {
            var index = 0;
            var value = Calculator.GetSupportedFigures().First(x => x.Name == "Triangle").FigureDefintionToAreaEquations.First()
                .Calculate(("a", inputOutput[index++]), ("b", inputOutput[index++]), ("c", inputOutput[index++]));
            Assert.True(Math.Abs( inputOutput[index]-value)< 0.01);
        }
    }
}