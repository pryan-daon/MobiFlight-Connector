using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    class ScriptCompilationException : ConfigErrorException
    {
        public List<ScriptCompilationError> CompilationErrors { get; internal set; }

        public ScriptCompilationException(List<ScriptCompilationError> errors, string p) : base(p)
        {
            CompilationErrors = errors;
        }

        public ScriptCompilationException(List<ScriptCompilationError> errors, string p, Exception e) : base(p, e)
        {
            CompilationErrors = errors;
        }

        public string GetCompilationErrorMessage()
        {
            if (CompilationErrors == null || CompilationErrors.Count == 0)
            {
                return string.Empty;
            }

            StringWriter sw = new StringWriter();
            CompilationErrors.ForEach(e => sw.WriteLine(e.ToString()));
            return sw.ToString();
        }
    }
}
