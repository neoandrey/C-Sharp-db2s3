 using System; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Data;
using System.Globalization;
using System.Reflection;

public class TestObject{

     public string name;

     public int number;

     public string info;
}
public  class S3Test{

              public static DataTable getDataTable<T> (List<T> items, string[] columns)
              
          {
               DataTable dataTable = new DataTable();

               foreach (var col in columns)
               {
                    dataTable.Columns.Add(col);
                    Console.WriteLine("ColumnName: "+col);
               }
               int it = 0;
               foreach (var item in items)
               {

                    var values = new object[columns.Length];
                    for (int i = 0; i < columns.Length; i++)
                    { 
                        string colName = columns[i];
                         values[i] =string.IsNullOrEmpty(item[colName])?"":item[colName];
                    }
                    dataTable.Rows.Add(values);
               }
               
               return dataTable;
          }
          
    public static void Main(string[] args){
        object testObject = new ArrayList() {'1','2'};
        long  testLong  = 12345;
        string testString = "This is a string";
        int  testInt   = 12;
        IList testArrList = (IList)testObject;
 
        foreach(var item in testArrList  ){
            Console.WriteLine(item);
        }

        List<TestObject> testList = new  List<TestObject>();
        testList.Add( new TestObject(){ name="Ade", number =1, info="here we go"});
        testList.Add( new TestObject(){ name="Banke", number =2, info="here we go again"});
        testList.Add( new TestObject(){ name="Zach", number =3, info="Efron"});

        DataTable tab = getDataTable(testList, new string[] {"name","number","info"} );

                    foreach (DataRow row in tab.Rows){
                foreach (DataColumn col in tab.Columns){
                    Console.WriteLine(row[col.ColumnName]);

                }

            }

        Console.WriteLine(testLong.GetType().ToString());
         Console.WriteLine(testString.GetType().ToString());
          Console.WriteLine(testInt.GetType().ToString());

    }
}