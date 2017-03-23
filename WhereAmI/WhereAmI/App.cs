using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace WhereAmI
{
    public class App : Application
    {
        IGeolocator locator { get; } = CrossGeolocator.Current;
        GeofenceManager geofenceManager { get; } = new GeofenceManager();
        bool automaticUpdates { get; set; } = false;
        public App()
        {
            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            btnToggleLocationUpdates.Clicked += BtnToggleLocationUpdates_Clicked;

            geofenceManager.OnEnteredGeofence += GeofenceManager_OnEnteredGeofence;
            geofenceManager.OnExitedGeofence += GeofenceManager_OnExitedGeofence;

            // Todo different screen.. probably a table for this PoC
            Geofence homeFence = new Geofence("Home", 40.596625, -75.591912, 30);
            Geofence workFence = new Geofence("Work", "POLYGON((-75.56702256202698 40.60780962922667,-75.56735515594482 40.60706841686434,-75.56759119033813 40.6062213069555,-75.56657195091248 40.60672631607828,-75.56594967842102 40.606986964455075,-75.56629300117493 40.60737793511422,-75.56671142578125 40.60783406465907,-75.56702256202698 40.60780962922667))");
            Geofence churchRdAndChapmansFence = new Geofence("Homt to Work Checkpoint", 40.607447, -75.572532, 10);
            Geofence allentownYMCAFence = new Geofence("Allentown YMCA", "POLYGON((-75.48591256141663 40.593485683191474,-75.48546195030212 40.593603813008954,-75.48523128032684 40.59307833739258,-75.48567652702332 40.59295205969104,-75.48591256141663 40.593485683191474))");
            Geofence tilghmanFence = new Geofence("Tilghman Street LTI to Wegmans", "LINESTRING(-75.57602405548101 40.59273444649959,-75.5604887008667 40.58914967802697,-75.55851459503174 40.58918226769682,-75.55048942565918 40.591430916562736,-75.54040431976318 40.5941683000797)", 7);

            geofenceManager.SubscribeGeofence(homeFence);
            geofenceManager.SubscribeGeofence(workFence);
            geofenceManager.SubscribeGeofence(churchRdAndChapmansFence);
            geofenceManager.SubscribeGeofence(allentownYMCAFence);
            geofenceManager.SubscribeGeofence(tilghmanFence);

            // The root page of your application
            var content = new ContentPage
            {
                Title = "WhereAmI",
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Padding = new Thickness(10, 20),
                    Children = {
                        actIndGettingLocation,
                        lblTimeStamp,
                        lblLocation,
                        lblHeading,
                        lblAddress,
                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children = {
                                new Label
                                {
                                    Text = "Accuracy:",
                                    HorizontalTextAlignment = TextAlignment.Start
                                },
                                entryDesiredAccuracy,
                                new Label
                                {
                                    Text = " meters",
                                    HorizontalTextAlignment = TextAlignment.Start
                                }
                            }
                        },
                        btnGetLocation,
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Update Interval:",
                                    HorizontalTextAlignment = TextAlignment.Start
                                },
                                entryUpdateInterval,
                                new Label
                                {
                                    Text = " seconds",
                                    HorizontalTextAlignment = TextAlignment.Start
                                }
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Min Update Distance:",
                                    HorizontalTextAlignment = TextAlignment.Center
                                },
                                entryMinimumDistance,
                                new Label
                                {
                                    Text = " meters",
                                    HorizontalTextAlignment = TextAlignment.Start
                                }
                            }
                        },
                        btnToggleLocationUpdates
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        private async void GeofenceManager_OnExitedGeofence(object sender, GeofenceExitedEventArgs e)
        {
            await MainPage.DisplayAlert("Entered Geofence!", e.Geofence.Name, "Ok");
        }

        private async void GeofenceManager_OnEnteredGeofence(object sender, GeofenceEnteredEventArgs e)
        {
            await MainPage.DisplayAlert("Exited Geofence!", e.Geofence.Name, "Ok");
        }

        private int GetLocatorAccuracy()
        {
            int accuracy;

            if (!Int32.TryParse(entryDesiredAccuracy.Text, out accuracy))
            {
                accuracy = defaultDesiredAccuracy;
            }

            return accuracy;
        }

        private int GetUpdateInterval()
        {
            int interval;
            if (!Int32.TryParse(entryUpdateInterval.Text, out interval))
            {
                interval = defaultUpdateInterval;
            }

            return interval;
        }

        private int GetMinimumDistance()
        {
            int distance;
            if (!Int32.TryParse(entryMinimumDistance.Text, out distance))
            {
                distance = defaultMinDistance;
            }

            return distance;
        }

        private async void BtnToggleLocationUpdates_Clicked(object sender, EventArgs e)
        {
            automaticUpdates = !automaticUpdates;

            // Automatically updating
            if (automaticUpdates)
            {
                locator.PositionChanged += Locator_PositionChanged;
                locator.PositionError += Locator_PositionError;
                locator.DesiredAccuracy = GetLocatorAccuracy();
                btnGetLocation.IsEnabled = false;
                btnToggleLocationUpdates.Text = "Stop Updates";
                await locator.StartListeningAsync(
                    minTime: GetUpdateInterval(),
                    minDistance: GetMinimumDistance(),
                    includeHeading: true);
            }
            // Not automatically updating
            else
            {
                await locator.StopListeningAsync();
                btnGetLocation.IsEnabled = true;
                btnToggleLocationUpdates.Text = "Start Updates";
                // unregister event handler so GC can collect reference
                locator.PositionChanged -= Locator_PositionChanged;
                locator.PositionError -= Locator_PositionError;
            }
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;

            UpdateUIWithNewPosition(position);
            geofenceManager.UpdateGeofences(position);
        }

        private void Locator_PositionError(object sender, PositionErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Error);
        }

        private async void BtnGetLocation_Clicked(object sender, EventArgs e)
        {
            locator.DesiredAccuracy = GetLocatorAccuracy();

            actIndGettingLocation.IsRunning = true;
            actIndGettingLocation.IsVisible = true;
            btnGetLocation.IsEnabled = false;
            btnToggleLocationUpdates.IsEnabled = false;
            
            var position = await locator.GetPositionAsync(timeoutMilliseconds: 5000);

            UpdateUIWithNewPosition(position);
            geofenceManager.UpdateGeofences(position);

            actIndGettingLocation.IsRunning = false;
            actIndGettingLocation.IsVisible = false;
            btnGetLocation.IsEnabled = true;
            btnToggleLocationUpdates.IsEnabled = true;
        }

        private void UpdateUIWithNewPosition(Position position)
        {
            if (position != null)
            {
                lblTimeStamp.Text = position.Timestamp.ToString();
                lblLocation.Text = String.Concat(position.Latitude, ", ", position.Longitude);
                lblHeading.Text = String.Concat(position.Heading, " deg rel N");

                // In v4.0
                //var addresses = await locator.GetAddressesForPositionAsync(position);
                //var address = addresses.FirstOrDefault();
                //if (address == null)
                //{
                //    lblAddress.Text = "No address found.";
                //}
                //else
                //{
                //    lblAddress.Text = String.Concat(address.Throughfare, Environment.NewLine, address.Locality, ", ", address.Country);
                //}
            }
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

        Label lblTimeStamp { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblLocation { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblHeading { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblAddress { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            LineBreakMode = LineBreakMode.WordWrap
        };

        Button btnGetLocation = new Button
        {
            Text = "Get Location",
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        static int defaultUpdateInterval = 20;
        Entry entryUpdateInterval = new Entry
        {
            Text = defaultUpdateInterval.ToString(),
            //Placeholder = "Interval in Seconds",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center,
            //HorizontalOptions = LayoutOptions.Center
        };

        static int defaultMinDistance = 0;
        Entry entryMinimumDistance = new Entry
        {
            Text = defaultMinDistance.ToString(),
            //Placeholder = "Distance in Meters",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center
        };

        Button btnToggleLocationUpdates = new Button
        {
            Text = "Start Updates",
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        static int defaultDesiredAccuracy = 0;
        Entry entryDesiredAccuracy = new Entry
        {
            Text = defaultDesiredAccuracy.ToString(),
            //Placeholder = "Accuracy in Meters",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center,
            //HorizontalOptions = LayoutOptions.Center,
        };

        ActivityIndicator actIndGettingLocation = new ActivityIndicator
        {
            IsRunning = false,
            IsVisible = false
        };
    }
}
