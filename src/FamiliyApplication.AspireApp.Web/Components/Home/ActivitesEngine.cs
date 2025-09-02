using FamiliyApplication.AspireApp.Web.CosmosDb.Family;
using FamiliyApplication.AspireApp.Web.Databuffer;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.Threading.Channels;

namespace FamiliyApplication.AspireApp.Web.Components.Home
{
    public class ActivitesEngine : IDisposable
    {
        private readonly ChannelReader<bool> reader;
        private readonly ChannelWriter<bool> writer;
        private readonly GlobalVm _vm;

        private Task _processingTask;

        public ObservableCollection<DtoFamilyActivites> ObsCollActivies = new();

        public ObservableCollection<FamilyTodoDto> ObsCollTodosToApprove = new();

        public event EventHandler? SomethingHasChanged;

        public ActivitesEngine(GlobalVm vm)
        {
            var options = new UnboundedChannelOptions { SingleReader = true, SingleWriter = true, AllowSynchronousContinuations = false };
            var channel = Channel.CreateUnbounded<bool>(options);
            reader = channel.Reader;
            writer = channel.Writer;
            _processingTask = ProcessUpdatesAsync();
            _vm = vm;
            foreach(var user in vm.UserDtos)
            {
                user.TodosToApprove.CollectionChanged += TodosToApprove_CollectionChanged;
            }
            Refresh();
            writer.TryWrite(true);


        }

        private void TodosToApprove_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            writer.TryWrite(true);
        }

        private void Refresh()
        {
            var changes = false;
            var allTodods = _vm.UserDtos.SelectMany(a => a.TodosToApprove);

            var toAdd = from myrows in allTodods
                        join existing in ObsCollTodosToApprove on myrows equals existing
                        into joined
                        from j in joined.DefaultIfEmpty()
                        where j == null
                        select myrows;

            var toRemove = (from existing in ObsCollTodosToApprove
                            join todo in allTodods on existing equals todo
                            into joined
                            from j in joined.DefaultIfEmpty()
                            where j == null
                            select existing).ToArray();

            foreach (var rad in toAdd)
            {
                changes = true;
                ObsCollTodosToApprove.Add(rad);
            }
            foreach (var rad in toRemove)
            {
                changes = true;
                ObsCollTodosToApprove.Remove(rad);
            }

            if (changes && SomethingHasChanged != null)
                SomethingHasChanged.Invoke(this, new EventArgs());
        }

        private async Task ProcessUpdatesAsync()
        {
            while (await reader.WaitToReadAsync())
            {
                while (reader.TryRead(out bool _))
                {
                    // Get rid of queue in case of mass updates
                }

                // Implement a delay mechanism here to support mass updates
                await Task.Delay(10);


                Refresh();
            }
        }

        public void Dispose()
        {
            foreach (var user in _vm.UserDtos)
            {
                user.TodosToApprove.CollectionChanged -= TodosToApprove_CollectionChanged;
            }
        }
    }
}
