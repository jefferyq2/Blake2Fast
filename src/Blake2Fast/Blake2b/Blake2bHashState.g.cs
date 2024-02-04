// Copyright © Clinton Ingram and Contributors.  Licensed under the MIT License.

//------------------------------------------------------------------------------
//	<auto-generated>
//		This code was generated from a template.
//		Manual changes will be overwritten if the code is regenerated.
//	</auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
#if HWINTRINSICS
using System.Runtime.Intrinsics.X86;
#endif

namespace Blake2Fast.Implementation;

/// <summary>Defines the state associated with an incremental BLAKE2b hashing operation.</summary>
/// <remarks>Instances of this struct must be created by <see cref="Blake2b.CreateIncrementalHasher()" />.  An instance created directly will be unusable.</remarks>
#if BLAKE_PUBLIC
public
#else
internal
#endif
unsafe partial struct Blake2bHashState : IBlake2Incremental
{
	internal const int WordSize = sizeof(ulong);
	internal const int BlockWords = 16;
	internal const int BlockBytes = BlockWords * WordSize;
	internal const int HashWords = 8;
	internal const int HashBytes = HashWords * WordSize;
	internal const int MaxKeyBytes = HashBytes;

	private fixed byte b[BlockBytes];
	private fixed ulong h[HashWords];
	private fixed ulong t[2];
	private fixed ulong f[2];
	private uint c;
	private uint outlen;

	private static ReadOnlySpan<byte> ivle => [
		0x08, 0xc9, 0xbc, 0xf3, 0x67, 0xe6, 0x09, 0x6a,
		0x3b, 0xa7, 0xca, 0x84, 0x85, 0xae, 0x67, 0xbb,
		0x2b, 0xf8, 0x94, 0xfe, 0x72, 0xf3, 0x6e, 0x3c,
		0xf1, 0x36, 0x1d, 0x5f, 0x3a, 0xf5, 0x4f, 0xa5,
		0xd1, 0x82, 0xe6, 0xad, 0x7f, 0x52, 0x0e, 0x51,
		0x1f, 0x6c, 0x3e, 0x2b, 0x8c, 0x68, 0x05, 0x9b,
		0x6b, 0xbd, 0x41, 0xfb, 0xab, 0xd9, 0x83, 0x1f,
		0x79, 0x21, 0x7e, 0x13, 0x19, 0xcd, 0xe0, 0x5b
	];

#if HWINTRINSICS
	private static ReadOnlySpan<byte> rormask => [
		3, 4, 5, 6, 7, 0, 1, 2, 11, 12, 13, 14, 15, 8, 9, 10, //r24
		2, 3, 4, 5, 6, 7, 0, 1, 10, 11, 12, 13, 14, 15, 8, 9  //r16
	];
#endif

	/// <inheritdoc />
	public readonly int DigestLength => (int)outlen;

	private void compress(ref byte input, uint offs, uint cb)
	{
		uint inc = Math.Min(cb, BlockBytes);

		fixed (byte* pinput = &input)
		fixed (Blake2bHashState* s = &this)
		{
			ulong* sh = s->h;
			byte* pin = pinput + offs;
			byte* end = pin + cb;

			do
			{
				t[0] += inc;
				if (t[0] < inc)
					t[1]++;

				ulong* m = (ulong*)pin;
#if HWINTRINSICS
#if NET8_0_OR_GREATER
				if (Avx512F.VL.IsSupported)
					mixAvx512(sh, m);
				else
#endif
				if (Avx2.IsSupported)
					mixAvx2(sh, m);
				else
				if (Sse41.IsSupported)
					mixSse41(sh, m);
				else
#endif
					mixScalar(sh, m);

				pin += inc;
			} while (pin < end);
		}
	}

