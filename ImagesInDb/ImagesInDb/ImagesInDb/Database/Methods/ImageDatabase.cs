using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImagesInDb.Database.Entity;
using Xamarin.Forms;

namespace ImagesInDb.Database.Methods
{
  public  class ImageDatabase
    {
        private SQLiteAsyncConnection database;

        public ImageDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

           // database.DropTableAsync<ImageEntity>().Wait();
            database.CreateTableAsync<ImageEntity>().Wait();
            
        }

        public Task<List<ImageEntity>> GetImagesAsync()
        {
            return database.Table<ImageEntity>().ToListAsync();
        }

        public Task<int> SaveImageAsync(ImageEntity item)
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

        public Task<int> DeleteImageAsync(ImageEntity item)
        {
            return database.DeleteAsync(item);
        }
    }
}
