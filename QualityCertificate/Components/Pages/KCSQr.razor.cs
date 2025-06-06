using HoangThach.AccountShared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using QualityCertificate.Datas.Entity;
using QualityCertificate.Services;
using System.Web;

namespace QualityCertificate.Components.Pages
{
    public partial class KCSQr
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IBulkCementCQ BulkCementCQ { get; set; }

        private BulkCementCQ Input = new();

        private bool IsSuccess = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var query = new Uri(NavigationManager.Uri).Query;
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);

            if (queryParams.TryGetValue("Gid", out var value))
            {
                bool isValid = Guid.TryParse(value, out var guidOutput);
                if (isValid)
                {
                    var data = await BulkCementCQ.GetBulkCementCQByGid(guidOutput);
                    if (data.IsSuccess)
                    {
                        Input = data.BulkCementCQ;
                        IsSuccess = true;
                    }
                }
            }
        }
        //private async Task PrintCertificate()
        //{
        //    await JS.InvokeVoidAsync("printCertificate");
        //}
    }
}