	internal void Init(int digestLength = HashBytes, ReadOnlySpan<byte> key = default)
	{
		uint keylen = (uint)key.Length;

		if (!BitConverter.IsLittleEndian) ThrowHelper.NoBigEndian();
		if (digestLength == 0 || (uint)digestLength > HashBytes) ThrowHelper.DigestInvalidLength(HashBytes);
		if (keylen > MaxKeyBytes) ThrowHelper.KeyTooLong(MaxKeyBytes);

		outlen = (uint)digestLength;

		Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref h[0]), ref MemoryMarshal.GetReference(ivle), HashBytes);
		h[0] ^= 0x01010000u ^ (keylen << 8) ^ outlen;

		if (keylen != 0)
		{
			Unsafe.CopyBlockUnaligned(ref b[0], ref MemoryMarshal.GetReference(key), keylen);
			c = BlockBytes;
		}
	}

	private void update(ReadOnlySpan<byte> input)
	{
		if (outlen == 0) ThrowHelper.HashNotInitialized();
		if (f[0] != 0) ThrowHelper.HashFinalized();

		uint consumed = 0;
		uint remaining = (uint)input.Length;
		ref byte rinput = ref MemoryMarshal.GetReference(input);

		uint blockrem = BlockBytes - c;
		if ((c != 0) && (remaining > blockrem))
		{
			if (blockrem != 0)
				Unsafe.CopyBlockUnaligned(ref b[c], ref rinput, blockrem);

			c = 0;
			compress(ref b[0], 0, BlockBytes);
			consumed += blockrem;
			remaining -= blockrem;
		}

		if (remaining > BlockBytes)
		{
			uint cb = (remaining - 1) & ~((uint)BlockBytes - 1);
			compress(ref rinput, consumed, cb);
			consumed += cb;
			remaining -= cb;
		}

		if (remaining != 0)
		{
			Unsafe.CopyBlockUnaligned(ref b[c], ref Unsafe.Add(ref rinput, (int)consumed), remaining);
			c += remaining;
		}
	}

	/// <inheritdoc />
	public void Update<T>(ReadOnlySpan<T> input) where T : struct
	{
		ThrowHelper.ThrowIfIsRefOrContainsRefs<T>();

		update(MemoryMarshal.AsBytes(input));
	}

	/// <inheritdoc />
	public void Update<T>(Span<T> input) where T : struct => Update((ReadOnlySpan<T>)input);

	/// <inheritdoc />
	public void Update<T>(ArraySegment<T> input) where T : struct => Update((ReadOnlySpan<T>)input);

	/// <inheritdoc />
	public void Update<T>(T[] input) where T : struct => Update((ReadOnlySpan<T>)input);

	/// <inheritdoc />
	public void Update<T>(T input) where T : struct
	{
		ThrowHelper.ThrowIfIsRefOrContainsRefs<T>();

		if (sizeof(T) > BlockBytes - c)
		{
			Update(new ReadOnlySpan<byte>(&input, sizeof(T)));
			return;
		}

		if (f[0] != 0) ThrowHelper.HashFinalized();

		Unsafe.WriteUnaligned(ref b[c], input);
		c += (uint)sizeof(T);
	}

	private void finish(Span<byte> hash)
	{
		if (outlen == 0) ThrowHelper.HashNotInitialized();
		if (f[0] != 0) ThrowHelper.HashFinalized();

		if (c < BlockBytes)
			Unsafe.InitBlockUnaligned(ref b[c], 0, BlockBytes - c);

		f[0] = ~0ul;
		compress(ref b[0], 0, c);

		Unsafe.CopyBlockUnaligned(ref hash[0], ref Unsafe.As<ulong, byte>(ref h[0]), outlen);
	}

	/// <inheritdoc />
	public byte[] Finish()
	{
		byte[] hash = new byte[outlen];
		finish(hash);

		return hash;
	}

	/// <inheritdoc />
	public void Finish(Span<byte> output)
	{
		if ((uint)output.Length < outlen) ThrowHelper.OutputTooSmall(DigestLength);

		finish(output);
	}

	/// <inheritdoc />
	public bool TryFinish(Span<byte> output, out int bytesWritten)
	{
		if ((uint)output.Length < outlen)
		{
			bytesWritten = 0;
			return false;
		}

		finish(output);
		bytesWritten = (int)outlen;
		return true;
	}
}
