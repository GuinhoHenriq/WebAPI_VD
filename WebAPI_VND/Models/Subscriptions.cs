using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    public class Subscriptions
    {
        
        public class ProductItem
        {
            public int product_id { get; set; }

            public ProductItem( int product_id)
            {
                this.product_id = product_id;
            }
        }

        public class PaymentProfile
        {
            public string gateway_token { get; set; }
        }

        public class Subs
        {
            public object start_at { get; set; }
            public int plan_id { get; set; }
            public int customer_id { get; set; }
            public object code { get; set; }
            public string payment_method_code { get; set; }
            public object installments { get; set; }
            public object billing_trigger_type { get; set; }
            public object billing_trigger_day { get; set; }
            public object billing_cycles { get; set; }
            public object metadata { get; set; }
            public List<ProductItem> product_items { get; set; }
            public PaymentProfile payment_profile { get; set; }
            public object invoice_split { get; set; }
        }


    }
}