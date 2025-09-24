using FamilyApplication.AspireApp.Web.CosmosDb.User;
using System.Collections.ObjectModel;

namespace FamilyApplication.AspireApp.Web.Components.UserWallet
{
    public class UserSaveGoalManager : IDisposable
    {
        public ObservableCollection<UserWalletSaveGoal> ActiveSaveGoals { get; set; } = new();
        public ObservableCollection<UserWalletSaveGoal> FinishedSaveGoals { get; set; } = new();


        public event EventHandler? SomeThingChanged;
        private readonly UserDto _user;
        private bool _disposed;

        public UserSaveGoalManager(UserDto user)
        {
            _user = user;
            Refresh();

            user.Wallet.SaveGoals.CollectionChanged += SaveGoals_CollectionChanged;

            foreach (var saveGoal in user.Wallet.SaveGoals)
            {
                saveGoal.PropertyChanged += SaveGoal_PropertyChanged;
            }

        }

        private void SaveGoal_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserWalletSaveGoal.FinishedAt))
                Refresh();

            SomeThingChanged?.Invoke(this, new EventArgs());
        }

        private Guid delayGuid;
        private async void SaveGoals_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var rad in e.NewItems.OfType<UserWalletSaveGoal>())
                    rad.PropertyChanged += SaveGoal_PropertyChanged;
            }

            if (e.OldItems != null)
            {

                foreach (var rad in e.OldItems.OfType<UserWalletSaveGoal>())
                    rad.PropertyChanged -= SaveGoal_PropertyChanged;
            }

            var tmpGuid = Guid.NewGuid();
            delayGuid = tmpGuid;
            await Task.Delay(50);
            if (delayGuid != tmpGuid)
                return;
            Refresh();
            SomeThingChanged?.Invoke(this, new EventArgs());

        }

        private bool Refresh()
        {
            var actives = from myrows in _user.Wallet.SaveGoals
                          where myrows.FinishedAt == null
                          select myrows;

            var finisheds = from myrows in _user.Wallet.SaveGoals
                            where myrows.FinishedAt != null
                            select myrows;


            var addActive = from myrows in actives
                            join existing in ActiveSaveGoals
                            on myrows equals existing
                            into joined
                            from j in joined.DefaultIfEmpty()
                            where j == null
                            select myrows;

            var removeActive = (from existing in ActiveSaveGoals
                                join active in actives
                                on existing equals active
                                into joined
                                from j in joined.DefaultIfEmpty()
                                where j == null
                                select existing).ToArray();


            var addFinished = from finished in finisheds
                              join existing in FinishedSaveGoals
                              on finished equals existing
                              into joined
                              from j in joined.DefaultIfEmpty()
                              where j == null
                              select finished;


            var removeFinished = (from existing in FinishedSaveGoals
                                  join finished in finisheds
                                  on existing equals finished
                                  into joined
                                  from j in joined.DefaultIfEmpty()
                                  where j == null
                                  select existing).ToArray();

            var changed = false;
            foreach (var remove in removeActive)
            {
                changed = true;
                ActiveSaveGoals.Remove(remove);
            }

            foreach (var remove in removeFinished)
            {
                FinishedSaveGoals.Remove(remove);
                changed = true;
            }

            foreach (var add in addActive)
            {
                ActiveSaveGoals.Add(add);
                changed = true;
            }

            foreach (var add in addFinished)
            {
                FinishedSaveGoals.Add(add);
                changed = true;
            }
            return changed;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // Unsubscribe from CollectionChanged
            _user.Wallet.SaveGoals.CollectionChanged -= SaveGoals_CollectionChanged;

            // Unsubscribe from PropertyChanged for each save goal
            foreach (var saveGoal in _user.Wallet.SaveGoals)
            {
                saveGoal.PropertyChanged -= SaveGoal_PropertyChanged;
            }

            // Clear collections to release references
            ActiveSaveGoals.Clear();
            FinishedSaveGoals.Clear();

            _disposed = true;
        }
    }
}
