using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    public class UpdateCustomer
    {
        public class Address
        {
            public string street { get; set; }
            public string number { get; set; }
            public string additional_details { get; set; }
            public string zipcode { get; set; }
            public string neighborhood { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
        }

        public class Phone
        {
            public object phone_type { get; set; }
            public object number { get; set; }
            public object extension { get; set; }

            public Phone(object phone_type, object number, object extension)
            {
                this.phone_type = phone_type;
                this.number = number;
                this.extension = extension;
            }
        }

        public class Customer
        {
            public string name { get; set; }
            public string email { get; set; }
            public string registry_code { get; set; }
            public string notes { get; set; }
            public Metadata metadata { get; set; }
            public Address address { get; set; }
            public List<Phone> phones { get; set; }
        }

        public class Metadata
        {
            public string Vendedor { get; set; }
        }
       
    }
}