
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.entity.ViewModels
{
  public class SectionListViewModel
  {
   
    public List<AddSectionViewModel>? Section { get; set; }
     public List<AddTableViewModel>? Table { get; set; }


  }
}