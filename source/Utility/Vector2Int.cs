
using System.Runtime.CompilerServices;

public readonly struct Vector2Int(int x, int z) {
    public readonly int X = x;
    public readonly int Z = z;

    public static Vector2Int Zero => new(0, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.X + b.X, a.Z + b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new(a.X - b.X, a.Z - b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"[{X}, {Z}]";
}