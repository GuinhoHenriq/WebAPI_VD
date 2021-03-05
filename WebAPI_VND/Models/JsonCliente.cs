using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    public class JsonCliente
    {

        #region Informação da Classe
        /*
         *      Author: Guilherme Henrique - 2021-01-15
         *      Note:   Class used to receive the JSON return from the
         *              VINDI API customer Method
         *              (https://sandbox-app.vindi.com.br/api/v1/customers)
         */
        #endregion

        public class Metadata
        {
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

        public class Phone
        {
            public int id { get; set; }
            public string phone_type { get; set; }
            public string number { get; set; }
            public object extension { get; set; }
        }

        public class Customer
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string registry_code { get; set; }
            public string code { get; set; }
            public string notes { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public Metadata metadata { get; set; }
            public Address address { get; set; }
            public List<Phone> phones { get; set; }
        }

        public class ListCliente
        {
            public List<Customer> customers { get; set; }
        }
    }
}