namespace ilikefrogs101.PiApproximationFinder;
public static class Utils {
    public static ulong GCD(ulong a, ulong b)
    {
        while (b > 0)
        {
            ulong rem = a % b;
            a = b;
            b = rem;
        }
        return a;
    }
}
