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
        public App()
        {
            GeoAPI.NetTopologySuiteBootstrapper.Bootstrap();

            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            btnToggleLocationUpdates.Clicked += BtnToggleLocationUpdates_Clicked;

            btnShowOnMap.Clicked += BtnShowOnMap_Clicked;

            geofenceManager.OnEnteredGeofence += GeofenceManager_OnEnteredGeofence;
            geofenceManager.OnExitedGeofence += GeofenceManager_OnExitedGeofence;

            // Todo different screen.. probably a table for this PoC
            //Geofence homeFence = new Geofence("Home", 40.596625, -75.591912, 50);
            //Geofence workFence = new Geofence("Work", "POLYGON((-75.56702256202698 40.60780962922667,-75.56735515594482 40.60706841686434,-75.56759119033813 40.6062213069555,-75.56657195091248 40.60672631607828,-75.56594967842102 40.606986964455075,-75.56629300117493 40.60737793511422,-75.56671142578125 40.60783406465907,-75.56702256202698 40.60780962922667))");
            //Geofence churchRdAndChapmansFence = new Geofence("Home to Work Checkpoint", 40.607447, -75.572532, 10);
            //Geofence allentownYMCAFence = new Geofence("Allentown YMCA", "POLYGON((-75.48591256141663 40.593485683191474,-75.48546195030212 40.593603813008954,-75.48523128032684 40.59307833739258,-75.48567652702332 40.59295205969104,-75.48591256141663 40.593485683191474))");
            //Geofence tilghmanFence = new Geofence("Tilghman Street From LTI to Wegmans", "LINESTRING(-75.57602405548101 40.59273444649959,-75.5604887008667 40.58914967802697,-75.55851459503174 40.58918226769682,-75.55048942565918 40.591430916562736,-75.54040431976318 40.5941683000797)", 20);

            //geofenceManager.SubscribeGeofence(homeFence);
            //geofenceManager.SubscribeGeofence(workFence);
            //geofenceManager.SubscribeGeofence(churchRdAndChapmansFence);
            //geofenceManager.SubscribeGeofence(allentownYMCAFence);
            //geofenceManager.SubscribeGeofence(tilghmanFence);

            geofenceManager.SubscribeGeofence(new Geofence("Old U.S. 22/Tilghman Street", "LINESTRING(-75.63256502151489 40.583270292645814,-75.62968969345093 40.58387325197076,-75.62299489974976 40.58516063341827,-75.61393976211548 40.586985739332,-75.60361862182617 40.58918557758428,-75.5944561958313 40.59094539608536,-75.5893063545227 40.59192305301892,-75.58518648147583 40.59270516827414,-75.58329820632935 40.59298216523328,-75.5811095237732 40.59312881021768,-75.57900667190552 40.59309622247117,-75.57701110839844 40.592949577415276,-75.57471513748169 40.59260740436753,-75.5670976638794 40.590733568532386,-75.56078910827637 40.58925075687317,-75.55967330932617 40.58915298791604,-75.55870771408081 40.58921816723667,-75.55750608444214 40.58942999958997,-75.55413722991943 40.59032620596762,-75.54830074310303 40.591988229639604,-75.54461002349854 40.593047340821606,-75.54040431976318 40.59413902248207,-75.52789449691772 40.59759318135387)", 10));
            geofenceManager.SubscribeGeofence(new Geofence("Church Street", 40.58326545469004, -75.63260793685913, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Nursery Street", 40.58386026599528, -75.62973260879517, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Route 100", 40.58449581195547, -75.62643885612488, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Snowdrift Road", 40.58758382751916, -75.6109356880188, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Farm Bureau Road", 40.58983253013347, -75.60011029243469, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Blue Barn Road", 40.59191006860644, -75.58931708335876, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Broadway", 40.589188887471586, -75.56073546409607, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Hausman Road", 40.58940886730601, -75.55749535560608, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Rt 309 S", 40.589588109599035, -75.55518865585327, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Rt 309 N", 40.59016657008576, -75.55306434631348, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Rt 309 S", 40.59058208086843, -75.55538177490234, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Rt 309 N", 40.59117682710392, -75.55326819419861, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Parkway Road", 40.59162492008865, -75.549556016922, 15));
            geofenceManager.SubscribeGeofence(new Geofence("N 40th Street", 40.593001768834874, -75.54455637931824, 15));
            geofenceManager.SubscribeGeofence(new Geofence("Springhouse Road", 40.594150478934836, -75.54040431976318, 15));
            geofenceManager.SubscribeGeofence(new Geofence("N Cedar Crest Blvd", 40.597596490824984, -75.52785158157349, 15));

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
                    btnShowOnMap,
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
                    Content = mainStack
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

        TollboothGeofence lastEnteredTollBooth { get; set; } = null;
        RouteGeofence currentRoute { get; set; } = null;

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
        }

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
            if (e.Geofence is TollboothGeofence)
            {
                lastEnteredTollBooth = e.Geofence as TollboothGeofence;
            }
            else if (e.Geofence is RouteGeofence)
            {
                if (lastEnteredTollBooth != null)
                {
                    currentRoute = e.Geofence as RouteGeofence;
                    AddMessageToUI($"Got on {currentRoute.Name} via the {lastEnteredTollBooth.Name} exit.");
                }
            }
        }

        private void AddMessageToUI(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                mainStack.Children.Add(new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    LineBreakMode = LineBreakMode.WordWrap,
                    Text = message
                });
            });
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

        Position latestPosition { get; set; }
        public Position LatestPosition => latestPosition;

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;

            latestPosition = position;

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
            latestPosition = position;

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
