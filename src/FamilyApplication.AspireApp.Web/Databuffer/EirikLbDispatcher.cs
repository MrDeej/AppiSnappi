using Eiriklb.Utils;
using Microsoft.AspNetCore.Components;

namespace FamilyApplication.AspireApp.Web.Databuffer
{
    public class EirikLbDispatcher : IEiriklbDispatcher
    {
        public Task InvokeAsync(Action action)
        {
            action();
            return Task.CompletedTask;
            //var disp = Dispatcher.CreateDefault();
            //await disp.InvokeAsync(action);
        }

        public async Task InvokeAsync(Func<Task> workItem)
        {
            await workItem();
        }

        public bool IsDispatchNeeded()
        {
            return true;
        }
    }
}
