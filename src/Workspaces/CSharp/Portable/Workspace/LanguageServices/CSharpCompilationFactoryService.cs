﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Microsoft.CodeAnalysis.CSharp
{
    [ExportLanguageService(typeof(ICompilationFactoryService), LanguageNames.CSharp), Shared]
    internal class CSharpCompilationFactoryService : ICompilationFactoryService
    {
        private static readonly CSharpCompilationOptions s_defaultOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication, concurrentBuild: false);

        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public CSharpCompilationFactoryService()
        {
        }

        Compilation ICompilationFactoryService.CreateCompilation(string assemblyName, CompilationOptions options)
        {
            return CSharpCompilation.Create(
                assemblyName,
                options: (CSharpCompilationOptions)options ?? s_defaultOptions);
        }

        Compilation ICompilationFactoryService.CreateSubmissionCompilation(string assemblyName, CompilationOptions options, Type? hostObjectType)
        {
            return CSharpCompilation.CreateScriptCompilation(
                assemblyName,
                options: (CSharpCompilationOptions)options,
                previousScriptCompilation: null,
                globalsType: hostObjectType);
        }

        CompilationOptions ICompilationFactoryService.GetDefaultCompilationOptions()
            => s_defaultOptions;

        GeneratorDriver? ICompilationFactoryService.CreateGeneratorDriver(ParseOptions parseOptions, ImmutableArray<ISourceGenerator> generators, ImmutableArray<AdditionalText> additionalTexts)
        {
            // https://github.com/dotnet/roslyn/issues/42565: for now we gate behind langver == preview. We'll remove this before final shipping, as the feature is langver agnostic
            if (((CSharpParseOptions)parseOptions).LanguageVersion != LanguageVersion.Preview)
            {
                return null;
            }
            return new CSharpGeneratorDriver(parseOptions, generators, additionalTexts);
        }
    }
}
