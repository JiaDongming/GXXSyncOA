using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace SyncOAdll
{

    [Serializable]
  public  class ProjectRequestInfo
    {
        static string ServerName = ConfigurationManager.AppSettings["ServerName"];
        static string UserToken = ConfigurationManager.AppSettings["UserToken"];

        
        public int ProjectID { get; set; }
        [DisplayName("任务ID")]
        public int BugID { get; set; }  //任务ID
        [DisplayName("名称")]
        public string BugName { get; set; }//名称
        public int? StatusID { get; set; }
        [DisplayName("状态")]
        public string StatusName { get; set; }// 状态
        public int? OwnerID { get; set; }
        [DisplayName("责任人")]
        public string OwnerName { get; set; }// 责任人
        [DisplayName("工号")]
        public string LoginName { get; set; }//工号
        public int? SubmitterID { get; set; }
        [DisplayName("提交者")]
        public string Submitter { get; set; }// 提交者
        [DisplayName("提交日期")]
        public string SubmitDate { get; set; }//提交日期
        [DisplayName("描述")]
        public string Desc { get; set; }//描述
        [DisplayName("主产品名称")]
        public string MainProductName { get; set; }//主产品名称
        [DisplayName("产品等级")]
        public string ProductLevel { get; set; }//产品等级
        [DisplayName("产品等级代码")]
        public string ProductLevelCode { get; set; }//产品等级代码，写死（A级是0，B级是1，C级是2）
        [DisplayName("产品经理")]
        public string ProductManager { get; set; }//产品经理
        public string ProductManagerName { get; set; }//产品经理名称
        [DisplayName("产品归属")]
        public string ProductBelong { get; set; }// 产品归属
        [DisplayName("产品归属代码")]
        public string ProductBelongCode { get; set; }//产品归属代码
        [DisplayName("产品类型")]
        public string ProductType { get; set; }//产品类型
        [DisplayName("产品代码")]
        public string ProductCode { get; set; }//产品代码
        [DisplayName("紧急程度")]
        public string UrgentLevel { get; set; }//紧急程度
        [DisplayName("产品重要紧急程度")]
        public string ProductUrgentLevel { get; set; }//产品重要紧急程度
        [DisplayName("要求完成时间")]
        public string FinishDate { get; set; }//要求完成时间
        [DisplayName("项目预算金额")]
        public string ProjectBudget { get; set; }// 项目预算金额
        [DisplayName("事业群总裁")]
        public string CEO { get; set; }//事业群总裁
        [DisplayName("项目组成员")]
        public string ProjectMebers { get; set; }//项目组成员
        public string ProjectMeberNames { get; set; }//项目组成员名称
        [DisplayName("项目预算说明")]
        public string ProjectBudgetDesc { get; set; }//项目预算说明
        
        public List<Attachment> Attachments { get; set; }//附件
        public string Attachments4OA { get; set; }

        public ProjectRequestInfo(ProjectBinder projectBinder)
        {
            using (GXX_DSEntities dbcontext = new GXX_DSEntities())
            {
                //Get current project request task info
                var bug = (from c in dbcontext.Bug where c.ProjectID == projectBinder.ProjectID && c.BugID == projectBinder.BugID select c).SingleOrDefault();
                {
                    ProjectID = bug.ProjectID;
                    BugID = bug.BugID;
                    BugName = bug.BugTitle;
                    StatusID = bug.ProgressStatusID;
                    OwnerID = bug.CurrentOwner;
                    SubmitterID = bug.CreatedByPerson;
                    SubmitDate = Convert.ToDateTime(bug.DateCreated).ToShortDateString();
                    Desc = bug.ProblemDescription;

                }

                //获取状态名称
                if (StatusID!=null)
                {
                    var status = (from s in dbcontext.ProgressStatusTypes where s.ProjectID == projectBinder.ProjectID && s.ProgressStatusID == StatusID select s).SingleOrDefault();
                    StatusName = status.ProgressStatusName;
                }
                else
                    StatusName = "";

                //Get all login table data
                var allMembers = from m in dbcontext.LogIn select m;

                //获取负责人名称
                if (OwnerID != null)
                {
                    if (allMembers.Where(s => s.PersonID == OwnerID).Count() == 1)
                    {
                        var Totalowner = (from o in allMembers where o.PersonID == OwnerID select o).SingleOrDefault();
                        OwnerName = Totalowner.FName+" "+Totalowner.LName;
                    }
                    else
                        OwnerName = "";
                }
                else
                    OwnerName = "";

                //获取提交人名称和工号
                if (SubmitterID != null)
                {
                    if (allMembers.Where(s => s.PersonID == SubmitterID).Count() == 1)
                    {
                        var TotalSubmitter = (from o in allMembers where o.PersonID == SubmitterID select o).SingleOrDefault();
                        Submitter = TotalSubmitter.FName + " " + TotalSubmitter.LName;
                        LoginName = (from o in allMembers where o.PersonID == SubmitterID select o.Login1).SingleOrDefault();
                    }
                    else
                    {
                        Submitter = "";
                        LoginName = "";
                    }
                        
                }
                else
                {
                    Submitter = "";
                    LoginName = "";
                }

                //Get table CustomerFieldTrackExt
                var customFields = (from fields in dbcontext.CustomerFieldTrackExt where fields.ProjectID == projectBinder.ProjectID && fields.BugID == projectBinder.BugID select fields).SingleOrDefault();

                MainProductName = customFields.Custom_1; //主产品名称

                ProductLevel = customFields.Custom_3; //产品等级
                Dictionary<string, string> ProductLevelDic = new Dictionary<string, string>();
                ProductLevelDic.Add("A级", "0");
                ProductLevelDic.Add("B级", "1");
                ProductLevelDic.Add("C级", "2");

                if (ProductLevel !=null)
                {
                    if (ProductLevelDic.ContainsKey(ProductLevel)  )
                    {
                        ProductLevelCode = ProductLevelDic[ProductLevel];
                    }
                    else

                        ProductLevelCode = null;
                }
                else
                    ProductLevelCode = null;

                ProductManagerName = customFields.Custom_2;//产品经理

                var newNames = from c in allMembers.ToList() select new { Name = c.FName + ' ' + c.LName, code=c.Login1 };
                if (ProductManagerName != null)
                {
                    ProductManager = (from c in newNames where c.Name == ProductManagerName select c.code).SingleOrDefault();
                }
                else
                    ProductManager = null;



                ProductBelong = customFields.Custom_4; // 产品归属
                if (ProductBelong!=null)
                {
                    if (ProductBelong.Contains("."))
                    {
                        ProductBelongCode = (from code in ProductBelong.Split('.') select code).FirstOrDefault();//产品归属代码
                    }
                    else
                        ProductBelongCode = null;
                }
                else
                    ProductBelongCode = null;


                ProductType = customFields.Desc_Custom_3;//产品类型


                UrgentLevel = customFields.Custom_311;//紧急程度
                ProductUrgentLevel = customFields.Custom_7;//产品重要紧急程度
                FinishDate = customFields.Custom_9.Substring(0,10) ;//要求完成时间
                ProjectBudget = customFields.Custom_6;// 项目预算金额

                ProjectBudgetDesc = customFields.Custom_10;//项目预算说明

                //get table CustomerFieldTrackExt2
                var customFields2 = (from fields2 in dbcontext.CustomerFieldTrackExt2 where fields2.ProjectID == projectBinder.ProjectID && fields2.IssueID == projectBinder.BugID select fields2);
                ProductCode = (from a in customFields2 where a.PageNumber == 7 select a.Custom_7).SingleOrDefault();//产品代码
                CEO = (from a in customFields2 where a.PageNumber == 6 select a.Custom_1).SingleOrDefault(); //事业群总裁

                ProjectMeberNames = (from a in customFields2 where a.PageNumber == 5 select a.Custom_3).SingleOrDefault(); //项目组成员
                if (ProjectMeberNames != null)
                {
                    List<string> members = (from uid in ProjectMeberNames.Split(',') select uid).ToList();
                    for (int i = 0; i < members.Count(); i++)
                    {
                        var logincode = (from c in newNames where c.Name == members[i].Trim() select c.code).SingleOrDefault();
                        if (logincode != null)
                        {
                            if (i == members.Count - 1)
                            {
                                ProjectMebers = ProjectMebers + logincode;
                            }
                            else
                                ProjectMebers = ProjectMebers + logincode + ',';
                        }

                    }
                }
               
               


                //attchments
                var attachments = from att in dbcontext.KWAttachments where att.ProjectID == projectBinder.ProjectID && att.ParentItemID == projectBinder.BugID select att;
                List<Attachment> issueAttachments = new List<Attachment>();
                StringBuilder oaattachments = new StringBuilder(@"<p style=""font-family:&#39;微软雅黑&#39;,&#39;Microsoft YaHei&#39;;font-size:12px;"">");
             
                foreach (var item in attachments)
                {
                    string url = string.Empty;
                    issueAttachments.Add(new Attachment()
                    {
                        AttachmentName = item.DisplayFileName,

                        AttachmentURL = "http://" + ServerName + "/DevSpecAPI/ReqAttachment?ProjectID=" + projectBinder.ProjectID + "&FileID=" + item.AttachmentFileID + "&viewtype=100&documenttype=1&VersionID=1&UserToken=" + UserToken

                    });
                    url = "http://" + ServerName + "/DevSpecAPI/ReqAttachment?ProjectID=" + projectBinder.ProjectID + "&FileID=" + item.AttachmentFileID + "&viewtype=100&documenttype=1&VersionID=1&UserToken=" + UserToken;
                    oaattachments.AppendFormat(@"<a href=""{0}"" target=""_blank"">{1}</a><br/>", url,item.DisplayFileName);
                }
                oaattachments.Append("</p>");
                Attachments4OA = oaattachments.ToString();
                Attachments = issueAttachments;

           //download url sample: http://localhost/DevSpecAPI/ReqAttachment?ProjectID=502&FileID=41047&viewtype=100&documenttype=1&VersionID=1&UserToken=C396131C-3092-42f8-A563-8837D906BCFD
           //传给OA的附件按OA要求的格式传递给对方，格式如下
              //<p style = "font-family:&#39;微软雅黑&#39;,&#39;Microsoft YaHei&#39;;font-size:12px;" >
             //<a href = "http://www.baidu.com " target = "_blank" > sohu </a ><br />
             //<a href = "http://www.qq.com" target = "_blank" > qq </a >
             //</p>
            }
        }

        
    }
  
}
