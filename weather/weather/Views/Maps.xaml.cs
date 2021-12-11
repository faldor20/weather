using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.UI.Forms;
using Mapsui.UI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mapsui.Styles;
using Mapsui;
using Mapsui.Projection;
using Mapsui.Utilities;
using Mapsui.Widgets;
using BruTile.MbTiles;
using Mapsui.Layers;
using SQLite;
using System.IO;
using System.Reflection;
using weather.fs;
using Mapsui.Rendering.Skia;

namespace weather.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Maps : ContentPage
    {
     

        public Maps()
        {
            
            InitializeComponent();
            MBMaps();
        }
        private void MBMaps()
        {
            weather.fs.Maps.init(mapView);
            
            mapView.Refresh();
        }

        private Pin MakePin(MapView mapView){
            var pin = new Pin(mapView)
            {
                Label = $"home",
                Position = new Position(-27.468968,153.023499),
                Address = "brisbane",
                Type = PinType.Pin,
                Color =  Xamarin.Forms.Color.Blue,
                Transparency = 0.9f,
                Scale =1f,
                RotateWithMap = true,
            };
            pin.Callout.Anchor = new Point(0, pin.Height* pin.Scale);
            pin.Callout.RectRadius = 5;
                        pin.Callout.ArrowHeight = 15;
                        pin.Callout.ArrowWidth = 10;
                        pin.Callout.ArrowAlignment = (ArrowAlignment) 3;
            pin.Callout.ArrowPosition = 0.8;
                        pin.Callout.StrokeWidth = 5;
                        pin.Callout.Padding = new Thickness(10, 10);
                        pin.Callout.BackgroundColor = Xamarin.Forms.Color.White;
                        pin.Callout.RotateWithMap = true;
                        pin.Callout.IsClosableByClick = true;
                        pin.Callout.Color = pin.Color;
            mapView.Pins.Add(pin);
            pin.ShowCallout();
            Console.WriteLine("Added pin at position -27.468968,153.023499");
            return pin;
        }
        protected override void OnAppearing()
        {
            mapView.IsVisible = true;
            mapView.Refresh();
        }
        private void SimpleMaps()
        {
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };

            var tileLayer = OpenStreetMap.CreateTileLayer();

            map.Layers.Add(tileLayer);
            map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Mapsui.Widgets.Alignment.Center, HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Left, VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom });
            mapView.Map = map;
        
        }
    }
}