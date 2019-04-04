using System;
using System.Collections.Generic;
using System.Linq;

namespace PalotaInterviewCS
{
    /// <summary>
    /// Structure to hold data for Country and its bordering nations' population
    /// </summary>
    public struct CountryAndBorderingCountryPopulation
    {
        public string CountryName;
        public long CountryPopulation;
        public int NumberOfBorderingCountries;
        public long BorderingCountryPopulation;
        public long TotalPopulation;
    }

    /// <summary>
    /// Class to calculate various facts of country. it has an array of country which will be used in calculation.
    /// </summary>
    public class CalculateCountryFacts
    {
        //country array
        private readonly Country[] _countries;

        public CalculateCountryFacts()
        {
            _countries = new Country[0];
        }

        public CalculateCountryFacts(Country[] countries)
        {
            _countries = countries;
        }

        /// <summary>
        /// Calculate GINI coefficient
        /// </summary>
        /// <param name="countryName">Name of country for which you want to calculate GINI coefficient</param>
        /// <returns></returns>
        public int GetGiniCoefficientForCountry(string countryName)
        {
            checkCountriesLenght();
            if (string.IsNullOrEmpty(nameof(countryName)))
            {
                throw new ArgumentNullException(nameof(countryName) + " must not be null or an empty string");
            }

            //First order by countries in GINI coefficient descending
            var countriesByGiniIndexDescending = _countries.Where(country => country.Gini.HasValue) //filter country which has gini's value
                                                    .OrderByDescending(country => country.Gini.Value)//order by descending
                                                    .Select(c => c);
            //calculate index of country
            var countryWithIndex = countriesByGiniIndexDescending.Select((country, index) => new { country, index }) //select index
                                                            .FirstOrDefault(c => c.country.Name.Equals(countryName, StringComparison.InvariantCultureIgnoreCase)); // compare country

            //as index start with zero we need +1 to return correct index
            return countryWithIndex == null ? -1 : countryWithIndex.index + 1;
        }

        /// <summary>
        /// Return country name which has lowest gini coefficient
        /// </summary>
        /// <returns></returns>
        public string GetCountryWithMinimumGiniCoefficientValue()
        {
            checkCountriesLenght();
            var country = _countries.Where(c => c.Gini.HasValue).OrderBy(c => c.Gini.Value).FirstOrDefault();
            return country == null ? "No country found" : country.Name;
        }

        /// <summary>
        /// Return region name which has highest number of time zones along with count of its time zones.
        /// </summary>
        /// <returns>Region Name and count of its time zones stored in key value pair</returns>
        public KeyValuePair<string, int> GetRegionWithMostNumbersOfTimeZones()
        {
            Dictionary<string, int> regionAndTimeZoneCount = new Dictionary<string, int>();

            //get countries region wise
            var countriesGroupByRegion = from c in _countries
                                         where c.Region != Region.Empty
                                         group c by c.Region.ToString().ToLower() into countriesByRegion
                                         select countriesByRegion;

            //calculate distinct time zones in each region. Add it into dictionary.
            foreach (var countriesInEachRegion in countriesGroupByRegion)
            {
                var distinctTimeZoneCount = (from country in countriesInEachRegion
                                             from tz in country.Timezones
                                             where !string.IsNullOrEmpty(tz)
                                             select tz.ToLower()).Distinct().Count();

                regionAndTimeZoneCount.Add(countriesInEachRegion.Key, distinctTimeZoneCount);
            }

            //return region and its time zone.
            return regionAndTimeZoneCount.OrderByDescending(item => item.Value).First();
        }

