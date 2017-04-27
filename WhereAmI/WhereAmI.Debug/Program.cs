using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhereAmI.Mock;

namespace WhereAmI.Debug
{
    class Program
    {
        static GeofenceManager manager { get; set; }

        static void Main(string[] args)
        {
            ApplicationState debuggingAppState = new ApplicationState();

            manager = new GeofenceManager();
            WhereAmI.Helpers.SubscribeGeofences(manager);
            manager.OnEnteredGeofence += GeofenceManager_OnEnteredGeofence;
            manager.OnExitedGeofence += GeofenceManager_OnExitedGeofence;

            IGeolocator mocklocator = new MockGeolocator();
            mocklocator.PositionChanged += Locator_PositionChanged;

            foreach (var position in debuggingAppState.LastFivePositions)
            {
                ((MockGeolocator)mocklocator).FeedPosition(position);

                Thread.Sleep(1000);
            }
        }

        static Position latestPosition { get; set; }
        static TollboothGeofence lastEnteredTollBooth { get; set; } = null;
        static RouteGeofence currentRoute { get; set; } = null;
        static FixedSizeQueue<Position> lastFivePositions { get; set; } = new FixedSizeQueue<Position>(5);

        private static void Locator_PositionChanged(object sender, PositionEventArgs e)
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

                //UpdateUIWithNewPosition(position);
                manager.UpdateGeofences(position);
            }
            catch (Exception ex)
            {
                string msg = $"Excepted On Locator Position Changed {Environment.NewLine}{ex}";
                AddMessageToUI(msg);
            }
        }

        private static void GeofenceManager_OnEnteredGeofence(object sender, GeofenceEnteredEventArgs e)
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
                AddMessageToUI(msg);
            }
        }

        private static void AddMessageToUI(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void GeofenceManager_OnExitedGeofence(object sender, GeofenceExitedEventArgs e)
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
                            AddMessageToUI($"Got off {currentRoute.Name} via unspecified exit at ({latestPosition.Latitude}, {latestPosition.Longitude})");
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
                AddMessageToUI(msg);
            }
        }
    }
}
