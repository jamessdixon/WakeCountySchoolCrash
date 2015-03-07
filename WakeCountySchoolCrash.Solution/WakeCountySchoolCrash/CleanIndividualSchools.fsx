#r "../packages/FSharp.Data.2.1.1/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data
open System.Linq

type SchoolInformation = {schoolCode: int; address:string; startTime:DateTime; endTime:DateTime }
type SchoolInformationContext = JsonProvider<"../data/schoolInformation.json">

let schoolInformations = SchoolInformationContext.Load("../data/schoolInformation.json")

let convertTimeStringToDateTime(time:string)=
    let tokens = time.Split(' ')
    let digits = tokens.[0] 
    let hoursMinutes = digits.Split(':')
    let hours = Int32.Parse(hoursMinutes.[0])
    let minutes = Int32.Parse(hoursMinutes.[1])
    let ampm = tokens.[1]
    let hours' = match ampm with
                        | "a.m." -> hours
                        | _ -> hours + 12
    new DateTime(2015,1,1,hours',minutes,0)

//convertTimeStringToDateTime("3:45 p.m.")

let SchoolInformations' = schoolInformations 
                            |> Seq.map(fun si -> si.SchoolCode,si.Address.Replace("Address - ",String.Empty),si.Schedule)
                            |> Seq.map(fun (c,a,s) -> c,a,s.Replace("2014-15 School Schedule - ",String.Empty))
                            |> Seq.map(fun (c,a,s) -> c,a,s.Split('-'))
                            |> Seq.map(fun (c,a,sa) -> c,a,sa.[0].Trim(),sa.[1].Trim())
                            |> Seq.map(fun (c,a,at,dt) -> c,a,convertTimeStringToDateTime(at),convertTimeStringToDateTime(dt))
                            |> Seq.map(fun (c,a,at,dt) -> {schoolCode=c;address=a;startTime=at;endTime=dt})

let createSchoolInformationJson(schoolInformation:SchoolInformation) =
    let schoolCode = schoolInformation.schoolCode.ToString()
    let startTime = schoolInformation.startTime.ToShortTimeString()
    let endTime = schoolInformation.endTime.ToShortTimeString()
    let result = JsonValue.Record [| 
                    "schoolCode", JsonValue.String schoolCode
                    "address", JsonValue.String schoolInformation.address
                    "startTime", JsonValue.String startTime
                    "endTime", JsonValue.String endTime|] 
    result

let writeSchoolInformationToDisk(schoolInformation: JsonValue) =
    File.AppendAllText(@"C:\Data\cleanedSchoolInformation.json",schoolInformation.ToString() + "," )

File.AppendAllText(@"C:\Data\cleanedSchoolInformation.json","[" )
SchoolInformations' |> Seq.map(fun si -> createSchoolInformationJson(si))
                    |> Seq.iter(fun jv -> writeSchoolInformationToDisk(jv))
File.AppendAllText(@"C:\Data\schoolInformation.json","]" )
