#r "../packages/FSharp.Data.2.1.1/lib/net40/FSharp.Data.dll"

open System
open FSharp.Data


type Intersection = {roadA:string; roadB:string; city:string}
type geoCode = JsonProvider<"../json/geocode.json">

let intersection = {roadA="I 440";roadB="Wake Forest Road";city="Raleigh"}

let apiKey = "AgCeYsGLrEUWs_Fc_OIK_kT1cd59yHUmF_nqlFWxEMnGaBjvDX2a07yADMBWN6Ef"
let queryString = String.Format("{0} AT {1} {2}, NC", intersection.roadA, intersection.roadB, intersection.city)

let geocodeRequest = new Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", queryString, apiKey));
let geocodeRequest' = geocodeRequest.ToString()

let result = geoCode.Load(geocodeRequest')
let coordinates = result.ResourceSets.[0].Resources.[0].Point.Coordinates
let latitude = coordinates.[0]
let longitude =coordinates.[1] 
