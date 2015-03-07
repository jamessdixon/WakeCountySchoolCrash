#r "../packages/FSharp.Data.2.1.1/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data
open System.Linq

type SchoolInformationContext = JsonProvider<"../data/cleanedSchoolInformation.json">
type GeoCodeContext = JsonProvider<"../json/geocode.json">
type SchoolInformation = {schoolCode: int; latitude: decimal; longitude:decimal ; startTime:DateTime; endTime:DateTime }

let schoolInformations = SchoolInformationContext.Load("../data/cleanedSchoolInformation.json")

let getSchoolLatLong(address: string)=
    try
        let apiKey = "AgCeYsGLrEUWs_Fc_OIK_kT1cd59yHUmF_nqlFWxEMnGaBjvDX2a07yADMBWN6Ef"
        let geocodeRequest = new Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", address, apiKey));
        let geocodeRequest' = geocodeRequest.ToString()
        let result = GeoCodeContext.Load(geocodeRequest')
        let coordinates = result.ResourceSets.[0].Resources.[0].Point.Coordinates
        let latitude = coordinates.[0]
        let longitude =coordinates.[1] 
        Some (latitude, longitude)
    with
          | :? System.IndexOutOfRangeException ->  None

//getSchoolLatLong "301 St. Mary's Street, Raleigh, NC 27605"

let filePath = @"C:\data\GeoCodeSchoolInformation.csv"

let schoolInformations' = schoolInformations
                            |> Seq.map(fun si -> si.SchoolCode, getSchoolLatLong(si.Address),si.StartTime,si.EndTime)
                            |> Seq.filter(fun (s,latLong,st,dt) -> latLong.IsSome = true)
                            |> Seq.map(fun (s,latLong,st,dt) -> s.ToString() + "," + st.ToShortTimeString() + "," + dt.ToShortTimeString() + "," + (fst latLong.Value).ToString() + "," + (snd latLong.Value).ToString() + Environment.NewLine)
                            |> Seq.iter(fun (s) -> File.AppendAllText(filePath, s))
                            


                                                                           