namespace CXLight.DataStructures.Shady
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A 64-bit Numerical Data Structure with multiple-personality disorder
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ShadyDouble
    {
        [FieldOffset(0)] public byte byte0;
        [FieldOffset(1)] public byte byte1;
        [FieldOffset(2)] public byte byte2;
        [FieldOffset(3)] public byte byte3;
        [FieldOffset(4)] public byte byte4;
        [FieldOffset(5)] public byte byte5;
        [FieldOffset(6)] public byte byte6;
        [FieldOffset(7)] public byte byte7;

        [FieldOffset(0)] public ushort ushort0;
        [FieldOffset(2)] public ushort ushort1;
        [FieldOffset(4)] public ushort ushort2;
        [FieldOffset(6)] public ushort ushort3;

        [FieldOffset(0)] public short short0;
        [FieldOffset(2)] public short short1;
        [FieldOffset(4)] public short short2;
        [FieldOffset(6)] public short short3;

        [FieldOffset(0)] public uint uint0;
        [FieldOffset(4)] public uint uint1;

        [FieldOffset(0)] public int int0;
        [FieldOffset(4)] public int int1;

        [FieldOffset(0)] public float float0;
        [FieldOffset(4)] public float float1;

        [FieldOffset(0)] public ulong ulong0;

        [FieldOffset(0)] public long long0;

        [FieldOffset(0)] public double double0;
    }
}
