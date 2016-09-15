using NReco.ImageGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuesPechkin;

namespace RodStu.RazorTemplateEngine.Pdf
{
    public class ImageCreator
    {
        public static byte[] MakeImage(string content)
        {

            var htmlToImage = new NReco.ImageGenerator.HtmlToImageConverter();
            htmlToImage.CustomArgs = $@"
                --javascript-delay 300
                --quality 85
                --encoding ""utf-8""
                --no-stop-slow-scripts"
                .Replace("\r\n", "")
                .ReplaceRx(@"\s+", " ");

            var image = htmlToImage.GenerateImage(content, ImageFormat.Jpeg);

            return image;

        }
    }
}