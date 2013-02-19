using System;
using System.Runtime;

//// NOTE: we will still get caller information, as long as we compile with a C# 5 (.NET 4.5) - or higher - compiler.
////       see: http://trelford.com/blog/post/CallMeMaybe.aspx

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }

    /// <summary>
    /// Allows you to obtain the line number in the source file at which the method is called.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are very short, and otherwise duplicates of the NOTE would have to be maintained.")]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }

    /// <summary>
    /// Allows you to obtain the method or property name of the caller to the method.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are very short, and otherwise duplicates of the NOTE would have to be maintained.")]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }
}
