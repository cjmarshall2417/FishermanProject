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
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Fishermen.Controllers
{
    public class TblHaulsController : Controller
    {
        public string[] validMonths = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private readonly phishermenContext fishHauls;

        public TblHaulsController(phishermenContext context)
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




        [HttpPost]
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
                             , FishCaught = haulGroup.Sum(h => h.Caught) / haulGroup.Count() });

            var bestMonthList = bestMonth.ToList();
            if (bestMonthList.Count() != 12)
            {
                for (int i = 0; i < validMonths.Count(); i++)
                {
                    if (bestMonthList.FindIndex(m => m.Month == validMonths[i]) == -1)
                    {
                        bestMonthList.Add(new { Month = validMonths[i], MonthNumber = i + 1, FishCaught = 0 });
                    }

                }
            }

            var result = bestMonthList.OrderBy(m => m.MonthNumber);
            return Json(result);
        }

        [HttpGet]
        [Route("api/RegionTopTenAreas")]
        public IActionResult RegionTopTenArea(string regionName)
        {
            var hauls = fishHauls.TblHauls;
            var locations = fishHauls.TblLocations;
            var regions = fishHauls.TblRegions;

            var data = from haul in hauls
                       join location in locations on haul.LocationId equals location.LocationId
                       join region in regions on location.RegionId equals region.RegionId
                       where region.RegionName == regionName
                       group haul by new { AreaNumber = location.AreaNumber, AreaName = location.AreaName } into haulGroup
                       select new { AreaNumber = haulGroup.Key.AreaNumber, AreaName = haulGroup.Key.AreaName, AverageHaul = haulGroup.Sum(h => h.Caught) / haulGroup.Count() };

            data = data.OrderByDescending(g => g.AverageHaul);

            return Json(data.Take(10));
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
        [Route("api/GetUserQueries")]
        public IActionResult GetUserQueries()
        {
            return Json(fishHauls.TblQueries);
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
            for (int i = 0; i < validMonths.Count(); i++)
            {
                validMonthObjectsAnon[i] = new { MonthNumber = i + 1, MonthName = validMonths[i] };
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
        //answer whatever question they choose.
        [HttpGet]
        [Route("api/CustomQuery")]
        public IActionResult CustomQuery(int[] years, int[] months,
            int[] areaNumbers, string[] areaNames, string[] regions, string[] systems, string sortBy, string groupBy = "None", string aggregate = "Average",
            int haulGreaterThan = -1, int haulLessThan = -1, int rows = 1000, bool sortAscending = true)
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
            //this applies the chosen filters to the current data
            //I'm thinking about how to clean this up
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

            //managed to clean up a lot of the repeated code through a mixture of reflection and switch expressions.
            //I knew about switch statements but not expressions and those turned out to be exactly what I needed

            //checks if we are doing a group by statement
            //group by queries will have different columns than non group bys so we have to duplicate the sorting inside of
            //this portion instead of sorting once for groupBy and non groupBy queries
            if (groupBy != "None")
            {
                //date is on it's own because sorting and grouping both require two variables.
                //I could get rid of this using more switch expressions but for the amount of code saved
                //it doesn't seem worth the hassle and more confusing layout.
                if (groupBy == "Date")
                {
                    var formattedData = allData.Select(h => new { Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(h.Month) + " " + h.Year.ToString(), FishCaught = h.FishCaught });
                    var groupedData = formattedData.ToList().GroupBy(g => g.Date);//the linq query breaks if I don't use ToList before the group by
                                                                                  
                    var result = groupedData.Select(g => new
                    {
                        GroupKey = g.Key,
                        FishCaught = aggregate switch
                        {
                            "Sum" => g.Sum(h => h.FishCaught),
                            "Average" => g.Sum(h => h.FishCaught) / g.Count(),
                            "Max" => g.Max(h => h.FishCaught),
                            "Min" => g.Min(h => h.FishCaught),
                            _ => g.Sum(h => h.FishCaught)
                        }
                    });

                    result = sortAscending switch
                    {
                        true => sortBy switch
                        {
                            "Group Name" => result.OrderBy(g => g.GroupKey.ToString().Split()[1]).ThenBy(g => Array.IndexOf(validMonths, g.GroupKey.ToString().Split()[0])),
                            "Fish Caught" => result.OrderBy(g => g.FishCaught),
                            _ => result.OrderBy(g => g.GroupKey)
                        },
                        false => sortBy switch
                        {
                            "Group Name" => result.OrderByDescending(g => g.GroupKey.ToString().Split()[1]).ThenByDescending(g => Array.IndexOf(validMonths, g.GroupKey.ToString().Split()[0])),
                            "Fish Caught" => result.OrderByDescending(g => g.FishCaught),
                            _ => result.OrderByDescending(g => g.GroupKey)
                        }

                    };
                    return Json(result);
                }
                //can use reflection for all groupBys other than date
                else
                {
                    var groupedData = allData.ToList().GroupBy(h => h.GetType().GetProperty(groupBy).GetValue(h, null));

                    var result = groupedData.Select(g => new
                    {
                        GroupKey = g.Key,
                        FishCaught = aggregate switch
                        {
                            "Sum" => g.Sum(h => h.FishCaught),
                            "Average" => g.Sum(h => h.FishCaught) / g.Count(),
                            "Max" => g.Max(h => h.FishCaught),
                            "Min" => g.Min(h => h.FishCaught),
                            _ => g.Sum(h => h.FishCaught)
                        }
                    });

                    result = sortAscending switch
                    {
                        true => sortBy switch
                        {
                            "Group Name" => result.OrderBy(g => g.GroupKey),
                            "Fish Caught" => result.OrderBy(g => g.FishCaught),
                            _ => result.OrderBy(g => g.GroupKey)
                        },
                        false => sortBy switch
                        {
                            "Group Name" => result.OrderByDescending(g => g.GroupKey),
                            "Fish Caught" => result.OrderByDescending(g => g.FishCaught),
                            _ => result.OrderByDescending(g => g.GroupKey)
                        }

                    };
                   
                    return Json(result);
                }
            }
            //if we're here that means no group by, so we need to sort before returning our chosen number of rows
            if (sortBy != null && sortBy != "Group Name" && sortBy != "Fish Caught")
            {
                if (sortAscending)
                {
                    var sortedData = allData.ToList().OrderBy(h => h.GetType().GetProperty(sortBy).GetValue(h));
                    return Json(sortedData.Take(rows));
                }
                else
                {
                    var sortedData = allData.ToList().OrderByDescending(h => h.GetType().GetProperty(sortBy).GetValue(h));
                    return Json(sortedData.Take(rows));
                }
            }

            return Json(allData.Take(rows));
            
        }
        


        [HttpPost]
        [Route("api/SaveQuery")]
        public void SaveQuery(string queryURL, string queryName) {
            TblQueries query = new TblQueries();
            query.QueryName = queryName;
            query.QueryUrl = queryURL;
            fishHauls.TblQueries.Add(query);
            fishHauls.SaveChanges();
            
        }

        //not currently needed but keeping it here for now.
        [NonAction]
        public string GetUrlPath()
        {
            return HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;
        }

        public class Group
        {
            public string GroupKey;
            public int FishCaught;
            public Group(string groupKey, int fishCaught)
            {
                this.GroupKey = groupKey;
                this.FishCaught = fishCaught;
            }
        }

       
    }
}