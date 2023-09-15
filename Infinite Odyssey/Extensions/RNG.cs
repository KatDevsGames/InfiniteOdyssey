using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class ConsumesRNGAttribute : Attribute
{
    public int MinConsumption { get; }

    public int MaxConsumption { get; }

    public ConsumesRNGAttribute() => MinConsumption = MaxConsumption = -1;

    public ConsumesRNGAttribute(int consumption) => MinConsumption = MaxConsumption = consumption;

    public ConsumesRNGAttribute(int minConsumption, int maxConsumption)
    {
        MinConsumption = minConsumption;
        MaxConsumption = maxConsumption;
    }
}

[SuppressMessage("ReSharper", "IdentifierTypo")]
public class RNG
{
    private const int MERS_N = 624;
    private const int MERS_M = 397;
    private const int MERS_U = 11;
    private const int MERS_S = 7;
    private const int MERS_T = 15;
    private const int MERS_L = 18;
    private const uint MERS_A = 0x9908B0DF;
    private const uint MERS_B = 0x9D2C5680;
    private const uint MERS_C = 0xEFC60000;

    [JsonProperty(PropertyName = "state")]
    private uint[] mt = new uint[MERS_N]; // state vector

    [JsonProperty(PropertyName = "index")]
    private uint mti;                     // index into mt

    public int Consumption { get; private set; }
        
    public RNG(string seed) : this(seed.GetHashCode()) { }
    public RNG(int seed) : this(unchecked((uint)seed)) { }
    public RNG(uint seed) => RandomInit(seed);
    public RNG(long seed) : this(unchecked((ulong)seed)) { }
    public RNG(ulong seed) => RandomInitByArray(unchecked(new[] { (uint)seed, (uint)(seed >> 32) }));
    public RNG(params uint[] seed) => RandomInitByArray(seed);

    [JsonConstructor]
    private RNG(uint[] mt, uint mti)
    {
        this.mt = mt;
        this.mti = mti;
    }

    private void RandomInit(uint seed)
    {
        mt[0] = seed;
        for (mti = 1; mti < MERS_N; mti++)
            mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
    }

    private void RandomInitByArray(uint[] seeds)
    {
        // seed by more than 32 bits
        uint i, j;
        int k;
        int length = seeds.Length;
        RandomInit(19650218U);
        if (length <= 0) return;
        i = 1; j = 0;
        k = (MERS_N > length ? MERS_N : length);
        for (; k != 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + seeds[j] + j;
            i++; j++;
            if (i >= MERS_N) { mt[0] = mt[MERS_N - 1]; i = 1; }
            if (j >= length) j = 0;
        }
        for (k = MERS_N - 1; k != 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i;
            if (++i >= MERS_N) { mt[0] = mt[MERS_N - 1]; i = 1; }
        }
        mt[0] = 0x80000000U; // MSB is 1; assuring non-zero initial array
    }

    /// <returns>random integer in the interval min <= x <= max</returns>
    public int IRandom(int min, int max)
    {
        int r;
        r = (int)((max - min + 1) * RandomDouble()) + min; // multiply interval with random and truncate
        if (r > max)
            r = max;
        if (max < min)
            return int.MinValue;
        return r;
    }
        
    /// <returns>random integer in the interval min <= x <= max</returns>
    public uint IRandom(uint min, uint max)
    {
        uint r;
        r = (uint)((max - min + 1) * RandomDouble()) + min; // multiply interval with random and truncate
        if (r > max)
            r = max;
        if (max < min)
            return uint.MaxValue;
        return r;
    }

    public double RandomDouble()
    {
        // output random float number in the interval 0 <= x < 1
        uint r = RandomUInt32(); // get 32 random bits
        if (BitConverter.IsLittleEndian)
        {
            byte[] i0 = BitConverter.GetBytes((r << 20));
            byte[] i1 = BitConverter.GetBytes(((r >> 12) | 0x3FF00000));
            byte[] bytes = { i0[0], i0[1], i0[2], i0[3], i1[0], i1[1], i1[2], i1[3] };
            double f = BitConverter.ToDouble(bytes, 0);
            return f - 1.0;
        }
        return r * (1.0 / (0xFFFFFFFF + 1.0));
    }

    public long RandomInt64() => unchecked((long)RandomUInt64());

    public ulong RandomUInt64() => RandomUInt32() | (((ulong)RandomUInt32()) << 32);

    public int RandomInt32() => unchecked((int)RandomUInt32());

    public uint RandomUInt32()
    {
        // generate 32 random bits
        uint y;

        if (mti >= MERS_N)
        {
            const uint LOWER_MASK = 2147483647;
            const uint UPPER_MASK = 0x80000000;
            uint[] mag01 = { 0, MERS_A };

            int kk;
            for (kk = 0; kk < MERS_N - MERS_M; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + MERS_M] ^ (y >> 1) ^ mag01[y & 1];
            }

            for (; kk < MERS_N - 1; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + (MERS_M - MERS_N)] ^ (y >> 1) ^ mag01[y & 1];
            }

            y = (mt[MERS_N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            mt[MERS_N - 1] = mt[MERS_M - 1] ^ (y >> 1) ^ mag01[y & 1];
            mti = 0;
        }

        y = mt[mti++];

        // Tempering (May be omitted):
        y ^= y >> MERS_U;
        y ^= (y << MERS_S) & MERS_B;
        y ^= (y << MERS_T) & MERS_C;
        y ^= y >> MERS_L;

        Consumption++;
        return y;
    }
}