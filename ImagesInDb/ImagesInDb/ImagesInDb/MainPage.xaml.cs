using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using TasteBeer.Database.Entity;
using Xamarin.Forms;

namespace ImagesInDb
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<BeerImage> beerImages;


        //Creating TapGestureRecognizers  
   

        public MainPage()
        {
            InitializeComponent();

            ToolbarItem toolbar = new ToolbarItem();
            toolbar.Icon = "camera.png";
                
          
            this.ToolbarItems.Add(new ToolbarItem {Icon = "camera.png", Command = new Command(this.TakePhoto)});


            beerImages =new ObservableCollection<BeerImage>(  App.Database.GetBeerImagesAsync().Result );
            /*
            image.Source = Xamarin.Forms.ImageSource.FromStream(
              () => new MemoryStream(Convert.FromBase64String(beerImages2[0].Image)));
            */
            if (beerImages.Count > 0) listviewWithImages.ItemsSource = beerImages;

        //    takePhoto.Clicked += async (sender, args) =>
           


        }

        private async void TakePhoto()
        {
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                await DisplayAlert("File Location", file.Path, "OK");

                BeerImage beer = new BeerImage();


                var stream2 = file.GetStream();

                System.IO.BinaryReader br = new System.IO.BinaryReader(stream2);
                Byte[] bytes = br.ReadBytes((Int32)stream2.Length);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                beer.ImageRaw = base64String;

                await App.Database.SaveBeerImageAsync(beer);

                beerImages.Add(beer);

            };
        }
    }
}
