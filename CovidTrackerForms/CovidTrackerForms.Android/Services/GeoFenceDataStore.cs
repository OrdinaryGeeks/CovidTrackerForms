using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CovidTrackerForms.Models;
using CovidTrackerForms.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Android.Net;
using Xamarin.Essentials;

namespace CovidTrackerForms.Droid.Services
{
    public class GeoFenceDataStore : IGeoFenceDataStore
    {
        System.Net.Http.HttpClient client;


        public GeoFenceDataStore()
        {
            client = new HttpClient(new AndroidClientHandler());
            client.BaseAddress = new Uri("http://www.alectosinterdimensionalblog.com/OrdinaryGeeksContagionTracker/");


        }

        bool IsConnected => Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;



        public async Task<List<GeoFence>> GetGeoFences()
        {


            var location = await Geolocation.GetLocationAsync();
            if (IsConnected)
            {
                try
                {

                    
                    var json = await client.GetStringAsync($"api/PlaceBlocks/0?Lat2="+location.Latitude+"&Long2=" + location.Longitude);
           
                List<GeoFence> allFences = await Task.Run(() => JsonConvert.DeserializeObject<List<GeoFence>>(json));

                    List<GeoFence> uniqueFences = new List<GeoFence>();

                    foreach (GeoFence fence in allFences)
                    {
                        if (!uniqueFences.Exists(p => p.UniqueID == fence.UniqueID))
                            uniqueFences.Add(fence);

                    }
                    return uniqueFences;
                }catch(HttpRequestException hre)
                {

                    Console.WriteLine(hre.StackTrace);
                }

            }

            return null;

        }
    }

}