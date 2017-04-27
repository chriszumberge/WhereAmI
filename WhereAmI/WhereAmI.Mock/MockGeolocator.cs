using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace WhereAmI.Mock
{
    public class MockGeolocator : IGeolocator
    {
        bool mAllowsBackgroundUpdates;
        public bool AllowsBackgroundUpdates
        {
            get
            {
                return mAllowsBackgroundUpdates;
            }

            set
            {
                mAllowsBackgroundUpdates = value;
            }
        }

        double mDesiredAccuracy;
        public double DesiredAccuracy
        {
            get
            {
                return mDesiredAccuracy;
            }

            set
            {
                mDesiredAccuracy = value;
            }
        }

        bool mIsGeolocationAvailable;
        public bool IsGeolocationAvailable
        {
            get
            {
                return mIsGeolocationAvailable;
            }
        }

        bool mIsGeolocationEnabled;
        public bool IsGeolocationEnabled
        {
            get
            {
                return mIsGeolocationEnabled;
            }
        }

        bool mIsListening { get; set; } = false;

        public bool IsListening
        {
            get
            {
                return mIsListening;
            }
        }

        bool mPausesLocationUpdatesAutomatically;
        public bool PausesLocationUpdatesAutomatically
        {
            get
            {
                return mPausesLocationUpdatesAutomatically;
            }

            set
            {
                mPausesLocationUpdatesAutomatically = value;
            }
        }

        bool mSupportsHeading;
        public bool SupportsHeading
        {
            get
            {
                return mSupportsHeading;
            }
        }

        public event EventHandler<PositionEventArgs> PositionChanged;
        public event EventHandler<PositionErrorEventArgs> PositionError;

        Position lastPosition;
        
        public Task<Position> GetPositionAsync(int timeoutMilliseconds = -1, CancellationToken? token = default(CancellationToken?), bool includeHeading = false)
        {
            return Task.Run(() => lastPosition);
        }

        int mMinTime { get; set; } = 0;
        double mMinDistance { get; set; } = 0;

        public Task<bool> StartListeningAsync(int minTime, double minDistance, bool includeHeading = false)
        {
            mIsListening = true;
            mMinTime = minTime;
            mMinDistance = minDistance;
            return Task.Run(() => mIsListening);
        }

        public Task<bool> StopListeningAsync()
        {
            mIsListening = false;
            return Task.Run(() => mIsListening);
        }

        public void FeedPosition(Position position)
        {
            lastPosition = position;
            
            if (mIsListening)
            {
                // TODO something with pythagoreas for min distance
                if ((position.Timestamp - lastPosition.Timestamp).TotalMinutes >= mMinTime)
                {
                    PositionChanged?.Invoke(this, new PositionEventArgs(position));
                }
            }
        }
    }
}
