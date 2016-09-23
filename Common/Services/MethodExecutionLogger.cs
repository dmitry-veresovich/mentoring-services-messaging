using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Common.Models;

namespace Common.Services
{
    public class MethodExecutionLogger : IMethodExecutionLogger
    {
        private const string LogFile = "interceptorLog.xml";

        public void Log(MethodExecutionLogInfo logInfo)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFile);

            var doc = new XmlDocument();
            var rootNode = GetRootNode(doc, path);

            var invocationNode = doc.CreateElement("invocation");

            var dateTimeAttribute = doc.CreateAttribute("dateTime");
            dateTimeAttribute.Value = logInfo.DateTime.ToString(CultureInfo.InvariantCulture);
            invocationNode.Attributes.Append(dateTimeAttribute);

            var methodNameAttribute = doc.CreateAttribute("methodName");
            methodNameAttribute.Value = logInfo.MethodName;
            invocationNode.Attributes.Append(methodNameAttribute);

            var parametersNode = CreateParametersNode(logInfo, doc);

            invocationNode.AppendChild(parametersNode);
            rootNode.AppendChild(invocationNode);

            Save(path, doc);
        }

        private static void Save(string path, XmlDocument doc)
        {
            using (var xmlWriter = new XmlTextWriter(path, Encoding.Unicode))
            {
                xmlWriter.Formatting = Formatting.Indented;
                doc.WriteTo(xmlWriter);
            }
        }

        private static XmlElement CreateParametersNode(MethodExecutionLogInfo logInfo, XmlDocument doc)
        {
            var parametersNode = doc.CreateElement("parameters");
            foreach (var parameter in logInfo.Parameters)
            {
                var parameterNode = doc.CreateElement("parameter");

                var parameterNameAttribute = doc.CreateAttribute("name");
                parameterNameAttribute.Value = parameter.Name;
                parameterNode.Attributes.Append(parameterNameAttribute);

                var parameterValueNode = doc.CreateElement("value");
                if (parameter.Value != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        try
                        {
                            var xmlSerializer = new XmlSerializer(parameter.Value.GetType());
                            xmlSerializer.Serialize(stream, parameter.Value);

                            stream.Position = 0;

                            parameterValueNode.InnerXml = XDocument.Load(stream).Root.Value;
                        }
                        catch (InvalidOperationException e)
                        {
                            parameterValueNode.InnerXml = "Nor Serializable";
                        }
                    }
                }
                else
                {
                    parameterValueNode.InnerXml = "null";
                }

                parameterNode.AppendChild(parameterValueNode);
                parametersNode.AppendChild(parameterNode);
            }
            return parametersNode;
        }

        private static XmlElement GetRootNode(XmlDocument doc, string path)
        {
            try
            {
                doc.Load(path);
                if (doc.DocumentElement != null && doc.DocumentElement.Name == "invocations")
                    return doc.DocumentElement;
            }
            catch (Exception)
            {
                // Something is wrong with the file so we need to init initial xml structure.
            }

            var rootNode = doc.CreateElement("invocations");
            doc.AppendChild(rootNode);
            return rootNode;
        }
    }
}
