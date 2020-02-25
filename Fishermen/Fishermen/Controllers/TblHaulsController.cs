using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fishermen.Models;
using System.Globalization;
using MoreLinq;
using Microsoft.Extensions.Configuration;

namespace Fishermen.Controllers
{
    public class TblHaulsController : Controller
    {
        private readonly PhishermenContext fishHauls;

        public TblHaulsController(PhishermenContext context)
        {
            fishHauls = context;
        }

        [HttpGet]
        [Route("api/TopTenHaulsByDate")]
        //returns the best ten hauls on a given date
        public List<Tuple<int,string>> TopTenHaulsByDate(int month, int year)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var topTenHauls = (from haul in hauls
                               join location in locations on haul.LocationId equals location.LocationId
                               orderby haul.Caught descending
                               where haul.Month == month && haul.Year == year
                               select new Tuple<int,string>(haul.Caught, location.AreaName))
                              .Take(10).ToList();

            return topTenHauls;
        }


        //this method answers the question that Luke gave as an example, you give a list of systems and a month and it returns
        //the 5 areas that historically have done the best in those systems during that month.
        [HttpGet]
        [Route("api/BestPlaceToFishDuringMonth")]
        public IActionResult BestPlaceToFishDuringMonth(string[] listOfSystems, int month)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var systems = fishHauls.TblSystems;
           

            var top5Areas = (from haul in hauls
                             join location in locations on haul.LocationId equals location.LocationId
                             join system in systems on location.SystemId equals system.SystemId
                             where listOfSystems.Contains(system.SystemName) && haul.Month == month
                             group haul by haul.LocationId into haulGroup
                             orderby haulGroup.Sum(h => h.Caught) descending
                             select new { 
                                LocationID = haulGroup.Key,
                                Sum = haulGroup.Sum(h => h.Caught) 
                             }).Take(5);

            var results = top5Areas.ToList();

            var finalResults = new List<Tuple<int, string, int>>();
            //trying to use the location ID to find the corresponding areaNumber and areaName in the original query
            //behaves strangely, think it has something to do with the haulGroup.Key portion, it selects for every haulgroup.key
            //instead of the current one, I'm not good enough at linq to deal with it so I did it this stupid way instead.
            for(int i = 0; i < results.Count; i++)
            {
                var areaNumber = (from location in locations
                                  where location.LocationId == results[i].LocationID
                                  select location.AreaNumber).ToList()[0];
                var areaName = (from location in locations
                                  where location.LocationId == results[i].LocationID
                                  select location.AreaName).ToList()[0];

                finalResults.Add(Tuple.Create(areaNumber, areaName, results[i].Sum));
            }
            return Json(finalResults);
        }

        //tells you when the best time to fish in a certain area is
        [HttpGet]
        [Route("api/BestMonthInArea")]
        public List<Tuple<string, int>> BestMonthInArea(int areaNumber)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var bestMonth = (from haul in hauls
                             join location in locations on haul.LocationId equals location.LocationId
                             where location.AreaNumber == areaNumber
                             group haul by haul.Month into haulGroup
                             orderby haulGroup.Sum(h => h.Caught) descending
                             select new Tuple<string, int>(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(haulGroup.Key), haulGroup.Sum(h => h.Caught)));
                               
                               
                               //select new Tuple<int, string>(haul.Caught, location.AreaName))
                              
            
            return bestMonth.ToList();
        }

        //returns the total fish caught per year in all areas
        [HttpGet]
        [Route("api/GetTotalHauls")]
        public IActionResult GetTotalHauls()
        {
            var hauls = fishHauls.TblHauls;

            var haulsByYear = from haul in hauls
                              group haul by haul.Year into haulGroup
                              orderby haulGroup.Key
                              select new { Year = haulGroup.Key, TotalHaul = haulGroup.Sum(h => h.Caught) };

            return Json(haulsByYear);

        }

        //returns a list of all areas, include number and name because name is not a unique identifier
        [HttpGet]
        [Route("api/GetAreas")]
        public IActionResult GetAreas()
        {
            var areas = fishHauls.TblLocations;
            var areaNumbers = from area in areas
                              group area by new { area.AreaNumber, area.AreaName } into areaGroup
                              select new {AreaNumber= areaGroup.Key.AreaNumber, AreaName = areaGroup.Key.AreaName};
            
            return Json(areaNumbers.ToList());

        }

        //returns a list of all systems
        [HttpGet]
        [Route("api/GetSystems")]
        public IActionResult GetSystems()
        {
            var systems = fishHauls.TblSystems;
            var systemsList = from system in systems
                              select system.SystemName;

            return Json(systemsList.ToList());
        }

        //returns a list of all 4 regions
        [HttpGet]
        [Route("api/GetRegions")]
        public IActionResult GetRegions()
        {
            var regions = fishHauls.TblRegions;
            var regionsList = from region in regions
                              select region.RegionName;

            return Json(regionsList.ToList());
        }

        //returns a list of all valid years, years that occur at least once in the database.
        //since this database won't be changed I will just hardcode the valid values based on what's currently in the database
        //for performance reasons. For a database that was being updated/changed you would need a different approach
        //to account for more valid years being added.
        [HttpGet]
        [Route("api/GetYears")]
        public IActionResult GetYears()
        {
            List<int> validYears = new List<int>();
            
            for(int i = 1961; i < 2018; i++)
            {
                validYears.Add(i);
            }
            return Json(validYears);
        }

        [HttpGet]
        [Route("api/GetMonths")]
        public IActionResult GetMonths()
        {
            var validMonths = DateTimeFormatInfo.CurrentInfo.MonthNames;
            //the previous variable has 13 values since some cultures have 13 months, so this is to remove the last null value
            validMonths = validMonths.Take(validMonths.Count() - 1).ToArray();
            return Json(validMonths);
        }

        //returns the full haul history of a given area, requires area number instead of
        //area name because area names can repeat
        [HttpGet]
        [Route("api/GetAreaHistory")]
        public IActionResult GetAreaHistory(int areaNumber)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var areaHistory = from haul in hauls
                              join location in locations on haul.LocationId equals location.LocationId
                              where location.AreaNumber == areaNumber
                              orderby haul.Year, haul.Month
                              select new { haul.Month, haul.Year, haul.Caught };

            return Json(areaHistory.ToList());
        }
        //this returns all the data we have with the only the most relevant columns included
        [HttpGet]
        [Route("api/GetAllData")]
        public IActionResult GetAllData()
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var regions = fishHauls.TblRegions;
            var systems = fishHauls.TblSystems;

            var allData = from haul in hauls
                          join location in locations on haul.LocationId equals location.LocationId
                          join system in systems on location.LocationId equals system.SystemId
                          join region in regions on location.LocationId equals region.RegionId
                          orderby haul.Year, haul.Month, location.AreaNumber
                          select new
                          {
                              Region = region.RegionName,
                              System = system.SystemName,
                              AreaNumber = location.AreaNumber,
                              AreaName = location.AreaName,
                              FishCaught = haul.Caught,
                              Year = haul.Year,
                              Month = haul.Month
                          };

            return Json(allData);
        }

    }
}