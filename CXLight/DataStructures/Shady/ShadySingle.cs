namespace CXLight.DataStructures.Shady
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A 32-bit Numerical Data Structure with multiple-personality disorder
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ShadySingle
    {
        [FieldOffset(0)] public byte byte0;
        [FieldOffset(1)] public byte byte1;
        [FieldOffset(2)] public byte byte2;
        [FieldOffset(3)] public byte byte3;

        [FieldOffset(0)] public ushort ushort0;
        [FieldOffset(2)] public ushort ushort1;

        [FieldOffset(0)] public short short0;
        [FieldOffset(2)] public short short1;        

        [FieldOffset(0)] public uint uint0;

        [FieldOffset(0)] public int int0;

        [FieldOffset(0)] public float float0;        
    }
}
