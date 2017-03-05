using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using TasteBeer.Database.Entity;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FFImageLoading;


namespace ImagesInDb
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<BeerImage> beerImages = new ObservableCollection<BeerImage>();
        public Func<CancellationToken, Task<Stream>> Stream { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            Title = "Main Page";
            Init();
       }

        private void Init()
        {
            CrossMedia.Current.Initialize().Wait();

            SetToolbarItems();
            GetBeersFromDbAndDislpay();

        }

        private void SetToolbarItems()
        {
            ToolbarItem toolbar = new ToolbarItem();
            toolbar.Icon = "camera.png";

            this.ToolbarItems.Add(new ToolbarItem { Icon = "camera.png", Command = new Command(this.TakePhotoAndSave) });
           
        }

        private void GetBeersFromDbAndDislpay()
        {
            beerImages = new ObservableCollection<BeerImage>(App.Database.GetBeerImagesAsync().Result);
            listviewWithImages.ItemsSource = beerImages;
            if (beerImages.Count > 1) DatabaseInfoLabel.Text = beerImages.Count + " images in database";
        }

        private string ConvertImageToBase64(MediaFile file)
        {
            BeerImage beer = new BeerImage();

            var stream2 = file.GetStream();

            System.IO.BinaryReader br = new System.IO.BinaryReader(stream2);
            Byte[] bytes = br.ReadBytes((Int32)stream2.Length);
         
            return Convert.ToBase64String(bytes, 0, bytes.Length);
           
        }
        private async Task< MediaFile > TakePhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
              await  DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Custom,
                CustomPhotoSize = 50,
                AllowCropping = true
            });

            if (file == null)
                return null;

            return file;
        }

        private async void TakePhotoAndSave()
        {

            MediaFile photo = await TakePhoto();
            if(photo == null) return;
            
            // Display taken image
 

            string base64 = await Task.Factory.StartNew( () =>  ConvertImageToBase64(photo));
            
            BeerImage beerImage = new BeerImage();
            beerImage.PicketDateTime = DateTime.Now;
            beerImage.ImageRaw = base64;

            TakenImageAbsoluteLayout.IsVisible = true;
            imageC.Source = ImageSource.FromFile(photo.Path);
            PathLabel.Text = photo.Path;
            DateTimeLabel.Text = beerImage.PicketDateTime.ToUniversalTime().ToString();

          int rows =  await App.Database.SaveBeerImageAsync(beerImage);

            if (rows > 0)
            {
                beerImages.Add(beerImage);
                DatabaseInfoLabel.Text = beerImages.Count + " images in database";
            
                await  DisplayAlert("Image Saved", "Image saved", "Ok");
            }
        }
    }
}
