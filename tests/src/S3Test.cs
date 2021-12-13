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
public  class S3Test{

    public static void Main(string[] args){
        object testObject = new ArrayList() {'1','2'};
        long  testLong  = 12345;
        string testString = "This is a string";
        int  testInt   = 12;
        IList testArrList = (IList)testObject;
 
        foreach(var item in testArrList  ){
            Console.WriteLine(item);
        }
        Console.WriteLine(testLong.GetType().ToString());
         Console.WriteLine(testString.GetType().ToString());
          Console.WriteLine(testInt.GetType().ToString());

    }
}