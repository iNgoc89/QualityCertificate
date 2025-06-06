using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using QualityCertificate.Datas.Entity;
using QualityCertificate.Datas.Models;
using QualityCertificate.Services;
using System.Security.Claims;
using System.Timers;

namespace QualityCertificate.Components.Pages
{
    public partial class CementCQ
    {
        [Inject] IBulkCementCQ BulkCementCQService { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        [Inject] WordOpenXmlService WordOpenXmlService { get; set; }

        [Inject] QRService QRService { get; set; }
        private bool isAuthenticated = false;
        private BulkCementCQ Input = new();
        private List<BulkCementCQ> BulkCementCQs = [];

        public string? UserId = "";
        public string? Gid = "";
        private string? authMessage;
        private string? UserName;

        private bool ShowProgress;
        private string searchString = "";

        public string ShowEdit = "d-none";


        public long Card34;
        public long CardP3;
        public string? QrCodeImage = "";
        public ReadCard ReadCard;
        System.Timers.Timer refreshTimer;

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        // đảm bảo đã hoàn thành render  
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var token = await AuthService.CheckTokenAndRefresh(NavigationManager.BaseUri);

                var authState = await authenticationState;
                var user = authState.User;

                if (user.Identity is not null && user.Identity.IsAuthenticated && !string.IsNullOrEmpty(token))
                {
                    isAuthenticated = true;
                    UserId = user!.Claims.First().Value;
                    Gid = user.FindFirst(c => c.Type == "Gid")?.Value;
                    UserName = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
                    await GetBulkCementCQ();

                    ReadCard = ReadCard.getInstance();
                    //khởi tạo
                    refreshTimer = new System.Timers.Timer(2000);
                    refreshTimer.Elapsed += RefreshTimer_Elapsed;
                    refreshTimer.AutoReset = true;
                    refreshTimer.Start();


                    StateHasChanged();
                }
                else
                {
                    NavigationManager.NavigateTo("/Logout");
                }
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            refreshTimer.Stop();
            try
            {
                LoadListCard();
                try
                {
                    base.InvokeAsync(() => StateHasChanged());
                }
                catch { }
            }
            catch (Exception)
            {
            }
            refreshTimer.Start();
        }
        public void LoadListCard()
        {
            Card34 = ReadCard.Card34;
            CardP3 = ReadCard.CardP3;
        }


        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (refreshTimer != null)
                    {
                        refreshTimer.Stop();
                        refreshTimer.Elapsed -= RefreshTimer_Elapsed;
                        refreshTimer.Dispose();
                        refreshTimer = null;
                    }
                }

                disposed = true;
            }
        }
        private async Task Select34()
        {
            await HanderSelect(ReadCard.getInstance().Card34.ToString());
        }
        private async Task SelectP3()
        {
            await HanderSelect(ReadCard.getInstance().CardP3.ToString());
        }

        private async Task HanderSelect(string cardNumber)
        {
            var (vehicleCode, isSuccess, errorMsg) = await GetVehicleCodeByCardNumber(cardNumber);
            if (isSuccess == true)
            {
                var data = BulkCementCQService.GetBulkCementCQByVehicle(vehicleCode).Result;
                if (data.IsSuccess == true)
                {
                    Input = data.BulkCementCQ;
                    Snackbar.Add(data.ErrorMsg, Severity.Success);
                    ShowEdit = "d-flex";
                    var qr = QRService.GenerateQrCode(vehicleCode);
                    QrCodeImage = "data:image/png;base64," + Convert.ToBase64String(qr);

                }
                else
                {
                    await Reload(data.ErrorMsg, Severity.Error);
                 
                }
            }
            else
            {
                await Reload(errorMsg, Severity.Error);
            }

            //xóa sau khi select
            //System.Timers.Timer time = new System.Timers.Timer();
            ////5s
            //time.Interval = 300 * 1000;
            //time.Elapsed += RefreshTimerCard_Elapsed;
            //time.Start();

            await Task.Delay(5000);
            ClearCard();
        }

        private void RefreshTimerCard_Elapsed(object sender, ElapsedEventArgs e)
        {
            refreshTimer.Stop();
            try
            {
                ClearCard();
                try
                {
                    base.InvokeAsync(() => StateHasChanged());
                }
                catch { }
            }
            catch (Exception)
            {
            }
            refreshTimer.Start();
        }
        private void ClearCard()
        {
            //Input.Vehicle_Code = string.Empty;
            ReadCard.Card34 = 0;
            ReadCard.CardP3 = 0;
            Card34 = 0;
            CardP3 = 0;
        }
        private async Task<List<BulkCementCQ>?> GetBulkCementCQ()
        {
            BulkCementCQs = await BulkCementCQService.GetBulkCementCQ() ?? [];
            return BulkCementCQs;
        }
        private async Task<(string?, bool, string?)> GetVehicleCodeByCardNumber(string cardNumber)
        {
            var data = await BulkCementCQService.GetVehicleByCardNumbers(cardNumber);
            return data;
        }
        
        private async Task Reload(string message, Severity severity)
        {
            await Task.Delay(300);
            ShowEdit = "d-none";
            Snackbar!.Add(message, severity);
            QrCodeImage = string.Empty;
            Input = new();
            await GetBulkCementCQ();
            await InvokeAsync(StateHasChanged);
            ShowProgress = false;
        }

        private async Task InMaQR()
        {
            ShowProgress = true;
            await Task.Delay(300);
            try
            {

                var (errors, isSucress) = WordOpenXmlService.PrintInfoWithQR(Input);
                if (isSucress)
                {
                    await Reload(errors, Severity.Success);
                }
                else
                {
                    await Reload(errors, Severity.Error);
                }

            }
            catch (Exception ex)
            {
                await Reload(ex.Message, Severity.Error);
            }


            ShowProgress = false;

        }
        private async Task OnValidSubmit(EditContext context)
        {
            await InMaQR();
            StateHasChanged();
        }

        private void SelectBulkCementCQ(int id)
        {
            ShowEdit = "d-flex";
            Input = BulkCementCQs.FirstOrDefault(x => x.Id == id) ?? new();
            var qr = QRService.GenerateQrCode(Input.Gid.ToString());
            QrCodeImage = "data:image/png;base64," + Convert.ToBase64String(qr);
        }

        private async Task Back()
        {
            await Reload("", Severity.Normal);
        }
        private bool FilterFunc(BulkCementCQ element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Vehicle_Code.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Second_Vehicle_Code.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.DeliveryCode.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void RowClickEvent(TableRowClickEventArgs<BulkCementCQ> tableRowClickEventArgs)
        {
            Input = new();
        }

    }
}
