#pragma checksum "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\Shop\IstraziPonude.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b6657a119a0ea1795c9a2ba2cbb6a249a4fb1255"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shop_IstraziPonude), @"mvc.1.0.view", @"/Views/Shop/IstraziPonude.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shop/IstraziPonude.cshtml", typeof(AspNetCore.Views_Shop_IstraziPonude))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\_ViewImports.cshtml"
using WebApplication5;

#line default
#line hidden
#line 2 "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\_ViewImports.cshtml"
using WebApplication5.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b6657a119a0ea1795c9a2ba2cbb6a249a4fb1255", @"/Views/Shop/IstraziPonude.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7ac7a6a20369a094c1643b76f0e92e19ec3cef6a", @"/Views/_ViewImports.cshtml")]
    public class Views_Shop_IstraziPonude : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication5.Models.Katalog>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(121, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
#line 5 "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\Shop\IstraziPonude.cshtml"
  
    ViewData["Title"] = "Istrazi ponude";

#line default
#line hidden
            BeginContext(175, 176, true);
            WriteLiteral("\r\n<h1>Istrazi ponude nasih gotovih menija i kataloga</h1>\r\n\r\n<div>\r\n    <h4>Katalog i Meni</h4>\r\n    <hr />\r\n    <dl class=\"row\">\r\n        <dt class = \"col-sm-2\">\r\n            ");
            EndContext();
            BeginContext(352, 41, false);
#line 16 "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\Shop\IstraziPonude.cshtml"
       Write(Html.DisplayNameFor(model => model.Naziv));

#line default
#line hidden
            EndContext();
            BeginContext(393, 63, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class = \"col-sm-10\">\r\n            ");
            EndContext();
            BeginContext(457, 37, false);
#line 19 "C:\Users\PC2\Desktop\WebApplication5\WebApplication5\Views\Shop\IstraziPonude.cshtml"
       Write(Html.DisplayFor(model => model.Naziv));

#line default
#line hidden
            EndContext();
            BeginContext(494, 43, true);
            WriteLiteral("\r\n        </dd>\r\n    </dl>\r\n</div>\r\n<div>\r\n");
            EndContext();
            BeginContext(654, 8, true);
            WriteLiteral("</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication5.Models.Katalog> Html { get; private set; }
    }
}
#pragma warning restore 1591
