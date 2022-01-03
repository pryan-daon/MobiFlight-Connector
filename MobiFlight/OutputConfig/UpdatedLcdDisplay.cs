using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class UpdatedLcdDisplay : IXmlSerializable, ICloneable
    {
        public const string Type = "UpdatedLcdDisplay";
        public String Address { get; set; }
        public String Script { get; set; }
        public List<String> Lines { get; set; }

        public UpdatedLcdDisplay()
        {
            Script = "";
        }

        public override bool Equals(object obj)
        {
            bool scriptsAreEqual = (obj as UpdatedLcdDisplay).Script.Equals(Script);

            return (
                obj != null && obj is UpdatedLcdDisplay &&
                this.Address == (obj as UpdatedLcdDisplay).Address &&
                scriptsAreEqual
            );
        }

        public object Clone()
        {
            UpdatedLcdDisplay clone = new UpdatedLcdDisplay();
            clone.Address = Address;
            clone.Script = Script;

            return clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["address"] != null && reader["address"] != "")
            {
                Address = reader["address"].ToString();
            }
            reader.Read();

            if (reader.LocalName == "script")
            {
                reader.Read();
                Script = reader.Value;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("address", Address);

            if (Script.Length > 0)
            { 
                writer.WriteAttributeString("script", Script);
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
