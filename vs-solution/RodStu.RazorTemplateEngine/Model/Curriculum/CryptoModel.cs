namespace RodStu.RazorTemplateEngine.Model.Curriculum
{
    public class CryptoModel
    {
        public string InitializationVector { get; set; }
        public string Salt { get; set; }
        public string CipherText { get; set; }
        public string Password { get; set; }
    }
}
