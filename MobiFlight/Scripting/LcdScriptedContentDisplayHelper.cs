using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Scripting
{
    public class LcdScriptedContentDisplayHelper
    {
        private static readonly string SCRIPTED_TYPE_NAME = "MobiFlight.Scripting.PreconditionEvaluator";
        private static readonly string EVALUATION_METHOD_NAME = "EvaluateLcdDisplayLines";
        private static readonly string SCRIPT_PLACEHOLDER = "/***%SCRIPT%***/";
        private static readonly List<String> REFERENCED_ASSEMBLIES = new List<string> { "System.dll", "System.Xml.dll", "MFConnector.exe" };
        private static readonly int SCRIPT_SOURCE_OFFSET_LINES = 16;

        private CachedSciptedInstanceProvider cachedSciptedInstanceProvider;

        public LcdScriptedContentDisplayHelper()
        {
            this.cachedSciptedInstanceProvider = new CachedSciptedInstanceProvider();
        }

        public LcdScriptedContentDisplayHelper(CachedSciptedInstanceProvider cachedSciptedInstanceProvider)
        {
            this.cachedSciptedInstanceProvider = cachedSciptedInstanceProvider;
        }

        public object GetScriptedInstance(string script)
        {
            // Within the assembly resources must be prefixed with "MobiFlight" followed by namespace
            CompilableScript compilableScript = new CompilableScript("MobiFlight." + SCRIPTED_TYPE_NAME + ".cs", script, SCRIPT_PLACEHOLDER, SCRIPTED_TYPE_NAME);

            try
            {
                return cachedSciptedInstanceProvider.GetScriptedInstance(compilableScript, REFERENCED_ASSEMBLIES);
            }
            catch (Exception ex)
            {
                if (ex is ScriptCompilationException)
                {
                    ScriptCompilationException scriptException = ex as ScriptCompilationException;
                    scriptException.CompilationErrors.ForEach(err => err.LineNumber -= SCRIPT_SOURCE_OFFSET_LINES);
                }
                throw ex;
            }
            
        }

        public List<String> GetScriptedLines(UpdatedLcdDisplay lcdConfig, List<ConfigRefValue> replacements)
        {
            object evaluatorInstance = GetScriptedInstance(lcdConfig.Script);

            if (evaluatorInstance != null)
            {
                return new List<String>();
            }

            List<String> evaluatedLines = (List<String>)evaluatorInstance.GetType().InvokeMember(
                     EVALUATION_METHOD_NAME,
                     BindingFlags.InvokeMethod,
                     null,
                     evaluatorInstance,
                     new object[] { lcdConfig, replacements }
                  );

            return evaluatedLines;
        }
    }
}
