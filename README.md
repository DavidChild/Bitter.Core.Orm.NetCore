# Bitter.Core  -- NETCORE ORM MAPPING 框架。

    一个全网最简单易用的高性能NETCORE/NETFRAMEWORK 数据库持久化框架（ORM）
    
    1： 支持 MSSQL,MYSQL
    
    2: 支持模型查询，以及 SQL 查询
    
    3：支持事务一次性提交
    
    4：支持BuldCopy 批量事务提交
    
    5：支持查询数据模型转换
    
    6：支持异常抛出，事务中断
    
    7：支持跨库事务提交（多次事务）
    
    8：支持SQL WITH优化
    
    9：支持 CONTOVER,SELECT ROW 模式的分页查询模式
    
    10: 支持多库
    
    11：支持读写分离
    
    12: 支持字段变更收集
  
 Bitter.Core  使得程序员变的更懒成为一件美丽的事！
   
# NUGET
NETCORE: https://www.nuget.org/packages/Bitter.Core.NetCore/

To install Bitter.Core.NetCore , run the following command in the Package Manager Console
```
PM> Install-Package Bitter.Core.NetCore
```

# 博客园

Cnblog: https://www.cnblogs.com/davidchildblog/category/1916170.html

# 示例Demo 

Github:https://github.com/DavidChild/Bitter.Core.Sample.git

## Database entities and Attributes 
```C#
[TableName("t_StudentScore")]
public class TStudentScoreInfo : BaseModel
{
   /// <summary>
   /// 主键
   /// </summary>
   [Key] //指定主键
   [Identity] //指定自增长
   public virtual Int32 FID { get; set; }

   /// <summary>
   /// 学生Id
   /// </summary>
   [Display(Name = @"学生Id")]
   public virtual Int32? FStudentId { get; set; }

   /// <summary>
   /// 学分
   /// </summary>
   public virtual Int32? FScore { get; set; }

   /// <summary>
   /// 插入时间
   /// </summary>
   public virtual DateTime? FAddTime { get; set; }

}

```
数据库模型创建说明请看博文： https://www.cnblogs.com/davidchildblog/articles/14276886.html

## Databases Connection Config Setting
```C#
{

  "connectionString":[
    {
    
     "dbType": "System.Data.SqlClient",
      "name": "MainData.Reader", 
      "value": "Min Pool Size=10;Max Pool Size=500;Connection Timeout=50;Data Source=192.168.99.66,12033;Initial Catalog=readdbname;Persist Security Info=True;User ID=username;Password=pwd; pooling=false"
    },
    {

      "dbType": "System.Data.SqlClient",
      "name": "MainData.Writer",
      "value": "Min Pool Size=10;Max Pool Size=500;Connection Timeout=50;Data Source=192.168.99.66,12033;Initial Catalog=writedbname;Persist Security Info=True;User ID=test;Password=pwd; pooling=false"
    }
   
   ] 
}
```
更多数据库连接配置： https://www.cnblogs.com/davidchildblog/articles/14276611.html


## SEARCH DEMO 
```C#
#region 根据条件全量查询  学生姓名为 HJB 的同学
BList<TStudentInfo> students = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find();

// 根据条件批量查询  学生姓名为 HJB 的同学
TStudentInfo student_1 = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find().FirstOrDefault(); //此FirstOrDefault 重构过,为安全模式,数据库如果查不到数据，返回为空对象,避免返回 NULL.
if (student_1.FID > 0) //说明查询到数据
{

}
#endregion

```

```C#
#region 高级查询直接SQL语句查询（非分页）
//查出分数>=90分的学生姓名以及具体学分

DataTable dt=  db.FindQuery(
                   @"SELECT score.FScore,student.FName as studentNameFROM  t_StudentScore score
                    LEFT JOIN t_student student  ON score.FStudentId = student.FID
                    WHERE score.FScore>=@score
                  ", new { score = 90 }).Find();
#endregion
```

```C#
#region  单表模型驱动查询--只查询符合条件的前 N 条数据，并且只返回具体的列（FAage,FName）：

students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c=>new object[] { c.FAage,c.FName}).Find(); //后面的 Select(columns)  方法指定了需要查询的列
students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c => new List<object>{ c.FAage, c.FName }).Find(); //后面的 Select(columns)   方法指定了需要查询的列

#endregion
```
更多查询DEMO示例： https://www.cnblogs.com/davidchildblog/articles/14276729.html
