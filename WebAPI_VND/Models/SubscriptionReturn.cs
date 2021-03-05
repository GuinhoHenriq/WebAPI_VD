using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VND.Models
{
    public class SubscriptionReturn
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Customer
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string code { get; set; }
        }

        public class Plan
        {
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
        }

        public class Product
        {
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
        }

        public class PricingSchema
        {
            public int id { get; set; }
            public string short_format { get; set; }
            public string price { get; set; }
            public object minimum_price { get; set; }
            public string schema_type { get; set; }
            public List<object> pricing_ranges { get; set; }
            public DateTime created_at { get; set; }
        }

        public class ProductItem
        {
            public int id { get; set; }
            public string status { get; set; }
            public int uses { get; set; }
            public object cycles { get; set; }
            public int quantity { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public Product product { get; set; }
            public PricingSchema pricing_schema { get; set; }
            public List<object> discounts { get; set; }
        }

        public class PaymentMethod
        {
            public int id { get; set; }
            public string public_name { get; set; }
            public string name { get; set; }
            public string code { get; set; }
            public string type { get; set; }
        }

        public class CurrentPeriod
        {
            public int id { get; set; }
            public DateTime billing_at { get; set; }
            public int cycle { get; set; }
            public DateTime start_at { get; set; }
            public string end_at { get; set; }
            public int duration { get; set; }
        }

        public class Metadata
        {
        }

        public class PaymentCompany
        {
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
        }

        public class PaymentProfile
        {
            public int id { get; set; }
            public string holder_name { get; set; }
            public object registry_code { get; set; }
            public object bank_branch { get; set; }
            public object bank_account { get; set; }
            public DateTime card_expiration { get; set; }
            public object allow_as_fallback { get; set; }
            public string card_number_first_six { get; set; }
            public string card_number_last_four { get; set; }
            public string token { get; set; }
            public DateTime created_at { get; set; }
            public PaymentCompany payment_company { get; set; }
        }

        public class Subscription
        {
            public int id { get; set; }
            public string status { get; set; }
            public DateTime start_at { get; set; }
            public object end_at { get; set; }
            public DateTime next_billing_at { get; set; }
            public object overdue_since { get; set; }
            public object code { get; set; }
            public object cancel_at { get; set; }
            public string interval { get; set; }
            public int interval_count { get; set; }
            public string billing_trigger_type { get; set; }
            public int billing_trigger_day { get; set; }
            public object billing_cycles { get; set; }
            public int installments { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public Customer customer { get; set; }
            public Plan plan { get; set; }
            public List<ProductItem> product_items { get; set; }
            public PaymentMethod payment_method { get; set; }
            public CurrentPeriod current_period { get; set; }
            public Metadata metadata { get; set; }
            public PaymentProfile payment_profile { get; set; }
            public bool invoice_split { get; set; }
        }

        public class GatewayResponseFields
        {
            public string nsu { get; set; }
        }

        public class Gateway
        {
            public int id { get; set; }
            public string connector { get; set; }
        }

        public class LastTransaction
        {
            public int id { get; set; }
            public string transaction_type { get; set; }
            public string status { get; set; }
            public string amount { get; set; }
            public int installments { get; set; }
            public string gateway_message { get; set; }
            public string gateway_response_code { get; set; }
            public string gateway_authorization { get; set; }
            public string gateway_transaction_id { get; set; }
            public GatewayResponseFields gateway_response_fields { get; set; }
            public object fraud_detector_score { get; set; }
            public object fraud_detector_status { get; set; }
            public object fraud_detector_id { get; set; }
            public DateTime created_at { get; set; }
            public Gateway gateway { get; set; }
            public PaymentProfile payment_profile { get; set; }
        }

        public class Charge
        {
            public int id { get; set; }
            public string amount { get; set; }
            public string status { get; set; }
            public DateTime due_at { get; set; }
            public object paid_at { get; set; }
            public int installments { get; set; }
            public int attempt_count { get; set; }
            public string next_attempt { get; set; }
            public object print_url { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public LastTransaction last_transaction { get; set; }
            public PaymentMethod payment_method { get; set; }
        }

        public class Bill
        {
            public int id { get; set; }
            public object code { get; set; }
            public string amount { get; set; }
            public int installments { get; set; }
            public string status { get; set; }
            public object billing_at { get; set; }
            public string due_at { get; set; }
            public string url { get; set; }
            public DateTime created_at { get; set; }
            public List<Charge> charges { get; set; }
            public PaymentProfile payment_profile { get; set; }
        }

        public class RetSubs
        {
            public Subscription subscription { get; set; }
            public Bill bill { get; set; }
        }


    }
}