using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace multibot.forms
{
    [Serializable]
    public class requestForm
    {
        [Prompt("Provide your registration number")]
        public string RegNum { get; set; }

        [Prompt("What's your request about?")]
        public string Request { get; set; }

        [Prompt("Select required service")]
        public ServiceOption? Length;

        public enum ServiceOption { Complaint = 1, Service };

        public static IForm<requestForm> BuildRequestForm()
        {
            return new FormBuilder<requestForm>().Message("Welcome to the sandwich order bot!")
                .Field(nameof(RegNum)).Field(nameof(Request)).Field(nameof(ServiceOption)).Build();



        }
    }
}