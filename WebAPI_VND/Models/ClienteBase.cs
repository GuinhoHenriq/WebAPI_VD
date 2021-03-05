using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{

    #region Class Information
    /*
     *          Author: Guilherme Henrique - 2021-01-19
     *          Note:   Class used to send information
     *                  for the VINDI API customer method
     *                  (https://sandbox-app.vindi.com.br/api/v1/customers)
     */
    #endregion
    public class ClienteBase
    {

        public class Customer
        {
            public string name { get; set; }
            public string email { get; set; }
            public string registry_code { get; set; }
            public string code { get; set; }
            public string notes { get; set; }
            public Metadata metadata { get; set; }
            public Address address { get; set; }
            public List<Phones> Phones { get; set; }
        }

        public class Metadata
        {
            public string Vendedor { get; set; }
        }

        public class Address
        {
            public object street { get; set; }
            public object number { get; set; }
            public object additional_details { get; set; }
            public object zipcode { get; set; }
            public object neighborhood { get; set; }
            public object city { get; set; }
            public object state { get; set; }
            public object country { get; set; }
        }

        public class Phones
        {
            public object phone_type { get; set; }
            public object number { get; set; }
            public object extension { get; set; }

            public Phones(object phone_type, object number, object extension)
            {
                this.phone_type = phone_type;
                this.number = number;
                this.extension = extension;
            }

        }

        public class ListCliente
        {
            public List<Customer> customers { get; set; }
        }
    }
}