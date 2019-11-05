using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace SyncOAdll
{
    [Serializable]
  public  class Attachment
    {
        [DisplayName("附件名称")]
        public string AttachmentName { get; set; }//附件名称
        [DisplayName("附件链接")]
        public string AttachmentURL { get; set; }//附件链接
 
    
   
        //AttachmentURL是类似如下可点击下载的地址
        //http://jiaxing.techexcel.com.cn/DevSpecAPI/ReqAttachmentDownload?ProjectID=190&FileID=162981&VersionID=0&FileType=0&UserToken=04FA8EF5-AB7A-433b-91C6-12233453162E
    }
}
