using System;

namespace RodStu.RazorTemplateEngine.Engine
{
    [Serializable]
    public class PdfConfiguration
    {

        public PdfConfiguration()
        {
            this.Title = "Rodrigo Stuchi";
            this.Zoom = 1;
            this.MarginTop = 10f;
            this.MarginLeft = 10f;
            this.MarginBottom = 10f;
            this.MarginRight = 10f;

            this.PageNumbersFooter = true;
            this.ProtectDocument = false;
            this.Quality = DpiQuality.Max;
        }

        /// <summary>
        /// Título documento (pdf)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Fator de zoom para documento PDF.
        /// <remarks>
        /// Padrão = 1
        /// </remarks>
        /// </summary>
        public float? Zoom { get; set; }

        /// <summary>
        /// Tamanho da margem superior em milimetros
        /// </summary>
        public float MarginTop { get; set; }

        /// <summary>
        /// Tamanho da esquerda superior em milimetros
        /// </summary>
        public float MarginLeft { get; set; }

        /// <summary>
        /// Tamanho da inferior superior em milimetros
        /// </summary>
        public float MarginBottom { get; set; }

        /// <summary>
        /// Tamanho da direita superior em milimetros
        /// </summary>
        public float MarginRight { get; set; }

        /// <summary>
        /// Exibe número de páginas no rodapé. Padrão (true).
        /// <remarks>
        /// Formato x / X
        /// </remarks>
        /// </summary>
        public bool PageNumbersFooter { get; set; }

        /// <summary>
        /// Permite apenas leitura (deficiente visual) e impressão
        /// </summary>
        public bool ProtectDocument { get; set; }

        /// <summary>
        /// Qualidade do arquivo em DPI.
        /// <remarks>
        /// Min = 100, Med = 200, Max = 600;
        /// </remarks>
        /// </summary>
        public DpiQuality Quality { get; set; }
    }

    public enum DpiQuality
    {
        Min = 100,
        Med = 200,
        Max = 600
    }
}
