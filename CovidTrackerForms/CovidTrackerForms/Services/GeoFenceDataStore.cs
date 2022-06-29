﻿using CovidTrackerForms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CovidTrackerForms.Services
{
    public class GeoFenceDataStore : IGeoFenceDataStore
    {
        HttpClient client;

        public GeoFenceDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://www.alectosinterdimensionalblog.com/OrdinaryGeeksContagionTracker/");

        }

        bool IsConnected => Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;

        public async Task<List<GeoFence>> GetGeoFences()
        {

            var location = await Geolocation.GetLocationAsync();
            if (IsConnected)
            {

                var json = await client.GetStringAsync($"api/PlaceBlocks/0?Lat2=" + location.Latitude + "&Long2=" + location.Longitude);
                List<GeoFence> allFences = await Task.Run(() => JsonConvert.DeserializeObject<List<GeoFence>>(json));

                List<GeoFence> uniqueFences = new List<GeoFence>();

                foreach (GeoFence fence in allFences)
                {
                    if (!uniqueFences.Exists(p => p.UniqueID == fence.UniqueID))
                        uniqueFences.Add(fence);

                }
                return uniqueFences;


            }

            return null;

        }
        public async Task<bool> PostCheckInAsync(CheckIn checkIn)
        {
            if (checkIn == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(checkIn);

            var response = await client.PostAsync($"api/CheckIns/" , new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }


    }
}
