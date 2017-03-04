using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TasteBeer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// TODO show image after take
// TODO refactor

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ImagesInDb
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application



            MainPage = new NavigationPage(new MainPage());

         
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        private static TodoItemDatabase _database;

        public static TodoItemDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new TodoItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("TodoSQLite.db3"));
                }
                return _database;
            }
        }
    }
}
