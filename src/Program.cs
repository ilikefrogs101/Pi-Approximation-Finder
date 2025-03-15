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
            helpMessageBuilder.AppendLine($"--{_option.Key}:\n{_option.Value.description}\nDefault: {_option.Value.value}");
        }
        Console.Write(helpMessageBuilder.ToString());
    }

    private static void RunCalculations() {
        HashSet<ulong> validNumerators = [];

        ulong startNumber = Convert.ToUInt64(options["start-number"].value);
        ulong endNumber = Convert.ToUInt64(options["end-number"].value);
        double accuracy = Convert.ToDouble(options["accuracy"].value);
        double pi = Convert.ToDouble(options["pi"].value);

        for(ulong _denominator = startNumber; _denominator <= endNumber; ++_denominator) {
            double _numerator = pi * _denominator;
            double _intNumerator = Math.Round(_numerator);
            double _accuracy = Math.Abs(_numerator - _intNumerator);

            if(_accuracy <= accuracy) {
                ulong GCD = Utils.GCD((ulong)_intNumerator, _denominator);
                ulong _nGCD = (ulong)_intNumerator / GCD;

                if(validNumerators.Contains(_nGCD)) {
                    continue;
                }
                validNumerators.Add(_nGCD);
                Console.WriteLine($"{_intNumerator}/{_denominator} is a valid pi approximation to an accuracy of {_accuracy}");
            }
        }
    }
}
public class Option(string _description, object _value) {
    public string description = _description; 
    public object value = _value;
}