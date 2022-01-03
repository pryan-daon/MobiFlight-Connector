using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Scripting
{
    public class CachedSciptedInstanceProvider
    {
        private RuntimeCompiler compiler;

        private Dictionary<string, object> scriptHashToCompiledInstanceMap;

        public CachedSciptedInstanceProvider()
        {
            compiler = new RuntimeCompiler();
            scriptHashToCompiledInstanceMap = new Dictionary<string, object>();
        }

        public object GetScriptedInstance(CompilableScript compilableScript, List<string> referencedAssemblyNames, String language = "c#")
        {
            string scriptHash = compilableScript.CalculateHash();
            if (scriptHashToCompiledInstanceMap.ContainsKey(scriptHash))
            {
                return scriptHashToCompiledInstanceMap[scriptHash];
            }

            Assembly assembly = compiler.Compile(compilableScript.Source(), language, referencedAssemblyNames);

            object scriptInstance = GetCompiledTypeInstance(compilableScript, assembly);

            scriptHashToCompiledInstanceMap[scriptHash] = scriptInstance;

            return scriptInstance;
        }

        private object GetCompiledTypeInstance(CompilableScript compilableScript, Assembly assembly)
        {
            Type type = assembly.GetType(compilableScript.ScriptTypeName);
            return Activator.CreateInstance(type);
        }
    }
}
