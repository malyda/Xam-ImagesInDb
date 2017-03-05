using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FFImageLoading;
using ImagesInDb.Database.Entity;


namespace ImagesInDb
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Images for UI
        /// </summary>
        public ObservableCollection<ImageEntity> Images = new ObservableCollection<ImageEntity>();
        public MainPage()
        {
            InitializeComponent();
            Title = "Images in Database";
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

            ToolbarItems.Add(new ToolbarItem { Icon = "camera.png", Command = new Command(this.TakePhotoAndSave) });
        }

        private void GetBeersFromDbAndDislpay()
        {
            Images = new ObservableCollection<ImageEntity>(App.Database.GetImagesAsync().Result);
            ListviewWithImages.ItemsSource = Images;
            if (Images.Count > 1) DatabaseInfoLabel.Text = Images.Count + " images in database";
        }

        /// <summary>
        /// Convert taken image to byte[] and then to base64 string which is saved to DB
        /// </summary>
        /// <param name="file">Photo</param>
        /// <returns>base64 string</returns>
        private string ConvertImageToBase64(MediaFile file)
        {
            var stream = file.GetStream();

            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            byte[] bytes = br.ReadBytes((Int32)stream.Length);
         
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
            
            // Convert photo to base64
            string base64 = await Task.Factory.StartNew( () =>  ConvertImageToBase64(photo));
            
            // Create new Entity
            ImageEntity imageEntity = new ImageEntity();
            imageEntity.PicketDateTime = DateTime.Now;
            imageEntity.ImageRaw = base64;

            // Show taken image
            TakenImageAbsoluteLayout.IsVisible = true;

            imageThumb.Source = ImageSource.FromFile(photo.Path);
            PathLabel.Text = photo.Path;
            DateTimeLabel.Text = imageEntity.PicketDateTime.ToUniversalTime().ToString();

            // Save image to database
            int rows = await App.Database.SaveImageAsync(imageEntity);

            // refresh UI
            if (rows > 0)
            {
                Images.Add(imageEntity);
                DatabaseInfoLabel.Text = Images.Count + " images in database";

                await DisplayAlert("Image Saved", "Image saved", "Ok");
            }
        }
    }
}
