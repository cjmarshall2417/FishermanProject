using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fishermen.Models;
using System.Globalization;
using MoreLinq;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;


namespace Fishermen.Controllers
{
    public class TblHaulsController : Controller
    {
        public string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private readonly PhishermenContext fishHauls;

        public TblHaulsController(PhishermenContext context)
        {
            fishHauls = context;
        }

        [HttpGet]
        [Route("api/TopTenHaulsByDate")]
        //returns the best (or worst) hauls on a given date
        public IActionResult TopTenHaulsByDate(int month, int year, bool ascendingSort, int rows = 10)
        {
            var url = GetUrlPath();

            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var topTenHauls = (from haul in hauls
                               join location in locations on haul.LocationId equals location.LocationId
                               orderby haul.Caught ascending
                               where haul.Month == month && haul.Year == year
                               select new { Caught = haul.Caught, AreaNumber = location.AreaNumber, AreaName = location.AreaName });

            if (!ascendingSort)
            {
                topTenHauls = topTenHauls.OrderByDescending(h => h.Caught);
            }
            return Json(topTenHauls.Take(rows).ToList());
        }


        //this method answers the question that Luke gave as an example, you give a list of systems and a month and it returns
        //the  areas that historically have done the best in those systems during that month.
        [HttpGet]
        [Route("api/BestPlaceToFishDuringMonth")]
        public IActionResult BestPlaceToFishDuringMonth(string[] listOfSystems, int month, int rows = 10)
        {

            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var systems = fishHauls.TblSystems;


            var topAreas = (from haul in hauls
                            join location in locations on haul.LocationId equals location.LocationId
                            join system in systems on location.SystemId equals system.SystemId
                            where listOfSystems.Contains(system.SystemName) && haul.Month == month
                            group haul by new { AreaNumber = location.AreaNumber, AreaName = location.AreaName } into haulGroup
                            orderby haulGroup.Sum(h => h.Caught) / haulGroup.Count() descending
                            select new {
                                AreaNumber = haulGroup.Key.AreaNumber,
                                AreaName = haulGroup.Key.AreaName,
                                AverageHaul = haulGroup.Sum(h => h.Caught) / haulGroup.Count()
                            }).Take(rows);

            return Json(topAreas);
        }

        //tells you when the best time to fish in a certain area is
        [HttpGet]
        [Route("api/BestMonthInArea")]
        public IActionResult BestMonthInArea(int areaNumber)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var bestMonth = (from haul in hauls
                             join location in locations on haul.LocationId equals location.LocationId
                             where location.AreaNumber == areaNumber
                             group haul by haul.Month into haulGroup
                             select new { Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(haulGroup.Key)//converts a month number to a month name
                             , MonthNumber = haulGroup.Key
                             ,FishCaught = haulGroup.Sum(h => h.Caught) });

            var bestMonthList = bestMonth.ToList();
            if(bestMonthList.Count() != 12)
            {
                for (int i = 0; i < months.Count(); i++)
                {
                    if (bestMonthList.FindIndex(m => m.Month == months[i]) == -1)
                    {
                        bestMonthList.Add(new {Month = months[i], MonthNumber = i + 1, FishCaught = 0 });
                    }

                }
            }

            var result = bestMonthList.OrderBy(m => m.MonthNumber);
            return Json(result);
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
                              select new { AreaNumber = areaGroup.Key.AreaNumber, AreaName = areaGroup.Key.AreaName };

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

            for (int i = 1961; i < 2018; i++)
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

            var validMonthObjectsAnon = new Object[12];
            //add the month number to each month name.
            for(int i = 0; i < validMonths.Count(); i++)
            {
                validMonthObjectsAnon[i] = new {MonthNumber = i + 1, MonthName = validMonths[i] };
            }
            
            return Json(validMonthObjectsAnon);
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
                              select new { Month = haul.Month, Year = haul.Year, Sum = haul.Caught };

