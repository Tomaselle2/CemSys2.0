using CemSys2.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.IO;

namespace CemSys2.Business
{
    public class PdfService : IPdfService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public PdfService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<byte[]> GeneratePdfAsync(string viewName, object model, HttpContext httpContext)
        {
            var html = await RenderViewToStringAsync(viewName, model, httpContext);

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);

            using var pdfStream = await page.PdfStreamAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "5mm",
                    Right = "5mm",
                    Bottom = "10mm",
                    Left = "5mm"
                }
            });

            using var memoryStream = new MemoryStream();
            await pdfStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model, HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());

            using var sw = new StringWriter();
            var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
