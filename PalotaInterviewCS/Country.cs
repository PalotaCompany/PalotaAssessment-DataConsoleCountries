using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;

namespace PalotaInterviewCS
{
    public partial class Country
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("topLevelDomain")]
        public string[] TopLevelDomain { get; set; }

        [JsonProperty("alpha2Code")]
        public string Alpha2Code { get; set; }

        [JsonProperty("alpha3Code")]
        public string Alpha3Code { get; set; }

        [JsonProperty("callingCodes")]
        public string[] CallingCodes { get; set; }

        [JsonProperty("capital")]
        public string Capital { get; set; }

        [JsonProperty("altSpellings")]
        public string[] AltSpellings { get; set; }

        [JsonProperty("region")]
        public Region Region { get; set; }

        [JsonProperty("subregion")]
        public string Subregion { get; set; }

        [JsonProperty("population")]
        public long Population { get; set; }

        [JsonProperty("latlng")]
        public double[] Latlng { get; set; }

        [JsonProperty("demonym")]
        public string Demonym { get; set; }

        [JsonProperty("area")]
        public double? Area { get; set; }

        [JsonProperty("gini")]
        public double? Gini { get; set; }

        [JsonProperty("timezones")]
        public string[] Timezones { get; set; }

        [JsonProperty("borders")]
        public string[] Borders { get; set; }

        [JsonProperty("nativeName")]
        public string NativeName { get; set; }

        [JsonProperty("numericCode")]
        public string NumericCode { get; set; }

        [JsonProperty("currencies")]
        public Currency[] Currencies { get; set; }

        [JsonProperty("languages")]
        public Language[] Languages { get; set; }

        [JsonProperty("translations")]
        public Translations Translations { get; set; }

        [JsonProperty("flag")]
        public Uri Flag { get; set; }

        [JsonProperty("regionalBlocs")]
        public RegionalBloc[] RegionalBlocs { get; set; }

        [JsonProperty("cioc")]
        public string Cioc { get; set; }
    }

    public class Currency
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }

    public class Language
    {
        [JsonProperty("iso639_1")]
        public string Iso6391 { get; set; }

        [JsonProperty("iso639_2")]
        public string Iso6392 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nativeName")]
        public string NativeName { get; set; }
    }

    public class RegionalBloc
    {
        [JsonProperty("acronym")]
        public string Acronym { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("otherAcronyms")]
        public string[] OtherAcronyms { get; set; }

        [JsonProperty("otherNames")]
        public string[] OtherNames { get; set; }
    }

    public class Translations
    {
        [JsonProperty("de")]
        public string De { get; set; }

        [JsonProperty("es")]
        public string Es { get; set; }

        [JsonProperty("fr")]
        public string Fr { get; set; }

        [JsonProperty("ja")]
        public string Ja { get; set; }

        [JsonProperty("it")]
        public string It { get; set; }

        [JsonProperty("br")]
        public string Br { get; set; }

        [JsonProperty("pt")]
        public string Pt { get; set; }

        [JsonProperty("nl")]
        public string Nl { get; set; }

        [JsonProperty("hr")]
        public string Hr { get; set; }

        [JsonProperty("fa")]
        public string Fa { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Region
    {
        [EnumMember(Value = "")]
        Empty,
        [EnumMember(Value = "Africa")]
        Africa,
        [EnumMember(Value = "Americas")]
        Americas,
        [EnumMember(Value = "Asia")]
        Asia,
        [EnumMember(Value = "Europe")]
        Europe,
        [EnumMember(Value = "Oceania")]
        Oceania,
        [EnumMember(Value = "Polar")]
        Polar
    }

    public partial class Country
    {
        public static Country[] FromJson(string json) => JsonConvert.DeserializeObject<Country[]>(json);
        public static string ToJson(Country[] self) => JsonConvert.SerializeObject(self);

        /// <summary>
        /// As newton soft's documentation at https://www.newtonsoft.com/json/help/html/Performance.htm
        /// if JSON is huge, Deserialize it using stream for better performance. 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Country[] FromJsonByUsingStream(string url)
        {
            try
            {
                Country[] countries = new Country[0];
                HttpClient httpClient = new HttpClient();
                using (Stream s = httpClient.GetStreamAsync(url).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    countries = serializer.Deserialize<Country[]>(reader);
                }
                return countries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deserialization  \n {ex.Message} ");
                throw ex;
            }
           
        }
    }
}