            return Json(areaHistory.ToList());
        }
        //this query exists to give the user more control over the data. While the other queries answer specific predetermined questions
        //this is closer to the functionality of an excel sheet where the user can apply a bunch of filters on many different columns to 
        //answer whatever question they choose. I'm still pondering the best way to handle giving the user the ability to use group bys.
        [HttpGet]
        [Route("api/CustomQuery")]
        public IActionResult CustomQuery(int[] years, int[] months,
            int[] areaNumbers, string[] areaNames, string[] regions, string[] systems, string groupBy, bool average = true,
            int haulGreaterThan = -1, int haulLessThan = -1, int rows = 1000)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var regionsTbl = fishHauls.TblRegions;
            var systemsTbl = fishHauls.TblSystems;

            //this messy query joins all our tables to return all of the data we currently have
            var allData = from haul in hauls
                          join location in locations on haul.LocationId equals location.LocationId
                          join system in systemsTbl on location.SystemId equals system.SystemId
                          join region in regionsTbl on location.RegionId equals region.RegionId
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
            //this applies our filters to the current data
            //I'm debating making some sort of filter object and iterating over it
            //but for now this works, even if it looks dumb.
            if (years.Count() > 0)
            {
                allData = allData.Where(h => years.Contains(h.Year));
            }
            if (months.Count() > 0)
            {
                allData = allData.Where(h => months.Contains(h.Month));
            }
            if (areaNames.Count() > 0)
            {
                allData = allData.Where(h => areaNames.Contains(h.AreaName));
            }
            if (areaNumbers.Count() > 0)
            {
                allData = allData.Where(h => areaNumbers.Contains(h.AreaNumber));
            }
            if (regions.Count() > 0)
            {
                allData = allData.Where(h => regions.Contains(h.Region));
            }
            if (systems.Count() > 0)
            {
                allData = allData.Where(h => systems.Contains(h.System));
            }
            if (haulGreaterThan != -1)
            {
                allData = allData.Where(h => h.FishCaught > haulGreaterThan);
            }
            if (haulLessThan != -1)
            {
                allData = allData.Where(h => h.FishCaught < haulLessThan);
            }
            //checks for groupBys and applies them if necessary
            //lots of code needs to be duplicated because the grouped by data will be in slightly different
            //based on where we grouped by. This prevents me from defining the groupedData variable outside of the inner if statements
            if(groupBy != "None")
            {
                if(groupBy == "Date")
                {
                    var groupedData = allData.GroupBy(h => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(h.Month) + " " + h.Year.ToString());
                    groupedData.OrderBy(g => g.Key);
                    if (average)
                    {
                        //I'm using vague property names here to make this data easier to work with on the front end
                        //If the property names were different in each group by they would have to access the keys by index rather than
                        //by name like has been done in the rest of the queries.
                        var result = groupedData.Select(g => new {GroupKey= g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new {GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught)});
                        return Json(result);
                    }
                }
                else if (groupBy == "Month")
                {
                    var groupedData = allData.GroupBy(h => h.Month);
                    groupedData.OrderBy(g => g.Key);
                    if (average)
                    {
                        var result = groupedData.Select(g => new {GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new {GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) });
                        return Json(result);
                    }
                }
                else if (groupBy == "Year")
                {
                    var groupedData = allData.GroupBy(h => h.Year);
                    
                    if (average)
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        result = result.OrderBy(g => g.GroupKey);
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) });
                        result = result.OrderBy(g => g.GroupKey);
                        return Json(result);
                    }
                }
                else if (groupBy == "Area")
                {
                    var groupedData = allData.GroupBy(h => h.AreaNumber);
                    groupedData.OrderBy(g => g.Key);
                    if (average)
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) });
                        return Json(result);
                    }
                }
                else if (groupBy == "Region")
                {
                    var groupedData = allData.GroupBy(h => h.Region);
                    groupedData.OrderBy(g => g.Key);
                    if (average)
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) });
                        return Json(result);
                    }
                }
                else if (groupBy == "System")
                {
                    var groupedData = allData.GroupBy(h => h.System);
                    groupedData.OrderBy(g => g.Key);
                    if (average)
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) / g.Count() });
                        return Json(result);
                    }
                    else
                    {
                        var result = groupedData.Select(g => new { GroupKey = g.Key, FishCaught = g.Sum(h => h.FishCaught) });
                        return Json(result);
                    }
                }
            }

            return Json(allData.Take(rows));//now that we've done all the filtering we can apply our row count filter
        }


        //not currently needed but keeping it here for now.
        [NonAction]
        public string GetUrlPath()
        {
            return HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;
        }
    }
}