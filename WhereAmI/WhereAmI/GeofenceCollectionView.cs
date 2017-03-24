using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WhereAmI
{
    public sealed class GeofenceCollectionView : ListView
    {
        GeofenceManager manager => GeofenceManagerProvider.Instance;

        private ObservableCollection<Geofence> Geofences { get; set; }

        public GeofenceCollectionView()
        {
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
                layout.SetBinding(StackLayout.BackgroundColorProperty, (Geofence geofence) => geofence.IsInside ? Color.Green.MultiplyAlpha(0.5) : Color.Red.MultiplyAlpha(0.5));

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
                return cell;
            });
        }
    }
}
