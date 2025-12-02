using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LinguaNews.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinguaNews.Pages.ParkWhiz
{
    public class IndexModel : PageModel
    {
        public IReadOnlyList<ParkLocation>? Locations { get; private set; }

        // Server-side fetch example (optional) — client JS will call the API; keep server fetch minimal
        public void OnGet()
        {
        }
    }
}