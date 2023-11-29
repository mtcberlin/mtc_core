using System;
using System.IO;
using System.Xml.Serialization;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Xml
{

    public class XmlDataProvider : IXmlDataProvider 
    {

        public XmlDataProvider()
        {

        }
        
        public T GetData<T>(string filepath) where T : class, new()
        {
	        var filePathWithoutExtension = filepath.Replace(".xml", string.Empty);

	        if (!filePathWithoutExtension.Contains(".edited") && File.Exists($"{filePathWithoutExtension}.edited.xml"))
	        {
		        filePathWithoutExtension = $"{filePathWithoutExtension}.edited";
	        }
            try
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

				using StreamReader sr = new StreamReader($"{filePathWithoutExtension}.xml");
				var content = (T)ser.Deserialize(sr);
				if (content is EditableXmlFileInfo)
				{
                    (content as EditableXmlFileInfo).FilePath = $"{filePathWithoutExtension}.xml";
				}
				return content;
            }
            catch
            {
                return new T();
            }
        }

		public void SetData<T>(T model, string filepath)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

			using StreamWriter sr = new StreamWriter(filepath);
			ser.Serialize(sr, model);
		}

        public MemoryStream AsMemoryStream<T>(T model)
        {

            XmlSerializer serializer = new XmlSerializer(model.GetType());
            MemoryStream memStream = new MemoryStream();
            serializer.Serialize(memStream, model);

            return memStream;
        }
    }

}
