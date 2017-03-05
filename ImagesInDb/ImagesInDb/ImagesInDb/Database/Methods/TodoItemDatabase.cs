using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasteBeer.Database.Entity;
using Xamarin.Forms;

namespace TasteBeer
{
  public  class TodoItemDatabase
    {
        private SQLiteAsyncConnection database;

        public TodoItemDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

           // database.DropTableAsync<BeerImage>().Wait();
            database.CreateTableAsync<BeerImage>().Wait();
            
        }


        public Task<List<BeerImage>> GetBeerImagesAsync()
        {
            return database.Table<BeerImage>().ToListAsync();
        }

        public Task<BeerImage> GetBeerImageAsync(int id)
        {
            return database.Table<BeerImage>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveBeerImageAsync(BeerImage item)
        {
            if (item.Id != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteBeerImageAsync(BeerImage item)
        {
            return database.DeleteAsync(item);
        }
    }
}
