using AsmResolver.DotNet;
using AsmResolver.DotNet.Serialized;
using AsmResolver.IO;

namespace Il2CppInterop.Generator.MetadataAccess;

// Assemblies live in the RuntimeContext; nothing is ever probed from disk.
internal sealed class Il2CppAssemblyResolver : AssemblyResolverBase
{
    public Il2CppAssemblyResolver() : base(new ModuleReaderParameters(new ByteArrayFileService()))
    {
    }

    public override string? ProbeAssemblyFilePath(AssemblyDescriptor assembly, ModuleDefinition? originModule) => null;

    public static RuntimeContext NewContext(AssemblyDescriptor? corLib = null)
        => new(DotNetRuntimeInfo.NetCoreApp(6, 0), new Il2CppAssemblyResolver(), corLib);
}
