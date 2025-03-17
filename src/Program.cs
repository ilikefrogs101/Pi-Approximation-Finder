using System.Text;

namespace ilikefrogs101.PiApproximationFinder;
public static class PiFinder {
    private static readonly Dictionary<string, Option> options = new() {
        {
            "start-number",
            new Option(
                "The number to start the checking at",
                1
            )
        },
        {
            "end-number",
            new Option(
                "The number to end the checking at",
                10000
            )
        },
        {
            "accuracy",
            new Option(
                "How close the numerator must be to an integer to be recorded as valid",
                0.001
            )
        },
        {
            "pi",
            new Option(
                "The value of pi you are searching for approximations for (e.g. extra decimal points)",
                3.1415926535897932
            )
        },
        {
            "output",
            new Option(
                "The file to output the calculated approximations to",
                ""
            )
        },
        {
            "sort-type",
            new Option(
                "The method used to order the approximations (options are: sum, accuracy)",
                "accuracy"
            )
        },
        {
            "sort-mode",
            new Option(
                "The mode used to order the approximations (ascending or descending)",
                "ascending"
            )
        },
    };
    public static void Main(string[] arguments)
    {
        for (int i = 0; i < arguments.Length; ++i)
        {
            if (arguments[i] == "help")
            {
                Help();
                return;
            }

            if (arguments[i].StartsWith("--"))
            {
                string key = arguments[i][2..];
                if (options.TryGetValue(key, out Option? value) && i + 1 < arguments.Length)
                {
                    value.value = arguments[++i];
                }
            }
        }

        RunCalculations();
    }
    private static void Help() {
        StringBuilder helpMessageBuilder = new();
        helpMessageBuilder.AppendLine("Options:");
        for(int i = 0; i < options.Count; ++i)
        {
            KeyValuePair<string, Option> _option = options.ElementAt(i);
            helpMessageBuilder.AppendLine($"--{_option.Key}:\n{_option.Value.description}\nDefault: {_option.Value.value ?? "None"}");
        }
        Console.Write(helpMessageBuilder.ToString());
    }

    private static void RunCalculations() {
        HashSet<ulong> validNumerators = [];
        List<Tuple<double, double, double>> validApproximations = [];

        ulong startNumber = Convert.ToUInt64(options["start-number"].value);
        ulong endNumber = Convert.ToUInt64(options["end-number"].value);
        double accuracy = Convert.ToDouble(options["accuracy"].value);
        double pi = Convert.ToDouble(options["pi"].value);

        for(ulong _denominator = startNumber; _denominator <= endNumber; ++_denominator) {
            double _numerator = pi * _denominator;
            double _intNumerator = Math.Round(_numerator);
            double _accuracy = Math.Abs(pi - (_intNumerator/_denominator));

            if(_accuracy <= accuracy) {
                ulong GCD = Utils.GCD((ulong)_intNumerator, _denominator);
                ulong normalisedNumerator = (ulong)_intNumerator / GCD;

                if(validNumerators.Contains(normalisedNumerator)) {
                    continue;
                }
                validNumerators.Add(normalisedNumerator);

                Tuple<double, double, double> newApproximation = new(_intNumerator, _denominator, _accuracy);
                validApproximations.Add(newApproximation);
            }
        }
        
        string sortType = Convert.ToString(options["sort-type"].value) ?? string.Empty;
        string sortMode = Convert.ToString(options["sort-mode"].value) ?? string.Empty;

        if(sortType == "sum") {
            if(sortMode == "ascending") {
                validApproximations.Sort((t1, t2) => (t1.Item1 + t1.Item2).CompareTo(t2.Item1 + t2.Item2));
            }
            else {
                validApproximations.Sort((t1, t2) => -(t1.Item1 + t1.Item2).CompareTo(t2.Item1 + t2.Item2));
            }
        }
        else {
            if(sortMode == "ascending") {
                validApproximations.Sort((t1, t2) => t1.Item3.CompareTo(t2.Item3));
            }
            else {
                validApproximations.Sort((t1, t2) => -t1.Item3.CompareTo(t2.Item3));
            }
        }

        string outputFile = Convert.ToString(options["output"].value) ?? string.Empty;
        StringBuilder output = new StringBuilder();
        foreach (var approximation in validApproximations)
        {
            string nonSciAccuracy = approximation.Item3.ToString("F99").TrimEnd('0');
            if(nonSciAccuracy.Length == 2) {
                nonSciAccuracy = nonSciAccuracy.Replace(".", " ");
            }
            Console.WriteLine($"{approximation.Item1}/{approximation.Item2} is a valid pi approximation with an inaccuracy of {nonSciAccuracy}");
            output.AppendLine($"{approximation.Item1},{approximation.Item2},{nonSciAccuracy}");
        }
        if (!string.IsNullOrEmpty(outputFile))
        {
            File.WriteAllText(outputFile, output.ToString());
        }
    }
}
public class Option(string _description, object _value) {
    public string description = _description; 
    public object value = _value;
}
