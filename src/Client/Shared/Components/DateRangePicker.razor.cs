using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class DateRangePicker
    {
        [Parameter] public EventCallback<DateRangeTerms> SelectedTermChanged { get; set; }

        [Parameter]
        public DateRangeTerms SelectedTerm
        {
            get => _selectedTerm;

            set
            {
                if (_selectedTerm == value) { return; }

                _selectedTerm = value;

                DateRangeTermValueChanged();
            }
        }

        private DateRangeTerms _selectedTerm { get; set; } = DateRangeTerms._All;

        [Parameter]
        public DateRange DateRange { get; set; } = new DateRange();

        private void SelectedTermsValueChanged(IEnumerable<DateRangeTerms> values)
        {
            DateRangeTermValueChanged();
        }

        private void DateRangeTermValueChanged()
        {
            var now = DateTime.Now.Date;

            DateRange.Start = now.GetBefore(_selectedTerm);
            DateRange.End = now;

            SelectedTermChanged.InvokeAsync(_selectedTerm);
        }
    }
}
