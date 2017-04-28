using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using GeoAPI;
//using Xamarin.Forms.Maps;

namespace WhereAmI
{
    public class App : Application
    {
        IGeolocator locator { get; } = CrossGeolocator.Current;
        GeofenceManager geofenceManager { get; } = GeofenceManagerProvider.Instance;
        bool automaticUpdates { get; set; } = false;
        FixedSizeQueue<Position> lastFivePositions { get; set; } = new FixedSizeQueue<Position>(5);
        public App()
        {
            GeoAPI.NetTopologySuiteBootstrapper.Bootstrap();

            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            btnToggleLocationUpdates.Clicked += BtnToggleLocationUpdates_Clicked;

            btnShowOnMap.Clicked += BtnShowOnMap_Clicked;

            btnDisplayActions.Clicked += (async (s, e) =>
            {
                const string txtShowLocation = "Show Location On Map";
                const string txtDumpApp = "Dump Application State";

                var action = await MainPage.DisplayActionSheet("Actions", "Cancel", null, txtShowLocation, txtDumpApp);
                switch (action)
                {
                    case txtShowLocation:
                        await ShowCurrentLocationOnMap();
                        break;
                    case txtDumpApp:
                        string appState = AddApplicationStateToMessage(String.Empty);
                        AddMessageToUI(appState, Color.Aqua);
                        break;
                    default:
                        break;
                }
            });

            geofenceManager.OnEnteredGeofence += GeofenceManager_OnEnteredGeofence;
            geofenceManager.OnExitedGeofence += GeofenceManager_OnExitedGeofence;

            Helpers.SubscribeGeofences(geofenceManager);

            mainStack = new StackLayout
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
                    //btnShowOnMap,
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
            };

            // The root page of your application
            var content = new ContentPage
            {
                Title = "WhereAmI",
                Content = new ScrollView
                {
                    Content = mainStack,
                    Orientation = ScrollOrientation.Vertical
                }
            };

            NavigationPage rootPage = new NavigationPage(content);
            content.ToolbarItems.Add(new ToolbarItem("Fences", String.Empty, async () =>
            {
                GeofenceCollectionView geofenceCollectionView = new GeofenceCollectionView();
                ContentPage geofencesPage = new ContentPage
                {
                    Content = geofenceCollectionView,
                    Title = "Geofences"
                };

                // Needed if modally presented
                //geofencesPage.ToolbarItems.Add(new ToolbarItem("Dismiss"), )

                geofencesPage.ToolbarItems.Add(new ToolbarItem("+", String.Empty, async () =>
                {
                    await geofencesPage.DisplayAlert("Create Geofence Not Implemented", "Working on it...", "F.U.");
                }));

                geofencesPage.Appearing += (s, e) => { geofenceCollectionView.RefreshCommand.Execute(null); };

                await rootPage.PushAsync(geofencesPage);
            }));


            MainPage = rootPage;
        }

        private async void BtnShowOnMap_Clicked(object sender, EventArgs e)
        {
            await ShowCurrentLocationOnMap();
        }

        private async System.Threading.Tasks.Task ShowCurrentLocationOnMap()
        {
            try
            {
                if (latestPosition == null)
                {
                    await MainPage.DisplayAlert("No Latest Position", "Try again when location services has located you.", "Ok");
                }
                else
                {
                    Xamarin.Forms.Maps.Map positionMap = new Xamarin.Forms.Maps.Map(
                        Xamarin.Forms.Maps.MapSpan.FromCenterAndRadius(
                            new Xamarin.Forms.Maps.Position(latestPosition.Latitude, latestPosition.Longitude), Xamarin.Forms.Maps.Distance.FromMeters(100)))
                    {
                        MapType = Xamarin.Forms.Maps.MapType.Hybrid,
                    };
                    positionMap.Pins.Add(new Xamarin.Forms.Maps.Pin()
                    {
                        Position = new Xamarin.Forms.Maps.Position(latestPosition.Latitude, latestPosition.Longitude),
                        Label = "YOU!",
                        Type = Xamarin.Forms.Maps.PinType.Generic
                    });

                    await MainPage.Navigation.PushAsync(new ContentPage
                    {
                        Content = positionMap,
                        Title = "Map"
                    });
                }
            }
            catch (Exception ex)
            {
                string msg = $"Excepted Showing Position on Map {Environment.NewLine}{ex}";
                AddMessageToUI(msg, Color.Red);
            }
        }

