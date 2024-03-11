// Copyright © Clinton Ingram and Contributors.  Licensed under the MIT License.

//------------------------------------------------------------------------------
//	<auto-generated>
//		This code was generated from a template.
//		Manual changes will be overwritten if the code is regenerated.
//	</auto-generated>
//------------------------------------------------------------------------------

using System;

#if BLAKE_CRYPTOGRAPHY
using System.Security.Cryptography;
#endif

using Blake2Fast.Implementation;

namespace Blake2Fast;

/// <summary>Static helper methods for BLAKE2b hashing.</summary>
#if BLAKE_PUBLIC
public
#else
internal
#endif
static class Blake2b
{
	/// <summary>The default hash digest length in bytes.  For BLAKE2b, this value is 64.</summary>
	public const int DefaultDigestLength = Blake2bHashState.HashBytes;

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(ReadOnlySpan<byte> source) => HashData(DefaultDigestLength, default, source);

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(int digestLength, ReadOnlySpan<byte> source) => HashData(digestLength, default, source);

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> source) => HashData(DefaultDigestLength, key, source);

	/// <summary>Perform a one-shot BLAKE2b hash computation.</summary>
	/// <remarks>If you have all the input available at once, this is the most efficient way to calculate the hash.</remarks>
	/// <param name="digestLength">The hash digest length in bytes.  Valid values are 1 to 64.</param>
	/// <param name="key">0 to 64 bytes of input for initializing a keyed hash.</param>
	/// <param name="source">The message bytes to hash.</param>
	/// <returns>The computed hash digest from the message bytes in <paramref name="source" />.</returns>
	public static byte[] HashData(int digestLength, ReadOnlySpan<byte> key, ReadOnlySpan<byte> source)
	{
		var hs = default(Blake2bHashState);
		hs.Init(digestLength, key);
		hs.Update(source);
		return hs.Finish();
	}

	/// <inheritdoc cref="HashData(ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte})" />
	public static void HashData(ReadOnlySpan<byte> source, Span<byte> destination) => HashData(DefaultDigestLength, default, source, destination);

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte})" />
	public static void HashData(int digestLength, ReadOnlySpan<byte> source, Span<byte> destination) => HashData(digestLength, default, source, destination);

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte})" />
	/// <param name="destination">Destination buffer into which the hash digest is written.  The buffer must have a capacity of at least <see cref="DefaultDigestLength" />(64) bytes.</param>
	public static void HashData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination) => HashData(DefaultDigestLength, key, source, destination);

	/// <summary>Perform a one-shot BLAKE2b hash computation and write the hash digest to <paramref name="destination" />.</summary>
	/// <remarks>If you have all the input available at once, this is the most efficient way to calculate the hash.</remarks>
	/// <param name="digestLength">The hash digest length in bytes.  Valid values are 1 to 64.</param>
	/// <param name="key">0 to 64 bytes of input for initializing a keyed hash.</param>
	/// <param name="source">The message bytes to hash.</param>
	/// <param name="destination">Destination buffer into which the hash digest is written.  The buffer must have a capacity of at least <paramref name="digestLength" /> bytes.</param>
	public static void HashData(int digestLength, ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination)
	{
		if (destination.Length < digestLength)
			throw new ArgumentException($"Output buffer must have a capacity of at least {digestLength} bytes.", nameof(destination));

		var hs = default(Blake2bHashState);
		hs.Init(digestLength, key);
		hs.Update(source);
		hs.Finish(destination);
	}

	/// <inheritdoc cref="CreateIncrementalHasher(int, ReadOnlySpan{byte})" />
	public static Blake2bHashState CreateIncrementalHasher() => CreateIncrementalHasher(DefaultDigestLength, default);

	/// <inheritdoc cref="CreateIncrementalHasher(int, ReadOnlySpan{byte})" />
	public static Blake2bHashState CreateIncrementalHasher(int digestLength) => CreateIncrementalHasher(digestLength, default);

	/// <inheritdoc cref="CreateIncrementalHasher(int, ReadOnlySpan{byte})" />
	public static Blake2bHashState CreateIncrementalHasher(ReadOnlySpan<byte> key) => CreateIncrementalHasher(DefaultDigestLength, key);

	/// <summary>Create and initialize an incremental BLAKE2b hash computation.</summary>
	/// <remarks>If you will receive the input in segments rather than all at once, this is the most efficient way to calculate the hash.</remarks>
	/// <param name="digestLength">The hash digest length in bytes.  Valid values are 1 to 64.</param>
	/// <param name="key">0 to 64 bytes of input for initializing a keyed hash.</param>
	/// <returns>An <see cref="Blake2bHashState" /> instance for updating and finalizing the hash.</returns>
	public static Blake2bHashState CreateIncrementalHasher(int digestLength, ReadOnlySpan<byte> key)
	{
		var hs = default(Blake2bHashState);
		hs.Init(digestLength, key);
		return hs;
	}

