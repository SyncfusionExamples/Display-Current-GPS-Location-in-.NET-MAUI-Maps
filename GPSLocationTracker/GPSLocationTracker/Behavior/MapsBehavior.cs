namespace GPSLocationTracker
{
    using Syncfusion.Maui.Maps;
    using Syncfusion.Maui.Buttons;

    public class MapsBehavior : Behavior<ContentPage>
    {
        private MapTileLayer? tileLayer;
        private SfButton? markerButton;
        private SfButton? zoomInButton;
        private SfButton? zoomOutButton;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            this.tileLayer = bindable.FindByName<MapTileLayer>("tileLayer");
            this.tileLayer.Center = new MapLatLng(37.3427441, -121.9757752);

            this.markerButton = bindable.FindByName<SfButton>("LocationButton");
            this.markerButton.Clicked += MarkerButton_Clicked;
            this.zoomInButton = bindable.FindByName<SfButton>("ZoomInButton");
            this.zoomInButton.Clicked += ZoomInButton_Clicked;
            this.zoomOutButton = bindable.FindByName<SfButton>("ZoomOutButton");
            this.zoomOutButton.Clicked += ZoomOutButton_Clicked;
        }

        private async void MarkerButton_Clicked(object? sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                return;
            }

            Location? location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.High,
                Timeout = TimeSpan.FromSeconds(30)
            });

            if (location != null && this.tileLayer != null)
            {
                this.tileLayer.Center = new MapLatLng(location.Latitude, location.Longitude);

                this.tileLayer.Markers = new MapMarkerCollection()
                {
                    new MapMarker
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    }
                };
            }
        }

        private void ZoomInButton_Clicked(object? sender, EventArgs e)
        {
            if (this.tileLayer != null)
            {
                this.tileLayer.ZoomPanBehavior.ZoomLevel ++;
            }

        }

        private void ZoomOutButton_Clicked(object? sender, EventArgs e)
        {
            if (this.tileLayer != null)
            {
                this.tileLayer.ZoomPanBehavior.ZoomLevel --;
            }
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
            if (this.tileLayer != null)
                this.tileLayer = null;

            if (this.markerButton != null && this.zoomInButton != null && this.zoomOutButton != null)
            {
                this.markerButton.Clicked -= MarkerButton_Clicked;
                this.zoomInButton.Clicked -= ZoomInButton_Clicked;
                this.zoomOutButton.Clicked -= ZoomOutButton_Clicked;

                this.markerButton = null;
                this.zoomInButton = null;
                this.zoomOutButton = null;
            }
        }
    }
}
