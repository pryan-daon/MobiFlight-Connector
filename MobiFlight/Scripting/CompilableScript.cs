using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Scripting
{
    public class CompilableScript
    {
        public string ScriptTypeName { get; internal set; }
        public string Script { get; internal set; }
        public string ScriptPlaceholder { get; internal set; }
        public string SourceResourceName { get; internal set; }

        private string scriptHash;
        private string formattedSource;

        public CompilableScript(
            string sourceResourceName, string script, string scriptPlaceholder, string scriptTypeName)
        {
            SourceResourceName = sourceResourceName;
            Script = script;
            ScriptPlaceholder = scriptPlaceholder;
            ScriptTypeName = scriptTypeName;
        }

        public string GetScriptHash()
        {
            if (scriptHash == null)
            {
                scriptHash = CalculateScriptHash();
            }
            return scriptHash;
        }

        public string GetFormattedSource()
        {
            if (formattedSource == null)
            {
                formattedSource = LoadAndFormatSource();
            }
            return formattedSource;
        }

        protected string CalculateScriptHash()
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Script);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        protected String LoadAndFormatSource()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string source = null;
            using (Stream stream = assembly.GetManifestResourceStream(SourceResourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                source = reader.ReadToEnd();
            }
            return FormatSource(source);
        }

        protected string FormatSource(string rawSource)
        {
            return rawSource.Replace(ScriptPlaceholder, Script);
        }
    }
}
