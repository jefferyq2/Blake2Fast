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

/// <summary>Defines the state associated with an incremental BLAKE2s hashing operation.</summary>
/// <remarks>Instances of this struct must be created by <see cref="Blake2s.CreateIncrementalHasher()" />.  An instance created directly will be unusable.</remarks>
#if BLAKE_PUBLIC
public
#else
internal
#endif
unsafe partial struct Blake2sHashState : IBlake2Incremental
{
	internal const int WordSize = sizeof(uint);
	internal const int BlockWords = 16;
	internal const int BlockBytes = BlockWords * WordSize;
	internal const int HashWords = 8;
	internal const int HashBytes = HashWords * WordSize;
	internal const int MaxKeyBytes = HashBytes;

	private fixed byte b[BlockBytes];
	private fixed uint h[HashWords];
	private fixed uint t[2];
	private fixed uint f[2];
	private uint c;
	private uint outlen;

	private static ReadOnlySpan<byte> ivle => [
		0x67, 0xe6, 0x09, 0x6a,
		0x85, 0xae, 0x67, 0xbb,
		0x72, 0xf3, 0x6e, 0x3c,
		0x3a, 0xf5, 0x4f, 0xa5,
		0x7f, 0x52, 0x0e, 0x51,
		0x8c, 0x68, 0x05, 0x9b,
		0xab, 0xd9, 0x83, 0x1f,
		0x19, 0xcd, 0xe0, 0x5b
	];

#if HWINTRINSICS
	private static ReadOnlySpan<byte> rormask => [
		2, 3, 0, 1, 6, 7, 4, 5, 10, 11, 8, 9, 14, 15, 12, 13, //r16
		1, 2, 3, 0, 5, 6, 7, 4, 9, 10, 11, 8, 13, 14, 15, 12  //r8
	];
#endif

	/// <inheritdoc />
	public readonly int DigestLength => (int)outlen;

	private void compress(ref byte input, uint cb)
	{
		uint inc = Math.Min(cb, BlockBytes);

		fixed (byte* pinput = &input)
		fixed (Blake2sHashState* s = &this)
		{
			uint* sh = s->h;
			uint* st = s->t;
			byte* pin = pinput;
			byte* end = pin + cb;

			do
			{
				*(ulong*)st += inc;

				uint* m = (uint*)pin;
#if HWINTRINSICS
#if NET8_0_OR_GREATER
				if (Avx512F.VL.IsSupported)
					mixAvx512(sh, m);
				else
#endif
				if (Ssse3.IsSupported)
					mixSsse3(sh, m);
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

		Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref h[0]), ref MemoryMarshal.GetReference(ivle), HashBytes);
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

		uint remaining = (uint)input.Length;
		ref byte rinput = ref MemoryMarshal.GetReference(input);

		uint blockrem;
		if ((c != 0) && (remaining > (blockrem = BlockBytes - c)))
		{
			if (blockrem != 0)
			{
				Unsafe.CopyBlockUnaligned(ref b[c], ref rinput, blockrem);
				rinput = ref Unsafe.Add(ref rinput, (nint)blockrem);
				remaining -= blockrem;
			}

			compress(ref b[0], BlockBytes);
			c = 0;
		}

		if (remaining > BlockBytes)
		{
			uint cb = (remaining - 1) & ~((uint)BlockBytes - 1);
			compress(ref rinput, cb);
			rinput = ref Unsafe.Add(ref rinput, (nint)cb);
			remaining -= cb;
		}

		if (remaining != 0)
		{
			Unsafe.CopyBlockUnaligned(ref b[c], ref rinput, remaining);
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

		if (c > BlockBytes - (uint)sizeof(T))
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

		f[0] = ~0u;
		compress(ref b[0], c);

		ref byte hout = ref MemoryMarshal.GetReference(hash);
		if (hash.Length is HashBytes)
			Unsafe.CopyBlockUnaligned(ref hout, ref Unsafe.As<uint, byte>(ref h[0]), HashBytes);
		else
			Unsafe.CopyBlockUnaligned(ref hout, ref Unsafe.As<uint, byte>(ref h[0]), outlen);
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
