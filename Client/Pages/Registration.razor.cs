using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Jobbvin.Client;
using Jobbvin.Client.Shared;
using System.ComponentModel.DataAnnotations;
using Jobbvin.Client.Services;
using Microsoft.AspNetCore.WebUtilities;
using Jobbvin.Shared.Models;
using static System.Net.WebRequestMethods;
using System.Diagnostics.Metrics;
using System.Reflection;
using BlazorBootstrap;

namespace Jobbvin.Client.Pages
{
    public partial class Registration
    {

        private temp_user model = new temp_user();
        private bool loading;
        private string error;
        string displaySubmitOTP = "none";
        string displaySubmitResendOTP = "none";
        string displayOTPText = "none";
        string displaySubmit = "block";
        string displayLogin = "none";
        
        string timeRemains = "";
        DateTime exp_time;
        int mins = 1;
        private int counter = 60;
        private static System.Timers.Timer aTimer;
        protected override void OnInitialized()
        {
            
        }

        public void StartTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += CountDownTimer;
            aTimer.Enabled = true;
            exp_time = DateTime.Now.AddMinutes(mins);
        }

        public void CountDownTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (counter > 1)
            {
                counter -= 1;
               
                var countDownDate = DateTime.Now;
                var distance = exp_time - countDownDate;
                timeRemains = "OTP Expires in : " + distance.Minutes + " mins : " + distance.Seconds + " secs";
            }
            else
            {
                aTimer.Enabled = false;
                timeRemains = "Expied!";
                model.mobile_val = null;
                displaySubmitResendOTP = "block";
                displaySubmitOTP = "none";
                counter = 60 * mins;
            }
            StateHasChanged();

        }

        private async void HandleValidSubmit()
        {
            loading = true;
            error = "";
            try
            {
                if (!string.IsNullOrEmpty(model.mobile_val))
                {
                    var response = await UserServiceClient.ValidateOtp(model);
                    if (response.Status)
                    {
                        timeRemains = response.Message;
                        displaySubmitOTP = "none";
                        displaySubmit = "none";
                        displaySubmitResendOTP = "none";
                        displayOTPText = "none";
                        displayLogin = "block";
                    }
                    else
                    {
                        error = response.Message;
                    }
                }
                else
                {
                    var response = await UserServiceClient.InsertTempUser(model);
                    if (response.Status)
                    {
                        displaySubmitOTP = "block";
                        displaySubmit = "none";
                        displaySubmitResendOTP = "none";
                        displayOTPText = "block";
                        StartTimer();
                    }
                    error = response.Message;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                loading = false;
            }
            loading = false;
            StateHasChanged();
        }
    }
}