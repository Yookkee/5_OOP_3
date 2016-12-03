// Guids.cs
// MUST match guids.h
using System;

namespace japroc_company.oop3
{
    static class GuidList
    {
        public const string guidoop3PkgString = "3caf6dc3-a7d7-4984-88d5-02069ecc2207";
        public const string guidoop3CmdSetString = "be460a5e-8a4f-4a6e-bdc6-f30373684db9";
        public const string guidToolWindowPersistanceString = "f13ebf7c-74d3-49e3-b5c3-549915cdbae0";
        public const string guidoop3EditorFactoryString = "9ac055c6-c615-4707-a97b-d91756b73b51";

        public static readonly Guid guidoop3CmdSet = new Guid(guidoop3CmdSetString);
        public static readonly Guid guidoop3EditorFactory = new Guid(guidoop3EditorFactoryString);
    };
}