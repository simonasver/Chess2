using Backend.Utilities.Interpreter.Enums;
using System.Text.RegularExpressions;

namespace Backend.Utilities.Interpreter;

public static class Interpreter
{
    /// Order should be: unit? action direction length/amount of moves
    public static InterpretedCommand? Interpret(string text)
    {
        string[] tokens = text.Split(' ');

        var unit = InterpretUnit(tokens[0]);

        var currentIndex = 0;

        if (unit is not null)
        {
            currentIndex = 1;
        }

        var action = InterpretAction(tokens[currentIndex]);
        currentIndex++;

        if (action is null)
        {
            return null;
        }

        var nullDirection = InterpretDirection(tokens[currentIndex]);
        currentIndex++;

        if (nullDirection is null)
        {
            return null;
        }

        var direction = nullDirection.Value;

        switch (action)
        {
            case Enums.Action.Shoot:
                var nullLength = InterpretLength(tokens[currentIndex]);

                if (nullLength is null)
                {
                    return null;
                }

                var length = nullLength.Value;

                return new InterpretedShootCommand(unit, direction, length);

            case Enums.Action.Move:
                var nullAmount = ParseNumber(tokens[currentIndex]);

                if (nullAmount is null)
                {
                    return null;
                }

                var amount = nullAmount.Value;

                return new InterpretedMoveCommand(unit, direction, amount);

            default:
                return null;
        }
    }

    private static SelectedUnit? InterpretUnit(string token)
    {
        if (IsHeli(token))
        {
            return SelectedUnit.Helicopter;
        }

        if (IsTank(token))
        {
            return SelectedUnit.Tank;
        }

        return null;
    }

    private static Enums.Action? InterpretAction(string token)
    {
        if (IsShoot(token))
        {
            return Enums.Action.Shoot;
        }

        if (IsMove(token))
        {
            return Enums.Action.Move;
        }

        return null;
    }

    private static Direction? InterpretDirection(string token)
    {
        if (IsUp(token))
        {
            return Direction.Up;
        }

        if (IsDown(token))
        {
            return Direction.Down;
        }

        if (IsLeft(token))
        {
            return Direction.Left;
        }

        if (IsRight(token))
        {
            return Direction.Right;
        }

        return null;
    }

    private static Length? InterpretLength(string token)
    {
        if (IsShort(token))
        {
            return Length.Short;
        }

        if (IsLong(token))
        {
            return Length.Long;
        }

        return null;
    }

    private static int? ParseNumber(string token)
    {
        try
        {
            return int.Parse(token);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsHeli(string token)
    {
        string pattern = @"^(heli|copter)+$";

        return IsMatching(token, pattern);
    }

    private static bool IsTank(string token)
    {
        string pattern = @"^tank$";

        return IsMatching(token, pattern);
    }

    private static bool IsUp(string token)
    {
        string pattern = @"^(u(p)?|\^)$";

        return IsMatching(token, pattern);
    }

    private static bool IsDown(string token)
    {
        string pattern = @"^(d(own)?|V)$";

        return IsMatching(token, pattern);
    }

    private static bool IsLeft(string token)
    {
        string pattern = @"^(l(eft)?|<)$";

        return IsMatching(token, pattern);
    }

    private static bool IsRight(string token)
    {
        string pattern = @"^(r(ight)?|>)$";

        return IsMatching(token, pattern);
    }

    private static bool IsShoot(string token)
    {
        string pattern = @"^(p[eo]w|s(h(oot)?)?|b(am|ang|oom)|murder|k(apow|ill)|d(estroy|amage))$";

        return IsMatching(token, pattern);
    }

    private static bool IsMove(string token)
    {
        string pattern = @"^(m(v|o+ve)?|r(u+n)?|w(alk)?)$";

        return IsMatching(token, pattern);
    }

    private static bool IsLong(string token)
    {
        string pattern = @"^(l(o+ng)?)$";

        return IsMatching(token, pattern);
    }

    private static bool IsShort(string token)
    {
        string pattern = @"^(s(h(o+rt)?)?)$";

        return IsMatching(token, pattern);
    }

    private static bool IsMatching(string token, string exp)
    {
        Regex regex = new Regex(exp, RegexOptions.IgnoreCase);

        return regex.IsMatch(token);
    }
}