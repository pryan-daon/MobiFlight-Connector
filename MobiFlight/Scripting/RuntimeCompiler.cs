using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            StringWriter sw = new StringWriter();
            foreach (CompilerError ce in results.Errors)
            {
                if (ce.IsWarning) continue;
                sw.WriteLine("{0}({1},{2}: error {3}: {4}", ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText);
            }
            string errorText = sw.ToString();
            if (errorText.Length > 0)
                throw new ApplicationException(errorText);
            return results.CompiledAssembly;
        }
    }
}
