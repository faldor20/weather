module weather.BOMAPI

open System
open System.Net
open Mapsui.Geometries
open FsHttp
open FsHttp.DslCE
type MeteyeQuery=
    {
        baseTime:DateTime
        issueTime:DateTime
        bbox:BoundingBox
        widthPX:int
        heightPX:int
            }
///The Meteye data converted into its textual format for assembly into the query
 type private MeteyeQuery_Converted=
    {
        baseTime:string
        issueTime:string
        bbox:string
        widthPX:string
        heightPX:string

    }
let private bboxConvert (bbox:BoundingBox)=
    let top =bbox.TopRight
    let bot=bbox.BottomLeft
    $"{bot.X},{bot.Y},{top.X},{top.Y}"
let private convertMeteyeQuery (queryData:MeteyeQuery)=
    let baseTime=queryData.baseTime.ToString("yyyyMMddHHmm")
    let issueTime=queryData.issueTime.ToString("yyyyMMddHHmmss")
    let bbox=queryData.bbox|>bboxConvert
    let widthPX=queryData.widthPX.ToString()
    let heightPX=queryData.heightPX.ToString()
    {baseTime=baseTime;issueTime=issueTime;bbox=bbox;widthPX=widthPX;heightPX=heightPX}


let getMeteye (queryData:MeteyeQuery)=
    let queryData=queryData|>convertMeteyeQuery
    async{
        let url= $"http://wvs2.bom.gov.au/mapcache/meteye?TRANSPARENT=true&FORMAT=image/png&SERVICE=WMS&VERSION=1.1.1
        &REQUEST=GetMap&STYLES=&layers=IDZ73033&TIMESTEP=0&BASETIME={queryData.baseTime}&ISSUETIME={queryData.issueTime}&SRS=EPSG:4326&BBOX={queryData.bbox}&WIDTH={queryData.widthPX}&HEIGHT={queryData.heightPX}"
        let! response=
            httpLazy{
                GET url
                sendAsync
            }
        return response
    }



    
    