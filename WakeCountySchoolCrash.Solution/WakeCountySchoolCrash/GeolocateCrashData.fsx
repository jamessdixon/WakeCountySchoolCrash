#r "../packages/FSharp.Data.2.1.1/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data
open System.Linq

type CrashSite = {roadA:string; roadB:string; city:string; date:DateTime; time:DateTime; severity:string}
type CrashSite' = {date:DateTime; time:DateTime; severity:string; latitude: decimal; longitude:decimal}
type GeoCodeContext = JsonProvider<"../json/geocode.json">
type TrafficCrashContext = CsvProvider<"../data/TrafficCrashData.csv">

let trafficCrashes = TrafficCrashContext.Load("../data/TrafficCrashData.csv")

let getCrashSiteLatLong(crashSite: CrashSite)=
    try
        let apiKey = "AgCeYsGLrEUWs_Fc_OIK_kT1cd59yHUmF_nqlFWxEMnGaBjvDX2a07yADMBWN6Ef"
        let queryString = String.Format("{0} AT {1} {2}, NC", crashSite.roadA, crashSite.roadB, crashSite.city)
        let geocodeRequest = new Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", queryString, apiKey));
        let geocodeRequest' = geocodeRequest.ToString()
        let result = GeoCodeContext.Load(geocodeRequest')
        let coordinates = result.ResourceSets.[0].Resources.[0].Point.Coordinates
        let latitude = coordinates.[0]
        let longitude =coordinates.[1] 
        Some (latitude, longitude)
    with
          | :? System.IndexOutOfRangeException ->  None

//getCrashSiteLatLong {roadA="I 440";roadB="Wake Forest Road";city="Raleigh"}

let filePath = @"C:\data\GeoCodeCrashSite.csv"

let trafficCrashes' = trafficCrashes.Rows 
                        |> Seq.map(fun tc -> {roadA=tc.Road;roadB=tc.FromRoad;city=tc.Municipality;date=tc.Date;time=tc.Time;severity=tc.Severity})
                        |> Seq.map(fun cs -> cs, getCrashSiteLatLong(cs))
                        |> Seq.filter(fun (cs,latLong) -> latLong.IsSome = true)
                        |> Seq.map(fun (cs,latLong) -> cs.date.ToShortDateString() + "," + cs.time.ToShortTimeString() + "," + cs.severity + "," + (fst latLong.Value).ToString() + "," + (snd latLong.Value).ToString() + Environment.NewLine)
                        |> Seq.iter(fun (s) -> File.AppendAllText(filePath, s))



