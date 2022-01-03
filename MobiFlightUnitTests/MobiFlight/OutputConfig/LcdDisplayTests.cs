using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class LcdDisplayTests
    {
        [TestMethod()]
        public void LcdDisplayTest()
        {
            UpdatedLcdDisplay o = new UpdatedLcdDisplay();
            Assert.IsNotNull(o, "Object is null");

            Assert.IsNull(o.Address, "Address is not null");
            Assert.AreEqual(0, o.Lines.Count, "Line Count not 0");
        }

        [TestMethod()]
        public void CloneTest()
        {
            UpdatedLcdDisplay o = new UpdatedLcdDisplay();
            o.Address = "Test";
            o.Lines.Add("TestLine1");
            Assert.IsNotNull(o, "Object is null");
            UpdatedLcdDisplay c = o.Clone() as UpdatedLcdDisplay;
            Assert.AreEqual(o.Address, c.Address, "Address are not the same");
            Assert.AreEqual(o.Lines.Count, c.Lines.Count, "Line.Count not the same");
            Assert.AreEqual(o.Lines[0], c.Lines[0], "Lines[0] not the same");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            UpdatedLcdDisplay o = new UpdatedLcdDisplay();

            Assert.IsNull(o.GetSchema(), "Schema not null");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            UpdatedLcdDisplay o = new UpdatedLcdDisplay();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\LcdDisplay\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("LCDDisplay", o.Address, "Address does not match.");
            Assert.AreEqual(1, o.Lines.Count, "Lines.Count does not match.");
            Assert.AreEqual("Read Test Line 1", o.Lines[0]);
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            UpdatedLcdDisplay o = new UpdatedLcdDisplay();
            o.Address = "LCDDisplay";
            o.Lines.Add("Write Test Line 1");

            xmlWriter.WriteStartElement("display");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\LcdDisplay\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            UpdatedLcdDisplay o1 = new UpdatedLcdDisplay();
            UpdatedLcdDisplay o2 = new UpdatedLcdDisplay();

            Assert.IsTrue(o1.Equals(o2));

            o1.Address = "123";
            o1.Lines.Add("Line1");

            Assert.IsFalse(o1.Equals(o2));

            o2.Address = "123";
            o2.Lines.Add("Line1");

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}