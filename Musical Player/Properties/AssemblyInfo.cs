using Musical_Player.Global;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General information about this assembly is provided by the following set
// of attributes. Change these attribute values to modify the information
// associated with the assembly.
[assembly: AssemblyTitle("Digital Audio Player")]
[assembly: AssemblyDescription("A simple audio player written in C#")]
[assembly: AssemblyConfiguration("Production")]
[assembly: AssemblyCompany("Untitled Uncommercial Unknown Company Inc.")]
[assembly: AssemblyProduct("DAP")]
[assembly: AssemblyCopyright("Copyright UUUC Inc.© 2024")]
[assembly: AssemblyTrademark("UNCOMMERCIAL PROJECT")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components. If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// To begin building localizable applications, set
// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
// inside a <PropertyGroup>. For example, if you are using US English
// in your source files, set the <UICulture> to en-US. Then uncomment
// the NeutralResourceLanguage attribute below. Update the "en-US" in
// the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, // Where theme dictionaries are located
                                     // (used if a resource is not found on the page,
                                     // or in application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly // Where the generic resource dictionary is located
                                              // (used if a resource is not found on the page,
                                              // in the application, or in any theme specific resource dictionaries)
)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.0.0.1")]
[assembly: AssemblyFileVersion(Config.VERSION)]
