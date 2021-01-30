using System;

namespace com.armatur.common.serialization
{
    public class XmlSerializer
    {
        private readonly Type _type;

/*        public XmlSerializer(Type type)
        {
            _type = type;
        }

        public static string Serialize(object obj)
        {
            var doc = new XDocument();
            var typeWrapper = TypeWrapperPool.Instance.GetWrapper(obj.GetType());
            doc.Add(SerializeObject(obj));
            return doc.ToString();
        }

        private static XElement SerializeObject(object obj)
        {
            new SaveProcessorXml().Save(new ObjectWrapper(obj));
            
            var typeWrapper = TypeWrapperPool.Instance.GetWrapper(obj.GetType());
            var res = new XElement(typeWrapper.Name);
            foreach (var member in typeWrapper.Members)
            {
                if (member.IsAttribute)
                    res.Add(new XAttribute(member.Name, member.GetInternalStringValue(obj)));
                else
                    res.Add(new XElement(member.Name, member.GetInternalStringValue(obj)));
            }
            return res;
        }

        public static object DeserizalizeObject(ComplexTypeWrapper typeWrapper, XElement element)
        {
            var result = typeWrapper.CreateDefault();
            foreach (var member in typeWrapper.Members)
            {
                var value = member.IsAttribute ? element.Attribute(member.Name)?.Value : element.Element(member.Name)?.Value;
                member.SetInternalStringValue(result, value);
            }
            return result;
        }

        public static object Deserialize(Type type, string xml) 
        {
            try
            {
                using (TextReader tr = new StringReader(xml))
                {
                    var xdoc = XDocument.Load(tr);
                    var baseElement = xdoc.Root;
                    var typeWrapper = TypeWrapperPool.Instance.GetWrapper(type);
                    return DeserizalizeObject(typeWrapper, baseElement);
                }
            }
            catch (XmlException ex)
            {
                return null;
            }

        }*/
    }
}