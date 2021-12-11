module weather.fs.MbTiles
open System.Data

open System.IO
open RepoDb
open Microsoft.Data.Sqlite
open FSharp.Reflection
type MBTileDB= SqliteConnection
[<CLIMutable>]
type map=
    {
        zoom_level:int
        tile_column:int
        tile_row: int
        tile_id:int
    }
[<CLIMutable>]
type images=
    {
        tile_id:int
        tile_data:byte array
    }
[<CLIMutable>]
type metadata={
    name:string
    value:string
}
///``bounds`` (string of comma-separated numbers): The maximum extent of the rendered map area. Bounds must define an area covered by all zoom levels.
/// The bounds are represented as WGS 84 latitude and longitude values, in the OpenLayers Bounds format (left, bottom, right, top). 
///For example, the bounds of the full Earth, minus the poles, would be: ``-180.0,-85,180,85.``
///
///``center`` (string of comma-separated numbers): The longitude, latitude, and zoom level of the default view of the map. Example: ``-122.1906,37.7599,11``
type MBTileMetaData={
    bounds:string
    center:string
    minzoom:string
    maxzoom:string
    }
type MbTileFile=
    {
        map:map list
    }

(* let addImage(db:MBTileDB) (image:images)=
//     let query="""
  //      INSERT INTO images (tile_id,tile_data)
    //    VALUES (@id, @tile)
//        """
//    use command=new SQLiteCommand(insertSql,db) 
    
    let id=db.Insert(image)
    id *)


let defaultTile fileName=
    RepoDb.SqliteBootstrap.Initialize();

    let connectionString = sprintf "Data Source=%s;" fileName  

    let dbConnection=new SqliteConnection(connectionString)
    dbConnection.Open();
    let sqlStructure1=
        """
        create table images(
             tile_id TEXT primary key autoincrement
        , tile_data blob 
        );

        create table map
        ( zoom_level integer primary key autoincrement
        , tile_column integer 
        , tile_row integer
        , tile_id TEXT references images(tile_id)
        );
        """
    let sqlStructure="""
        CREATE TABLE IF NOT EXISTS [images] (
            tile_id TEXT PRIMARY KEY,
            tile_data BLOB NOT NULL
        );
        CREATE TABLE IF NOT EXISTS  [map] (
            zoom_level  INTEGER,
            tile_column INTEGER,
            tile_row    INTEGER,
            tile_id     TEXT
        );
        CREATE TABLE IF NOT EXISTS [metadata] (
            name  TEXT,
            value TEXT
        );

        """
    let views="""
        CREATE VIEW tiles AS
            SELECT map.zoom_level as zoom_level,
                map.tile_column as tile_column,
                map.tile_row as tile_row,
                images.tile_data as tile_data
            FROM map 
                JOIN 
                images ON map.tile_id = images.tile_id 
        """
    let structureCommand = new SqliteCommand(sqlStructure,dbConnection)
    let viewCommand= new SqliteCommand(views,dbConnection)
    let res1=viewCommand.ExecuteNonQuery()
    let res=structureCommand.ExecuteNonQuery()



    dbConnection
let readBlob filePath=
    let file=File.Open(filePath,FileMode.Open)
    let blob:byte array=Array.zeroCreate(int file.Length-1)
    let read=file.Read(blob,0,int file.Length-1)
    if read=int file.Length then printfn "finished reading data"
    else printfn "failed reading data, only read %d bytes of %d" read file.Length
    blob
///Uses some slightly nasty reflection trickery to get the names of properties of the metadata record
///It uses the naems nd values to generate a list of objects taht can then be serialized into the rows of the mbtiles DB
let createMetaData (meta:MBTileMetaData)=
    let fields=Reflection.FSharpType.GetRecordFields(typeof<MBTileMetaData>)
    fields|>Array.map(fun f -> 
        let value=FSharpValue.GetRecordField(meta,f):?>string
        let name=f.Name
        {name=name; value=value} 
         )
let test()=
    let db=defaultTile("test.mbtiles")

    let blob= readBlob "meteye2.png"
    let imageRow={tile_data=blob;tile_id=0}
    let centerCords="133.5,-12,3"// i don't know if this actually sets the center point of tiles, or if it just sets the default view position
    let boundCords="-46,112,-8,156"
    let meta:MBTileMetaData={bounds=boundCords;center=centerCords;minzoom="2";maxzoom="10"}
    let metaD= createMetaData meta
    
    let id=db.Insert<images,int>(imageRow)
    let tile={zoom_level=3; tile_column=0;tile_row=0;tile_id=id;}
    let tID=db.Insert<map,int>(tile)
    db.InsertAll<metadata>(metaD)|>ignore
    
    printfn"id %d" id
    ()
test()