#if BLAKE_CRYPTOGRAPHY
	/// <inheritdoc cref="CreateHashAlgorithm(int)" />
	public static HashAlgorithm CreateHashAlgorithm() => CreateHashAlgorithm(DefaultDigestLength);

	/// <summary>Creates and initializes a <see cref="HashAlgorithm" /> instance that implements BLAKE2b hashing.</summary>
	/// <remarks>Use this only if you require an implementation of <see cref="HashAlgorithm" />.  It is less efficient than the direct methods.</remarks>
	/// <param name="digestLength">The hash digest length in bytes.  Valid values are 1 to 64.</param>
	/// <returns>A <see cref="HashAlgorithm" /> instance.</returns>
	public static HashAlgorithm CreateHashAlgorithm(int digestLength) => new BlakeHmac(BlakeHmac.Algorithm.Blake2b, digestLength, default);

	/// <inheritdoc cref="CreateHMAC(int, ReadOnlySpan{byte})" />
	public static HMAC CreateHMAC(ReadOnlySpan<byte> key) => CreateHMAC(DefaultDigestLength, key);

	/// <summary>Creates and initializes an <see cref="HMAC" /> instance that implements BLAKE2b keyed hashing.  Uses BLAKE2's built-in support for keyed hashing rather than the normal 2-pass approach.</summary>
	/// <remarks>Use this only if you require an implementation of <see cref="HMAC" />.  It is less efficient than the direct methods.</remarks>
	/// <param name="digestLength">The hash digest length in bytes.  Valid values are 1 to 64.</param>
	/// <param name="key">0 to 64 bytes of input for initializing the keyed hash.</param>
	/// <returns>An <see cref="HMAC" /> instance.</returns>
	public static HMAC CreateHMAC(int digestLength, ReadOnlySpan<byte> key) => new BlakeHmac(BlakeHmac.Algorithm.Blake2b, digestLength, key);
#endif

#if BLAKE_PUBLIC
	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(byte[] source) => HashData(DefaultDigestLength, default, new ReadOnlySpan<byte>(source));

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(int digestLength, byte[] source) => HashData(digestLength, default, new ReadOnlySpan<byte>(source));

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(byte[] key, byte[] source) => HashData(DefaultDigestLength, new ReadOnlySpan<byte>(key), new ReadOnlySpan<byte>(source));

	/// <inheritdoc cref="HashData(int, ReadOnlySpan{byte}, ReadOnlySpan{byte})" />
	public static byte[] HashData(int digestLength, byte[] key, byte[] source) => HashData(digestLength, new ReadOnlySpan<byte>(key), new ReadOnlySpan<byte>(source));

	private const string obsoleteMsg = "Use the new HashData method instead.";

#pragma warning disable 1591
	[Obsolete(obsoleteMsg)]
	public static byte[] ComputeHash(ReadOnlySpan<byte> input) => HashData(DefaultDigestLength, default, input);

	[Obsolete(obsoleteMsg)]
	public static byte[] ComputeHash(int digestLength, ReadOnlySpan<byte> input) => HashData(digestLength, default, input);

	[Obsolete(obsoleteMsg)]
	public static byte[] ComputeHash(ReadOnlySpan<byte> key, ReadOnlySpan<byte> input) => HashData(DefaultDigestLength, key, input);

	[Obsolete(obsoleteMsg)]
	public static byte[] ComputeHash(int digestLength, ReadOnlySpan<byte> key, ReadOnlySpan<byte> input) => HashData(digestLength, key, input);

	[Obsolete(obsoleteMsg)]
	public static void ComputeAndWriteHash(ReadOnlySpan<byte> input, Span<byte> output) => HashData(DefaultDigestLength, default, input, output);

	[Obsolete(obsoleteMsg)]
	public static void ComputeAndWriteHash(int digestLength, ReadOnlySpan<byte> input, Span<byte> output) => HashData(digestLength, default, input, output);

	[Obsolete(obsoleteMsg)]
	public static void ComputeAndWriteHash(ReadOnlySpan<byte> key, ReadOnlySpan<byte> input, Span<byte> output) => HashData(DefaultDigestLength, key, input, output);

	[Obsolete(obsoleteMsg)]
	public static void ComputeAndWriteHash(int digestLength, ReadOnlySpan<byte> key, ReadOnlySpan<byte> input, Span<byte> output) => HashData(digestLength, key, input, output);
#endif
}