using RodStu.RazorTemplateEngine.Engine;
using RodStu.RazorTemplateEngine.Model.Curriculum;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using static System.Console;

namespace RodStu.CMD
{
    class Program
    {
        static ProgressBar p;
        static void Main(string[] args)
        {
            Console.Title = "RodStu CV Generator ;)";
            GenerateFiles(args);

            Console.ReadKey();
        }

        static void GenerateFiles(string[] args)
        {
            var duration = Stopwatch.StartNew();
            CurriculumModel model = LoadPersonalInformation();

            if (model == null)
                return;

            WriteLine("Generating files: \r\n");
            ItemMark[] itemsMark = new ItemMark[7];
            itemsMark[0] = new ItemMark("JPG file");
            itemsMark[1] = new ItemMark("HTML default");
            itemsMark[2] = new ItemMark("HTML password");
            itemsMark[3] = new ItemMark("PDF default ");
            itemsMark[4] = new ItemMark("PDF no copy, no printing");
            itemsMark[5] = new ItemMark("PDF password");
            itemsMark[6] = new ItemMark("PDF password, no copy, no printing");

            Write("\r\n\r\n");

            p = ProgressBar.WorkItemMessage("Generating files ...: ");
            double inc = 0;
            double v = (double)1 / (itemsMark.Length * 2);

            TemplateBuilder builder = new TemplateBuilder("Curriculum", "curriculum");

            string fileName = "cv.rodrigo.stuchi";
            string basePath = Path.Combine(Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents"), "__FILES");

            string basePathFile = GetPathToSave(basePath, fileName);

            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            var img = builder.BuildImage(model);
            using (MemoryStream memo = new MemoryStream(img))
            {
                Image.FromStream(memo).Save($@"{basePathFile}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            p.Report(inc += v);
            itemsMark[0].Check = true;
            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            string html = builder.BuildHtml(model);
            using (TextWriter wr = new StreamWriter($@"{basePathFile}.html", false))
            {
                wr.Write(html);
            }
            p.Report(inc += v);
            itemsMark[1].Check = true;

            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            string htmlPass = builder.BuildHtml(model, "123456");
            using (TextWriter wr = new StreamWriter($@"{basePathFile}_pass[123456].html", false))
            {
                wr.Write(htmlPass);
            }
            p.Report(inc += v);
            itemsMark[2].Check = true;
            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            var pdf = builder.BuildPdf(model);
            File.WriteAllBytes($@"{basePathFile}.pdf", pdf);
            p.Report(inc += v);
            itemsMark[3].Check = true;
            // ----------------------------------------------------------------------------------------------------
            PdfConfiguration pdfConfig = new PdfConfiguration();
            pdfConfig.ProtectDocument = true;

            p.Report(inc += v);
            var pdfProtected = builder.BuildPdf(model, null, pdfConfig);
            File.WriteAllBytes($@"{basePathFile}_protected.pdf", pdfProtected);
            p.Report(inc += v);
            itemsMark[4].Check = true;

            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            var pdfPass = builder.BuildPdf(model, "123456");
            File.WriteAllBytes($@"{basePathFile}_pass[123456].pdf", pdfPass);
            p.Report(inc += v);
            itemsMark[5].Check = true;
            // ----------------------------------------------------------------------------------------------------
            p.Report(inc += v);
            var pdfProtectedPass = builder.BuildPdf(model, "123456", pdfConfig);
            File.WriteAllBytes($@"{basePathFile}_protected+pass[123456].pdf", pdfProtectedPass);
            p.Report(inc += v);
            itemsMark[6].Check = true;

            
            p.Done();
            WriteLine($"\r\nFiles were generated in: {Path.GetFullPath(basePath)}");
            duration.Stop();
            WriteLine(@"Elapsed: {0:ss\s\ fff\m\s}", duration.Elapsed);

        }

        private static CurriculumModel LoadPersonalInformation()
        {
            CurriculumModel model;
            string curriculumDataFile = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), ".curriculum.data.xml");

            if (File.Exists(curriculumDataFile))
            {
                model = File.ReadAllText(
                    curriculumDataFile
                    ).XmlDeserializeFromString<CurriculumModel>();
            }
            else
            {
                model = new CurriculumModel()
                {
                    Name = "Nome Sobrenome",
                    BirthDate = new DateTime(1980, 1, 1),
                    MaritalStatus = "solteiro",
                    Nationality = "nacionalidade",
                    Address = "Rua rua da rua, numero, Cidade, UF",
                    PhoneFormatted = "11 9 9999-8888",
                    Email = "nome.sobrenome@domain.com",
                    Twitter = "@user",
                    GitHub = "user"
                };

                string xml = model.XmlSerializeToString();
                File.WriteAllText(curriculumDataFile, xml, Encoding.UTF8);

                var defColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLine($"File [{curriculumDataFile}] not found, but it was created with generic information.\r\n");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(xml + "\r\n");
                Console.ForegroundColor = defColor;
            }

            return model;
        }

        private static string GetPathToSave(string basePath, string fileName)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            string basePathFile = Path.Combine(basePath, fileName);
            return basePathFile;
        }


        
    }
}
