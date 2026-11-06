global using Il2CppInterop.Generator.Extensions.AsmResolver6;

using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace Il2CppInterop.Generator.Extensions.AsmResolver6;

// AsmResolver 6 moved resolution onto an explicit RuntimeContext and dropped the params overloads the generator uses.
// Each descriptor's own module knows its context (Cpp2IL's assemblies arrive with one), and old call sites keep working.
internal static class AsmResolver6Compat
{
    private static RuntimeContext? ContextOf(IMemberDescriptor member) => member.ContextModule?.RuntimeContext;

    public static TypeDefinition? Resolve(this ITypeDescriptor type)
        => type.Resolve(ContextOf(type), out var definition) == ResolutionStatus.Success ? definition : null;

    public static MethodDefinition? Resolve(this IMethodDescriptor method)
        => method.Resolve(ContextOf(method), out var definition) == ResolutionStatus.Success ? definition : null;

    public static FieldDefinition? Resolve(this IFieldDescriptor field)
        => field.Resolve(ContextOf(field), out var definition) == ResolutionStatus.Success ? definition : null;

    public static IMemberDefinition? Resolve(this MemberReference member)
        => ((IMemberDescriptor)member).Resolve(ContextOf(member), out var definition) == ResolutionStatus.Success ? definition : null;

    public static bool IsValueTypeOrFalse(this ITypeDescriptor type) => type.TryGetIsValueType(ContextOf(type)) ?? false;

    public static TypeSignature ToTypeSignature(this ITypeDescriptor type) => type switch
    {
        TypeSignature signature => signature,
        ITypeDefOrRef typeDefOrRef => typeDefOrRef.ToTypeSignature(typeDefOrRef.IsValueTypeOrFalse()),
        _ => type.ToTypeSignature(ContextOf(type)),
    };

    public static GenericInstanceTypeSignature MakeGenericInstanceType(this ITypeDescriptor type, params TypeSignature[] typeArguments)
        => new(type.ToTypeDefOrRef(), type.IsValueTypeOrFalse(), typeArguments);

    public static MethodSpecification MakeGenericInstanceMethod(this IMethodDefOrRef method, params TypeSignature[] typeArguments)
        => method.MakeGenericInstanceMethod((IEnumerable<TypeSignature>)typeArguments);
}

internal static class Sigs
{
    public static MethodSignature Static(TypeSignature returnType, params TypeSignature[] parameters)
        => MethodSignature.CreateStatic(returnType, (IEnumerable<TypeSignature>)parameters);

    public static MethodSignature Static(TypeSignature returnType, IEnumerable<TypeSignature> parameters)
        => MethodSignature.CreateStatic(returnType, parameters);

    public static MethodSignature Static(TypeSignature returnType, int genericParameters, params TypeSignature[] parameters)
        => MethodSignature.CreateStatic(returnType, genericParameters, parameters);

    public static MethodSignature Static(TypeSignature returnType, int genericParameters, IEnumerable<TypeSignature> parameters)
        => MethodSignature.CreateStatic(returnType, genericParameters, parameters);

    public static MethodSignature Instance(TypeSignature returnType, params TypeSignature[] parameters)
        => MethodSignature.CreateInstance(returnType, (IEnumerable<TypeSignature>)parameters);

    public static MethodSignature Instance(TypeSignature returnType, IEnumerable<TypeSignature> parameters)
        => MethodSignature.CreateInstance(returnType, parameters);

    public static MethodSignature Instance(TypeSignature returnType, int genericParameters, params TypeSignature[] parameters)
        => MethodSignature.CreateInstance(returnType, genericParameters, (IEnumerable<TypeSignature>)parameters);

    public static MethodSignature Instance(TypeSignature returnType, int genericParameters, IEnumerable<TypeSignature> parameters)
        => MethodSignature.CreateInstance(returnType, genericParameters, parameters);

    public static GenericInstanceTypeSignature GenericInst(ITypeDefOrRef genericType, bool isValueType, params TypeSignature[] typeArguments)
        => new(genericType, isValueType, typeArguments);
}