        TollboothGeofence lastEnteredTollBooth { get; set; } = null;
        RouteGeofence currentRoute { get; set; } = null;

        private void GeofenceManager_OnEnteredGeofence(object sender, GeofenceEnteredEventArgs e)
        {
            //await MainPage.DisplayAlert("Exited Geofence!", e.Geofence.Name, "Ok");
            //mainStack.Children.Add(new Label
            //{
            //    HorizontalTextAlignment = TextAlignment.Start,
            //    HorizontalOptions = LayoutOptions.StartAndExpand,
            //    LineBreakMode = LineBreakMode.WordWrap,
            //    Text = $"Entered geofence {e.Geofence.Name} at {e.DateTimeEntered}"
            //});
            try
            {
                if (e.Geofence is TollboothGeofence)
                {
                    lastEnteredTollBooth = e.Geofence as TollboothGeofence;
                }
                else if (e.Geofence is RouteGeofence)
                {
                    // handle crossing over routes
                    if (currentRoute == null)
                    {
                        if (lastEnteredTollBooth != null)
                        {
                            currentRoute = e.Geofence as RouteGeofence;
                            AddMessageToUI($"Got on {currentRoute.Name} via the {lastEnteredTollBooth.Name} exit.");
                        }
                    }
                }
                else
                {
                    AddMessageToUI($"Entered geofence {e.Geofence.Name} at {e.DateTimeEntered}");
                }

            }
            catch (Exception ex)
            {
                string msg = $"Excepted On Entered Geofence {Environment.NewLine}{ex}";
                msg = AddApplicationStateToMessage(msg);

                AddMessageToUI(msg, Color.Red);
            }
        }

        private string AddApplicationStateToMessage(string msg)
        {
            var applicationState = GetApplicationState();
            if (applicationState != null)
            {
                try
                {
                    var appStateString = applicationState.PrintState();
                    if (!String.IsNullOrEmpty(appStateString))
                    {
                        msg = String.Concat(msg, Environment.NewLine, "Application State for Debugging:", Environment.NewLine, appStateString);
                    }
                    else
                    {
                        msg = String.Concat(msg, Environment.NewLine, "Failed to dump application state for debugging.");
                    }
                }
                catch
                {
                    msg = String.Concat(msg, Environment.NewLine, "Failed to dump application state for debugging.");
                }
            }
            else
            {
                msg = String.Concat(msg, Environment.NewLine, "Failed to dump application state for debugging.");
            }

            return msg;
        }

        private void GeofenceManager_OnExitedGeofence(object sender, GeofenceExitedEventArgs e)
        {
            //await MainPage.DisplayAlert("Entered Geofence!", e.Geofence.Name, "Ok");
            //mainStack.Children.Add(new Label
            //{
            //    HorizontalTextAlignment = TextAlignment.Start,
            //    HorizontalOptions = LayoutOptions.StartAndExpand,
            //    LineBreakMode = LineBreakMode.WordWrap,
            //    Text = $"Exited geofence {e.Geofence.Name} at {e.DateTimeExited}"
            //});
            try
            {
                if (e.Geofence is TollboothGeofence)
                {
                    //lastEnteredTollBooth = e.Geofence as TollboothGeofence;
                    lastEnteredTollBooth = null;
                }
                else if (e.Geofence is RouteGeofence)
                {
                    if (e.Geofence == currentRoute)
                    {
                        if (e.Geofence == null)
                        {
                            AddMessageToUI($"Got off {currentRoute.Name} via unspecified exit at ({LatestPosition.Latitude}, {LatestPosition.Longitude})");
                        }
                        else
                        {
                            AddMessageToUI($"Got off {currentRoute.Name} via the {lastEnteredTollBooth.Name} exit.");
                        }
                        currentRoute = null;
                        lastEnteredTollBooth = null;
                    }
                }
                else
                {
                    AddMessageToUI($"Exited geofence {e.Geofence.Name} at {e.DateTimeExited}");
                }
            }
            catch (Exception ex)
            {
                
                string msg = $"Excepted On Exited Geofence {Environment.NewLine}{ex}{Environment.NewLine}";
                msg = AddApplicationStateToMessage(msg);

                AddMessageToUI(msg, Color.Red);
            }
        }

