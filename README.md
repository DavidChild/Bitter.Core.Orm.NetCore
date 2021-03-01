# Bitter.Core  -- NETCORE ORM MAPPING Framework。

   A simple and easy to use high performance NETCORE / netframework database persistence framework (ORM)

1: Support MSSQL, MySQL

2: Support model query and SQL query

3: One time transaction commit is supported

4: Support bulk transaction commit of buldcopy

5: Support query data model transformation

6: Support exception throwing and transaction interruption

7: Support cross database transaction commit (multiple transactions)

8: Support SQL with optimization

9: Pagination query mode supporting convert, select row mode

10: Support multiple databases

11: Read write separation is supported

12: Support field change collection
  
Bitter.Core It's a beautiful thing to make programmers lazier!
   
# NUGET
NETCORE: https://www.nuget.org/packages/Bitter.Core.NetCore/

To install Bitter.Core.NetCore , run the following command in the Package Manager Console
```
PM> Install-Package Bitter.Core.NetCore
```

# Blog

Cnblogs: https://www.cnblogs.com/davidchildblog/category/1916170.html

# Simple-Demo 

Github:https://github.com/DavidChild/Bitter.Core.Sample.git

## Database entities and Attributes 
```C#
[TableName("t_StudentScore")]
public class TStudentScoreInfo : BaseModel
{
   /// <summary>
   /// It's a Primary key
   /// </summary>
   [Key] //designate the filed is  Primary key
   [Identity] //designate the filed is  Identity
   public virtual Int32 FID { get; set; }

   /// <summary>
   ///  student Id
   /// </summary>
   [Display(Name = @"student Id")]
   public virtual Int32? FStudentId { get; set; }

   /// <summary>
   /// score
   /// </summary>
   public virtual Int32? FScore { get; set; }

   /// <summary>
   /// add time
   /// </summary>
   public virtual DateTime? FAddTime { get; set; }

}

```
refer to the blog for the description of database model creation： https://www.cnblogs.com/davidchildblog/articles/14276886.html

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
db Connection Setting： https://www.cnblogs.com/davidchildblog/articles/14276611.html


## SEARCH DEMO 
```C#
#region According to the conditions of full inquiry students name HJB students
BList<TStudentInfo> students = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find();

// Batch query the students whose names are HJB according to the conditions
TStudentInfo student_1 = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find().FirstOrDefault(); //The FirstOrDefault method  After refactoring, it is in security mode. If the database can't find the data, it will return null object to avoid returning null
if (student_1.FID > 0) //Description query data
{

}
#endregion

```

```C#
#region Advanced query direct SQL statement query (non pagination)
//Find out the name of the student whose score is > = 90 and the specific credit

DataTable dt=  db.FindQuery(
                   @"SELECT score.FScore,student.FName as studentNameFROM  t_StudentScore score
                    LEFT JOIN t_student student  ON score.FStudentId = student.FID
                    WHERE score.FScore>=@score
                  ", new { score = 90 }).Find();
#endregion
```

```C#
#region  Single table model driven query only queries the first n qualified data, and only returns specific columns (faage, fname)

students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c=>new object[] { c.FAage,c.FName}).Find(); //The later select (columns) method specifies the columns to query
students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c => new List<object>{ c.FAage, c.FName }).Find(); //The later select (columns) method specifies the columns to query

#endregion
```
More Demo Simple： https://www.cnblogs.com/davidchildblog/articles/14276729.html