        /// <summary>
        /// Return highest used currency along with count of countries which used it
        /// </summary>
        /// <returns>Currency and count of countries which used it stored in key value pair</returns>
        public KeyValuePair<string, int> GetMostUsedCurrencyAmongCountries()
        {
            Dictionary<string, int> currencyAndCountriesCount = new Dictionary<string, int>();

            //Get distinct currencies.
            var currencies = (from country in _countries
                              from currency in country.Currencies
                              where !string.IsNullOrEmpty(currency.Name)
                              select currency.Name.ToLower()).Distinct();

            //count number of countries which use each currency
            foreach (var curName in currencies)
            {
                var countOfCountries = _countries.Count(c => c.Currencies.Where(cur => !string.IsNullOrEmpty(cur.Name)).Select(cur => cur.Name.ToLower()).Contains(curName));
                currencyAndCountriesCount.Add(curName, countOfCountries);
            }

            //return highest used currency and countries' count which used it.
            return currencyAndCountriesCount.OrderByDescending(item => item.Value).First();
        }

        /// <summary>
        /// Return highest spoken languages along with count of countries which speak it
        /// </summary>
        /// <returns>Top 3 most spoken language, and count of countries that speak it</returns>
        public List<KeyValuePair<string, int>> GetMostPopularLanguaguesAmongCountries()
        {
            Dictionary<string, int> languageAndCountriesCount = new Dictionary<string, int>();

            //get distinct language
            var languages = (from country in _countries
                             from language in country.Languages
                             where !string.IsNullOrEmpty(language.Name)
                             select language.Name.ToLower()).Distinct();

            //count a number of countries which speak each language
            foreach (var lanName in languages)
            {
                var countOfCountries = _countries.Count(c => c.Languages.Where(l => !string.IsNullOrEmpty(l.Name)).Select(l => l.Name.ToLower()).Contains(lanName));
                languageAndCountriesCount.Add(lanName, countOfCountries);
            }

            //return 3 highest spoken language and count of its countries.
            return languageAndCountriesCount.OrderByDescending(item => item.Value).Take(3).ToList();
        }

        /// <summary>
        /// Return country, number of its bordering nations and total population of the country and its bordering nation
        /// </summary>
        /// <returns>Country along with its bordering nations and population of both stored in structure</returns>
        public CountryAndBorderingCountryPopulation GetCountryWithHighestCombinePopulation()
        {
            List<CountryAndBorderingCountryPopulation> countryAndBorderingCountryPopulations = new List<CountryAndBorderingCountryPopulation>();

            //group countries by their border nations
            var countryAlongWithBorders = from c in _countries
                                          where c.Borders != null && c.Borders.Length > 0
                                          group c by c.Borders into countriesAndBordersGroup
                                          select countriesAndBordersGroup;

            string countryWithBorderingCountries = string.Empty;
            int numberOfBorderingCountries = 0;
            long combinedPopulation = 0;

            //calculate total population of each border nations group along with country
            foreach (var countryAndBorder in countryAlongWithBorders)
            {
                //calculate population of border nations
                combinedPopulation = _countries.Where(
                                                    c => countryAndBorder.Key.Contains(c.Alpha3Code))
                                                            .Sum(c => c.Population);
                //get population of the country
                long countryPopulation = _countries.Where(
                                                    c => c.Name == countryAndBorder.Select(cnt => cnt.Name).First())
                                                    .Sum(c => c.Population);
                //get total numbers of border nation
                numberOfBorderingCountries = countryAndBorder.Key.Length;
                countryWithBorderingCountries = countryAndBorder.Select(c => c.Name).FirstOrDefault();
                countryAndBorderingCountryPopulations.Add(
                    new CountryAndBorderingCountryPopulation
                    {
                        CountryName = countryWithBorderingCountries,
                        CountryPopulation = countryPopulation,
                        BorderingCountryPopulation = combinedPopulation,
                        NumberOfBorderingCountries = numberOfBorderingCountries,
                        TotalPopulation = countryPopulation + combinedPopulation
                    });
            }
            //return country with highest combine population along with total number of border nation.
            return countryAndBorderingCountryPopulations.OrderByDescending(c => c.TotalPopulation).FirstOrDefault();
        }

