namespace ByteBurrow.Core;

public class SaveableVersion
{
    public int major;
    public int minor;
    public int patch;
    
    public SaveableVersion(string fromString)
    {
        var values = fromString.Trim().Split(".");
        if(values.Length != 3) 
            throw new FormatException();
        
        major = int.Parse(values[0]);
        minor = int.Parse(values[1]);
        patch = int.Parse(values[2]);
    }

    public int CompareTo(SaveableVersion? other)
    {
        if (other == null) return 1;

        if (major != other.major) return major.CompareTo(other.major);
        if (minor != other.minor) return minor.CompareTo(other.minor);
        return patch.CompareTo(other.patch);
    }

    public static bool operator ==(SaveableVersion? left, SaveableVersion? right)
    {
        if (ReferenceEquals(left, right)) return true; // same object or both null
        if (left is null || right is null) return false; // only one is null
        return left.CompareTo(right) == 0;
    }

    public static bool operator !=(SaveableVersion? left, SaveableVersion? right) => !(left == right);

    public static bool operator >(SaveableVersion? left, SaveableVersion? right)
    {
        if (left is null) return false; // null is never greater
        if (right is null) return true; // non-null > null
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(SaveableVersion? left, SaveableVersion? right)
    {
        if (left is null) return right is null;
        if (right is null) return true;
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <(SaveableVersion? left, SaveableVersion? right)
    {
        if (left is null) return right is not null;
        if (right is null) return false;
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(SaveableVersion? left, SaveableVersion? right)
    {
        if (left is null) return true;
        if (right is null) return false;
        return left.CompareTo(right) <= 0;
    }

    public override bool Equals(object? obj) => obj is SaveableVersion other && this == other;
    public override int GetHashCode() => (major, minor, patch).GetHashCode();

    public override string ToString() => $"{major}.{minor}.{patch}";
}