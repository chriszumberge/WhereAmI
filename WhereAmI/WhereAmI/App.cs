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
        bool automaticUpdates { get; set; } = false;
        public App()
        {
            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            btnToggleLocationUpdates.Clicked += BtnToggleLocationUpdates_Clicked;

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

        IGeolocator locator { get; } = CrossGeolocator.Current;

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
