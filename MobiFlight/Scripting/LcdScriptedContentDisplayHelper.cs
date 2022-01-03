using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Scripting
{
    public class LcdScriptedContentDisplayHelper
    {
        private static readonly string preSource =
@"
using System;
using System.Collections.Generic;
using MobiFlight;
using MobiFlight.OutputConfig;

namespace MobiFlight
{
    public class ProconditionEvaluator
    {
        public List<String> EvaluateLcdDisplayLines(UpdatedLcdDisplay updatedLcdDisplay, List<ConfigRefValue> configRefValues)
        {
            List<String> lines = new List<String>();
";

        private static readonly string postSource =
@"          return lines;
        } 
    }
}";

        private static readonly string scriptedTypeName = "MobiFlight.ProconditionEvaluator";

        private static readonly string evaluationMethodName = "EvaluateLcdDisplayLines";

        private static readonly List<String> referencedAssemblies = new List<string> { "System.dll", "System.Xml.dll", "MFConnector.exe" };

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
            CompilableScript compilableScript = new CompilableScript(script, preSource, postSource, scriptedTypeName);

            return cachedSciptedInstanceProvider.GetScriptedInstance(compilableScript, referencedAssemblies);
        }

        public List<String> GetScriptedLines(UpdatedLcdDisplay lcdConfig, List<ConfigRefValue> replacements)
        {
            object evaluatorInstance = GetScriptedInstance(lcdConfig.Script);

            List<String> evaluatedLines = (List<String>)evaluatorInstance.GetType().InvokeMember(
                     evaluationMethodName,
                     BindingFlags.InvokeMethod,
                     null,
                     evaluatorInstance,
                     new object[] { lcdConfig, replacements }
                  );

            return evaluatedLines;
        }
    }
}
