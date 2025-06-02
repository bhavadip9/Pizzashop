using Pizzashop.entity.ViewModels;
namespace Pizzashop.entity.ViewModels;
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public string? HtmlTitleTag { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}