﻿using System;
using System.Collections.Generic;
using System.Linq;
using de4dot.blocks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace eazdevirt
{
	/// <summary>
	/// Extensions for detecting original instruction type (opcode).
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Attempt to identify a virtual instruction with its original CIL opcode.
		/// </summary>
		/// <param name="ins">Virtual instruction</param>
		/// <exception cref="OriginalOpcodeUnknownException">Thrown if unable to identify original CIL opcode</exception>
		/// <remarks>What this method does could probably be better done through reflection/attributes</remarks>
		/// <returns>CIL opcode</returns>
		public static Code Identify(this EazVirtualInstruction ins)
		{
			if (ins.Is_Add())
				return Code.Add;
			else if (ins.Is_Add_Ovf())
				return Code.Add_Ovf;
			else if (ins.Is_Add_Ovf_Un())
				return Code.Add_Ovf_Un;
			else if (ins.Is_And())
				return Code.And;
			else if (ins.Is_Bge())
				return Code.Bge;
			else if (ins.Is_Blt())
				return Code.Blt;
			else if (ins.Is_Cgt())
				return Code.Cgt;
			else if (ins.Is_Clt())
				return Code.Clt;
			else if (ins.Is_Conv_R8())
				return Code.Conv_R8;
			else if (ins.Is_Div())
				return Code.Div;
			else if (ins.Is_Div_Un())
				return Code.Div_Un;
			else if (ins.Is_Ldarg())
				return Code.Ldarg;
			else if (ins.Is_Ldarg_S())
				return Code.Ldarg_S;
			else if (ins.Is_Ldarga())
				return Code.Ldarga;
			else if (ins.Is_Ldarga_S())
				return Code.Ldarga_S;
			else if (ins.Is_Ldarg_0())
				return Code.Ldarg_0;
			else if (ins.Is_Ldarg_1())
				return Code.Ldarg_1;
			else if (ins.Is_Ldarg_2())
				return Code.Ldarg_2;
			else if (ins.Is_Ldarg_3())
				return Code.Ldarg_3;
			else if (ins.Is_Ldc_I4_0())
				return Code.Ldc_I4_0;
			else if (ins.Is_Ldc_I4_1())
				return Code.Ldc_I4_1;
			else if (ins.Is_Ldc_I4_2())
				return Code.Ldc_I4_2;
			else if (ins.Is_Ldc_I4_3())
				return Code.Ldc_I4_3;
			else if (ins.Is_Ldc_I4_4())
				return Code.Ldc_I4_4;
			else if (ins.Is_Ldc_I4_5())
				return Code.Ldc_I4_5;
			else if (ins.Is_Ldc_I4_6())
				return Code.Ldc_I4_6;
			else if (ins.Is_Ldc_I4_7())
				return Code.Ldc_I4_7;
			else if (ins.Is_Ldc_I4_8())
				return Code.Ldc_I4_8;
			else if (ins.Is_Ldc_I4_M1())
				return Code.Ldc_I4_M1;
			else if (ins.Is_Or())
				return Code.Or;
			else if (ins.Is_Rem())
				return Code.Rem;
			else if (ins.Is_Rem_Un())
				return Code.Rem_Un;
			else if (ins.Is_Shl())
				return Code.Shl;
			else if (ins.Is_Shr())
				return Code.Shr;
			else if (ins.Is_Shr_Un())
				return Code.Shr_Un;
			else if (ins.Is_Starg())
				return Code.Starg;
			else if (ins.Is_Starg_S())
				return Code.Starg_S;
			else if (ins.Is_Sub())
				return Code.Sub;
			else if (ins.Is_Sub_Ovf())
				return Code.Sub_Ovf;
			else if (ins.Is_Sub_Ovf_Un())
				return Code.Sub_Ovf_Un;
			else if (ins.Is_Xor())
				return Code.Xor;

			throw new OriginalOpcodeUnknownException(ins);
		}

		public static Boolean TryIdentify(this EazVirtualInstruction ins, out Code code)
		{
			try
			{
				code = ins.Identify();
				return true;
			}
			catch (OriginalOpcodeUnknownException)
			{
				code = Code.UNKNOWN2;
				return false;
			}
		}

		public static Boolean Is_And(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirect(
				new Code[] { Code.Ldloc_S, Code.Ldloc_S, Code.And, Code.Callvirt, Code.Ldloc_0, Code.Ret }
			);
		}

		public static Boolean Is_Xor(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirect(
				new Code[] { Code.Ldloc_S, Code.Ldloc_S, Code.Xor, Code.Callvirt, Code.Ldloc_0, Code.Ret }
			);
		}

		public static Boolean Is_Shl(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirect(
				new Code[] { Code.Ldloc_S, Code.Ldloc_S, Code.Ldc_I4_S, Code.And, Code.Shl, Code.Stloc_S,
				Code.Newobj, Code.Stloc_0, Code.Ldloc_0, Code.Ldloc_S, Code.Callvirt, Code.Ldloc_0, Code.Ret }
			);
		}

		/// <summary>
		/// OpCode pattern seen in the Shr_* helper method.
		/// </summary>
		private static readonly Code[] Pattern_Shr = new Code[] {
			Code.Ldc_I4_S, Code.And, Code.Shr, Code.Callvirt, Code.Ldloc_0, Code.Ret
		};

		public static Boolean Is_Shr(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(true, Pattern_Shr);
		}

		public static Boolean Is_Shr_Un(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(false, Pattern_Shr);
		}

		/// <summary>
		/// OpCode pattern seen in the Sub_* helper method.
		/// </summary>
		private static readonly Code[] Pattern_Sub = new Code[] {
			Code.Ldloc_0, Code.Ldloc_1, Code.Sub, Code.Stloc_2, Code.Newobj, Code.Stloc_3,
			Code.Ldloc_3, Code.Ldloc_2, Code.Callvirt, Code.Ldloc_3, Code.Ret
		};

		public static Boolean Is_Sub(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(false, false, Pattern_Sub);
		}

		public static Boolean Is_Sub_Ovf(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(true, false, Pattern_Sub);
		}

		public static Boolean Is_Sub_Ovf_Un(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(true, true, Pattern_Sub);
		}

		/// <summary>
		/// OpCode pattern seen in the Add_* helper method.
		/// </summary>
		private static readonly Code[] Pattern_Add = new Code[] {
			Code.Ldloc_0, Code.Ldloc_1, Code.Add, Code.Stloc_2, Code.Newobj, Code.Stloc_3,
			Code.Ldloc_3, Code.Ldloc_2, Code.Callvirt, Code.Ldloc_3, Code.Ret
		};

		public static Boolean Is_Add(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(false, false, Pattern_Add);
		}

		public static Boolean Is_Add_Ovf(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(true, false, Pattern_Add);
		}

		public static Boolean Is_Add_Ovf_Un(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean2(true, true, Pattern_Add);
		}

		/// <summary>
		/// OpCode pattern seen in the Less-Than helper method.
		/// Used in: Clt, Blt, Bge (negated)
		/// </summary>
		private static readonly Code[] Pattern_Clt = new Code[] {
			Code.Ldloc_S, Code.Ldloc_S, Code.Blt_S,
			Code.Ldloc_S, Code.Call, Code.Brtrue_S, // System.Double::IsNaN(float64)
			Code.Ldloc_S, Code.Call, Code.Br_S      // System.Double::IsNaN(float64)
		};

		public static Boolean Is_Clt(this EazVirtualInstruction ins)
		{
			return ins.Matches(new Code[] {
				Code.Call, Code.Brtrue_S, Code.Ldc_I4_0, Code.Br_S, Code.Ldc_I4_1,
				Code.Callvirt, Code.Ldloc_2, Code.Call, Code.Ret
			}) && ins.MatchesIndirect(Pattern_Clt);
		}

		public static Boolean Is_Bge(this EazVirtualInstruction ins)
		{
			return ins.Matches(new Code[] {
				Code.Call, Code.Brtrue_S, Code.Ldarg_1, Code.Castclass
			}) && ins.MatchesIndirect(Pattern_Clt);
		}

		public static Boolean Is_Blt(this EazVirtualInstruction ins)
		{
			return ins.Matches(new Code[] {
				Code.Call, Code.Brfalse_S, Code.Ldarg_1, Code.Castclass
			}) && ins.MatchesIndirect(Pattern_Clt);
		}

		/// <summary>
		/// OpCode pattern seen in the Div_* helper method.
		/// </summary>
		private static readonly Code[] Pattern_Div = new Code[] {
			Code.Ldloc_S, Code.Ldloc_S, Code.Div, Code.Callvirt, Code.Ldloc_0, Code.Ret
		};

		public static Boolean Is_Div(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(false, Pattern_Div);
		}

		public static Boolean Is_Div_Un(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(true, Pattern_Div);
		}

		/// <remarks>Unsure</remarks>
		public static Boolean Is_Conv_R8(this EazVirtualInstruction ins)
		{
			// This *might* not be Conv_R8

			// This doesn't get the System.Double methods, probably because they're in
			// a different module?
			IList<MethodDef> called = ins.GetCalledMethods();

			Boolean isNaNCalled = called.Any((method) => {
				return method.FullName.Contains("System.Double::IsNaN");
			}), isInfinityCalled = called.Any((method) => {
				return method.FullName.Contains("System.Double::IsInfinity");
			});

			// This detects correctly, but the Code sequence is rather generic and seems
			// likely to be accidentally found elsewhere
			return /* isNaNCalled && isInfinityCalled && */ ins.Matches(new Code[] {
				Code.Ldloc_0, Code.Callvirt, Code.Call, Code.Brtrue_S,
				Code.Ldloc_0, Code.Callvirt, Code.Call, Code.Brfalse_S
			});
		}

		/// <summary>
		/// OpCode pattern seen in the Greater-Than helper method.
		/// Used in: Cgt
		/// </summary>
		/// <remarks>Greater-than for Double, Int32, Int64 but not-equal for other?</remarks>
		private static readonly Code[] Pattern_Cgt = new Code[] {
			Code.Ldarg_0, Code.Castclass, Code.Callvirt, Code.Ldarg_1,
			Code.Castclass, Code.Callvirt, Code.Cgt_Un, Code.Stloc_0
		};

		/// <remarks>Unsure</remarks>
		public static Boolean Is_Cgt(this EazVirtualInstruction ins)
		{
			return ins.Matches(new Code[] {
				Code.Call, Code.Brtrue_S, Code.Ldc_I4_0, Code.Br_S, Code.Ldc_I4_1,
				Code.Callvirt, Code.Ldloc_2, Code.Call, Code.Ret
			}) && ins.MatchesIndirect(Pattern_Cgt);
		}

		/// <summary>
		/// OpCode pattern seen in the Rem_* helper method.
		/// </summary>
		private static readonly Code[] Pattern_Rem = new Code[] {
			Code.Ldloc_S, Code.Ldloc_S, Code.Rem, Code.Callvirt, Code.Ldloc_0, Code.Ret
		};

		public static Boolean Is_Rem(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(false, Pattern_Rem);
		}

		public static Boolean Is_Rem_Un(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirectWithBoolean(true, Pattern_Rem);
		}

		public static Boolean Is_Or(this EazVirtualInstruction ins)
		{
			return ins.MatchesIndirect(new Code[] {
				Code.Ldloc_S, Code.Ldloc_S, Code.Or, Code.Callvirt, Code.Ldloc_0, Code.Ret
			});
		}

		/// <summary>
		/// OpCode pattern seen in the Starg_* delegate methods.
		/// </summary>
		private static readonly Code[] Pattern_Starg = new Code[] {
			Code.Ldarg_0, Code.Ldfld, Code.Ldloc_0, Code.Callvirt, Code.Ldelem
		};

		/// <summary>
		/// OpCode pattern seen at the "tail" of the Starg_* delegate methods.
		/// </summary>
		/// <remarks>
		/// Without this tail check, multiple delegate methods were being associated
		/// with both Starg and Starg_S
		/// </remarks>
		private static readonly Code[] Pattern_Tail_Starg = new Code[] {
			Code.Callvirt, Code.Pop, Code.Ret
		};

		public static Boolean Is_Starg(this EazVirtualInstruction ins)
		{
			var sub = ins.Find(Pattern_Starg);
			return sub != null && sub[3].Operand is MethodDef
				&& ((MethodDef)sub[3].Operand).ReturnType.FullName.Equals("System.UInt16")
				&& ins.Matches(Pattern_Tail_Starg);
		}

		public static Boolean Is_Starg_S(this EazVirtualInstruction ins)
		{
			var sub = ins.Find(Pattern_Starg);
			return sub != null && sub[3].Operand is MethodDef
				&& ((MethodDef)sub[3].Operand).ReturnType.FullName.Equals("System.Byte")
				&& ins.Matches(Pattern_Tail_Starg);
		}

		/// <summary>
		/// OpCode pattern seen in the Ldarg_C delegate methods (Ldarg_0, Ldarg_1, Ldarg_2, Ldarg_3).
		/// </summary>
		private static readonly Code[] Pattern_Ldarg_C = new Code[] {
			Code.Ldarg_0, Code.Ldarg_0, Code.Ldfld, Code.Ldc_I4, Code.Ldelem, // Code.Ldc_I4 changes depending on _C
			Code.Callvirt, Code.Call, Code.Ret
		};

		private static Boolean Is_Ldarg_C(EazVirtualInstruction ins, Code code)
		{
			// Ldarg_C delegates will reference the arguments field in their Ldfld, which sets them apart from
			// other very similar delegates
			return ins.Matches(ins.ModifyPattern(Pattern_Ldarg_C, Code.Ldc_I4, code))
				&& ((FieldDef)ins.DelegateMethod.Body.Instructions[2].Operand).MDToken == ins.Virtualization.ArgumentsField.MDToken;
		}

		public static Boolean Is_Ldarg_0(this EazVirtualInstruction ins)
		{
			return Is_Ldarg_C(ins, Code.Ldc_I4_0);
		}

		public static Boolean Is_Ldarg_1(this EazVirtualInstruction ins)
		{
			return Is_Ldarg_C(ins, Code.Ldc_I4_1);
		}

		public static Boolean Is_Ldarg_2(this EazVirtualInstruction ins)
		{
			return Is_Ldarg_C(ins, Code.Ldc_I4_2);
		}

		public static Boolean Is_Ldarg_3(this EazVirtualInstruction ins)
		{
			return Is_Ldarg_C(ins, Code.Ldc_I4_3);
		}

		/// <summary>
		/// OpCode pattern seen in the Ldarga, Ldarga_S delegate methods.
		/// </summary>
		private static readonly Code[] Pattern_Ldarga = new Code[] {
			Code.Ldloc_1, Code.Ldarg_0, Code.Ldfld, Code.Ldloc_0, Code.Callvirt,
			Code.Ldelem, Code.Callvirt, Code.Ldloc_1, Code.Call, Code.Ret
		};

		/// <remarks>Unsure</remarks>
		public static Boolean Is_Ldarga(this EazVirtualInstruction ins)
		{
			var sub = ins.Find(Pattern_Ldarga);
			return sub != null
				&& ((FieldDef)sub[2].Operand).MDToken == ins.Virtualization.ArgumentsField.MDToken
				&& ((MethodDef)sub[4].Operand).ReturnType.FullName.Equals("System.UInt16");
		}

		/// <remarks>Unsure</remarks>
		public static Boolean Is_Ldarga_S(this EazVirtualInstruction ins)
		{
			var sub = ins.Find(Pattern_Ldarga);
			return sub != null
				&& ((FieldDef)sub[2].Operand).MDToken == ins.Virtualization.ArgumentsField.MDToken
				&& ((MethodDef)sub[4].Operand).ReturnType.FullName.Equals("System.Byte");
		}

		/// <summary>
		/// OpCode pattern seen in the Ldarg, Ldarg_S delegate methods.
		/// </summary>
		/// <remarks>There are other delegate methods that match this exact pattern.</remarks>
		private static readonly Code[] Pattern_Ldarg = new Code[] {
			Code.Ldarg_1, Code.Castclass, Code.Stloc_0, Code.Ldarg_0, Code.Ldarg_0,
			Code.Ldfld, Code.Ldloc_0, Code.Callvirt, Code.Ldelem, Code.Callvirt,
			Code.Call, Code.Ret
		};

		public static Boolean Is_Ldarg(this EazVirtualInstruction ins)
		{
			return ins.MatchesEntire(Pattern_Ldarg)
				&& ((MethodDef)ins.DelegateMethod.Body.Instructions[7].Operand)
				   .ReturnType.FullName.Equals("System.UInt16")
				&& ((FieldDef)ins.DelegateMethod.Body.Instructions[5].Operand)
				   .MDToken == ins.Virtualization.ArgumentsField.MDToken;
		}

		public static Boolean Is_Ldarg_S(this EazVirtualInstruction ins)
		{
			return ins.MatchesEntire(Pattern_Ldarg)
				&& ((MethodDef)ins.DelegateMethod.Body.Instructions[7].Operand)
				   .ReturnType.FullName.Equals("System.Byte")
				&& ((FieldDef)ins.DelegateMethod.Body.Instructions[5].Operand)
				   .MDToken == ins.Virtualization.ArgumentsField.MDToken;
		}

		/// <summary>
		/// OpCode pattern seen in the Ldc_I4_C delegate methods.
		/// </summary>
		private static readonly Code[] Pattern_Ldc_I4_C = new Code[] {
			Code.Ldarg_0, Code.Newobj, Code.Stloc_0, Code.Ldloc_0, Code.Ldc_I4, // Code.Ldc_I4 changes depending on _C
			Code.Callvirt, Code.Ldloc_0, Code.Call, Code.Ret
		};

		private static Boolean Is_Ldc_I4_C(EazVirtualInstruction ins, Code code)
		{
			return ins.MatchesEntire(ins.ModifyPattern(Pattern_Ldc_I4_C, Code.Ldc_I4, code));
		}

		public static Boolean Is_Ldc_I4_0(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_0);
		}

		public static Boolean Is_Ldc_I4_1(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_1);
		}

		public static Boolean Is_Ldc_I4_2(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_2);
		}

		public static Boolean Is_Ldc_I4_3(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_3);
		}

		public static Boolean Is_Ldc_I4_4(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_4);
		}

		public static Boolean Is_Ldc_I4_5(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_5);
		}

		public static Boolean Is_Ldc_I4_6(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_6);
		}

		public static Boolean Is_Ldc_I4_7(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_7);
		}

		public static Boolean Is_Ldc_I4_8(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_8);
		}

		public static Boolean Is_Ldc_I4_M1(this EazVirtualInstruction ins)
		{
			return Is_Ldc_I4_C(ins, Code.Ldc_I4_M1);
		}
	}
}
