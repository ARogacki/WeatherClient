using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WeatherClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private string ApiKey = "";
        string port = "5432";


        public void GenerateKey()
        {
            string date = ((long)DateTime.Now.Date.Ticks).ToString();
            string key = "";
            for (int i = 0; i < date.Length - 1; i++)
            {
                int value = int.Parse(date[i] + "" + date[i + 1]);
                if (value != 0)
                {
                    if (value >= 'a')
                    {
                        value = value % ('z' - 'a');
                        value = value + 'a';

                    }
                    else if (value >= 'A')
                    {
                        value = value % ('Z' - 'A');
                        value = 'A' + value;
                    }
                    else
                    {
                        value = value % 10;
                        value = '0' + value;
                    }
                    key += (char)value;
                }
            }
            ApiKey = key;
        }

        public string GetKey()
        {
            if (ApiKey.Length < 1)
            {
                GenerateKey();
            }
            return ApiKey;
        }

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        private static IEnumerable<WeatherForecast> ConvertToForecast(DataTable dataTable)
        {
            List<WeatherForecast> list = new List<WeatherForecast>();
            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                var data = new WeatherForecast();
                if (dataTable.Columns.Contains("Date"))
                    data.Date = Convert.ToDateTime(row["Date"]);
                if (dataTable.Columns.Contains("Temperature"))
                    data.Temp = Convert.ToDouble(row["Temperature"]);
                if (dataTable.Columns.Contains("Pressure"))
                    data.Pressure = Convert.ToDouble(row["Pressure"]);
                if (dataTable.Columns.Contains("Humidity"))
                    data.Humidity = Convert.ToDouble(row["Humidity"]);
                if (dataTable.Columns.Contains("Precipitation"))
                    data.Precipitation = Convert.ToDouble(row["Precipitation"]);
                if (dataTable.Columns.Contains("Wind_speed"))
                    data.Wind_speed = Convert.ToDouble(row["Wind_speed"]);
                if (dataTable.Columns.Contains("Wind_direction"))
                    data.Wind_direction = Convert.ToDouble(row["Wind_direction"]);
                list.Add(data);
            }
            return list;
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataByCityAverageOverall(string city)
        {
            try
            {
                var cs = "Server=127.0.0.1;Port="+port+";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT AVG(\"Temperature\") as Temperature, AVG(\"Pressure\") as Pressure, AVG(\"Humidity\") as Humidity, AVG(\"Precipitation\") as Precipitation, AVG(\"Wind_speed\") as Wind_speed, AVG(\"Wind_direction\") as Wind_direction FROM public.\"" + city + "\"", con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataInPolandByAverageOverall()
        {
            try
            {
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT AVG(\"Temperature\") as Temperature, AVG(\"Pressure\") as Pressure, AVG(\"Humidity\") as Humidity, AVG(\"Precipitation\") as Precipitation, AVG(\"Wind_speed\") as Wind_speed, AVG(\"Wind_direction\") as Wind_direction " +
                    "FROM (" +
                    "SELECT * FROM public.\"Lodz\" UNION ALL " +
                    "SELECT * FROM public.\"Krakow\" UNION ALL " +
                    "SELECT * FROM public.\"Rzeszow\" UNION ALL " +
                    "SELECT * FROM public.\"Suwalki\" UNION ALL " +
                    "SELECT * FROM public.\"Szczecin\" UNION ALL " +
                    "SELECT * FROM public.\"Warsaw\" UNION ALL " +
                    "SELECT * FROM public.\"Wroclaw\" UNION ALL " +
                    "SELECT * FROM public.\"Gdansk\") " +
                    "as subquery", con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataByCity(string city)
        {
            try
            {
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT * FROM public.\"" + city + "\"", con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataForCity(string c, string d)
        {
            try
            {
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT \"Date\", AVG(\"Temperature\") as Temperature, AVG(\"Pressure\") as Pressure, AVG(\"Humidity\") as Humidity, AVG(\"Precipitation\") as Precipitation, AVG(\"Wind_speed\") as Wind_speed, AVG(\"Wind_direction\") as Wind_direction " +
                    "FROM public.\"" + c + "\"" +
                    "GROUP BY \"Date\"" +
                    "ORDER BY \"Date\" DESC LIMIT " + d, con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataForCityWithProperties(string c, string p, string d)
        {
            try
            {
                switch (p)
                {
                    case "Temperature":
                        p = "AVG(\"Temperature\") as Temperature ";
                        break;
                    case "Pressure":
                        p = "AVG(\"Pressure\") as Pressure ";
                        break;
                    case "Humidity":
                        p = "AVG(\"Humidity\") as Humidity ";
                        break;
                    case "Precipitation":
                        p = "AVG(\"Precipitation\") as Precipitation ";
                        break;
                    case "Wind_speed":
                        p = "AVG(\"Wind_speed\") as Wind_speed ";
                        break;
                    case "Wind_direction":
                        p = "AVG(\"Wind_direction\") as Wind_direction ";
                        break;
                }
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT \"Date\", " + p +
                    "FROM public.\"" + c + "\"" +
                    "GROUP BY \"Date\"" +
                    "ORDER BY \"Date\" DESC LIMIT " + d, con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataForPoland(string d)
        {
            try
            {
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT \"Date\", AVG(\"Temperature\") as Temperature, AVG(\"Pressure\") as Pressure, AVG(\"Humidity\") as Humidity, AVG(\"Precipitation\") as Precipitation, AVG(\"Wind_speed\") as Wind_speed, AVG(\"Wind_direction\") as Wind_direction " +
                    "FROM (" +
                    "SELECT * FROM public.\"Lodz\" UNION ALL " +
                    "SELECT * FROM public.\"Krakow\" UNION ALL " +
                    "SELECT * FROM public.\"Rzeszow\" UNION ALL " +
                    "SELECT * FROM public.\"Suwalki\" UNION ALL " +
                    "SELECT * FROM public.\"Szczecin\" UNION ALL " +
                    "SELECT * FROM public.\"Warsaw\" UNION ALL " +
                    "SELECT * FROM public.\"Wroclaw\" UNION ALL " +
                    "SELECT * FROM public.\"Gdansk\") as subquery " +
                    "GROUP BY \"Date\" " +
                    "ORDER BY \"Date\" DESC LIMIT " + d, con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }


        private KeyValuePair<bool, IEnumerable<WeatherForecast>> FetchDataForPolandWithProperties(string p, string d)
        {
            try
            {
                switch (p)
                {
                    case "Temperature":
                        p = "AVG(\"Temperature\") as Temperature ";
                        break;
                    case "Pressure":
                        p = "AVG(\"Pressure\") as Pressure ";
                        break;
                    case "Humidity":
                        p = "AVG(\"Humidity\") as Humidity ";
                        break;
                    case "Precipitation":
                        p = "AVG(\"Precipitation\") as Precipitation ";
                        break;
                    case "Wind_speed":
                        p = "AVG(\"Wind_speed\") as Wind_speed ";
                        break;
                    case "Wind_direction":
                        p = "AVG(\"Wind_direction\") as Wind_direction ";
                        break;
                }
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT \"Date\", " + p +
                    "FROM (" +
                    "SELECT * FROM public.\"Lodz\" UNION ALL " +
                    "SELECT * FROM public.\"Krakow\" UNION ALL " +
                    "SELECT * FROM public.\"Rzeszow\" UNION ALL " +
                    "SELECT * FROM public.\"Suwalki\" UNION ALL " +
                    "SELECT * FROM public.\"Szczecin\" UNION ALL " +
                    "SELECT * FROM public.\"Warsaw\" UNION ALL " +
                    "SELECT * FROM public.\"Wroclaw\" UNION ALL " +
                    "SELECT * FROM public.\"Gdansk\") as subquery " +
                    "GROUP BY \"Date\" " +
                    "ORDER BY \"Date\" DESC LIMIT " + d, con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(true, ConvertToForecast(data));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, IEnumerable<WeatherForecast>>(false, null);
            }
        }

        [HttpGet]
        [Route("poland")]
        public ActionResult<IEnumerable<WeatherForecast>> GetPolandWithProperties([FromQuery] string d, [FromQuery] string p = "", [FromHeader] string ApiKey = "")
        {
            if (ApiKey != GetKey())
            {
                return StatusCode(401);
            }
            KeyValuePair<bool, IEnumerable<WeatherForecast>> data;
            if (p == null)
            {
                data = FetchDataForPoland(d);
            }
            else if (p == "")
            {
                data = FetchDataForPoland(d);
            }
            else
            {
                data = FetchDataForPolandWithProperties(p, d);
            }
            if (data.Key)
                return Ok(data.Value);
            else
                return StatusCode(400);
        }

        [HttpGet]
        [Route("average")]
        public ActionResult<IEnumerable<WeatherForecast>> GetCityWithProperties([FromQuery] string c, [FromQuery] string d, [FromQuery] string p = "",  [FromHeader] string ApiKey = "")
        {
            if (ApiKey != GetKey())
            {
                return StatusCode(401);
            }
            KeyValuePair<bool, IEnumerable<WeatherForecast>> data;
            if (p == null)
            {
                data = FetchDataForCity(c, d);
            }
            else if (p == "")
            {
                data = FetchDataForCity(c, d);
            }
            else
            {
                data = FetchDataForCityWithProperties(c, p, d);
            }
            if (data.Key)
                return Ok(data.Value);
            else
                return StatusCode(400);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                Temp = rng.Next(-20, 55),
                Pressure = 333,
                Humidity = 333,
                Precipitation = 333,
                Wind_speed = 333,
                Wind_direction = 0,
            })
            .ToArray();
        }

        private KeyValuePair<bool, string> FetchDayAmount()
        {
            try
            {
                var cs = "Server=127.0.0.1;Port=" + port + ";User Id=postgres;Password=postgres;Database=WeatherForecast";
                var con = new NpgsqlConnection(cs);
                con.Open();
                var cmd = new NpgsqlCommand("SELECT COUNT(ids) as amount FROM (SELECT COUNT(\"ID\") as ids FROM public.\"Lodz\" GROUP BY \"Date\") as subquery", con);
                var reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                return new KeyValuePair<bool, string>(true, Convert.ToString(data.Rows[0]["amount"]));
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, null);
            }
        }

        [HttpGet]
        [Route("dbsize")]
        public ActionResult<string> GetDatabaseSize([FromHeader] string ApiKey = "")
        {
            if (ApiKey != GetKey())
            {
                return StatusCode(401);
            }
            KeyValuePair<bool, string> data;
            data = FetchDayAmount();
            if (data.Key)
                return Ok(data.Value);
            else
                return StatusCode(400);
        }
        /*
        [HttpPost]
        public IEnumerable<WeatherForecast> Post()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        */
    }
}
