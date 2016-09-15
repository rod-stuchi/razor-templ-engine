using iTextSharp.text.pdf;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RodStu.RazorTemplateEngine.Model;
using RodStu.RazorTemplateEngine.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodStu.RazorTemplateEngine.Engine
{
    public sealed class TemplateBuilder : EngineBase, IDisposable
    {
        internal static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string userProfilePath = Environment.ExpandEnvironmentVariables(@"%userprofile%");
        private static string fileNameSecret = ".RodStu-razor-secret";
        private static string secretPath = Path.Combine(userProfilePath, fileNameSecret);

        public event EventHandler<double> OnProgressChange;

        private IRazorEngineService razorNgSrv;

        private byte[] ProtectPdf(byte[] pdf)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(pdf))
                {
                    PdfEncryptor.Encrypt(reader, output, true, null, File.ReadAllText(secretPath),
                        PdfWriter.ENCRYPTION_AES_256 |
                        PdfWriter.ALLOW_SCREENREADERS
                    );
                }

                return output.ToArray();
            }
        }

        private byte[] CryptoPdf(byte[] pdf, string cryptoPassword, PdfConfiguration pdfConfig)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(pdf))
                {
                    if (pdfConfig == null)
                    {
                        PdfEncryptor.Encrypt(reader, output, true, cryptoPassword, File.ReadAllText(secretPath),
                            PdfWriter.ALLOW_ASSEMBLY |
                            PdfWriter.ALLOW_COPY |
                            PdfWriter.ALLOW_DEGRADED_PRINTING |
                            PdfWriter.ALLOW_FILL_IN |
                            PdfWriter.ALLOW_PRINTING |
                            PdfWriter.ALLOW_SCREENREADERS
                            );
                    }
                    else if (pdfConfig.ProtectDocument)
                    {
                        PdfEncryptor.Encrypt(reader, output, true, cryptoPassword, File.ReadAllText(secretPath),
                           PdfWriter.ENCRYPTION_AES_256 |
                           PdfWriter.ALLOW_SCREENREADERS
                           );
                    }
                }

                return output.ToArray();
            }
        }

        private string ParseHtml<T>(T wrapper)
        {
            string resultHtml = String.Empty;
            OnProgressChanged(0.3);

            try
            {
                if (razorNgSrv.IsTemplateCached(TemplateName, typeof(T)))
                {
                    resultHtml = razorNgSrv.Run(TemplateName, typeof(T), wrapper);
                }
                else
                {
                    resultHtml = razorNgSrv.RunCompile(BaseTemplateContent, TemplateName, typeof(T), wrapper);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erro ao fazer parser do template [{0}.cshtml]\r\nObject:\r\n{1}", TemplateName, wrapper.ToJson(true)), ex);
                throw;
            }

            return resultHtml;
        }

        private string CryptoHtml(string html, string password)
        {
            Cryptography crypto = new Cryptography(password);
            string cipherHtml = crypto.Encrypt(html);

            Model.Curriculum.CryptoModel model = new Model.Curriculum.CryptoModel();
            model.CipherText = Cryptography.FormatCipherHtml(cipherHtml);
            model.InitializationVector = crypto.InitializeVector;
            model.Salt = crypto.SaltStr;
            model.Password = password;

            using (TemplateBuilder ngBuilderCipher = new TemplateBuilder("Curriculum", "crypto"))
            {
                OnProgressChanged(0.5);
                return ngBuilderCipher.BuildHtml<Model.Curriculum.CryptoModel>(model);
            }
        }

        private void OnProgressChanged(double progress)
        {
            if (OnProgressChange != null)
            {
                OnProgressChange(this, progress);
            }
        }


        /// <summary>
        /// Construtor padrão (folder / template)
        /// </summary>
        /// <param name="folderName">Nome do sub-diretório de Views</param>
        /// <param name="templateName">Nome do template sem extensão</param>
        public TemplateBuilder(string folderName, string templateName)
            : base(folderName, templateName)
        {

            if (!File.Exists(secretPath))
                throw new FileNotFoundException($"Arquivo segredo não encontrado em (${userProfilePath})", fileNameSecret);

            TemplateServiceConfiguration config = new TemplateServiceConfiguration();
            config.DisableTempFileLocking = true;
            config.CachingProvider = new DefaultCachingProvider(t => { });

            razorNgSrv = RazorEngineService.Create(config);
        }

        /// <summary>
        /// Gera HTML com base no template fornecido no construtor <see cref="TemplateBuilder"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wrapper">Dados para preencher template</param>
        /// <param name="cryptoPassword">Senha para criptografar o arquivo</param>
        /// <returns>HTML text</returns>
        public string BuildHtml<T>(T wrapper, string cryptoPassword = null)
        {
            string html = ParseHtml<T>(wrapper);

            if (String.IsNullOrEmpty(cryptoPassword))
                return html;
            else
                return CryptoHtml(html, cryptoPassword);
        }

        /// <summary>
        /// Gera PDF com base no template fornecido no construtor <see cref="EngineTemplateBuilder"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wrapper">Dados para preencher template</param>
        /// <param name="cryptoPassword">Senha para criptografar o arquivo</param>
        /// <param name="pdfConfig">Parametros de configuração do PDF</param>
        /// <returns>PDF binary</returns>
        public byte[] BuildPdf<T>(T wrapper, string cryptoPassword = null, PdfConfiguration pdfConfig = null)
        {
            string html = ParseHtml<T>(wrapper);

            OnProgressChanged(0.6);
            byte[] pdf = PdfCreator.MakePdf(html, pdfConfig);

            if (String.IsNullOrEmpty(cryptoPassword))
            {
                if (pdfConfig != null && pdfConfig.ProtectDocument)
                {
                    return ProtectPdf(pdf);
                }
                else
                {
                    return pdf;
                }
            }
            else
            {
                return CryptoPdf(pdf, cryptoPassword, pdfConfig);
            }
        }

        /// <summary>
        /// Gera uma imagem com base no template fornecido no construtor <see cref="EngineTemplateBuilder"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wrapper"></param>
        /// <returns>Image binary</returns>
        public byte[] BuildImage<T>(T wrapper)
        {

            OnProgressChanged(0.3);
            string html = ParseHtml<T>(wrapper);

            OnProgressChanged(0.5);
            byte[] image = ImageCreator.MakeImage(html);

            return image;
        }

        #region Disposable
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (razorNgSrv != null)
                    {
                        razorNgSrv.Dispose();
                        razorNgSrv = null;
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
