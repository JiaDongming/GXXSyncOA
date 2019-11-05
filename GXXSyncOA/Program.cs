using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SyncOAdll;

namespace GXXSyncOA
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //接收参数，封装成实体类
                ProjectBinder binder = new ProjectBinder()
                {
                    ProjectID = Convert.ToInt32(args[0]),
                    BugID = Convert.ToInt32(args[1]),
                };
                ProjectRequestInfo info = null;
                try
                {
                     info = new ProjectRequestInfo(binder);
                }
                catch (Exception ex)
                {

                    LogHelper.Error("获取待发项目信息出现异常", ex);
                }
               

                LogHelper.WriteLog("---------------以下是同步到OA的立项信息------------");

                foreach (System.Reflection.PropertyInfo item in info.GetType().GetProperties())
                {
                    var attrs = item.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    var displayName = "";
                    if (attrs.Count() >0)
                    {
                            displayName = ((DisplayNameAttribute)attrs[0]).DisplayName;
                            LogHelper.WriteLog(displayName + "是：" + info.GetType().GetProperty(item.Name).GetValue(info, null));
                    
                    }
          
                }
                if (info.Attachments.Count() > 0)
                {
                    LogHelper.WriteLog("当前立项条目的附件信息如下:");
                    for (int i = 0; i < info.Attachments.Count(); i++)
                    {
                        LogHelper.WriteLog("第" + (i + 1) + "个附件名称：" + info.Attachments[i].AttachmentName);
                        LogHelper.WriteLog("第" + (i + 1) + "个附件下载地址：" + info.Attachments[i].AttachmentURL);
                    }
                    
                }
                else
                    LogHelper.WriteLog("附件信息：无附件");

                OAService.PLMService oaService = new OAService.PLMService();
                OAService.PLMVo[] plmvos = new OAService.PLMVo[1]
                {
                    new OAService.PLMVo()
                    {
                        pid = info.BugID.ToString(),//任务ID
                        projectname=info.BugName,//名称
                        createby= info.LoginName,//工号
                        createdate = info.SubmitDate,//提交日期
                        text=info.Desc,//描述
                        cpname=info.MainProductName,//主产品名称
                        cplevel=info.ProductLevel,//产品等级
                        workcode=info.ProductManager,//产品经理
                        cpx=info.ProductBelong,//产品归属
                        sotype=info.ProductType,//产品类型
                        cpcode=info.ProductCode,//产品代码
                        cpzycd=info.ProductUrgentLevel,//产品重要紧急程度
                        enddate=info.FinishDate,//要求完成时间
                        ysmoney=info.ProjectBudget,//项目预算金额
                        zc=info.CEO,//事业群总裁
                        xmcy=info.ProjectMebers,//项目组成员
                        projectys=info.ProjectBudgetDesc,//项目预算说明
                        ftpfile=info.Attachments4OA//附件信息（按OA要求，组织后，发给对方）

                  }

                 };
                try
                {
                 OAService.PLMVo[] result=   oaService.SaveData(plmvos);
                 LogHelper.WriteLog("调用OA传递立项返回数据成功:");
                }
                catch (Exception ex)
                {

                    LogHelper.Error("调用OA传递数据出错", ex);
                }
                

            }
            catch (Exception ex)
            {

                LogHelper.Error("接收参数出现异常!", ex);
            }
            
           
        }
    }
}
