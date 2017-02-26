using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace TasteBeer.Database.Entity
{
   public class BeerImage
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get;  set; }
        [NotNull]
        public int Beer_Id { get; set; }
        
        public string ImageRaw { get; set; }

        [Ignore]
        public ImageSource Image
        {
            get { return Xamarin.Forms.ImageSource.FromStream(
              () => new MemoryStream(Convert.FromBase64String(ImageRaw)));
            }
        }
    }
}
