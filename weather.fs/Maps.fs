module weather.fs.Maps

open Mapsui
open System
open System.Collections.Generic
open System.Linq
open System.Threading
open Mapsui.Projection
open Mapsui.Utilities
open System.IO
open BruTile.MbTiles
open Mapsui.Layers
open SQLite
open Mapsui.UI.Forms
open Mapsui.Rendering
open Mapsui.Geometries;
open Mapsui.Rendering.Skia
type Potato = { a: int }

let createMbTilesLayer path name =
    try 
        printfn "map file %s exists: %b and is size(KB): %i" path (File.Exists(path)) (FileInfo(path).Length/1000L)

        let mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true))

        printfn("got sql")

        let mbTilesLayer = new TileLayer(mbTilesTileSource, Name = name)

        Some(mbTilesLayer)
    with e -> 
        printfn "excerption %A  %A " e.Message e
        None
let setupMap tiles =
    let map =  new Mapsui.Map(
                                CRS = "EPSG:4326",
                                Transformation = new MinimalTransformation()
                            ) 
    map.Layers.Add tiles

    map.Widgets.Add(
        new Mapsui.Widgets.ScaleBar.ScaleBarWidget(
            map,
            TextAlignment = Mapsui.Widgets.Alignment.Center,
            HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Left,
            VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom
        )
    )

    map

let retrieveEmbeddedFile basePath embeddedName =
    let filePath = Path.Combine(basePath, embeddedName)
    let assemblyName="weather.fs."
    let assembly = typeof<Potato>.Assembly
    printfn "Resource Names %A" (assembly.GetManifestResourceNames())
    
    use stream = assembly.GetManifestResourceStream (assemblyName+embeddedName)
    use file = File.Create(filePath)
    stream.CopyTo(file)
    printfn "Created file %s and copied data from assembly into it" filePath
    FileInfo(filePath)
 ///We can make the icon an svg if we want.
let renderPin (mapView:MapView) position label colour icon=
    
    let pin = new Pin (
        mapView,
        Label = label,
        Position = position,
        Address = "brisbane",
        Type = PinType.Pin,
        Color =  colour,
        Transparency = 0.9f,
        Scale =1f,
        RotateWithMap = true
        )
    icon|>Option.iter(fun x->pin.Icon<-x)
    
    mapView.Pins.Add(pin);
    pin.ShowCallout();
    Console.WriteLine("Added pin at position -27.468968,153.023499");
    pin

let geoTiff()=
    let basePath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    let geoTiffFile=retrieveEmbeddedFile basePath "example.tif"
    let tfw=retrieveEmbeddedFile basePath "example.tfw"
    let geo=Providers.GeoTiffProvider(geoTiffFile.FullName)
    new Layer("WeatherTiff",DataSource=geo,Opacity=0.5)
    
let rasterFeature path=
    let basePath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    let bitmap=(retrieveEmbeddedFile basePath "meteye2.png").FullName|>File.ReadAllBytes 
    //The position here is extremely important, get it wrong and you will see nothing rendering.
    let raster = new Raster(new MemoryStream(bitmap), new BoundingBox(Position(-9.188870,155.830078).ToMapsui(),Position(-45.089036,112.148438).ToMapsui()))
    let fet=new Providers.Feature(Geometry=raster)
    fet
    
let weatherLayer()=
    let features=Providers.Features()
    features.Add(rasterFeature "meteye2.png")
    new MemoryLayer
        ("RasterLayer",
        DataSource=
            Providers.MemoryProvider(features),
        CRS="EPSG:4326",
        Opacity=0.5)
let init (mapView:MapView) =
    let basePath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    Console.Write()

    //Here we load the mebedded map data, read it and then write it to a locally available file.
    
    let worldTiles=retrieveEmbeddedFile basePath "world.mbtiles"
    let weatherTiles= retrieveEmbeddedFile basePath "meteye2.mbtiles"
    
    let Lay=Layers.ImageLayer("hi")
    
    let baseTiles = 
        createMbTilesLayer worldTiles.FullName "base"
        |>Option.map (fun x->
            x.Opacity<-0.5
            x.CRS<-"EPSG:3857"
            x:>ILayer)
    let weatherTiles=
        createMbTilesLayer weatherTiles.FullName "weather"
        |>Option.map (fun x->
                x.Opacity<-0.5
                x.CRS<-"EPSG:4326"
                x:>ILayer)
    
    (* let osmTiles = OpenStreetMap.CreateTileLayer()
    osmTiles.Opacity<-0.5 *)
    //map.Layers.Add tiles 
    let layers:ILayer array= 
        [| baseTiles;weatherLayer():>ILayer|>Some|]
        |>Array.choose id

    mapView.Map <- setupMap layers
    renderPin mapView (Position(-27.468968,153.023499)) "home" Xamarin.Forms.Color.Red None|>ignore
    renderPin mapView (Position(-9.188870,155.830078)) "edgeR" Xamarin.Forms.Color.Red None|>ignore
    renderPin mapView (Position(-45.089036,112.148438)) "edgeL" Xamarin.Forms.Color.Blue None|>ignore
    renderPin mapView (Position(-45.089036,112.148438)) "edgeL" Xamarin.Forms.Color.Blue None|>ignore
    ()
    
    




