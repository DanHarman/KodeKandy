// Magic attributes to support CallerMemberName without taking a dependency on the BCL nuget package
namespace System.Runtime.CompilerServices
{
    // Summary:
    //     Allows you to obtain the method or property name of the caller to the method.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    sealed class CallerMemberNameAttribute : Attribute { }

    // Summary:
    //     Allows you to obtain the line number in the source file at which the method
    //     is called.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute { }

    // Summary:
    //     Allows you to obtain the full path of the source file that contains the caller.
    //     This is the file path at the time of compile.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute { }
}
