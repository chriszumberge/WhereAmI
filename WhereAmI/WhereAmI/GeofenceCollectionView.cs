using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace WhereAmI
{
    public sealed class GeofenceCollectionView : ListView
    {
        GeofenceManager manager => GeofenceManagerProvider.Instance;

        private ObservableCollection<Geofence> Geofences { get; set; }

        public GeofenceCollectionView()
        {
            this.Geofences = new ObservableCollection<Geofence>(manager.Geofences);

            this.ItemsSource = this.Geofences;

            this.IsPullToRefreshEnabled = true;
            this.RefreshCommand = new Command(() => Geofences = new ObservableCollection<Geofence>(manager.Geofences), () => true);

            this.ItemTemplate = new DataTemplate(() =>
            {
                Label lblName = new Label
                {
                    FontSize = 28,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Start,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                lblName.SetBinding(Label.TextProperty, "Name");

                StackLayout layout = new StackLayout();
                //layout.SetBinding(StackLayout.BackgroundColorProperty, (Geofence geofence) => geofence.IsInside ? Color.Green.MultiplyAlpha(0.5) : Color.Red.MultiplyAlpha(0.5));

                ViewCell cell = new ViewCell
                {
                    View = lblName
                };

                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, "Name");
                deleteAction.Clicked += (object sender, EventArgs e) =>
                {
                    var geofenceName = ((MenuItem)sender).CommandParameter.ToString();
                    if (!String.IsNullOrEmpty(geofenceName))
                    {
                        Geofence fence = manager.Geofences.Where(g => g.Name.Equals(geofenceName)).FirstOrDefault();
                        if (fence != null)
                        {
                            manager.UnsubscribeGeofence(fence);
                            this.RefreshCommand.Execute(null);
                        }
                    }
                };

                cell.ContextActions.Add(deleteAction);
                cell.Tapped += async (object sender, EventArgs e) =>
                {
                    Geofence selectedGeofence = ((sender as ViewCell).BindingContext as Geofence);

                    double x1 = selectedGeofence.Geometry.Coordinates.Min(c => c.X);
                    double y1 = selectedGeofence.Geometry.Coordinates.Min(c => c.Y);
                    double x2 = selectedGeofence.Geometry.Coordinates.Max(c => c.X);
                    double y2 = selectedGeofence.Geometry.Coordinates.Max(c => c.Y);

                    double centerX = x1 + ((x2 - x1) / 2);
                    double centerY = y1 + ((y2 - y1) / 2);
                    double radius = Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));

                    DrawableMap geofenceMap = new DrawableMap(MapSpan.FromCenterAndRadius(new Position(centerY, centerX),
                                                                                          Distance.FromMeters((radius / 0.0000089987) + 100)))
                    {
                        MapType = MapType.Hybrid,
                        DrawableOptions = new GeometryDrawableOptions
                        {
                            Alpha = 0.8,
                            FillColor = Color.Red,
                            StrokeColor = Color.White,
                            LineWidth = 1
                        }
                    };
                    if ((App.Current as App).LatestPosition != null)
                    {
                        geofenceMap.Pins.Add(new Pin()
                        {
                            Position = new Position((App.Current as App).LatestPosition.Latitude, (App.Current as App).LatestPosition.Longitude),
                            Label = "YOU!",
                            Type = PinType.Generic
                        });
                    }

                    // TODO need to figure out how to get geometry coordinates from the tap
                    geofenceMap.Shapes.Add(new MapShape()
                    {
                        ShapeCoordinates = 
                            selectedGeofence.Geometry.Coordinates.Select((GeoAPI.Geometries.Coordinate arg) => new Position(arg.Y, arg.X)).ToList()
                    });

                    await this.Navigation.PushAsync(new ContentPage
                    {
                        Content = geofenceMap,
                        Title = selectedGeofence.Name
                    });
                };

                return cell;
            });

            this.RefreshCommand.Execute(null);
        }
    }
}