        /// <summary>
        /// Return country with either highest density or lowest density
        /// </summary>
        /// <param name="IsLowestDensityCountry">true to get lowest density county, false other wise</param>
        /// <returns>Highest or lowest dense country along with density</returns>
        public KeyValuePair<string, double> GetCountryWithDensityData(bool IsLowestDensityCountry)
        {
            KeyValuePair<string, double> countryWithDensityData = new KeyValuePair<string, double>();
            //get country name and density data
            var countriesWithDensityData = from c in _countries
                                           where c.Area.HasValue && c.Area.Value > 0
                                           let den = c.Population / c.Area.Value
                                           select new { c.Name, Density = den };
            //calculate density: order by on density data and return firs element. which will be either highest or lowest density.
            if (IsLowestDensityCountry)
            {
                var lowestDensityCountry = countriesWithDensityData.OrderBy(c => c.Density).First();
                countryWithDensityData = new KeyValuePair<string, double>(lowestDensityCountry.Name, lowestDensityCountry.Density);
            }
            else
            {
                var highestDenistyCountry = countriesWithDensityData.OrderByDescending(c => c.Density).First();
                countryWithDensityData = new KeyValuePair<string, double>(highestDenistyCountry.Name, highestDenistyCountry.Density);
            }
            return countryWithDensityData;
        }

        /// <summary>
        /// Return sub region name and total area of sub region (sum of area of all countries in region)
        /// </summary>
        /// <returns>Subregion name and total area of subregion stored in key value pair</returns>
        public KeyValuePair<string, double> GetLargestSubRegion()
        {
            var largestSubRegionData = new KeyValuePair<string, double>();

            //group country by subregion and calculate total area of all countries in that subregion
            var countriesBySubRegions = from c in _countries
                                        where !string.IsNullOrEmpty(c.Subregion) && c.Area.HasValue
                                        group c by c.Subregion into countriesBySubregion
                                        select new
                                        {
                                            SubRegion = countriesBySubregion.Key,
                                            TotalArea = countriesBySubregion.Sum(c => c.Area.Value)
                                        };
            //get largest subregion by order descending on total area of subregion
            var largestSubRegion = countriesBySubRegions.OrderByDescending(sr => sr.TotalArea).FirstOrDefault();
            if (largestSubRegion != null)
            {
                largestSubRegionData = new KeyValuePair<string, double>(largestSubRegion.SubRegion, largestSubRegion.TotalArea);
            }
            return largestSubRegionData;
        }

        /// <summary>
        /// Return regional block which has lowest average GINI coefficient
        /// </summary>
        /// <returns>Regional block name and its lowest average GINI coefficient stored in key value pair</returns>
        public KeyValuePair<string, double> GetRegionalBlockWithLowestGini()
        {
            var regionalBlockWithLowestGiniData = new KeyValuePair<string, double>();
            List<KeyValuePair<string, double>> regionalBlocksAndAvgGiniEfficient = new List<KeyValuePair<string, double>>();

            //get all regional blocks
            var regionalBlocks = (from c in _countries
                                  from rb in c.RegionalBlocs
                                  where !string.IsNullOrEmpty(rb.Name)
                                  select rb.Name.ToLower()).Distinct();

            //for each regional block calculate average of GINI coefficient
            foreach (string regionalBloc in regionalBlocks)
            {
                var avgGiniForRegionalBlock = _countries.Where(
                                                    c => c.RegionalBlocs.Select(rb => rb.Name.ToLower()).Contains(regionalBloc) &&
                                                    c.Gini.HasValue)
                                              .Average(c => c.Gini.Value);

                regionalBlocksAndAvgGiniEfficient.Add(new KeyValuePair<string, double>(regionalBloc, avgGiniForRegionalBlock));
            }

            //return regional block which has lowest GINI coefficient
            regionalBlockWithLowestGiniData = regionalBlocksAndAvgGiniEfficient.OrderBy(rb => rb.Value).FirstOrDefault();

            return regionalBlockWithLowestGiniData;
        }

        /// <summary>
        /// Hygienic check that countries array is not an empty array.
        /// </summary>
        private void checkCountriesLenght()
        {
            if (_countries.Length == 0)
            {
                throw new InvalidOperationException("Countries array is empty. Can not calculate anything...");
            }
        }
    }
}