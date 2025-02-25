namespace GameEngine;

public static class Utils
{
    public static string SepThousands(long value)
    {
        var s = value.ToString();
        var ret = "";
        var len = 0;
        foreach (var c in s.Reverse())
        {
            if (len % 3 == 0)
            {
                ret += " ";
            }

            ret += c;
            len += 1;
        }

        return string.Join("", ret.Reverse()).Trim();
    }
}