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
        public const string Type = "LcdDisplay";
        public String Address { get; set; }
        public String Script { get; set; }
        public List<String> Lines { get; set; }

        public UpdatedLcdDisplay()
        {
            Script = "";
            Lines = new List<String>();
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
            foreach (string line in Lines)
            {
                clone.Lines.Add(line);
            }

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

            if (reader.LocalName == "line")
            {
                while (reader.LocalName == "line")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        Lines.Add(reader.Value);
                        if (reader.NodeType == XmlNodeType.Text)
                            reader.Read();
                        reader.ReadEndElement(); //line
                    }
                    else
                    {
                        Lines.Add("");
                        reader.Read();
                    }
                }
            }

            if (reader.LocalName == "script")
            {
                reader.Read();
                Script = reader.Value;
                reader.Read();
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("address", Address);

            if (Lines.Count > 0)
            {
                foreach (string line in Lines)
                {
                    writer.WriteElementString("line", line);
                }
            }

            if (Script.Length > 0)
            { 
                writer.WriteElementString("script", Script);
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
