// Copyright © Clinton Ingram and Contributors.  Licensed under the MIT License.

//------------------------------------------------------------------------------
//	<auto-generated>
//		This code was generated from a template.
//		Manual changes will be overwritten if the code is regenerated.
//	</auto-generated>
//------------------------------------------------------------------------------

#if HWINTRINSICS
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Blake2Fast.Implementation;

unsafe partial struct Blake2sHashState
{
	// SIMD algorithm described in https://eprint.iacr.org/2012/275.pdf
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	private static void mixSsse3(uint* sh, uint* m)
	{
		// This nonsense breaks CSE of these reads, ensuring JIT allocates low regsiters to them.
		nuint zero = Sse2.CompareGreaterThan(Vector128<int>.Zero, Vector128<int>.Zero).AsUInt32().ToScalar();
		byte* prm = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rormask)) + zero;
		var r16 = Sse2.LoadVector128(prm);
		var r8  = Sse2.LoadVector128(prm + Vector128<byte>.Count);

		uint* piv = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(ivle)) + zero;
		var row3 = Sse2.LoadVector128(piv);
		var row4 = Sse2.LoadVector128(piv + Vector128<uint>.Count);

		// Again breaking CSE, otherwise JIT wastes good registers caching these reads that aren't used again until the end.
		var row1 = Sse2.LoadVector128(sh + zero);
		var row2 = Sse2.LoadVector128(sh + zero + Vector128<uint>.Count);

		row4 = Sse2.Xor(row4, Sse2.LoadVector128(sh + Vector128<uint>.Count * 2)); // t[] and f[]

		var m0 = Sse2.LoadVector128(m);
		var m1 = Sse2.LoadVector128(m + Vector128<uint>.Count);
		var m2 = Sse2.LoadVector128(m + Vector128<uint>.Count * 2);
		var m3 = Sse2.LoadVector128(m + Vector128<uint>.Count * 3);

		//ROUND 1
		var b0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_10_00_10_00).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_11_01_11_01).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		var t0 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_10_00_10_00).AsUInt32();
		b0 = Sse2.Shuffle(t0, 0b_10_01_00_11);

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_11_01_11_01).AsUInt32();
		b0 = Sse2.Shuffle(t0, 0b_10_01_00_11);

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 2
		t0 = Sse.Shuffle(m3.AsSingle(), m1.AsSingle(), 0b_10_00_11_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), b0.AsSingle(), 0b_11_01_10_00).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(m2.AsSingle(), t0.AsSingle(), 0b_11_01_00_10).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_11_01_10_01).AsUInt32();
		var t1 = Sse2.Shuffle(m0.AsDouble(), m2.AsDouble(), 0b_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_11_00_00_10).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t1 = Sse2.Shuffle(m0.AsDouble(), m3.AsDouble(), 0b_01).AsUInt32();
		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_11_01_10_01).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 3
		t0 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_01_00_11_00).AsUInt32();
		t1 = Sse2.Shuffle(m1.AsDouble(), m3.AsDouble(), 0b_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_11_01_10_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(t0.AsSingle(), m0.AsSingle(), 0b_10_00_11_00).AsUInt32();
		b0 = Sse2.Shuffle(t0, 0b_01_11_10_00);

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_11_10_11_01).AsUInt32();
		b0 = Sse.Shuffle(m2.AsSingle(), t0.AsSingle(), 0b_11_01_10_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_00_10_10_00).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 4
		t1 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_10_01_11_00).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_01_10_01_11).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(m0.AsSingle(), m2.AsSingle(), 0b_10_01_01_00).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), m3.AsSingle(), 0b_10_00_01_10).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t1 = Sse2.UnpackHigh(m3, m0);
		b0 = Sse.Shuffle(t1.AsSingle(), m1.AsSingle(), 0b_00_01_01_10).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t1 = Sse.Shuffle(m1.AsSingle(), m2.AsSingle(), 0b_01_00_10_01).AsUInt32();
		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_00_11_01_10).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 5
		t0 = Sse2.UnpackHigh(m0, m2);
		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_01_00_00_11).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(m0.AsSingle(), m3.AsSingle(), 0b_11_10_11_00).AsUInt32();
		t1 = Sse.Shuffle(t0.AsSingle(), m1.AsSingle(), 0b_11_00_11_00).AsUInt32();
		b0 = Sse2.Shuffle(t1, 0b_01_10_11_00);

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t1 = Sse2.UnpackHigh(m2, m1);
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_01_10_10_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse2.UnpackLow(m3, m0);
		t1 = Sse2.UnpackLow(m3, m2);
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_01_00_11_10).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 6
		t0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_10_00_10_01).AsUInt32();
		t1 = Sse.Shuffle(m0.AsSingle(), m2.AsSingle(), 0b_11_00_11_00).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_10_00_11_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		var t2 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_01_00_10_01).AsUInt32();
		b0 = Sse.Shuffle(t2.AsSingle(), t1.AsSingle(), 0b_01_11_01_10).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t1 = Sse.Shuffle(m1.AsSingle(), m3.AsSingle(), 0b_11_10_11_01).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_11_01_10_00).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t1 = Sse.Shuffle(m1.AsSingle(), m3.AsSingle(), 0b_11_10_01_00).AsUInt32();
		b0 = Sse.Shuffle(t2.AsSingle(), t1.AsSingle(), 0b_10_01_11_00).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 7
		t0 = Sse2.Shuffle(m0.AsDouble(), m3.AsDouble(), 0b_00).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_00_10_01_10).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse2.Shuffle(m2.AsDouble(), m3.AsDouble(), 0b_01).AsUInt32();
		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_00_11_11_01).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t0 = Sse2.UnpackLow(m0, m2);
		t1 = Sse.Shuffle(m1.AsSingle(), m2.AsSingle(), 0b_11_01_11_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_10_00_00_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(t1.AsSingle(), m0.AsSingle(), 0b_10_11_01_11).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 8
		t0 = Sse2.Shuffle(m3.AsDouble(), m1.AsDouble(), 0b_10).AsUInt32();
		t1 = Sse2.Shuffle(m3.AsDouble(), m0.AsDouble(), 0b_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_11_00_11_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(m2.AsSingle(), m3.AsSingle(), 0b_11_10_11_00).AsUInt32();
		t1 = Sse.Shuffle(m0.AsSingle(), m2.AsSingle(), 0b_10_01_00_01).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_10_00_10_01).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t2 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_01_00_11_10).AsUInt32();
		b0 = Sse.Shuffle(t2.AsSingle(), t0.AsSingle(), 0b_00_11_11_00).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(t1.AsSingle(), m1.AsSingle(), 0b_10_00_01_11).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 9
		t0 = Sse2.UnpackHigh(m1, m3);
		t1 = Sse.Shuffle(m0.AsSingle(), m2.AsSingle(), 0b_11_00_00_11).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_01_11_01_00).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t0 = Sse.Shuffle(m3.AsSingle(), m2.AsSingle(), 0b_01_00_11_10).AsUInt32();
		b0 = Sse.Shuffle(t0.AsSingle(), t1.AsSingle(), 0b_10_00_11_01).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t0 = Sse2.UnpackLow(m3, m0);
		t1 = Sse2.Shuffle(m3.AsDouble(), m2.AsDouble(), 0b_10).AsUInt32();
		b0 = Sse.Shuffle(t1.AsSingle(), t0.AsSingle(), 0b_11_10_00_10).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		t1 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_01_00_01_10).AsUInt32();
		b0 = Sse.Shuffle(t1.AsSingle(), m1.AsSingle(), 0b_00_11_00_11).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		//ROUND 10
		t0 = Sse.Shuffle(m0.AsSingle(), m1.AsSingle(), 0b_11_10_01_00).AsUInt32();
		b0 = Sse.Shuffle(m2.AsSingle(), t0.AsSingle(), 0b_01_11_00_10).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(t1.AsSingle(), m1.AsSingle(), 0b_01_10_10_00).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//DIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_10_01_00_11);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_00_11_10_01);

		t0 = Sse.Shuffle(m0.AsSingle(), m2.AsSingle(), 0b_11_01_11_00).AsUInt32();
		b0 = Sse.Shuffle(m3.AsSingle(), t0.AsSingle(), 0b_01_10_11_01).AsUInt32();

		//G1
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r16).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 12), Sse2.ShiftLeftLogical(row2, 32 - 12));

		b0 = Sse.Shuffle(t0.AsSingle(), m3.AsSingle(), 0b_00_10_11_00).AsUInt32();

		//G2
		row1 = Sse2.Add(Sse2.Add(row1, b0), row2);
		row4 = Sse2.Xor(row4, row1);
		row4 = Ssse3.Shuffle(row4.AsByte(), r8).AsUInt32();

		row3 = Sse2.Add(row3, row4);
		row2 = Sse2.Xor(row2, row3);
		row2 = Sse2.Xor(Sse2.ShiftRightLogical(row2, 7), Sse2.ShiftLeftLogical(row2, 32 - 7));

		//UNDIAGONALIZE
		row1 = Sse2.Shuffle(row1, 0b_00_11_10_01);
		row4 = Sse2.Shuffle(row4, 0b_01_00_11_10);
		row3 = Sse2.Shuffle(row3, 0b_10_01_00_11);

		row1 = Sse2.Xor(row1, row3);
		row2 = Sse2.Xor(row2, row4);
		row1 = Sse2.Xor(row1, Sse2.LoadVector128(sh));
		row2 = Sse2.Xor(row2, Sse2.LoadVector128(sh + Vector128<uint>.Count));

		Sse2.Store(sh, row1);
		Sse2.Store(sh + Vector128<uint>.Count, row2);
	}
}
#endif