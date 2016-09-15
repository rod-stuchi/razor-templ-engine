using System;
using System.IO;
using System.Reflection;

namespace RodStu.RazorTemplateEngine.Engine
{
    public class EngineBase
    {
        internal readonly string BaseTemplateContent;
        internal readonly string TemplateName;

        public EngineBase(string folderName, string templateName)
        {
            TemplateName = templateName;
            string TemplateFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Views", folderName);

            string _path = Path.Combine(TemplateFolderPath, templateName + ".cshtml");

            if (File.Exists(_path))
                BaseTemplateContent = File.ReadAllText(_path);
            else
                throw new FileNotFoundException(String.Format("Template não encontrado [{0}]", _path));
        }
    }
}
