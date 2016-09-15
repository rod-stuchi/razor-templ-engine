using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RodStu.RazorTemplateEngine.Model.Curriculum
{
    [Serializable]
    public class CurriculumModel
    {
        public DateTime BirthDate { get; set; }
        public string Age
        {
            get
            {
                var zero = new DateTime(1, 1, 1);
                var diff = DateTime.Now.Subtract(BirthDate);
                var year = (zero + diff).Year - 1;
                return year > 1 ? $"{year} anos" : $"{year} ano";

            }
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string Twitter { get; set; }
        public string GitHub { get; set; }
        public string Address { get; set; }
        public string Phone {
            get
            {
                return PhoneFormatted.ReplaceRx(@"[\(\)\s+\-\.]", "");
            }
        }
        public string PhoneFormatted { get; set; }
        
    }
}