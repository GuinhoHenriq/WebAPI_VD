using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    public class ReturnCard
    {
        public int Cliente_Clicodigo { get; set; }
        public string Cliente_NumCartao { get; set; }
        public string Cliente_DtValidadeCartao { get; set; }
        public string Cliente_CVVCartao { get; set; }
        public string Num_Retorno { get; set; }
    }
}