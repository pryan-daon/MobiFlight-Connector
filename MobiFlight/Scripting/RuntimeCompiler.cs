using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace MobiFlight.Scripting
{
    public class RuntimeCompiler
    {
        public Assembly Compile(string source, String language, List<String> referencedAssemblyNames)
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = true;
            CodeDomProvider provider = CodeDomProvider.CreateProvider(language);
            if (referencedAssemblyNames != null)
            {
                foreach (String referencedAssemblyName in referencedAssemblyNames)
                {
                    compilerParameters.ReferencedAssemblies.Add(referencedAssemblyName);
                }
            }
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParameters, source);
            List<ScriptCompilationError> errors = GetErrors(results);

            if (errors.Count > 0)
            {
                throw new MobiFlight.ScriptCompilationException(errors, i18n._tr("uiMessageConfigWizard_ScriptCompilationFailure"));
            }
            return results.CompiledAssembly;
        }

        protected List<ScriptCompilationError> GetErrors(CompilerResults results)
        {
            List<ScriptCompilationError> errors = new List<ScriptCompilationError>();
            foreach (CompilerError ce in results.Errors)
            {
                if (ce.IsWarning) continue;

                errors.Add(new ScriptCompilationError(ce.Line, ce.Column, ce.ErrorText));
            }
            return errors;
        }
    }
}
