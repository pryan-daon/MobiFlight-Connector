using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Scripting
{
    public class CompilableScript
    {
        public string Script { get; internal set; }
        public string ScriptSourcePrefix { get; internal set; }
        public string ScriptSourceSuffix { get; internal set; }
        public string CompiledScriptHash { get; internal set; }
        public string ScriptTypeName { get; internal set; }

        public CompilableScript(
            string script, string scriptSourcePrefix, string scriptSourceSuffix, string scriptTypeName)
        {
            Script = script;
            ScriptSourcePrefix = scriptSourcePrefix;
            ScriptSourceSuffix = scriptSourceSuffix;
            ScriptTypeName = scriptTypeName;
        }

        public string CalculateHash()
        {
            // Could be quicker to calculate an MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Script);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                CompiledScriptHash = sb.ToString();
                return CompiledScriptHash;
            }
        }

        public string Source()
        {
            string source = Script;
            if (ScriptSourcePrefix != null)
            {
                source = ScriptSourcePrefix + source;
            }

            if (ScriptSourceSuffix != null)
            {
                source = source + ScriptSourceSuffix;
            }

            return source;
        }
    }
}
