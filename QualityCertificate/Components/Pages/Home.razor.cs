using Dapper;
using HoangThach.AccountShared.Models.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cors.Infrastructure;
using MudBlazor;
using QRCoder;
using QualityCertificate.Datas.Entity;
using QualityCertificate.Datas.Models;
using QualityCertificate.Services;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QualityCertificate.Components.Pages
{
    public partial class Home
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