        private void AddMessageToUI(string message)
        {
            AddMessageToUI(message, Color.Black);
        }

        private void AddMessageToUI(string message, Color textColor)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    mainStack.Children.Add(new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Text = message,
                        TextColor = textColor
                    });
                });
            }
            catch { }
        }

        private ApplicationState GetApplicationState()
        {
            ApplicationState appState = null;
            try
            {
                appState = new ApplicationState
                {
                    LastFivePositions = lastFivePositions.ToList(),
                    LatestPosition = latestPosition,
                    LastEnteredTollBooth = lastEnteredTollBooth,
                    CurrentRoute = currentRoute
                };
            }
            catch
            { }
            return appState;
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
            try
            {
                automaticUpdates = !automaticUpdates;

                // Automatically updating
                if (automaticUpdates)
                {
                    locator.PositionChanged += Locator_PositionChanged;
                    locator.PositionError += Locator_PositionError;
                    locator.DesiredAccuracy = GetLocatorAccuracy();
                    btnGetLocation.IsEnabled = false;
                    entryUpdateInterval.IsEnabled = false;
                    entryMinimumDistance.IsEnabled = false;
                    entryDesiredAccuracy.IsEnabled = false;
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
                    entryUpdateInterval.IsEnabled = true;
                    entryMinimumDistance.IsEnabled = true;
                    entryDesiredAccuracy.IsEnabled = true;
                    btnToggleLocationUpdates.Text = "Start Updates";
                    // unregister event handler so GC can collect reference
                    locator.PositionChanged -= Locator_PositionChanged;
                    locator.PositionError -= Locator_PositionError;
                }
            }
            catch (Exception ex)
            {
                string msg = $"Excepted Toggling Location Updates {Environment.NewLine}{ex}";
                AddMessageToUI(msg, Color.Red);
            }
        }

        Position latestPosition { get; set; }
        public Position LatestPosition => latestPosition;

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            try
            {
                var position = e.Position;

                latestPosition = position;

                try
                {
                    lastFivePositions.Enqueue(latestPosition);
                }
                catch { }

                UpdateUIWithNewPosition(position);
                geofenceManager.UpdateGeofences(position);
            }
            catch (Exception ex)
            {
                string msg = $"Excepted On Locator Position Changed {Environment.NewLine}{ex}";
                AddMessageToUI(msg, Color.Red);
            }
        }

        private void Locator_PositionError(object sender, PositionErrorEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Error);
            string msg = $"Locator Position Error {Environment.NewLine}{e.Error}";
            AddMessageToUI(msg, Color.Red);
        }

        private async void BtnGetLocation_Clicked(object sender, EventArgs e)
        {
            try
            {
                locator.DesiredAccuracy = GetLocatorAccuracy();

                actIndGettingLocation.IsRunning = true;
                actIndGettingLocation.IsVisible = true;
                btnGetLocation.IsEnabled = false;
                btnToggleLocationUpdates.IsEnabled = false;

                var position = await locator.GetPositionAsync(timeoutMilliseconds: 5000);
                latestPosition = position;

                try
                {
                    lastFivePositions.Enqueue(latestPosition);
                }
                catch { }

                UpdateUIWithNewPosition(position);
                geofenceManager.UpdateGeofences(position);

                actIndGettingLocation.IsRunning = false;
                actIndGettingLocation.IsVisible = false;
                btnGetLocation.IsEnabled = true;
                btnToggleLocationUpdates.IsEnabled = true;
            }
            catch (Exception ex)
            {
                string msg = $"Excepted Getting Location {Environment.NewLine}{ex}";
                AddMessageToUI(msg, Color.Red);
            }
        }

        private void UpdateUIWithNewPosition(Position position)
        {
            if (position != null)
            {
                //lblTimeStamp.Text = position.Timestamp.ToString();
                lblTimeStamp.Text = position.Timestamp.LocalDateTime.AddHours(1).ToString();
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

        StackLayout mainStack { get; set; }

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

        Button btnShowOnMap = new Button
        {
            Text = "Show On Map",
            HorizontalOptions = LayoutOptions.FillAndExpand
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

        Button btnDisplayActions = new Button
        {
            Text = "Actions",
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
