using RazorEngine.Templating;
using RazorEngine.Text;
using System;
using System.IO;
using System.Reflection;

namespace RodStu.RazorTemplateEngine.Model
{
    public class TemplateModel<T> : TemplateBase<T>
    {
        public new T Model
        {
            get { return base.Model; }
            set { base.Model = value; }
        }

        public TemplateModel()
        {

        }

        public IEncodedString EmbedAsset(string path)
        {
            string asset;
            try
            {
                string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                asset = File.ReadAllText(Path.Combine(directory, path));
            }
            catch (Exception)
            {
                asset = "";
            }

            return Raw(asset);
        }
    }
}
