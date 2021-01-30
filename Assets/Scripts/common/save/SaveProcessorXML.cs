using System;
using System.Linq;
using System.Xml.Linq;
using com.armatur.common.util;

namespace com.armatur.common.save
{
    public class SaveProcessorXml : SaveProcessor
    {
        private XElement _element;
        

        public SaveProcessorXml(string text, SaveLoadParams @params = null)
        {
            base.@params = @params;
            ResetData();
            XDoc = XDocument.Parse(text);
            _element = XDoc.Root;
        }

        public SaveProcessorXml(SaveLoadParams @params = null)
        {
            base.@params = @params;
            ResetData();
            XDoc = new XDocument();
        }

        public override void SaveObject(object o)
        {
            base.SaveObject(o);
            while (true)
            {
                var nodes = XDoc.Descendants().Where(element => !element.HasAttributes && !element.HasElements && element.IsEmpty);
                var xElements = nodes.ToList();
                if (xElements.Any())
                    xElements.ForEach(element => element.Remove());
                else
                    break;
            }
        }

        public XDocument XDoc { get; }

        public override void ForEachLevel(Action<string> action)
        {
            var xElement = _element;
            var childs = xElement.Elements();
            childs.ForEach(element =>
            {
                _element = element;
                var nameLocalName = element.Name.LocalName;
                action(nameLocalName);
            });
            _element = xElement;
        }

        private int index = 0;
        public override bool AddLevel(string name, bool omited, bool save)
        {
            if (omited)
                return true;
//            var value = AddLevelToStructure(name);
            var res = false;
            if (save)
            {
                var newElement = new XElement(name);
                if (_element != null)
                    _element.Add(newElement);
                else
                    XDoc.Add(newElement);
                _element = newElement;
                res = true;
            }
            else
            {
                var childs = _element.Elements_NamespaceNeutral(name);
                var xElements = childs as XElement[] ?? childs.ToArray();
                if (xElements.Length > 1)
                    throw new Exception("Too much elements with name " + name);
                
                if (xElements.Length == 1)
                {
                    res = true;
                    _element = xElements.ElementAt(0);
                }
                else
                {
//                    RemoveLevelFromStructure();
                }
            }

            if (res)
                ++index;
            return res;
        }

        public override void RemoveOneLevel(bool omited)
        {
            if (omited) return;
            index--;
                
            _element = _element.Parent;
//            RemoveLevelFromStructure();
        }

        public override void AddData(string value)
        {
            _element.Value = value;
        }

        public override string GetData(string name = null)
        {
            if (name != null && !AddLevel(name, false, false)) return null;
            var res = _element.Value;
            if (null != name)
                RemoveOneLevel(false);
            return res;
        }

        public override void AddField(string name, string value)
        {
            _element.Add(new XAttribute(name, value));
        }

        public override string GetField(string name)
        {
            var attr = _element.Attribute(name);
            return attr?.Value;
        }
    }

    public class SaveLoadParams
    {
        public bool skipSavables = false;
        public bool savableOnly = false;
    }
}