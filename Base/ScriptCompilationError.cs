using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class ScriptCompilationError
    {
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string ErrorMessage { get; set; }

        public ScriptCompilationError(int lineNumber, int columnNumber, string errorMessage)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return "@" + LineNumber + "," + ColumnNumber + ": " + ErrorMessage;
        }
    }
}