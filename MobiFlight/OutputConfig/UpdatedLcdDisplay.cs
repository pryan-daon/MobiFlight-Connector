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
        public List<String> Lines { get; set; }
        public String Script { get; set; }

        public UpdatedLcdDisplay()
        {
            Lines = new List<string>();
        }

        public override bool Equals(object obj)
        {
            bool linesAreEqual = true && Lines.Count == (obj as UpdatedLcdDisplay).Lines.Count;

            if (linesAreEqual)
                for (int i = 0; i != Lines.Count; i++)
                {
                    linesAreEqual = linesAreEqual && (Lines[i] == (obj as UpdatedLcdDisplay).Lines[i]);
                }

            return (
                obj != null && obj is UpdatedLcdDisplay &&
                this.Address == (obj as UpdatedLcdDisplay).Address &&
                linesAreEqual
            );
        }

        public object Clone()
        {
            UpdatedLcdDisplay clone = new UpdatedLcdDisplay();
            clone.Address = Address;
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
            else if (reader.LocalName == "script")
            {
                reader.Read();
                Script = reader.Value;
            }
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
            else if (Script.Length > 0)
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
