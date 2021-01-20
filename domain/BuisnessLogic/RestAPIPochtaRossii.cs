using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class RestAPIPochtaRossii
    {
        private readonly string dogovor = "69000";
        private readonly string from = "69000";

        public float GetPrice(string to, float waight)
        {
            float price = float.NaN;
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HHmm");

            WebRequest request = WebRequest.Create($"tariff.pochta.ru/v2/calculate/tariff?json&object=27030&dogovor={dogovor}&from={from}&to={to}&weight={waight}&pack=10&date={date}&time={time}");

            WebResponse response = request.GetResponse();

            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                string json;
                if ((json = stream.ReadToEnd()) != null)
                {

                    JObject jObject = JObject.Parse(json);
                    JToken _price = jObject["paynds"];
                    price = _price.ToObject<float>();
                }
            }

            return price;
        }
    }
}
