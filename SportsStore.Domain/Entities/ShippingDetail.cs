using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetail
    {
        [Required(ErrorMessage = "请输入名称")]
        [Display(Name ="名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入第一个地址")]
        [Display(Name = "地址1")]
        public string Line1 { get; set; }

        [Display(Name = "地址2")]
        public string Line2 { get; set; }

        [Display(Name = "地址3")]
        public string Line3 { get; set; }

        [Required(ErrorMessage = "请输入城市名")]
        [Display(Name = "城市名")]
        public string City { get; set; }

        [Required(ErrorMessage = "请输入省份名")]
        [Display(Name = "省份名")]
        public string State { get; set; }

        //[Display(Name = "压缩")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "请输入国家名")]
        [Display(Name = "国家名")]
        public string Country { get; set; }

        [Display(Name = "是否包装")]
        public bool GiftWrap { get; set; }
    }
}
