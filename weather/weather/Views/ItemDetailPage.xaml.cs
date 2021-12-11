using System.ComponentModel;
using weather.ViewModels;
using Xamarin.Forms;

namespace weather.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}