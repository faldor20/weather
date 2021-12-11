using System;
using System.Collections.Generic;
using System.ComponentModel;
using weather.Models;
using weather.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace weather.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}