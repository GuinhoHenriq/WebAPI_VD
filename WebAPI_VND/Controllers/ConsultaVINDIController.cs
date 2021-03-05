using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI_VINDI.DAL;
using WebAPI_VINDI.Models;
using RestSharp.Serialization.Json;
using RestSharp.Deserializers;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using System.Threading;

namespace WebAPI_VND.Controllers
{
    public class ConsultaVINDIController : ApiController
    {
        #region Variaveis Globais
        string ReturnCustomerGet, ReturnCustomerPost, ReturnCustomerPut, ReturnPaymentProfilePost, ReturnSubscriptionPost, ReturnCardMessage, ReturnSubscriptionDelete;
        #endregion

        #region Method GET - ConsultCliente
        /* 
         *      Author:     Guilherme Henrique - 2021-01-19
         *      Note:       Method GET used to receive information about
         *                  client or register new client in data base the VINDI
         */
        #endregion

        [HttpGet]
        public HttpResponseMessage ConsultaCliente(int Clicodigo)
        {
            #region Instantiating Class

            // Class Models
            ClienteBaseTMKT.Customer ObjClienteTMKT = new ClienteBaseTMKT.Customer();
            ClienteBaseTMKT.Address ObjAddressCliTMKT = new ClienteBaseTMKT.Address();
            ClienteBaseTMKT.Metadata ObjMetadataTMKT = new ClienteBaseTMKT.Metadata();
            UpdateCustomer.Customer ObjUpdateCustomer = new UpdateCustomer.Customer();
            UpdateCustomer.Address ObjUpdateAddress = new UpdateCustomer.Address();
            UpdateCustomer.Metadata ObjUpdateMetadata = new UpdateCustomer.Metadata();
            JsonCliente ObjClienteJSON = new JsonCliente();
            JsonCliente.Customer ObjClienteJSONCust = new JsonCliente.Customer();
            Payment ObjPayment = new Payment();
            PaymentReturn ObjReturnPayment = new PaymentReturn();
            ReturnCard ObjReturnCard = new ReturnCard();
            Subscriptions.PaymentProfile ObjSubsPayment = new Subscriptions.PaymentProfile();
            Subscriptions.ProductItem ObjsSubsProduct = new Subscriptions.ProductItem(0);
            Subscriptions.Subs ObjSubscriptions = new Subscriptions.Subs();
            SubscriptionReturn.RetSubs ObjRetSubscription = new SubscriptionReturn.RetSubs();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Class DAL
            CarregaInfoCliente Dados = new CarregaInfoCliente();
            GravaLog LogDotz = new GravaLog();

            DataSet ds = new DataSet();

            #endregion

            #region Consulting DB with the Clicodigo
            try
            {
                ds = Dados.ConsultaCliente(Clicodigo);

                ObjClienteTMKT.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString();
            }
            
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            #endregion

            #region Information about the Method
            /*
             * Does a request in the VINDI API
             * If there is registration in the VINDI DB, go directly to the Payment Profile method
             * If not, it goes to the customers method where it save a new register
            */
            #endregion

            #region Solicitação - Metodo Customers API VINDI

            #region Send CPF to API VINDI
                   
            try
            {
                
                var CustomerGet = new RestClient(ConfigurationManager.AppSettings.Get("CustomerGet").ToString()+ObjClienteTMKT.registry_code);
                var requestCustomerGet = new RestRequest(Method.GET);
                requestCustomerGet.AddHeader("Authorization",(ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                requestCustomerGet.AddHeader("Content-Type", "application/json");
                var responseCustomerGet = CustomerGet.Get(requestCustomerGet);
                ReturnCustomerGet = responseCustomerGet.StatusCode.ToString();

                var retCustomerGet = JsonConvert.DeserializeObject<Models.JsonCliente.ListCliente>(responseCustomerGet.Content.ToString()); // Deserializing Object

                LogDotz.GravaReturn(Clicodigo, responseCustomerGet.Content.ToString(), "CustomerGet");

            #endregion
                

                if (retCustomerGet == null || retCustomerGet.customers.Count == 0 || responseCustomerGet.StatusCode == 0  )
                {
                    #region Without Registry

                    ds = Dados.ConsultaCliente(Clicodigo);

                            // User Information 

                            ObjClienteTMKT.name = ds.Tables[0].Rows[0]["NOME"].ToString(); //"Guilherme Henrique";
                            ObjClienteTMKT.email = ds.Tables[0].Rows[0]["EMAIL_PESSOAL"].ToString(); //"teste@teste.com.br"; 
                            ObjClienteTMKT.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString(); // "25949032806";
                            ObjClienteTMKT.code = ds.Tables[0].Rows[0]["CLICODIGO"].ToString(); //"999915";
                            ObjClienteTMKT.notes = "";
                            ObjMetadataTMKT.Vendedor = "TMKT";

                            // Address Information
                            ObjAddressCliTMKT.street = ""; //ds.Tables[0].Rows[0]["ENDERECO"].ToString(); ;
                            ObjAddressCliTMKT.number = ""; //ds.Tables[0].Rows[0]["NUMERO"].ToString();
                            ObjAddressCliTMKT.additional_details = "";// ds.Tables[0].Rows[0]["COMPLEMENTO"].ToString(); ;
                            ObjAddressCliTMKT.zipcode = ""; //ds.Tables[0].Rows[0]["CEP"].ToString();
                            ObjAddressCliTMKT.neighborhood = ""; //ds.Tables[0].Rows[0]["BAIRRO"].ToString(); //"Calmon Viana";
                            ObjAddressCliTMKT.city = ""; //ds.Tables[0].Rows[0]["CIDADE"].ToString();
                            ObjAddressCliTMKT.state = "";//ds.Tables[0].Rows[0]["UF"].ToString();
                            ObjAddressCliTMKT.country = "";//ds.Tables[0].Rows[0]["REGIAO"].ToString();

                            ObjClienteTMKT.Phones = new List<Models.ClienteBaseTMKT.Phones>();

                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                ObjClienteTMKT.Phones.Add(new Models.ClienteBaseTMKT.Phones("mobile", ds.Tables[0].Rows[i]["TEL"].ToString(), ""));
                            }

                            ObjClienteTMKT.address = ObjAddressCliTMKT;
                            ObjClienteTMKT.metadata = ObjMetadataTMKT;

                            string jsonObjClienteTMKT = JsonConvert.SerializeObject(ObjClienteTMKT);

                            LogDotz.GravaReturn(Clicodigo, jsonObjClienteTMKT, "jsonObjClienteTMKT");

                            var CustomerPost = new RestClient(ConfigurationManager.AppSettings.Get("CustomerPost").ToString());
                    var requestCustomerPost = new RestRequest(Method.POST);
                    requestCustomerPost.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                    requestCustomerPost.AddHeader("Content-Type", "application/json");
                    requestCustomerPost.AddJsonBody(jsonObjClienteTMKT);
                    var responseCustomerPost = CustomerPost.Post(requestCustomerPost);
                    ReturnCustomerPost = responseCustomerPost.StatusCode.ToString();

                    var retCustomerPost = JsonConvert.DeserializeObject<Models.JsonCliente.ListCliente>(responseCustomerGet.Content.ToString()); // Deserializing Object

                    LogDotz.GravaReturn(Clicodigo, responseCustomerPost.Content.ToString(), "CustomerPost");

                    if (ReturnCustomerPost == "422")
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Erro 422 - Clicodigo ja está em uso" );
                    }
                    #endregion

                    #region Consult the customer again
                    var CustomerGet1 = new RestClient(ConfigurationManager.AppSettings.Get("CustomerGet").ToString()+ObjClienteTMKT.registry_code);
                var requestCustomerGet1 = new RestRequest(Method.GET);
                requestCustomerGet1.AddHeader("Authorization",(ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                requestCustomerGet1.AddHeader("Content-Type", "application/json");
                var responseCustomerGet1 = CustomerGet1.Get(requestCustomerGet1);
                ReturnCustomerGet = responseCustomerGet1.StatusCode.ToString();

                LogDotz.GravaReturn(Clicodigo, responseCustomerGet1.Content.ToString(), "CustomerGet1");

                retCustomerGet = JsonConvert.DeserializeObject<Models.JsonCliente.ListCliente>(responseCustomerGet1.Content.ToString()); // Deserializing Object
                
            #endregion

                    #region Payment Method

                    #region Get AKIVA API Get Date AKIVA API

                    try
                    {
                    var ReturnCardGet = new RestClient(ConfigurationManager.AppSettings.Get("AkivaAPI").ToString() + Clicodigo);
                    var requestReturnCardGet = new RestRequest(Method.POST);
                    requestReturnCardGet.AddHeader("Content-Type", "application/json");
                    var responseCardGet = ReturnCardGet.Get(requestReturnCardGet);
                    ReturnCardMessage = responseCardGet.StatusCode.ToString();

                    var Return = JsonConvert.DeserializeObject<Models.ReturnCard>(responseCardGet.Content.ToString()); // Deserializing Object

                    LogDotz.GravaReturn(Clicodigo, responseCardGet.Content.ToString(), "CardGet");

                    ds = Dados.ConsultaCliente(Clicodigo);

                    ObjPayment.holder_name = ds.Tables[0].Rows[0]["NOME"].ToString();
                    ObjPayment.card_expiration = ds.Tables[0].Rows[0]["DT_VENC_CART"].ToString();
                    ObjPayment.card_number = Return.Cliente_NumCartao; 
                    ObjPayment.card_cvv = Return.Cliente_CVVCartao;
                    ObjPayment.payment_method_code = "credit_card";
                    ObjPayment.payment_company_code = ds.Tables[0].Rows[0]["BANDEIRA_CART"].ToString();
                    ObjPayment.customer_id = retCustomerGet.customers[0].id;

                    string jsonObjPayment = JsonConvert.SerializeObject(ObjPayment);

                    LogDotz.GravaReturn(Clicodigo, jsonObjPayment, "JsonObjPayment");
                     #endregion

                        var PaymentProfilePost = new RestClient(ConfigurationManager.AppSettings.Get("PaymentPost").ToString());
                    var requestPaymentProfilePost = new RestRequest(Method.POST);
                    requestPaymentProfilePost.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PublicKey").ToString()));// Adding Authenticator
                    requestPaymentProfilePost.AddHeader("Content-Type", "application/json");
                    requestPaymentProfilePost.AddJsonBody(jsonObjPayment);
                    var responsePaymentProfile = PaymentProfilePost.Post(requestPaymentProfilePost);
                    ReturnPaymentProfilePost = responsePaymentProfile.StatusDescription.ToString();

                    var retPaymentProfilePost = JsonConvert.DeserializeObject<Models.PaymentReturn.Payment>(responsePaymentProfile.Content.ToString()); // Deserializing Object

                    LogDotz.GravaReturn(Clicodigo, responsePaymentProfile.Content.ToString(), "PaymentPost");
                    #endregion
                    
                    #region Subscription Method

                    ObjSubscriptions.start_at = "";
                    ObjSubscriptions.plan_id = Convert.ToInt32(ds.Tables[0].Rows[0]["PLAN_ID"].ToString()); //8527;  
                    ObjSubscriptions.customer_id = retCustomerGet.customers[0].id;
                    ObjSubscriptions.code = "";
                    ObjSubscriptions.payment_method_code = "credit_card";
                    ObjSubscriptions.installments = "";
                    ObjSubscriptions.billing_trigger_type = "";
                    ObjSubscriptions.billing_trigger_day = "";
                    ObjSubscriptions.billing_cycles = "";
                    ObjSubscriptions.metadata = "TMKT";

                    ObjSubscriptions.product_items = new List<Subscriptions.ProductItem>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ObjSubscriptions.product_items.Add(new Subscriptions.ProductItem(Convert.ToInt32(ds.Tables[0].Rows[i]["PRODUCT_ID"].ToString())));
                    }

                    ObjSubsPayment.gateway_token = retPaymentProfilePost.payment_profile.gateway_token.ToString();
                    

                    ObjSubscriptions.payment_profile = ObjSubsPayment;

                    string jsonObjSubscription = JsonConvert.SerializeObject(ObjSubscriptions);

                    LogDotz.GravaReturn(Clicodigo, jsonObjSubscription, "jsonObjSubscription");

                    var SubscriptionPost = new RestClient(ConfigurationManager.AppSettings.Get("SubscriptionsPost").ToString());
                    var requestSubscriptionPost = new RestRequest(Method.POST);
                    requestSubscriptionPost.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                    requestSubscriptionPost.AddHeader("Content-Type", "application/json");
                    requestSubscriptionPost.AddJsonBody(jsonObjSubscription);
                    var responseSubscriptionPost = SubscriptionPost.Post(requestSubscriptionPost);
                    ReturnSubscriptionPost = responseSubscriptionPost.StatusDescription.ToString();
 
                    var retSubscriptionPost = JsonConvert.DeserializeObject<Models.SubscriptionReturn.RetSubs>(responseSubscriptionPost.Content.ToString()); // Deserializing Object

                    LogDotz.GravaReturn(Clicodigo, responseSubscriptionPost.Content.ToString(), "SubscriptionPost");
                    #endregion

                    #region Subscription method Delete

                    if (retSubscriptionPost.bill.charges[0].last_transaction.status != "success" && retSubscriptionPost.subscription.current_period.cycle == 1) 
                    {
                        var SubscriptionDelete = new RestClient(ConfigurationManager.AppSettings.Get("SubscriptionsDelete").ToString() + retSubscriptionPost.subscription.id); Thread.Sleep(4000);
                        var requestSubscriptionDelete = new RestRequest(Method.POST);
                        requestSubscriptionDelete.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestSubscriptionDelete.AddHeader("Content-Type", "application/json");
                        requestSubscriptionDelete.AddJsonBody(jsonObjSubscription);
                        var responseSubscriptionDelete = SubscriptionDelete.Delete(requestSubscriptionDelete);
                        ReturnSubscriptionDelete = responseSubscriptionDelete.StatusDescription.ToString();
                        LogDotz.GravaReturn(Clicodigo, SubscriptionDelete.BaseUrl.ToString(), "URLSubscriptionDelete");
                        LogDotz.GravaReturn(Clicodigo, responseSubscriptionDelete.Content.ToString(), "SubscriptionDelete");

                        return Request.CreateResponse(HttpStatusCode.OK, "Transação não Autorizada");
                    }


                    else
                    {

                    }
                    #endregion

                    try
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, retSubscriptionPost.bill.charges[0].last_transaction.gateway_message);
                    }
                    catch
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, responseSubscriptionPost.StatusDescription);
                    }
                    }
                    catch
                    {
                        try
                        {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ReturnSubscriptionPost);
                        }
                        catch
                        {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ReturnPaymentProfilePost);
                        }
                    }
                         
                }
                else 
                {
                    if (retCustomerGet.customers[0].phones.Count == 0)
                    {
                        #region Update Customer

                        // Custumer Information
                        ObjUpdateCustomer.name = ObjClienteTMKT.name = ds.Tables[0].Rows[0]["NOME"].ToString(); //"Guilherme Henrique";
                        ObjUpdateCustomer.email = ds.Tables[0].Rows[0]["EMAIL_PESSOAL"].ToString(); //"teste@teste.com.br"; 
                        ObjUpdateCustomer.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString(); // "25949032806";
                        ObjUpdateCustomer.notes = "TMKT";
                        ObjUpdateMetadata.Vendedor = "TMKT";

                        // Address Information
                        ObjUpdateAddress.street = ""; //ds.Tables[0].Rows[0]["ENDERECO"].ToString(); ;
                        ObjUpdateAddress.number = ""; //ds.Tables[0].Rows[0]["NUMERO"].ToString();
                        ObjUpdateAddress.additional_details = "";// ds.Tables[0].Rows[0]["COMPLEMENTO"].ToString(); ;
                        ObjUpdateAddress.zipcode = ""; //ds.Tables[0].Rows[0]["CEP"].ToString();
                        ObjUpdateAddress.neighborhood = ""; //ds.Tables[0].Rows[0]["BAIRRO"].ToString(); //"Calmon Viana";
                        ObjUpdateAddress.city = ""; //ds.Tables[0].Rows[0]["CIDADE"].ToString();
                        ObjUpdateAddress.state = "";//ds.Tables[0].Rows[0]["UF"].ToString();
                        ObjUpdateAddress.country = "";//ds.Tables[0].Rows[0]["REGIAO"].ToString();

                        ObjUpdateCustomer.phones = new List<Models.UpdateCustomer.Phone>();

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ObjUpdateCustomer.phones.Add(new Models.UpdateCustomer.Phone("mobile", ds.Tables[0].Rows[i]["TEL"].ToString(), ""));
                        }

                        ObjUpdateCustomer.address = ObjUpdateAddress;
                        ObjUpdateCustomer.metadata = ObjUpdateMetadata;

                        string jsonObjUpdateCustomer = JsonConvert.SerializeObject(ObjUpdateCustomer);

                        LogDotz.GravaReturn(Clicodigo, jsonObjUpdateCustomer, "jsonObjUpdateCustomer");

                        var CustomerPut = new RestClient(ConfigurationManager.AppSettings.Get("CustomerPut").ToString()+retCustomerGet.customers[0].id);
                        var requestCustomerPut = new RestRequest(Method.PUT);
                        requestCustomerPut.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestCustomerPut.AddHeader("Content-Type", "application/json");
                        requestCustomerPut.AddJsonBody(jsonObjUpdateCustomer);
                        var responseCustomerPut = CustomerPut.Put(requestCustomerPut);
                        ReturnCustomerPut = responseCustomerPut.StatusCode.ToString();

                        LogDotz.GravaReturn(Clicodigo, responseCustomerPut.Content.ToString(), "CustomerPut");
                    #endregion
                    }
                    
                    else if (retCustomerGet.customers[0].phones.Count >= 3 )
                    {
                        #region Update Customer
                        // Custumer Information
                        ObjUpdateCustomer.name = ObjClienteTMKT.name = ds.Tables[0].Rows[0]["NOME"].ToString(); //"Guilherme Henrique";
                        ObjUpdateCustomer.email = ds.Tables[0].Rows[0]["EMAIL_PESSOAL"].ToString(); //"teste@teste.com.br"; 
                        ObjUpdateCustomer.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString(); // "25949032806";
                        ObjUpdateCustomer.notes = "TMKT";
                        ObjUpdateMetadata.Vendedor = "TMKT";

                        // Address Information
                        ObjUpdateAddress.street = ""; //ds.Tables[0].Rows[0]["ENDERECO"].ToString(); ;
                        ObjUpdateAddress.number = ""; //ds.Tables[0].Rows[0]["NUMERO"].ToString();
                        ObjUpdateAddress.additional_details = "";// ds.Tables[0].Rows[0]["COMPLEMENTO"].ToString(); ;
                        ObjUpdateAddress.zipcode = ""; //ds.Tables[0].Rows[0]["CEP"].ToString();
                        ObjUpdateAddress.neighborhood = ""; //ds.Tables[0].Rows[0]["BAIRRO"].ToString(); //"Calmon Viana";
                        ObjUpdateAddress.city = ""; //ds.Tables[0].Rows[0]["CIDADE"].ToString();
                        ObjUpdateAddress.state = "";//ds.Tables[0].Rows[0]["UF"].ToString();
                        ObjUpdateAddress.country = "";//ds.Tables[0].Rows[0]["REGIAO"].ToString();

                        ObjUpdateCustomer.address = ObjUpdateAddress;
                        ObjUpdateCustomer.metadata = ObjUpdateMetadata;

                        string jsonObjUpdateCustomer = JsonConvert.SerializeObject(ObjUpdateCustomer);

                        LogDotz.GravaReturn(Clicodigo, jsonObjUpdateCustomer, "jsonObjUpdateCustomer");

                        var CustomerPut = new RestClient(ConfigurationManager.AppSettings.Get("CustomerPut").ToString() + retCustomerGet.customers[0].id);
                        var requestCustomerPut = new RestRequest(Method.PUT);
                        requestCustomerPut.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestCustomerPut.AddHeader("Content-Type", "application/json");
                        requestCustomerPut.AddJsonBody(jsonObjUpdateCustomer);
                        var responseCustomerPut = CustomerPut.Put(requestCustomerPut);
                        ReturnCustomerPut = responseCustomerPut.StatusCode.ToString();

                        LogDotz.GravaReturn(Clicodigo, responseCustomerPut.Content.ToString(), "CustomerPut");

                        #endregion 
                    }

                    else if (retCustomerGet.customers[0].phones[0].number == ds.Tables[0].Rows[0]["TEL"].ToString())//|| //retCustomerGet.customers[0].phones[1].number == ds.Tables[0].Rows[0]["TEL"].ToString() || retCustomerGet.customers[0].phones[2].number == ds.Tables[0].Rows[0]["TEL"].ToString())
                    {
                        #region Update Customer
                        // Custumer Information
                        ObjUpdateCustomer.name = ObjClienteTMKT.name = ds.Tables[0].Rows[0]["NOME"].ToString(); //"Guilherme Henrique";
                        ObjUpdateCustomer.email = ds.Tables[0].Rows[0]["EMAIL_PESSOAL"].ToString(); //"teste@teste.com.br"; 
                        ObjUpdateCustomer.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString(); // "25949032806";
                        ObjUpdateCustomer.notes = "TMKT";
                        ObjUpdateMetadata.Vendedor = "TMKT";

                        // Address Information
                        ObjUpdateAddress.street = ""; //ds.Tables[0].Rows[0]["ENDERECO"].ToString(); ;
                        ObjUpdateAddress.number = ""; //ds.Tables[0].Rows[0]["NUMERO"].ToString();
                        ObjUpdateAddress.additional_details = "";// ds.Tables[0].Rows[0]["COMPLEMENTO"].ToString(); ;
                        ObjUpdateAddress.zipcode = ""; //ds.Tables[0].Rows[0]["CEP"].ToString();
                        ObjUpdateAddress.neighborhood = ""; //ds.Tables[0].Rows[0]["BAIRRO"].ToString(); //"Calmon Viana";
                        ObjUpdateAddress.city = ""; //ds.Tables[0].Rows[0]["CIDADE"].ToString();
                        ObjUpdateAddress.state = "";//ds.Tables[0].Rows[0]["UF"].ToString();
                        ObjUpdateAddress.country = "";//ds.Tables[0].Rows[0]["REGIAO"].ToString();

                        ObjUpdateCustomer.address = ObjUpdateAddress;
                        ObjUpdateCustomer.metadata = ObjUpdateMetadata;

                        string jsonObjUpdateCustomer = JsonConvert.SerializeObject(ObjUpdateCustomer);

                        LogDotz.GravaReturn(Clicodigo, jsonObjUpdateCustomer, "jsonObjUpdateCustomer");

                        var CustomerPut = new RestClient(ConfigurationManager.AppSettings.Get("CustomerPut").ToString() + retCustomerGet.customers[0].id);
                        var requestCustomerPut = new RestRequest(Method.PUT);
                        requestCustomerPut.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestCustomerPut.AddHeader("Content-Type", "application/json");
                        requestCustomerPut.AddJsonBody(jsonObjUpdateCustomer);
                        var responseCustomerPut = CustomerPut.Put(requestCustomerPut);
                        ReturnCustomerPut = responseCustomerPut.StatusCode.ToString();

                        LogDotz.GravaReturn(Clicodigo, responseCustomerPut.Content.ToString(), "CustomerPut");

                        #endregion
                    }
                    
                    else
                    {
                        #region Update Customer

                        // Custumer Information
                        ObjUpdateCustomer.name = ObjClienteTMKT.name = ds.Tables[0].Rows[0]["NOME"].ToString(); //"Guilherme Henrique";
                        ObjUpdateCustomer.email = ds.Tables[0].Rows[0]["EMAIL_PESSOAL"].ToString(); //"teste@teste.com.br"; 
                        ObjUpdateCustomer.registry_code = ds.Tables[0].Rows[0]["CPFCNPJ"].ToString(); // "25949032806";
                        ObjUpdateCustomer.notes = "TMKT";
                        ObjUpdateMetadata.Vendedor = "TMKT";
                       

                        // Address Information
                        ObjUpdateAddress.street = ""; //ds.Tables[0].Rows[0]["ENDERECO"].ToString(); ;
                        ObjUpdateAddress.number = ""; //ds.Tables[0].Rows[0]["NUMERO"].ToString();
                        ObjUpdateAddress.additional_details = "";// ds.Tables[0].Rows[0]["COMPLEMENTO"].ToString(); ;
                        ObjUpdateAddress.zipcode = ""; //ds.Tables[0].Rows[0]["CEP"].ToString();
                        ObjUpdateAddress.neighborhood = ""; //ds.Tables[0].Rows[0]["BAIRRO"].ToString(); //"Calmon Viana";
                        ObjUpdateAddress.city = ""; //ds.Tables[0].Rows[0]["CIDADE"].ToString();
                        ObjUpdateAddress.state = "";//ds.Tables[0].Rows[0]["UF"].ToString();
                        ObjUpdateAddress.country = "";//ds.Tables[0].Rows[0]["REGIAO"].ToString();

                        ObjUpdateCustomer.phones = new List<Models.UpdateCustomer.Phone>();

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ObjUpdateCustomer.phones.Add(new Models.UpdateCustomer.Phone("mobile", ds.Tables[0].Rows[i]["TEL"].ToString(), ""));
                        }

                        ObjUpdateCustomer.address = ObjUpdateAddress;
                        ObjClienteTMKT.metadata = ObjMetadataTMKT;

                        string jsonObjUpdateCustomer = JsonConvert.SerializeObject(ObjUpdateCustomer);

                        LogDotz.GravaReturn(Clicodigo, jsonObjUpdateCustomer, "jsonObjUpdateCustomer");

                        var CustomerPut = new RestClient(ConfigurationManager.AppSettings.Get("CustomerPut").ToString() + retCustomerGet.customers[0].id);
                        var requestCustomerPut = new RestRequest(Method.PUT);
                        requestCustomerPut.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestCustomerPut.AddHeader("Content-Type", "application/json");
                        requestCustomerPut.AddJsonBody(jsonObjUpdateCustomer);
                        var responseCustomerPut = CustomerPut.Put(requestCustomerPut);
                        ReturnCustomerPut = responseCustomerPut.StatusCode.ToString();

                        LogDotz.GravaReturn(Clicodigo, responseCustomerPut.Content.ToString(), "CustomerPut");

                        #endregion
                    }

                    #region Payment Method

                    try
                    {
                        var ReturnCardGet = new RestClient(ConfigurationManager.AppSettings.Get("AkivaAPI").ToString() + Clicodigo);
                    var requestReturnCardGet = new RestRequest(Method.POST);
                    requestReturnCardGet.AddHeader("Content-Type", "application/json");
                    var responseCardGet = ReturnCardGet.Get(requestReturnCardGet);
                    ReturnCardMessage = responseCardGet.StatusCode.ToString();

                    var Return = JsonConvert.DeserializeObject<Models.ReturnCard>(responseCardGet.Content.ToString()); // Deserializing Object

                    ds = Dados.ConsultaCliente(Clicodigo);

                    ObjPayment.holder_name = ds.Tables[0].Rows[0]["NOME"].ToString();
                    ObjPayment.card_expiration = ds.Tables[0].Rows[0]["DT_VENC_CART"].ToString();
                    ObjPayment.card_number = Return.Cliente_NumCartao; //"5167454851671773"; 
                    ObjPayment.card_cvv = Return.Cliente_CVVCartao; //"458"; 
                    ObjPayment.payment_method_code = "credit_card";
                    ObjPayment.payment_company_code = ds.Tables[0].Rows[0]["BANDEIRA_CART"].ToString();
                    ObjPayment.customer_id = retCustomerGet.customers[0].id;

                    string jsonObjPayment = JsonConvert.SerializeObject(ObjPayment);

                    LogDotz.GravaReturn(Clicodigo, jsonObjPayment, "jsonObjPayment");

                    var PaymentProfilePost = new RestClient(ConfigurationManager.AppSettings.Get("PaymentPost").ToString());
                    var requestPaymentProfilePost = new RestRequest(Method.POST);
                    requestPaymentProfilePost.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PublicKey").ToString()));// Adding Authenticator
                    requestPaymentProfilePost.AddHeader("Content-Type", "application/json");
                    requestPaymentProfilePost.AddJsonBody(jsonObjPayment);
                    var responsePaymentProfile = PaymentProfilePost.Post(requestPaymentProfilePost);
                    ReturnPaymentProfilePost = responsePaymentProfile.StatusCode.ToString() + "-" + responsePaymentProfile.StatusDescription ;

                    LogDotz.GravaReturn(Clicodigo, responsePaymentProfile.Content.ToString(), "PaymentPost");

                    var retPaymentProfilePost = JsonConvert.DeserializeObject<Models.PaymentReturn.Payment>(responsePaymentProfile.Content.ToString()); // Deserializing Object

                    #endregion

                    #region Subscription Method

                    ObjSubscriptions.start_at = "";
                    ObjSubscriptions.plan_id = Convert.ToInt32(ds.Tables[0].Rows[0]["PLAN_ID"].ToString()); //8527;  
                    ObjSubscriptions.customer_id = retCustomerGet.customers[0].id;
                    ObjSubscriptions.code = "";
                    ObjSubscriptions.payment_method_code = "credit_card";
                    ObjSubscriptions.installments = "";
                    ObjSubscriptions.billing_trigger_type = "";
                    ObjSubscriptions.billing_trigger_day = "";
                    ObjSubscriptions.billing_cycles = "";
                    ObjSubscriptions.metadata = "TMKT";

                    ObjSubscriptions.product_items = new List<Subscriptions.ProductItem>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ObjSubscriptions.product_items.Add(new Subscriptions.ProductItem(Convert.ToInt32(ds.Tables[0].Rows[i]["PRODUCT_ID"].ToString())));
                    }

                    ObjSubsPayment.gateway_token = retPaymentProfilePost.payment_profile.gateway_token.ToString();
                    
                    ObjSubscriptions.payment_profile = ObjSubsPayment;

                    string jsonObjSubscription = JsonConvert.SerializeObject(ObjSubscriptions);

                    LogDotz.GravaReturn(Clicodigo, jsonObjSubscription, "jsonObjSubscription");

                    var SubscriptionPost = new RestClient(ConfigurationManager.AppSettings.Get("SubscriptionsPost").ToString());
                    var requestSubscriptionPost = new RestRequest(Method.POST);
                    requestSubscriptionPost.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                    requestSubscriptionPost.AddHeader("Content-Type", "application/json");
                    requestSubscriptionPost.AddJsonBody(jsonObjSubscription);
                    var responseSubscriptionPost = SubscriptionPost.Post(requestSubscriptionPost);
                    ReturnSubscriptionPost = responseSubscriptionPost.StatusCode.ToString() + "-" + responseSubscriptionPost.StatusDescription;
 
                    var retSubscriptionPost = JsonConvert.DeserializeObject<Models.SubscriptionReturn.RetSubs>(responseSubscriptionPost.Content.ToString()); // Deserializing Object

                    LogDotz.GravaReturn(Clicodigo, responseSubscriptionPost.Content.ToString(), "SubscriptionPost");

                    #endregion

                    #region Subscription method Delete

                    
                    if (retSubscriptionPost.bill.charges[0].last_transaction.status != "success" && retSubscriptionPost.subscription.current_period.cycle == 1)
                    {
                        var SubscriptionDelete = new RestClient(ConfigurationManager.AppSettings.Get("SubscriptionsDelete").ToString()+retSubscriptionPost.subscription.id); Thread.Sleep(4000);
                        var requestSubscriptionDelete = new RestRequest(Method.DELETE);
                        requestSubscriptionDelete.AddHeader("Authorization", (ConfigurationManager.AppSettings.Get("PrivateKey").ToString()));// Adding Authenticator
                        requestSubscriptionDelete.AddHeader("Content-Type", "application/json");
                        requestSubscriptionDelete.AddJsonBody(jsonObjSubscription);
                        var responseSubscriptionDelete = SubscriptionDelete.Delete(requestSubscriptionDelete);
                        ReturnSubscriptionDelete = responseSubscriptionDelete.StatusDescription.ToString();
                        LogDotz.GravaReturn(Clicodigo, SubscriptionDelete.BaseUrl.ToString(), "URLSubscriptionDelete");
                        LogDotz.GravaReturn(Clicodigo, responseSubscriptionDelete.Content.ToString(), "SubscriptionDelete");

                        return Request.CreateResponse(HttpStatusCode.OK, "Transação não Autorizada");
                    }

                    else
                    {

                    }

                    #endregion

                    try
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, retSubscriptionPost.bill.charges[0].last_transaction.gateway_message);
                    }
                    catch
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, responseSubscriptionPost.StatusDescription);
                    }
                    }
                    catch
                    {
                        try
                        {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ReturnSubscriptionPost);
                        }
                        catch
                        {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ReturnPaymentProfilePost);
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex );
            }
            #endregion
        }
    }
}