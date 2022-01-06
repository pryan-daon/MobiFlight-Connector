using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;

namespace MobiFlight.Scripting
{
    public class PreconditionEvaluator
    {
        public List<String> EvaluateLcdDisplayLines(UpdatedLcdDisplay lcdDisplay, List<ConfigRefValue> configRefValues)
        {
            EvaluatorHelper helper = new EvaluatorHelper(lcdDisplay, configRefValues);

            List<String> lines = new List<String>();

            try
            {
                /***%SCRIPT%***/
            }
            catch (Exception ex)
            {
                Log.Instance.log("Failed to evaluate script " + ex.StackTrace, LogSeverity.Error);
            }

            return lines;
        }

        internal class EvaluatorHelper
        {
            public UpdatedLcdDisplay lcdDisplay { get; internal set; }
            public List<ConfigRefValue> configRefValues { get; internal set; }

            public EvaluatorHelper(UpdatedLcdDisplay lcdDisplay, List<ConfigRefValue> configRefValues)
            {
                this.lcdDisplay = lcdDisplay;
                this.configRefValues = configRefValues;
            }

            public string StringConfigRefValue(string placeholder)
            {
                ConfigRefValue configRef = configRefValues.Find(x => x.ConfigRef.Placeholder.Equals(placeholder));
                if (configRef == null)
                {
                    Log.Instance.log("No config ref found for placeholder " + placeholder, LogSeverity.Error);
                    return "";
                }
                return configRef.Value;
            }

            public int IntConfigRefValue(string placeholder)
            {
                int result = -1;
                if (!int.TryParse(StringConfigRefValue(placeholder), out result))
                {
                    Log.Instance.log("Placeholder " + placeholder + " could not be resolved to an int value", LogSeverity.Error);
                }
                return result;
            }
        }
    }
}