using InternetCrawler.RealEstate;
using InternetCrawler.Transport;

namespace InternetCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Cars car = new Cars();
            car.Parse("/lv/transport/cars/");
            Flat flat = new Flat();
            flat.Parse("/lv/real-estate/flats/");
        }
    }
}
