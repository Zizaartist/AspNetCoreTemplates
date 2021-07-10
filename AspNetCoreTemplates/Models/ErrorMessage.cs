using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTemplates.Models
{
    public class ErrorMessage
    {
        public ErrorMessage(string text) => errorText = text; 

        public string errorText { get; set; }
    }
}
