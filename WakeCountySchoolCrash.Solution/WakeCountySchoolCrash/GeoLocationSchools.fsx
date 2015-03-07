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

let schoolInformations' = schoolInformations
                            |>Seq.map(fun si -> si.SchoolCode, getSchoolLatLong(si.Address),si.StartTime,si.EndTime)
                            |>Seq.map(fun (s,latLong,st,dt) -> match latLong.IsSome with
                                                                        | true -> Some {schoolCode=s;latitude=fst latLong.Value;longitude=snd latLong.Value;startTime=st;endTime=dt}
                                                                        | false -> None) 
                            |>Seq.toArray

let createSchoolInformationJson(schoolInformation:option<SchoolInformation>) =
    match schoolInformation.IsSome with
    | true -> 
        let schoolCode = schoolInformation.Value.schoolCode.ToString()
        let latitude = schoolInformation.Value.latitude.ToString()
        let longitude = schoolInformation.Value.longitude.ToString()    
        let startTime = schoolInformation.Value.startTime.ToShortTimeString()
        let endTime = schoolInformation.Value.endTime.ToShortTimeString()
        let result = JsonValue.Record [| 
                        "schoolCode", JsonValue.String schoolCode
                        "latitude", JsonValue.String latitude
                        "longitude", JsonValue.String longitude
                        "startTime", JsonValue.String startTime
                        "endTime", JsonValue.String endTime|] 
        Some result
    | false -> None

let writeSchoolInformationToDisk(schoolInformation: option<JsonValue>) =
    match schoolInformation.IsSome with
    | true -> File.AppendAllText(@"C:\Data\geolocatedSchoolInformation.json",schoolInformation.Value.ToString() + "," )
    | false -> ()

File.AppendAllText(@"C:\Data\geolocatedSchoolInformation.json","[" )
schoolInformations' |> Seq.map(fun si -> createSchoolInformationJson(si))
                    |> Seq.iter(fun jv -> writeSchoolInformationToDisk(jv))
File.AppendAllText(@"C:\Data\geolocatedSchoolInformation.json","]" )

                                                                                   