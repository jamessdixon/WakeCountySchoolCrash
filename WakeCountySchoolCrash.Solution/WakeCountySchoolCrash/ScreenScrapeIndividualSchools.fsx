//http://new.wcpss.net/school-directory/304.html

#r "../packages/FSharp.Data.2.1.1/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data
open System.Linq

type SchoolInfoContext = HtmlProvider<"../html/individualSchool.html">
type SchoolListContext = CsvProvider<"../data/WakeCountySchoolInfo.txt">
type SchoolInformation = {schoolCode: int; address:string; schedule:string }

let createUri(schoolCode: int) = 
    schoolCode, "http://new.wcpss.net/school-directory/"+schoolCode.ToString()+".html" 

let getSchoolWebData(schoolCode: int, uri: string)=
    try
        let body = SchoolInfoContext.Load(uri).Html.Body()
        let divs = body.Descendants("DIV") |> Seq.toList
        let scheduleDiv = divs.[21]
        let addressDiv = divs.[22]
        let scheduleP = scheduleDiv.Descendants("p") |> Seq.toList
        let schedule = scheduleP.[2].InnerText()
        let addressP = addressDiv.Descendants("p") |> Seq.toList
        let address = addressP.[3].InnerText()

        let result = {schoolCode=schoolCode; address=address; schedule=schedule}
        Some result
    with
          | :? System.ArgumentException ->  None

let getSchoolInformation(code:int)=
    let uri = createUri(code)
    getSchoolWebData(uri)

let schoolList = SchoolListContext.Load("../data/WakeCountySchoolInfo.txt")
let schoolRows = schoolList.Rows
let schoolList' = schoolRows |> Seq.map(fun r -> r.Code) 
let schoolInformation = schoolList' |> Seq.map(fun c -> getSchoolInformation(c))|> Seq.toList

let createSchoolInformationJson(schoolInformation:option<SchoolInformation>) =
    match schoolInformation.IsSome with
    | true ->
        let schoolCode = schoolInformation.Value.schoolCode.ToString()
        let result = JsonValue.Record [| 
                        "schoolCode", JsonValue.String schoolCode
                        "address", JsonValue.String schoolInformation.Value.address
                        "schedule", JsonValue.String schoolInformation.Value.schedule |] 
        Some result
    | false -> None

let writeSchoolInformationToDisk(schoolInformation: option<JsonValue>) =
    match schoolInformation.IsSome with
    | true -> File.AppendAllText(@"C:\Data\schoolInformation.json",schoolInformation.Value.ToString() + "," )
    | false -> ()

schoolInformation |> Seq.map(fun si -> createSchoolInformationJson(si))
                  |> Seq.iter(fun jv -> writeSchoolInformationToDisk(jv))  

