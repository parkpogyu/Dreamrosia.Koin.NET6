using MudBlazor;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Helps
{
    public partial class Help
    {
        private bool _loaded;

        private readonly Color Color = Color.Info;
        private string SelectedValue { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loaded = true;
        }
    }
}