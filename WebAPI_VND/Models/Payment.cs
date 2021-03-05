using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    #region Class Information
    /*
     *          Author: Guilherme Henrique 2021-01-19
     *          Note:   Class used to send information
     *                  for the VINDI API customer method
     *                  (https://sandbox-app.vindi.com.br/api/v1/public/payment_profiles)
     */
    #endregion

    public class Payment
    {
        public string holder_name { get; set; }
        public string card_expiration { get; set; }
        public string card_number { get; set; }
        public string card_cvv { get; set; }
        public string payment_method_code { get; set; }
        public string payment_company_code { get; set; }
        public int customer_id { get; set; }
    }
}