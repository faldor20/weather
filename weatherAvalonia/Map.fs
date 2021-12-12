module weather.Map

open Avalonia
open Avalonia.Media
open Mapsui
open Mapsui.UI.Avalonia
open Mapsui.UI
open System
open System.IO
open System.Linq
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Markup.Xaml
open Mapsui.Extensions
open Mapsui.Layers
open Mapsui.Layers.Tiling
open Mapsui.UI
open Mapsui.UI.Avalonia
open Mapsui.Utilities
open FUI
open FUI.Avalonia
open FUI.ObservableValue
open FUI.ObservableCollection
open FUI.Avalonia.DSL
open FUI.IfBuilder
open FUI.FragmentBuilder
open BruTile

type Model =
    { Counter: int var
      Items: int col
      ButtonColor: Color var 
      Map:MapControl var}

let createMap()=
    let map= new MapControl()
    map.Map.Layers.Add(OpenStreetMap.CreateTileLayer())    
    map.Width<- 1000.0
    map.Height <-1000.0
    map.MaxHeight <-4000

    map.MaxWidth<-4000
    let hey=ImageLayer("")
    let image 
    RasterFeature(MRaster())
    map
let init () =
    { Counter = var 0
      Items = col [1; 2; 3]
      ButtonColor = var (Colors.RosyBrown)
      Map=var(createMap()) 
      }
    
let view (model: Model) =
    //model.Map.Value.Refresh()
    
    (* StackPanel().Children.Add(
        model.Map.Value
        Button(Command=(fun x->model.Map.Value.Refresh()))
    ) *)

    StackPanel {
        Fragment {
            Label {
                let txt = (model.Counter |> Ov.map string)
                txt
            }
        }
        DSL.Button {
            onClick(fun x->
                model.Counter.Update((+)1 )
                model.Map.Value.Refresh())
        }
        Fragment{
            model.Map.Value
        }
    
    }
        