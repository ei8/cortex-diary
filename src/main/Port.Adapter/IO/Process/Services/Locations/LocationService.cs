//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using System;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Locations;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ISettingsService settingsService;
        private readonly IRequestProvider _requestProvider;

        public LocationService(ISettingsService settingsService, IRequestProvider requestProvider)
        {
            this.settingsService = settingsService;
            _requestProvider = requestProvider;
        }

        public /* async */ Task UpdateUserLocation(Location newLocReq, string token)
        {
            //UriBuilder builder = new UriBuilder(this.settingsService.LocationEndpoint);

            //builder.Path = "api/v1/locations";

            //string uri = builder.ToString();

            //await _requestProvider.PostAsync(uri, newLocReq, token);

            return Task.CompletedTask;
        }
    }
}