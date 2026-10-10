using System.Runtime.CompilerServices;

namespace BlockGame.World;

readonly public struct ChunkData {
    readonly private int[] data = new int[Chunk.WIDTH * Chunk.HEIGHT * Chunk.WIDTH];

    public ChunkData() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(int x, int y, int z) {
        return x + (z * Chunk.WIDTH) + (y * Chunk.WIDTH * Chunk.WIDTH);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBlock(int x, int y, int z) {
        return data[GetIndex(x, y, z)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBlock(int x, int y, int z, int value) {
        data[GetIndex(x, y, z)] = value;
    }
}