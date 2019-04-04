﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PalotaInterviewCS
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string countriesEndpoint = "https://restcountries.eu/rest/v2/all";

        private static void Main(string[] args)
        {
            Country[] countries = GetCountries(countriesEndpoint).GetAwaiter().GetResult();
            var countriesData = new CalculateCountryFacts(countries);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Palota Interview: Country Facts");
            Console.WriteLine();
            Console.ResetColor();

            Random rnd = new Random(); // random to populate fake answer - you can remove this once you use real values

            //TODO use data operations and data structures to optimally find the correct value (N.B. be aware of null values)

            /*
             * HINT: Sort the list in descending order to find South Africa's place in terms of gini coefficients
             * `Country.Gini` is the relevant field to use here
             */
            int southAfricanGiniPlace = countriesData.GetGiniCoefficientForCountry("South Africa");
            Console.WriteLine($"1. South Africa's Gini coefficient is the {GetOrdinal(southAfricanGiniPlace)} highest");

            /*
             * HINT: Sort the list in ascending order or just find the Country with the minimum gini coeficient
             * use `Country.Gini` for the ordering then return the relevant country's name `Country.Name`
             */
            string lowestGiniCountry = countriesData.GetCountryWithMinimumGiniCoefficientValue(); // Use correct value
            Console.WriteLine($"2. {lowestGiniCountry} has the lowest Gini Coefficient");

            /*
             * HINT: Group by regions (`Country.Region`), then count the number of unique timezones that the countries in each region span
             * Once you have done the grouping, find the group `Region` with the most timezones and return it's name and the number of unique timezones found
             */
            var regionWithMostTZ = countriesData.GetRegionWithMostNumbersOfTimeZones();
            string regionWithMostTimezones = regionWithMostTZ.Key;
            int amountOfTimezonesInRegion = regionWithMostTZ.Value;
            Console.WriteLine($"3. {regionWithMostTimezones} is the region that spans most timezones at {amountOfTimezonesInRegion} timezones");

            /*
             * HINT: Count occurrences of each currency in all countries (check `Country.Currencies`)
             * Find the name of the currency with most occurrences and return it's name (`Currency.Name`) also return the number of occurrences found for that currency
             */
            var famousCurrency = countriesData.GetMostUsedCurrencyAmongCountries();
            string mostPopularCurrency = famousCurrency.Key; // Use correct value
            int numCountriesUsedByCurrency = famousCurrency.Value;// Use correct value
            Console.WriteLine($"4. {mostPopularCurrency} is the most popular currency and is used in {numCountriesUsedByCurrency} countries");

            /*
             * HINT: Count the number of occurrences of each language (`Country.Languages`) and sort then in descending occurrences count (i.e. most popular first)
             * Once done return the names of the top three languages (`Language.Name`)
             */
            string[] mostPopularLanguages = countriesData.GetMostPopularLanguaguesAmongCountries().Select(item => item.Key).ToArray(); // Use correct values
            Console.WriteLine($"5. The top three popular languages are {mostPopularLanguages[0]}, {mostPopularLanguages[1]} and {mostPopularLanguages[2]}");

            /*
             * HINT: Each country has an array of Bordering countries `Country.Borders`, The array has a list of alpha3 codes of each bordering country `Country.alpha3Code`
             * Sum up the population of each country (`Country.Population`) along with all of its bordering countries's population. Sort this list according to the combined population descending
             * Find the country with the highest combined (with bordering countries) population the return that country's name (`Country.Name`), the number of it's Bordering countries (`Country.Borders.length`) and the combined population
             * Be wary of null values
             */

            var countryWithHeighestCombinePopulation = countriesData.GetCountryWithHighestCombinePopulation();
            long numberOfBorderingCountries = countryWithHeighestCombinePopulation.NumberOfBorderingCountries;
            long combinedPopulation = countryWithHeighestCombinePopulation.TotalPopulation;
            string countryWithBorderingCountries = countryWithHeighestCombinePopulation.CountryName;
            Console.WriteLine($"6. {countryWithBorderingCountries} and it's {numberOfBorderingCountries} bordering countries has the highest combined population of {combinedPopulation}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the lowest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of null values when doing calculations
             */
            var lowestDensityCountry = countriesData.GetCountryWithDensityData(true);
            string lowPopDensityName = lowestDensityCountry.Key;
            double lowPopDensity = lowestDensityCountry.Value;
            Console.WriteLine($"7. {lowPopDensityName} has the lowest population density of {lowPopDensity}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the highest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of any null values when doing calculations. Consider reusing work from above related question
             */
            var highestDensityCountry = countriesData.GetCountryWithDensityData(false);
            string highPopDensityName = highestDensityCountry.Key;
            double highPopDensity = Math.Round(highestDensityCountry.Value, 2);
            Console.WriteLine($"8. {highPopDensityName} has the highest population density of {highPopDensity}");

            /*
             * HINT: Group by subregion `Country.Subregion` and sum up the area (`Country.Area`) of all countries per subregion
             * Sort the subregions by the combined area to find the maximum (or just find the maximum)
             * Return the name of the subregion
             * Be wary of any null values when summing up the area
             */
            var largestSubRegion = countriesData.GetLargestSubRegion();
            string largestAreaSubregion = largestSubRegion.Key; // Use correct value
            Console.WriteLine($"9. {largestAreaSubregion} is the subregion that covers the most area");

            /*
             * HINT: Group by regional blocks (`Country.RegionalBlocks`). For each regional block, average out the gini coefficient (`Country.Gini`) of all member countries
             * Sort the regional blocks by the average country gini coefficient to find the lowest (or find the lowest without sorting)
             * Return the name of the regional block (`RegionalBloc.Name`) along with the calculated average gini coefficient
             */
            var mostEqualRegionalBlockData = countriesData.GetRegionalBlockWithLowestGini();
            string mostEqualRegionalBlock = mostEqualRegionalBlockData.Key;
            double lowestRegionalBlockGini = Math.Round(mostEqualRegionalBlockData.Value, 2);
            Console.WriteLine($"10. {mostEqualRegionalBlock} is the regional block with the lowest average Gini coefficient of {lowestRegionalBlockGini}");
        }

        /// <summary>
        /// Gets the countries from a specified endpoint
        /// </summary>
        /// <returns>The countries.</returns>
        /// <param name="path">Path endpoint for the API.</param>
        private static async Task<Country[]> GetCountries(string path)
        {
            Country[] countries = null;
            countries = Country.FromJsonByUsingStream(path);
            return countries;
        }

        /// <summary>
        /// Gets the ordinal value of a number (e.g. 1 to 1st)
        /// </summary>
        /// <returns>The ordinal.</returns>
        /// <param name="num">Number.</param>
        public static string GetOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";

                case 2:
                    return num + "nd";

                case 3:
                    return num + "rd";

                default:
                    return num + "th";
            }
        }
    }
}