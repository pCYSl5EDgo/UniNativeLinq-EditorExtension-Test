﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetMinMaxRefFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetMinMaxRefFunc(ISingleApi api)
        {
            Api = api;
            isMax = Api.Name == "TryGetMax";
            processType = Api.Description.Split(new[] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private readonly string processType;
        private readonly bool isMax;
        private string Name => isMax ? "Max" : "Min";
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            var returnTypeReference = CalculateReturnType(mainModule);
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + processType + "RefFuncHelper"));

            if (Api.TryGetEnabled("TEnumerable", out var genericEnabled) && genericEnabled)
                GenerateGeneric(@static, mainModule, returnTypeReference);

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, returnTypeReference);
            }
        }

        private TypeReference CalculateReturnType(ModuleDefinition mainModule)
        {
            switch (processType)
            {
                case "Double":
                    return mainModule.TypeSystem.Double;
                case "Single":
                    return mainModule.TypeSystem.Single;
                case "Int64":
                    return mainModule.TypeSystem.Int64;
                case "Int32":
                    return mainModule.TypeSystem.Int32;
                case "UInt32":
                    return mainModule.TypeSystem.UInt32;
                case "UInt64":
                    return mainModule.TypeSystem.UInt64;
                default: return null;
            }
        }

        private OpCode OpCodeLdInd
        {
            get
            {
                switch (processType)
                {
                    case "Double":
                        return OpCodes.Ldind_R8;
                    case "Single":
                        return OpCodes.Ldind_R4;
                    case "UInt32":
                        return OpCodes.Ldind_U4;
                    case "UInt64":
                    case "Int64":
                        return OpCodes.Ldind_I8;
                    case "Int32":
                        return OpCodes.Ldind_I4;
                    default: return default;
                }
            }
        }

        private OpCode OpCodeBgeS
        {
            get
            {
                switch (processType)
                {
                    case "Double":
                    case "Single":
                    case "UInt32":
                    case "UInt64":
                        return OpCodes.Bge_Un_S;
                    case "Int64":
                    case "Int32":
                        return OpCodes.Bge_S;
                    default: return default;
                }
            }
        }

        private OpCode OpCodeBleS
        {
            get
            {
                switch (processType)
                {
                    case "Double":
                    case "Single":
                    case "UInt32":
                    case "UInt64":
                        return OpCodes.Ble_Un_S;
                    case "Int64":
                    case "Int32":
                        return OpCodes.Ble_S;
                    default: return default;
                }
            }
        }

        private void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule, TypeReference returnTypeReference)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            var TRefFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
            {
                GenericArguments = { T, returnTypeReference }
            };
            var Calc = TRefFunc.FindMethod("Invoke");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnTypeReference)));
            method.Parameters.Add(new ParameterDefinition("operator", ParameterAttributes.None, TRefFunc));

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(returnTypeReference));
            body.Variables.Add(new VariableDefinition(T));
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);
            var shortJump = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)
                .LdLocA(0)
                .LdLocA(2)
                .TryMoveNextEnumerator(TEnumerator)
                .BrTrueS(shortJump)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret()
                .Add(shortJump)
                .LdArg(2)
                .LdLocA(2)
                .CallVirtual(Calc)
                .StObj(returnTypeReference)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(2)
                .LdLocA(2)
                .CallVirtual(Calc)
                .StLoc(1)
                .Add(Instruction.Create(OpCodeLdInd))
                .LdLoc(1)
                .Add(Instruction.Create(isMax ? OpCodeBgeS : OpCodeBleS, condition))
                .LdArg(1)
                .LdLocA(1)
                .CpObj(returnTypeReference)
                .Add(condition)
                .LdLocA(2)
                .TryMoveNextEnumerator(TEnumerator)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(true)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, TypeReference returnTypeReference)
        {
            var method = new MethodDefinition("TryGet" + Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);
            var TRefFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
            {
                GenericArguments = { T, returnTypeReference }
            };
            var Calc = TRefFunc.FindMethod("Invoke");

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(mainModule, method, returnTypeReference, baseEnumerable, T, TRefFunc, Calc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(mainModule, method, returnTypeReference, baseEnumerable, T, TRefFunc, Calc);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], returnTypeReference, T, TRefFunc, Calc);
            }
        }

        private void GenerateNativeArray(ModuleDefinition mainModule, MethodDefinition method, TypeReference returnTypeReference, TypeReference baseEnumerable, TypeReference T, TypeReference TRefFunc, MethodReference Calc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnTypeReference)));
            method.Parameters.Add(new ParameterDefinition("operator", ParameterAttributes.None, TRefFunc));

            var body = method.Body;
            body.Variables.Add(new VariableDefinition(mainModule.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(returnTypeReference));

            var il09 = Instruction.Create(OpCodes.Ldarg_1);
            var il12 = Instruction.Create(OpCodes.Ldarg_2);
            var il1E = Instruction.Create(OpCodes.Ldloc_0);
            var il22 = Instruction.Create(OpCodes.Ldloc_0);

            var getLength = baseEnumerable.FindMethod("get_Length", 0);
            var getItem = baseEnumerable.FindMethod("get_Item", 1);

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(getLength)
                .BrTrueS(il09)
                .LdC(false)
                .Ret()

                .Add(il09)
                    .LdArg(2)
                        .LdArg(0)
                        .LdC(0)
                        .Call(getItem)
                    .StLoc(1)
                    .LdLocA(1)
                .CallVirtual(Calc)
                .StObj(returnTypeReference)
                .LdC(1)
                .StLoc(0)
                .BrS(il22)

                .Add(il12)
                    .LdArg(0)
                    .LdLoc(0)
                    .Call(getItem)
                .StLoc(1)
                .LdLocA(1)
                .CallVirtual(Calc)
                .Dup()
                .StLoc(2)
                .LdArg(1)
                .LdInd(returnTypeReference)
                .Add(Instruction.Create(isMax ? OpCodeBleS : OpCodeBgeS, il1E))
                .LdArg(1)
                .LdLocA(2)
                .CpObj(returnTypeReference)
                .Add(il1E)
                .LdC(1)
                .Add()
                .StLoc(0)
                .Add(il22)
                .LdArg(0)
                .Call(getLength)
                .BltS(il12)
                .LdC(true)
                .Ret();
        }

        private void GenerateArray(ModuleDefinition mainModule, MethodDefinition method, TypeReference returnTypeReference, TypeReference baseEnumerable, TypeReference T, TypeReference TRefFunc, MethodReference Calc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnTypeReference)));
            method.Parameters.Add(new ParameterDefinition("operator", ParameterAttributes.None, TRefFunc));

            var body = method.Body;
            body.Variables.Add(new VariableDefinition(mainModule.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(returnTypeReference));

            var il09 = Instruction.Create(OpCodes.Ldarg_1);
            var il12 = Instruction.Create(OpCodes.Ldarg_2);
            var il1E = Instruction.Create(OpCodes.Ldloc_0);
            var il22 = Instruction.Create(OpCodes.Ldloc_0);
            body.GetILProcessor()
                .ArgumentNullCheck(0, 2, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrTrueS(il09)
                .LdC(false)
                .Ret()
                .Add(il09)
                .LdArg(2)
                    .LdArg(0)
                    .LdC(0)
                    .LdElemA(T)
                .CallVirtual(Calc)
                .StObj(returnTypeReference)
                .LdC(1)
                .StLoc(0)
                .BrS(il22)
                .Add(il12)
                    .LdArg(0)
                    .LdLoc(0)
                    .LdElemA(T)
                .CallVirtual(Calc)
                .Dup()
                .StLoc(1)
                .LdArg(1)
                .LdInd(returnTypeReference)
                .Add(Instruction.Create(isMax ? OpCodeBleS : OpCodeBgeS, il1E))
                .LdArg(1)
                .LdLocA(1)
                .CpObj(returnTypeReference)
                .Add(il1E)
                .LdC(1)
                .Add()
                .StLoc(0)
                .Add(il22)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BltS(il12)
                .LdC(true)
                .Ret();
        }

        private void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference returnTypeReference, TypeReference T, TypeReference TRefFunc, MethodReference Calc)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnTypeReference)));
            method.Parameters.Add(new ParameterDefinition("operator", ParameterAttributes.None, TRefFunc));

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(returnTypeReference));
            body.Variables.Add(new VariableDefinition(T));
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);
            var shortJump = Instruction.Create(OpCodes.Ldarg_1);

            var enumeratorDispose = enumerator.FindMethod("Dispose", 0);
            var enumeratorTryMoveNext = enumerator.FindMethod("TryMoveNext", 1);
            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .LdLocA(0)
                .LdLocA(2)
                .Call(enumeratorTryMoveNext)
                .BrTrueS(shortJump)
                .LdLocA(0)
                .Call(enumeratorDispose)
                .LdC(false)
                .Ret()
                .Add(shortJump)
                .LdArg(2)
                .LdLocA(2)
                .CallVirtual(Calc)
                .StObj(returnTypeReference)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(2)
                .LdLocA(2)
                .CallVirtual(Calc)
                .StLoc(1)
                .Add(Instruction.Create(OpCodeLdInd))
                .LdLoc(1)
                .Add(Instruction.Create(isMax ? OpCodeBgeS : OpCodeBleS, condition))
                .LdArg(1)
                .LdLocA(1)
                .CpObj(returnTypeReference)
                .Add(condition)
                .LdLocA(2)
                .Call(enumeratorTryMoveNext)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumeratorDispose)
                .LdC(true)
                .Ret();
        }
    }
}